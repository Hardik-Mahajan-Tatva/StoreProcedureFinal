using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Models;
using Pizzashop.Repository.Interfaces;
using PizzaShop.Repository.ViewModels;

namespace Pizzashop.Repository.Implementations
{
    public class ModifierRepository : IModifierRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor

        public ModifierRepository(PizzaShopContext context)
        {
            _context = context;
        }

        #endregion

        #region GetAllModifierGroups
        public List<Modifiergroup> GetAllModifierGroups()
        {
            return _context.Modifiergroups.ToList();
        }
        #endregion

        #region GetModifiersByModifiergroupId
        public async Task<IEnumerable<Modifier>> GetModifiersByModifiergroupId(int modifiergroupId)
        {
            return await _context.ModifierGroupModifierMappings
                .Where(
                    m =>
                        m.ModifierGroupId == modifiergroupId
                        && m.Modifier != null
                        && (m.Modifier.Isdeleted == false || m.Modifier.Isdeleted == null)
                )
                .Select(m => m.Modifier)
                .Distinct()
                .ToListAsync();
        }
        #endregion

        #region GetPaginatedModifiersByGroupIdAsync
        public async Task<(IEnumerable<Modifier>, int)> GetPaginatedModifiersByGroupIdAsync(
            int modifierGroupId,
            int pageNumber,
            int pageSize,
            string searchQuery = ""
        )
        {
            var query = _context.ModifierGroupModifierMappings
                .Include(m => m.ModifierGroup)
                .Where(
                    m =>
                        m.ModifierGroupId == modifierGroupId
                        && m.ModifierGroup != null
                        && (m.ModifierGroup.Isdeleted == false || m.ModifierGroup.Isdeleted == null)
                )
                .Select(m => m.Modifier)
                .Where(modifier => modifier != null && modifier.Isdeleted == false)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                string trimmedSearch = searchQuery.Trim().ToLower();
                query = query.Where(
                    modifier =>
                        modifier.Modifiername != null
                        && modifier.Modifiername.ToLower().Contains(trimmedSearch)
                );
            }

            var totalCount = await query.CountAsync();
            var modifiers = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (modifiers, totalCount);
        }
        #endregion

        #region AddModifier
        public bool AddModifier(Modifier modifier, List<int> modifierGroupIds)
        {
            _context.Modifiers.Add(modifier);
            _context.SaveChanges();

            if (modifierGroupIds != null && modifierGroupIds.Any())
            {
                var modifierGroupMappings = modifierGroupIds
                    .Select(
                        groupId =>
                            new ModifierGroupModifierMapping
                            {
                                ModifierGroupId = groupId,
                                ModifierId = modifier.Modifierid
                            }
                    )
                    .ToList();

                _context.ModifierGroupModifierMappings.AddRange(modifierGroupMappings);
                _context.SaveChanges();
            }
            return true;
        }
        #endregion

        #region AddModifierGroupMapping
        public bool AddModifierGroupMapping(
            ModifierGroupModifierMapping modifierGroupModifierMapping
        )
        {
            _context.ModifierGroupModifierMappings.Add(modifierGroupModifierMapping);
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region GetModifierById
        public Modifier GetModifierById(int modifierId)
        {
            var modifier =
                _context.Modifiers
                    .Include(m => m.ModifierGroupModifierMappings)
                    .FirstOrDefault(m => m.Modifierid == modifierId && m.Isdeleted == false)
                ?? throw new InvalidOperationException("Modifier not found.");
            return modifier;
        }
        #endregion

        #region UpdateModifierAsync 
        public async Task<bool> UpdateModifierAsync(Modifier modifier, List<int> modifierGroupIds)
        {
            var existingModifier = await _context.Modifiers.FindAsync(modifier.Modifierid);
            if (existingModifier == null)
                return false;

            existingModifier.Modifiername = modifier.Modifiername;
            existingModifier.Rate = modifier.Rate;
            existingModifier.Quantity = modifier.Quantity;
            existingModifier.Description = modifier.Description;
            existingModifier.Unitid = modifier.Unitid;

            _context.Modifiers.Update(existingModifier);

            if (modifierGroupIds != null)
            {
                var existingMappings = _context.ModifierGroupModifierMappings
                    .Where(m => m.ModifierId == modifier.Modifierid)
                    .ToList();

                var existingGroupIds = existingMappings.Select(m => m.ModifierGroupId).ToList();

                var newGroupIds = modifierGroupIds.Except(existingGroupIds).ToList();
                var mappingsToAdd = newGroupIds
                    .Select(
                        groupId =>
                            new ModifierGroupModifierMapping
                            {
                                ModifierGroupId = groupId,
                                ModifierId = modifier.Modifierid
                            }
                    )
                    .ToList();

                var mappingsToRemove = existingMappings
                    .Where(m => !modifierGroupIds.Contains(m.ModifierGroupId))
                    .ToList();

                if (mappingsToRemove.Any())
                    _context.ModifierGroupModifierMappings.RemoveRange(mappingsToRemove);

                if (mappingsToAdd.Any())
                    _context.ModifierGroupModifierMappings.AddRange(mappingsToAdd);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        #endregion

        #region DeleteModifierAsync
        public async Task<bool> DeleteModifierAsync(int modifierId)
        {
            var modifier = await _context.Modifiers.FindAsync(modifierId);

            if (modifier == null)
                return false;

            modifier.Isdeleted = true;

            _context.Modifiers.Update(modifier);
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion

        #region SoftDeleteModifiers
        public void SoftDeleteModifiers(List<int> modifierIds)
        {
            var modifiers = _context.Modifiers
                .Where(i => modifierIds.Contains(i.Modifierid))
                .ToList();
            if (modifiers.Any())
            {
                foreach (var modifier in modifiers)
                {
                    modifier.Isdeleted = true;
                }
                _context.SaveChanges();
            }
        }
        #endregion

        #region GetModifiersAsync
        public async Task<PaginatedList<Modifier>> GetModifiersAsync(
            int pageIndex,
            int pageSize,
            string searchQuery = ""
        )
        {
            var query = _context.Modifiers.Where(m => (bool)!m.Isdeleted!).AsQueryable();
            var trimmedSearch = searchQuery.Trim().ToLower();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(m => m.Modifiername.ToLower().Contains(trimmedSearch));
            }

            return await PaginatedList<Modifier>.CreateAsync(query, pageIndex, pageSize);
        }
        #endregion

        #region GetModifiersByGroupId
        public List<Modifier> GetModifiersByGroupId(int modifierGroupId)
        {
            return _context.ModifierGroupModifierMappings
                .Where(mgm => mgm.ModifierGroupId == modifierGroupId)
                .Select(mgm => mgm.Modifier)
                .Distinct()
                .ToList();
        }
        #endregion

        #region GetModifiersByGroupIds
        public List<Modifier> GetModifiersByGroupIds(List<int> modifierGroupIds)
        {
            return _context.ModifierGroupModifierMappings
                .Where(mgm => modifierGroupIds.Contains(mgm.ModifierGroupId))
                .Select(mgm => mgm.Modifier)
                .Distinct()
                .Include(m => m.ModifierGroupModifierMappings)
                .ToList();
        }
        #endregion

        #region GetAllModifierIds
        public async Task<List<int>> GetAllModifierIds(int modifierGroupId)
        {
            return await _context.Modifiers
                .Include(mgp => mgp.ModifierGroupModifierMappings)
                .Where(modifier => modifier.Isdeleted == false && modifier.ModifierGroupModifierMappings.Any(mgm => mgm.ModifierGroupId == modifierGroupId))
                .Select(modifier => modifier.Modifierid)
                .ToListAsync();
        }
        #endregion

        #region GetAllModifierIdsExisting
        public async Task<List<int>> GetAllModifierIdsExisting()
        {
            var result = await _context.Modifiers
                .Include(mgp => mgp.ModifierGroupModifierMappings)
                .Where(modifier => modifier.Isdeleted == false)
                .Select(modifier => modifier.Modifierid)
                .ToListAsync();
            return result;
        }
        #endregion
        public async Task<List<Modifier>> GetAllModifierIdsWithNamesAsync()
        {
            return await _context.Modifiers.ToListAsync();
        }

    }
}


using PizzaShop.Repository.Models;
using Pizzashop.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.ViewModels;
using Npgsql;

namespace Pizzashop.Repository.Implementations
{
    public class ModifiergoupRepository : IModifiergoupRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public ModifiergoupRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region GetAll
        public IEnumerable<Modifiergroup> GetAll()
        {
            return _context.Modifiergroups
                .Where(mg => mg.Isdeleted == false)
                .OrderBy(mg => mg.Sortorder)
                .ToList();
        }
        #endregion

        #region AddAsync
        public async Task<bool> AddAsync(Modifiergroup modifierGroup)
        {
            await _context.Modifiergroups.AddAsync(modifierGroup);
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion

        #region GetByIdAsync
        public async Task<Modifiergroup?> GetByIdAsync(int modifiergroupId)
        {
            return await _context.Modifiergroups
                .Include(mg => mg.ModifierGroupModifierMappings)
                .ThenInclude(mgm => mgm.Modifier)
                .FirstOrDefaultAsync(mg => mg.Modifiergroupid == modifiergroupId);
        }
        #endregion

        #region UpdateModifierGroupMappingsAsync
        public async Task UpdateModifierGroupMappingsAsync(
            int modifierGroupId,
            List<int> modifierIds
        )
        {
            var existingMappings = _context.ModifierGroupModifierMappings
                .Where(mgm => mgm.ModifierGroupId == modifierGroupId)
                .ToList();

            var existingModifierIds = existingMappings.Select(m => m.ModifierId).ToList();

            var mappingsToRemove = existingMappings
                .Where(mgm => !modifierIds.Contains(mgm.ModifierId))
                .ToList();

            _context.ModifierGroupModifierMappings.RemoveRange(mappingsToRemove);

            var newModifierIds = modifierIds.Except(existingModifierIds).ToList();

            foreach (var modifierId in newModifierIds)
            {
                _context.ModifierGroupModifierMappings.Add(
                    new ModifierGroupModifierMapping
                    {
                        ModifierGroupId = modifierGroupId,
                        ModifierId = modifierId
                    }
                );
            }

            await _context.SaveChangesAsync();
        }
        #endregion

        #region UpdateAsync
        public async Task<bool> UpdateAsync(Modifiergroup modifierGroup)
        {
            var existingModifierGroup = await _context.Modifiergroups.FindAsync(
                modifierGroup.Modifiergroupid
            );

            if (existingModifierGroup == null)
            {
                return false;
            }

            existingModifierGroup.Modifiergroupname = modifierGroup.Modifiergroupname;
            existingModifierGroup.Description = modifierGroup.Description;

            _context.Modifiergroups.Update(existingModifierGroup);
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion

        #region SoftDeleteAsync 
        public async Task<bool> SoftDeleteAsync(int modifierGroupId)
        {
            var modifierGroup = await _context.Modifiergroups.FindAsync(modifierGroupId);
            if (modifierGroup == null)
            {
                return false;
            }

            modifierGroup.Isdeleted = true;
            _context.Modifiergroups.Update(modifierGroup);
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion

        #region GetModifierGroupById
        public Modifiergroup? GetModifierGroupById(int modifierGroupId)
        {
            return _context.Modifiergroups.FirstOrDefault(
                mg => mg.Modifiergroupid == modifierGroupId
            );
        }
        #endregion

        #region GetModifierGroupsByIds
        public List<Modifiergroup> GetModifierGroupsByIds(List<int> modifierGroupIds)
        {
            return _context.Modifiergroups
                .Where(g => modifierGroupIds.Contains(g.Modifiergroupid))
                .ToList();
        }
        #endregion

        #region SaveAsync
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        #endregion

        #region AddModifierMappingsAsync
        public async Task<bool> AddModifierMappingsAsync(int modifierGroupId, List<int> modifierIds)
        {
            var mappings = modifierIds
                .Select(
                    modifierId =>
                        new ModifierGroupModifierMapping
                        {
                            ModifierGroupId = modifierGroupId,
                            ModifierId = modifierId
                        }
                )
                .ToList();

            await _context.ModifierGroupModifierMappings.AddRangeAsync(mappings);
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion

        #region ModifierGroupNameExistsAsync
        public async Task<bool> ModifierGroupNameExistsAsync(string modifierGroupName)
        {
            return await _context.Modifiergroups.AnyAsync(
                mg =>
                    mg.Modifiergroupname.ToLower() == modifierGroupName.ToLower()
                    && (mg.Isdeleted == false || mg.Isdeleted == null)
            );
        }
        #endregion

        #region ModifierGroupNameExistsAsync
        public async Task<bool> ModifierGroupNameExistsAsync(
            string modifierGroupName,
            int excludeId
        )
        {
            return await _context.Modifiergroups.AnyAsync(
                mg =>
                    mg.Modifiergroupname.ToLower() == modifierGroupName.ToLower()
                    && mg.Modifiergroupid != excludeId
            );
        }
        #endregion


        public async Task<List<ItemModifierGroupMapRaw>> GetMappingByItemIdSPAsync(int itemId)
        {
            try
            {
                var itemIdParam = new NpgsqlParameter("item_id_input", itemId);

                var result = await _context
                    .ItemModifierGroupMapRaw
                    .FromSqlRaw("SELECT * FROM get_item_modifier_group_mappings(@item_id_input)", itemIdParam)
                    .ToListAsync();

                return result;
            }
            catch (PostgresException pgEx)
            {
                // PostgreSQL-specific errors
                Console.WriteLine("PostgreSQL error occurred:");
                Console.WriteLine($"Message: {pgEx.Message}");
                Console.WriteLine($"Detail: {pgEx.Detail}");
                Console.WriteLine($"Where: {pgEx.Where}");
                Console.WriteLine($"Code: {pgEx.SqlState}");
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                // Entity Framework DB update issues
                Console.WriteLine("EF Core DB update error occurred:");
                Console.WriteLine($"Message: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {dbEx.InnerException.Message}");
                }
                throw;
            }
            catch (InvalidOperationException invEx)
            {
                // Invalid ops like mapping problems
                Console.WriteLine("Invalid operation:");
                Console.WriteLine($"Message: {invEx.Message}");
                if (invEx.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {invEx.InnerException.Message}");
                }
                throw;
            }
            catch (Exception ex)
            {
                // General fallback
                Console.WriteLine("An unknown error occurred:");
                Console.WriteLine($"Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }


    }
}
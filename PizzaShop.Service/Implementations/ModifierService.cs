using Pizzashop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class ModifierService : IModifierService
    {
        private readonly IModifierRepository _modifierRepository;

        public ModifierService(IModifierRepository modifierRepository)
        {
            _modifierRepository = modifierRepository;
        }

        public List<Modifiergroup> GetAllModifierGroups()
        {
            return _modifierRepository.GetAllModifierGroups();
        }

        public async Task<IEnumerable<ModifierViewModel>> GetModifiersByModifiergroupId(int modifiergroupId)
        {
            var modifiers = await _modifierRepository.GetModifiersByModifiergroupId(modifiergroupId);

            return modifiers.Select(m => new ModifierViewModel
            {
                ModifierId = m.Modifierid,
                Modifiername = m.Modifiername,
                Modifiergroupid = modifiergroupId,
                Rate = m.Rate,
                Quantity = m.Quantity,
                Unitid = m.Unitid,
                Description = m.Description,
                Isdeleted = m.Isdeleted
            }).ToList();
        }


        public async Task<PaginatedList<ModifierViewModel>> GetPaginatedModifiersByGroupIdAsync(int modifierGroupId, int pageNumber, int pageSize, string searchQuery = "")
        {
            var (modifiers, totalCount) = await _modifierRepository.GetPaginatedModifiersByGroupIdAsync(modifierGroupId, pageNumber, pageSize, searchQuery);
            var mappedModifiers = modifiers.Select(modifier => new ModifierViewModel
            {
                ModifierId = modifier.Modifierid,
                Modifiername = modifier.Modifiername,
                Unitid = modifier.Unitid,
                Rate = modifier.Rate,
                Quantity = modifier.Quantity,
            }).ToList();

            return new PaginatedList<ModifierViewModel>(mappedModifiers, totalCount, pageNumber, pageSize);
        }

        public bool AddModifier(AddModifierViewModel model)
        {
            try
            {
                var modifier = new Modifier
                {
                    Modifiername = model.ModifierName!,
                    Rate = (decimal)model.Rate!,
                    Quantity = model.Quantity,
                    Unitid = (int)model.Unitid!,
                    Description = model.Description
                };

                bool isAdded = _modifierRepository.AddModifier(modifier, model.ModifierGroupIds!);

                return isAdded;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public AddModifierViewModel GetModifierById(int id)
        {
            var modifier = _modifierRepository.GetModifierById(id);
            if (modifier == null) return new AddModifierViewModel();

            return new AddModifierViewModel
            {
                ModifierId = modifier.Modifierid,
                ModifierName = modifier.Modifiername,
                Rate = modifier.Rate,
                Quantity = modifier.Quantity ?? 0,
                Unitid = modifier.Unitid,
                Description = modifier.Description,
                ModifierGroupIds = modifier.ModifierGroupModifierMappings.Select(mg => mg.ModifierGroupId).ToList()
            };
        }



        public async Task<bool> UpdateModifierAsync(AddModifierViewModel model)
        {
            if (model == null)
                return false;

            var modifier = new Modifier
            {
                Modifierid = model.ModifierId ?? 0,
                Modifiername = model.ModifierName!,
                Rate = (decimal)model.Rate!,
                Quantity = model.Quantity,
                Description = model.Description,
                Unitid = (int)model.Unitid!,
                Modifiedat = DateTime.UtcNow
            };

            return await _modifierRepository.UpdateModifierAsync(modifier, model.ModifierGroupIds!);
        }



        public async Task<bool> DeleteModifierAsync(int modifierId)
        {
            return await _modifierRepository.DeleteModifierAsync(modifierId);
        }
        public List<Modifier> GetModifiersByGroupId(int modifierGroupId)
        {
            return _modifierRepository.GetModifiersByGroupId(modifierGroupId);
        }

        public List<Modifier> GetModifiersByGroupIds(List<int> modifierGroupIds)
        {
            return _modifierRepository.GetModifiersByGroupIds(modifierGroupIds);
        }

        public void SoftDeleteModifiers(List<int> modifierIds)
        {
            _modifierRepository.SoftDeleteModifiers(modifierIds);
        }
        public async Task<PaginatedList<ModifierViewModel>> GetModifiersAsync(int pageIndex, int pageSize, string searchQuery = "")
        {
            var pagedModifiers = await _modifierRepository.GetModifiersAsync(pageIndex, pageSize, searchQuery);

            var modifierViewModels = pagedModifiers.Select(m => new ModifierViewModel
            {
                ModifierId = m.Modifierid,
                Modifiername = m.Modifiername,
                Rate = m.Rate,
                Quantity = m.Quantity
            }).ToList();

            return new PaginatedList<ModifierViewModel>(modifierViewModels, pagedModifiers.TotalItems, pageIndex, pageSize);
        }
        public async Task<List<int>> GetAllModifierIds(int modifierGroupId)
        {
            return await _modifierRepository.GetAllModifierIds(modifierGroupId);
        }
        public async Task<List<int>> GetAllModifierIdsExisting()
        {
            return await _modifierRepository.GetAllModifierIdsExisting();
        }
        public async Task<List<ModifierViewModel>> GetAllModifierIdsWithNamesAsync()
        {
            var modifiers = await _modifierRepository.GetAllModifierIdsWithNamesAsync();

            return modifiers.Select(m => new ModifierViewModel
            {
                ModifierId = m.Modifierid,
                Modifiername = m.Modifiername
            }).ToList();
        }
    }

}


using System.Text.Json;
using Pizzashop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class ModifiergroupService : IModifiergroupService
    {
        private readonly IModifiergoupRepository _modifiergoupRepository;

        public ModifiergroupService(IModifiergoupRepository modifiergoupRepository)
        {
            _modifiergoupRepository = modifiergoupRepository;
        }

        public IEnumerable<ModifiergroupViewModel> GetAll()
        {
            var modifierGroups = _modifiergoupRepository.GetAll();

            return modifierGroups.Select(mg => new ModifiergroupViewModel
            {
                ModifierGroupId = mg.Modifiergroupid,
                ModifierGroupName = mg.Modifiergroupname
            }).ToList();
        }

        public async Task<bool> CreateModifierGroupAsync(ModifiergroupViewModel viewModel)
        {
            bool exists = await _modifiergoupRepository.ModifierGroupNameExistsAsync(viewModel.ModifierGroupName!);
            if (exists)
            {
                throw new InvalidOperationException("Modifier Group Name already exists.");
            }
            if (string.IsNullOrWhiteSpace(viewModel.ModifierGroupName))
            {
                throw new ArgumentException("Modifier Group Name cannot be null or empty.");
            }
            var modifierGroup = new Modifiergroup
            {
                Modifiergroupname = viewModel.ModifierGroupName,
                Description = viewModel.Description
            };

            var isAdded = await _modifiergoupRepository.AddAsync(modifierGroup);
            if (isAdded && viewModel.ModifierIds != null && viewModel.ModifierIds.Any())
            {
                await _modifiergoupRepository.AddModifierMappingsAsync(modifierGroup.Modifiergroupid, viewModel.ModifierIds);
            }

            return isAdded;
        }


        public async Task<ModifiergroupViewModel> GetModifierGroupByIdAsync(int id)
        {
            var modifierGroup = await _modifiergoupRepository.GetByIdAsync(id);

            if (modifierGroup == null)
            {
                return null!;
            }

            return new ModifiergroupViewModel
            {

                ModifierGroupId = modifierGroup.Modifiergroupid,
                ModifierGroupName = modifierGroup.Modifiergroupname,
                Description = modifierGroup.Description,
                Modifiers = modifierGroup.ModifierGroupModifierMappings
                    .Select(mgm => new ModifierViewModel
                    {
                        ModifierId = mgm.Modifier.Modifierid,
                        Modifiername = mgm.Modifier.Modifiername,
                        ModifierType = (ModifierType?)mgm.Modifier.Modifiertype
                    }).ToList()
            };
        }



        public async Task<bool> UpdateModifierGroupAsync(ModifiergroupViewModel model)
        {
            bool exists = await _modifiergoupRepository.ModifierGroupNameExistsAsync(model.ModifierGroupName!, model.ModifierGroupId);
            if (exists)
            {
                throw new InvalidOperationException("Modifier Group Name already exists.");
            }

            var modifierGroup = await _modifiergoupRepository.GetByIdAsync(model.ModifierGroupId);

            if (modifierGroup == null)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(model.ModifierGroupName))
            {
                throw new ArgumentException("Modifier Group Name cannot be null or empty.");
            }
            modifierGroup.Modifiergroupname = model.ModifierGroupName;
            modifierGroup.Description = model.Description;

            await _modifiergoupRepository.UpdateModifierGroupMappingsAsync(modifierGroup.Modifiergroupid, model.ModifierIds ?? new List<int>());

            return true;
        }



        public async Task<bool> SoftDeleteModifierGroupAsync(int id)
        {
            return await _modifiergoupRepository.SoftDeleteAsync(id);
        }

        public Modifiergroup? GetModifierGroupById(int modifierGroupId)
        {
            return _modifiergoupRepository.GetModifierGroupById(modifierGroupId);
        }

        public List<Modifiergroup> GetModifierGroupsByIds(List<int> modifierGroupIds)
        {
            return _modifiergoupRepository.GetModifierGroupsByIds(modifierGroupIds);
        }

        public async Task UpdateModifierGroupOrderAsync(List<int> orderedModifierGroupIds)
        {
            var modifierGroups = _modifiergoupRepository.GetAll();

            foreach (var (modifierGroupId, index) in orderedModifierGroupIds.Select((id, idx) => (id, idx)))
            {
                var modifierGroup = modifierGroups.FirstOrDefault(mg => mg.Modifiergroupid == modifierGroupId);
                if (modifierGroup != null)
                {
                    modifierGroup.Sortorder = index + 1;
                }
            }

            await _modifiergoupRepository.SaveAsync();
        }
        public async Task<bool> ModifierGroupNameExistsAsync(string name)
        {
            return await _modifiergoupRepository.ModifierGroupNameExistsAsync(name);
        }


        public async Task<List<ItemModifierGroupMapViewModel>> GetMappingByItemIdSPAsync(int itemId)
        {
            var rawResults = await _modifiergoupRepository.GetMappingByItemIdSPAsync(itemId);

            var mapped = rawResults.Select(r => new ItemModifierGroupMapViewModel
            {
                ItemId = r.ItemId,
                ModifierGroupId = r.ModifierGroupId,
                ModifierGroupName = r.ModifierGroupName,
                MinValue = r.MinValue,
                MaxValue = r.MaxValue,
                ModifierItems = JsonSerializer.Deserialize<List<ModifierItemViewModel>>(r.ModifierItems) ?? new()
            }).ToList();

            return mapped;
        }


    }
}


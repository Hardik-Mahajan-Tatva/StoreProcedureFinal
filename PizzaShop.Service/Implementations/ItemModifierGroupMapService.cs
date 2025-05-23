using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class ItemModifierGroupMapService : IItemModifierGroupMapService
    {
        private readonly IItemModifierGroupMapRepository _itemModifierGroupMapRepository;

        public ItemModifierGroupMapService(IItemModifierGroupMapRepository itemModifierGroupMapRepository)
        {
            _itemModifierGroupMapRepository = itemModifierGroupMapRepository;
        }

        public async Task AddItemModifierGroupMap(ItemModifierGroupMapViewModel itemModifierGroupMapViewModel)
        {
            var itemModifierGroupMap = new Itemmodifiergroupmap
            {
                Itemid = itemModifierGroupMapViewModel.ItemId,
                Modifiergroupid = itemModifierGroupMapViewModel.ModifierGroupId,
                Minselectionrequired = (short?)itemModifierGroupMapViewModel.MinValue,
                Maxselectionallowed = (short?)itemModifierGroupMapViewModel.MaxValue
            };

            await _itemModifierGroupMapRepository.AddItemModifierGroupMap(itemModifierGroupMap);
        }

        public async Task<List<ItemModifierGroupMapViewModel>> GetMappingByItemIdAsync(int itemId)
        {
            var mappings = await _itemModifierGroupMapRepository.GetMappingByItemIdAsync(itemId);

            var mappedViewModels = mappings.Select(m => new ItemModifierGroupMapViewModel
            {
                ItemModifierGroupMapId = m.ItemModifierGroupMapId,
                ItemId = m.ItemId,
                ModifierGroupId = m.ModifierGroupId,
                ModifierGroupName = m.ModifierGroupName,
                MinValue = m.MinValue,
                MaxValue = m.MaxValue,
                ModifierItems = m.ModifierItems,
                ModifierType = m.ModifierType
            }).ToList();

            return mappedViewModels;
        }

        public async Task DeleteItemModifierGroupMapsByItemId(int itemId)
        {
            await _itemModifierGroupMapRepository.DeleteItemModifierGroupMapsByItemId(itemId);
        }



    }
}
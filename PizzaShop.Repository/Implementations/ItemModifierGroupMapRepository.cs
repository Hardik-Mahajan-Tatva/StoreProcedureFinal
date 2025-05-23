using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Repository.Implementations
{
    public class ItemModifierGroupMapRepository : IItemModifierGroupMapRepository
    {
        private readonly PizzaShopContext _dbContext;

        #region Constructor
        public ItemModifierGroupMapRepository(PizzaShopContext dbContext)
        {
            _dbContext = dbContext;
        }
        #endregion

        #region AddItemModifierGroupMap
        public async Task AddItemModifierGroupMap(Itemmodifiergroupmap itemModifierGroupMap)
        {
            _dbContext.Itemmodifiergroupmaps.Add(itemModifierGroupMap);
            await _dbContext.SaveChangesAsync();
        }
        #endregion

        #region GetMappingByItemIdAsync
        public async Task<List<ItemModifierGroupMapViewModel>> GetMappingByItemIdAsync(int itemId)
        {
            var mappings = await (
                from im in _dbContext.Itemmodifiergroupmaps
                join mg in _dbContext.Modifiergroups on im.Modifiergroupid equals mg.Modifiergroupid
                where im.Itemid == itemId
                select new ItemModifierGroupMapViewModel
                {
                    ItemId = im.Itemid,
                    ModifierGroupId = mg.Modifiergroupid,
                    ModifierGroupName = mg.Modifiergroupname,
                    MinValue = im.Minselectionrequired,
                    MaxValue = im.Maxselectionallowed,
                    ModifierItems = (
                        from mgm in _dbContext.ModifierGroupModifierMappings
                        join mod in _dbContext.Modifiers on mgm.ModifierId equals mod.Modifierid
                        where mgm.ModifierGroupId == mg.Modifiergroupid
                        select new ModifierItemViewModel
                        {
                            ModifierItemId = mod.Modifierid,
                            ModifierItemName = mod.Modifiername,
                            Price = mod.Rate,
                            ModifierType = (ModifierType?)mod.Modifiertype,
                        }
                    ).ToList()
                }
            ).ToListAsync();

            return mappings;
        }
        #endregion

        #region DeleteItemModifierGroupMapsByItemId
        public async Task DeleteItemModifierGroupMapsByItemId(int itemId)
        {
            var existingMappings = await _dbContext.Itemmodifiergroupmaps
                .Where(m => m.Itemid == itemId)
                .ToListAsync();

            if (existingMappings.Any())
            {
                _dbContext.Itemmodifiergroupmaps.RemoveRange(
                    (IEnumerable<Itemmodifiergroupmap>)existingMappings
                );
                await _dbContext.SaveChangesAsync();
            }
        }
        #endregion
    }
}

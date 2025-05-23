using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Repository.Interfaces
{
    public interface IItemModifierGroupMapRepository
    {
        /// <summary>
        /// Adds a new item-modifier group mapping to the database.
        /// </summary>
        /// <param name="itemModifierGroupMap">The item-modifier group mapping to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddItemModifierGroupMap(Itemmodifiergroupmap itemModifierGroupMap);

        /// <summary>
        /// Retrieves a list of item-modifier group mappings for a specific item.
        /// </summary>
        /// <param name="itemId">The ID of the item to retrieve mappings for.</param>
        /// <returns>A task that returns a list of item-modifier group mappings.</returns>
        Task<List<ItemModifierGroupMapViewModel>> GetMappingByItemIdAsync(int itemId);

        /// <summary>
        /// Deletes all item-modifier group mappings associated with a specific item.
        /// </summary>
        /// <param name="itemId">The ID of the item whose mappings should be deleted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteItemModifierGroupMapsByItemId(int itemId);
    }
}

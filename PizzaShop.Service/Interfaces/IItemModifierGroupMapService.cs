using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces
{
    public interface IItemModifierGroupMapService
    {
        /// <summary>
        /// Adds a new item-modifier group mapping asynchronously.
        /// </summary>
        /// <param name="itemModifierGroupMapViewModel">The view model containing the item-modifier group mapping details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddItemModifierGroupMap(ItemModifierGroupMapViewModel itemModifierGroupMapViewModel);

        /// <summary>
        /// Retrieves a list of item-modifier group mappings for a specific item asynchronously.
        /// </summary>
        /// <param name="itemId">The ID of the item to retrieve mappings for.</param>
        /// <returns>A task that returns a list of item-modifier group mappings.</returns>
        Task<List<ItemModifierGroupMapViewModel>> GetMappingByItemIdAsync(int itemId);

        /// <summary>
        /// Deletes all item-modifier group mappings associated with a specific item asynchronously.
        /// </summary>
        /// <param name="itemId">The ID of the item whose mappings should be deleted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteItemModifierGroupMapsByItemId(int itemId);
    }
}

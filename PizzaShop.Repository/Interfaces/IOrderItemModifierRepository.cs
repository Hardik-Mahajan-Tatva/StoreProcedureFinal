using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces
{
    public interface IOrderItemModifierRepository
    {
        /// <summary>
        /// Adds a mapping between an ordered item and a modifier if it does not already exist.
        /// </summary>
        /// <param name="orderItemModifier">The ordered item modifier to be added.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddOrderedItemModifierMappingIfNotExistsAsync(Ordereditemmodifer orderItemModifier);

        /// <summary>
        /// Retrieves all modifier mappings associated with a specific ordered item ID.
        /// </summary>
        /// <param name="orderedItemId">The ID of the ordered item.</param>
        /// <returns>A task that returns a list of ordered item modifiers.</returns>
        Task<List<Ordereditemmodifer>> GetModifierMappingsByOrderedItemIdAsync(int orderedItemId);

        /// <summary>
        /// Deletes all modifier mappings associated with a specific ordered item ID.
        /// </summary>
        /// <param name="orderedItemId">The ID of the ordered item.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAllModifierMappingsForOrderedItemIdAsync(int orderedItemId);
    }
}

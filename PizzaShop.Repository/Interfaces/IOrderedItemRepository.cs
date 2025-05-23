using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Repository.Interfaces
{
    public interface IOrderedItemRepository
    {
        /// <summary>
        /// Updates the ready quantity of an ordered item asynchronously.
        /// </summary>
        /// <param name="orderedItemId">The ID of the ordered item to update.</param>
        /// <param name="readyQuantity">The new ready quantity to set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        // Task UpdateReadyQuantityAsync(int orderedItemId, int readyQuantity);

        /// <summary>
        /// Updates the quantity of an ordered item asynchronously.
        /// </summary>
        /// <param name="orderedItemId">The ID of the ordered item to update.</param>
        /// <param name="Quantity">The new quantity to set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        // Task UpdateQuantityAsync(int orderedItemId, int Quantity);

        /// <summary>
        /// Updates the status of an order asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order to update.</param>
        /// <param name="status">The new status to set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateOrderStatusAsync(int orderId, int status);

        /// <summary>
        /// Adds a new ordered item to the repository.
        /// </summary>
        /// <param name="orderItem">The ordered item to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Add(Ordereditem orderItem);

        /// <summary>
        /// Retrieves an ordered item by order ID, item ID, and modifiers.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="modifiers">The list of modifiers for the item.</param>
        /// <returns>The ordered item if found, otherwise null.</returns>
        Task<Ordereditem?> GetByOrderIdAndItemIdAsync(
            int orderId,
            int itemId,
            List<OrderItemModifierRequestModel> modifiers
        );

        /// <summary>
        /// Updates an existing ordered item asynchronously.
        /// </summary>
        /// <param name="item">The ordered item with updated information.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAsync(Ordereditem item);

        /// <summary>
        /// Retrieves all ordered items associated with a specific order ID.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <returns>A collection of ordered items associated with the order.</returns>
        Task<IEnumerable<Ordereditem>> GetItemsByOrderIdAsync(int orderId);

        /// <summary>
        /// Deletes an ordered item asynchronously by its ID.
        /// </summary>
        /// <param name="orderItemId">The ID of the ordered item to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(int orderItemId);

        /// <summary>
        /// Retrieves an ordered item by its ID.
        /// </summary>
        /// <param name="orderedItemId">The ID of the ordered item.</param>
        /// <returns>The ordered item if found, otherwise null.</returns>
        Task<Ordereditem?> GetByIdAsync(int orderedItemId);

        /// <summary>
        /// Checks if an item is available to delete based on its ID.
        /// </summary>
        /// <param name="itemId">The ID of the item to check.</param>
        /// <returns>True if the item is available to delete, otherwise false.</returns>
        Task<bool> IsItemAvailableToDeleteAsync(int itemId);

        /// <summary>
        /// Retrieves the top-selling items within a specified date range.
        /// </summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <param name="count">The number of top-selling items to retrieve.</param>
        /// <returns>A list of top-selling items.</returns>
        Task<List<ItemSale>> GetTopSellingItemsAsync(
            DateTime? startDate,
            DateTime? endDate,
            int count
        );

        /// <summary>
        /// Retrieves the least-selling items within a specified date range.
        /// </summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <param name="count">The number of least-selling items to retrieve.</param>
        /// <returns>A list of least-selling items.</returns>
        Task<List<ItemSale>> GetLeastSellingItemsAsync(
            DateTime? startDate,
            DateTime? endDate,
            int count
        );

        Task UpdateReadyQuantitiesAsync(List<ReadyQuantityUpdateViewModel> updates);

        Task UpdateQuantitiesAsync(List<ReadyQuantityUpdateViewModel> updates);
    }
}

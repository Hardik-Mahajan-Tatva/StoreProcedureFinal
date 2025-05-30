using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Repository.Interfaces
{
    public interface IOrderRepository
    {
        /// <summary>
        /// Retrieves a paginated list of orders based on search criteria, status, date range, and sorting options.
        /// </summary>
        /// <param name="search">The search term to filter orders.</param>
        /// <param name="status">The status of the orders to filter.</param>
        /// <param name="startDate">The start date for filtering orders by creation date.</param>
        /// <param name="endDate">The end date for filtering orders by creation date.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of orders per page.</param>
        /// <param name="sortColumn">The column to sort the results by.</param>
        /// <param name="sortDirection">The direction of sorting (e.g., ascending or descending).</param>
        /// <returns>A task that returns a paginated list of orders.</returns>
        Task<PaginatedList<Order>> GetOrdersAsync(
            string search,
            string status,
            DateTime? startDate,
            DateTime? endDate,
            int page,
            int pageSize,
            string sortColumn,
            string sortDirection
        );

        /// <summary>
        /// Retrieves all orders as an IQueryable for further filtering or querying.
        /// </summary>
        /// <returns>An IQueryable of all orders.</returns>
        IQueryable<Order> GetAll();

        /// <summary>
        /// Retrieves an order along with its details by order ID asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve.</param>
        /// <returns>A task that returns the order with its details if found, otherwise null.</returns>
        Task<Order?> GetOrderWithDetailsAsync(int orderId);

        /// <summary>
        /// Retrieves a list of order items for a specific order asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve items for.</param>
        /// <returns>A task that returns a list of order items.</returns>
        Task<List<OrderItemViewModel>> GetOrderItemsAsync(int orderId);

        /// <summary>
        /// Retrieves a list of ordered items for a specific order asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve items for.</param>
        /// <returns>A task that returns a list of ordered items.</returns>
        Task<List<Ordereditem>> GetOrderedItemsAsync(int orderId);

        /// <summary>
        /// Retrieves a list of ordered item modifiers for a specific order asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve modifiers for.</param>
        /// <returns>A task that returns a list of ordered item modifiers.</returns>
        Task<List<Ordereditemmodifer>> GetOrderedItemModifiersAsync(int orderId);

        /// <summary>
        /// Retrieves a list of items by their IDs asynchronously.
        /// </summary>
        /// <param name="itemIds">The list of item IDs to retrieve.</param>
        /// <returns>A task that returns a list of items.</returns>
        Task<List<Item>> GetItemsByIdsAsync(List<int> itemIds);

        /// <summary>
        /// Retrieves a list of modifiers by their IDs asynchronously.
        /// </summary>
        /// <param name="modifierIds">The list of modifier IDs to retrieve.</param>
        /// <returns>A task that returns a list of modifiers.</returns>
        Task<List<Modifier>> GetModifiersByIdsAsync(List<int> modifierIds);

        /// <summary>
        /// Creates a new order asynchronously.
        /// </summary>
        /// <param name="order">The order to create.</param>
        /// <returns>A task that returns the ID of the newly created order.</returns>
        Task<int> CreateOrderAsync(Order order);

        /// <summary>
        /// Retrieves a paginated list of KOT (Kitchen Order Ticket) orders based on item status and category name.
        /// </summary>
        /// <param name="itemStatus">The status of the items to filter.</param>
        /// <param name="categoryName">The name of the category to filter.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of orders per page.</param>
        /// <returns>A task that returns a tuple containing the list of orders and the total count.</returns>
        Task<(List<Order>, int)> GetKOTOrdersAsync(
            string itemStatus,
            string categoryName,
            int page,
            int pageSize
        );

        /// <summary>
        /// Retrieves a list of all orders along with their details asynchronously.
        /// </summary>
        /// <returns>A task that returns a list of orders with their details.</returns>
        Task<List<Order>> GetOrdersWithDetailsAsync();

        /// <summary>
        /// Retrieves a dictionary of items with their details (name, rate, and category ID).
        /// </summary>
        /// <returns>A task that returns a dictionary where the key is the item ID and the value is a tuple containing the name, rate, and category ID.</returns>
        Task<Dictionary<int, (string Name, decimal Rate, int CategoryId)>> GetItemDictionaryAsync();

        /// <summary>
        /// Retrieves a dictionary of modifiers with their details (name and rate).
        /// </summary>
        /// <returns>A task that returns a dictionary where the key is the modifier ID and the value is a tuple containing the name and rate.</returns>
        Task<Dictionary<int, (string Name, decimal Rate)>> GetModifierDictionaryAsync();

        /// <summary>
        /// Retrieves an order along with its details by order ID asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve.</param>
        /// <returns>The order with its details if found, otherwise null.</returns>
        Task<Order?> GetOrderWithDetails(int? orderId);

        /// <summary>
        /// Saves a comment for a specific order asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order to save the comment for.</param>
        /// <param name="comment">The comment to save.</param>
        /// <returns>True if the comment was saved successfully, otherwise false.</returns>
        Task<bool> SaveOrderComment(int orderId, string comment);

        /// <summary>
        /// Retrieves an order by its ID asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve.</param>
        /// <returns>The order if found, otherwise null.</returns>
        Task<Order> GetOrderById(int orderId);

        /// <summary>
        /// Retrieves the special instruction for a specific ordered item in an order asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="orderedItemId">The ID of the ordered item.</param>
        /// <returns>The special instruction if found, otherwise null.</returns>
        Task<string> GetSpecialInstruction(int orderId, int orderedItemId);

        /// <summary>
        /// Saves a special instruction for a specific ordered item in an order asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="orderedItemId">The ID of the ordered item.</param>
        /// <param name="instruction">The special instruction to save.</param>
        /// <returns>True if the instruction was saved successfully, otherwise false.</returns>
        Task<bool> SaveSpecialInstruction(int orderId, int orderedItemId, string instruction);

        /// <summary>
        /// Retrieves a list of order items for a specific order and modifier asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="modifierId">The ID of the modifier.</param>
        /// <returns>A task that returns a list of order items.</returns>
        Task<List<OrderItemViewModel>> GetOrderItemsAsync(int orderId, int modifierId);

        /// <summary>
        /// Retrieves an order by its ID asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve.</param>
        /// <returns>The order if found, otherwise null.</returns>
        Task<Order> GetByIdAsync(int orderId);

        /// <summary>
        /// Updates an existing order asynchronously.
        /// </summary>
        /// <param name="order">The order with updated information.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAsync(Order order);

        /// <summary>
        /// Retrieves an order along with its table details by order ID asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve.</param>
        /// <returns>The order with its table details if found, otherwise null.</returns>
        Task<Order?> GetOrderWithTableAsync(int orderId);

        /// <summary>
        /// Saves changes to the repository asynchronously.
        /// </summary>
        /// <returns>True if the changes were saved successfully, otherwise false.</returns>
        Task<bool> SaveChangesAsync();

        /// <summary>
        /// Retrieves the total sales within a specified time range asynchronously.
        /// </summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <returns>The total sales within the time range.</returns>
        Task<decimal> GetTotalSalesByTimeRangeAsync(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Retrieves the total number of orders within a specified time range asynchronously.
        /// </summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <returns>The total number of orders within the time range.</returns>
        Task<int> GetTotalOrdersByTimeRangeAsync(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Retrieves the average order value within a specified time range asynchronously.
        /// </summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <returns>The average order value within the time range.</returns>
        Task<decimal> GetAvgOrderValueByTimeRangeAsync(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Retrieves the total sales for a specific day asynchronously.
        /// </summary>
        /// <param name="date">The date to retrieve sales for.</param>
        /// <returns>The total sales for the specified day.</returns>
        Task<decimal> GetTotalSalesBySpecificDayAsync(DateTime date);

        /// <summary>
        /// Retrieves the latest order for a specific customer by customer ID asynchronously.
        /// </summary>
        /// <param name="customerId">The ID of the customer to retrieve the latest order for.</param>
        /// <returns>The latest order if found, otherwise null.</returns>
        Task<Order?> GetLatestOrderByCustomerIdAsync(int customerId);

        /// <summary>
        /// Retrieves the total sales grouped by day within a specified date range asynchronously.
        /// </summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <returns>A list of chart data points representing the total sales grouped by day.</returns>
        Task<List<ChartDataPoint>> GetTotalSalesGroupedByDayAsync(
            DateTime startDate,
            DateTime endDate
        );

        /// <summary>
        /// Updates multiple orders asynchronously.
        /// </summary>
        /// <param name="orders">The list of orders to update.</param>
        /// <returns>True if the orders were updated successfully, otherwise false.</returns>
        Task<bool> UpdateOrdersAsync(IEnumerable<Order> orders);

        /// <summary>
        /// Checks if a customer has any orders with specific statuses asynchronously.
        /// </summary>
        /// <param name="customerId">The ID of the customer to check.</param>
        /// <param name="statuses">The array of statuses to check for.</param>
        /// <returns>True if the customer has orders with the specified statuses, otherwise false.</returns>
        Task<bool> HasOrderWithStatusAsync(int customerId, int[] statuses);

        Task<List<KOTFlatData>> GetKOTDataFromProcedureAsync(int pageNumber, int pageSize, int categoryId, string orderStatus, string itemStatus);
        Task<Order> GetOrderByIdSP(int orderId);
        Task<bool> SaveOrderCommentSP(int orderId, string comment);
        Task<string> GetSpecialInstructionSP(int orderId, int orderedItemId);
        Task<bool> SaveSpecialInstructionSP(
            int orderId,
            int orderedItemId,
            string instruction
        );
        Task<bool> CancelOrderByStoredProcAsyncSP(int orderId);
    }
}

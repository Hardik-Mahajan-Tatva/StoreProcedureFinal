using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Repository.Interfaces
{
    public interface ICustomerRepository
    {
        /// <summary>
        /// Retrieves a paginated list of customers based on search criteria, status, date range, and sorting options.
        /// </summary>
        /// <param name="search">The search term to filter customers.</param>
        /// <param name="status">The status of the customers to filter.</param>
        /// <param name="startDate">The start date for filtering customers by creation date.</param>
        /// <param name="endDate">The end date for filtering customers by creation date.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of customers per page.</param>
        /// <param name="sortColumn">The column to sort the results by.</param>
        /// <param name="sortDirection">The direction of sorting (e.g., ascending or descending).</param>
        /// <returns>A task that returns a paginated list of customers.</returns>
        Task<PaginatedList<Customer>> GetPaginatedCustomersAsync(
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
        /// Retrieves all customers as list for further filtering or querying.
        /// </summary>
        /// <returns>A list of all customers.</returns>
        Task<List<Customer>> GetAllCustomersAsync();
        IQueryable<Customer> GetAllCustomersQueryable();

        /// <summary>
        /// Retrieves a customer along with their associated orders by customer ID.
        /// </summary>
        /// <param name="customerId">The ID of the customer to retrieve.</param>
        /// <returns>The customer with their orders if found, otherwise null.</returns>
        Task<Customer?> GetCustomerWithOrdersAsync(int customerId);

        /// <summary>
        /// Retrieves a customer along with their latest order by customer ID.
        /// </summary>
        /// <param name="customerId">The ID of the customer to retrieve.</param>
        /// <returns>The customer with their latest order if found, otherwise null.</returns>
        Task<Customer?> GetCustomerWithLatestOrderAsync(int customerId);

        /// <summary>
        /// Retrieves a customer by their customer ID.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <returns>The customer if found, otherwise null.</returns>
        Task<Customer?> GetCustomerByCustomerIdAsync(int customerId);

        /// <summary>
        /// Checks if a customer exists by their name, excluding a specific category ID.
        /// </summary>
        /// <param name="customerName">The name of the customer to check.</param>
        /// <param name="excludeCategoryId">The category ID to exclude from the check.</param>
        /// <returns>True if the customer exists, otherwise false.</returns>
        Task<bool> DoesCustomerExistAsync(string customerName, int excludeCategoryId);

        /// <summary>
        /// Retrieves all orders associated with a specific customer ID.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <returns>A list of orders associated with the customer.</returns>
        Task<List<Order>> GetOrdersByCustomerIdAsync(int customerId);

        /// <summary>
        /// Retrieves a customer by their name, phone, and email.
        /// </summary>
        /// <param name="Customeremail">The email of the customer.</param>
        /// <returns>The customer if found, otherwise null.</returns>
        Task<Customer?> GetCustomerByNamePhoneAndEmailAsync(string Customeremail);

        /// <summary>
        /// Updates an existing customer's information.
        /// </summary>
        /// <param name="customer">The customer object with updated information.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateCustomerAsync(Customer customer);

        /// <summary>
        /// Adds a new customer to the database.
        /// </summary>
        /// <param name="customer">The customer to add.</param>
        /// <returns>The ID of the newly added customer.</returns>
        Task<int> AddCustomerAsync(Customer customer);

        /// <summary>
        /// Updates customer data asynchronously.
        /// </summary>
        /// <param name="customer">The customer object with updated data.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        Task<bool> UpdateCustomerDataAsync(Customer customer);

        /// <summary>
        /// Retrieves the count of new customers within a specified time range.
        /// </summary>
        /// <param name="startDate">The start date of the time range.</param>
        /// <param name="endDate">The end date of the time range.</param>
        /// <returns>The count of new customers within the time range.</returns>
        Task<int> GetNewCustomerCountByTimeRange(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Retrieves a customer by their email address.
        /// </summary>
        /// <param name="customerEmail">The email address of the customer.</param>
        /// <returns>The customer if found, otherwise null.</returns>
        Task<Customer?> GetCustomerByEmailAsync(string customerEmail);
        Task<(bool Success, string Message, int OrderId)> AssignCustomerToOrderSPAsync(
             string customerName,
             string email,
             string mobileNumber,
             int noOfPersons,
             int[] tableIds);


    }
}

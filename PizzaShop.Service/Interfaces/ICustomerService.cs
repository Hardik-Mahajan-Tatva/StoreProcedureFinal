using Microsoft.AspNetCore.Mvc;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces
{
    public interface ICustomerService
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
        /// <returns>A task that returns a paginated list of customer view models.</returns>
        Task<PaginatedList<CustomerViewModel>> GetCustomerAsync(
            string search,
            string status,
            DateTime? startDate,
            DateTime? endDate,
            int page,
            int pageSize,
            string sortColumn,
            string sortDirection);

        /// <summary>
        /// Retrieves a filtered list of customers based on search text, date range, order status, and sorting options.
        /// </summary>
        /// <param name="searchText">The search term to filter customers.</param>
        /// <param name="startDate">The start date for filtering customers by creation date.</param>
        /// <param name="endDate">The end date for filtering customers by creation date.</param>
        /// <param name="orderStatus">The status of the orders to filter.</param>
        /// <param name="sortColumn">The column to sort the results by.</param>
        /// <param name="sortOrder">The direction of sorting (e.g., ascending or descending).</param>
        /// <returns>A collection of filtered customers.</returns>
        Task<IEnumerable<Customer>> GetFilteredOrders(
            string searchText,
            DateTime? startDate,
            DateTime? endDate,
            int? orderStatus,
            string sortColumn,
            string sortOrder);

        /// <summary>
        /// Retrieves the history of a customer by their ID.
        /// </summary>
        /// <param name="customerId">The ID of the customer to retrieve the history for.</param>
        /// <returns>A view model containing the customer's history.</returns>
        Task<CustomerHistoryViewModel> GetCustomerHistory(int customerId);

        /// <summary>
        /// Creates a new customer asynchronously.
        /// </summary>
        /// <param name="customer">The customer view model to create.</param>
        /// <returns>A task that returns the ID of the newly created customer.</returns>
        Task<int> CreateCustomerAsync(CustomerViewModel customer);
        Task<CustomerUpdateViewModal?> GetCustomerDetails(int customerId);
        Task<bool> UpdateAsync(CustomerUpdateViewModal customer);
        // ICustomerService.cs
        Task<bool> UpdateCustomerAsync(CustomerViewModel customer);

        /// <summary>
        /// Retrieves a customer by their ID asynchronously.
        /// </summary>
        /// <param name="customerId">The ID of the customer to retrieve.</param>
        /// <returns>A task that returns the customer view model, or null if not found.</returns>
        Task<CustomerViewModel?> GetCustomerByIdAsync(int customerId);

        /// <summary>
        /// Retrieves the customer with their latest order asynchronously based on their email.
        /// </summary>
        /// <param name="email">The email of the customer to retrieve.</param>
        /// <returns>A task that returns the waiting token view model, or null if not found.</returns>
        Task<WaitingTokenViewModel?> GetCustomerWithLatestOrderAsync(string email);

        /// <summary>
        /// Checks if a customer has an order with any of the specified statuses.
        /// </summary>
        /// <param name="email">The email of the customer to check.</param>
        /// <param name="statuses">An array of order statuses to check against.</param>
        /// <returns>A task that returns true if the customer has an order with the specified statuses, otherwise false.</returns>
        Task<bool> HasCustomerOrderWithStatusAsync(string email, int[] statuses);

        /// <summary>
        /// Exports customer data to an Excel file based on search criteria, date range, order status, and sorting options.
        /// </summary>
        /// <param name="searchText">The search term to filter customers.</param>
        /// <param name="startDate">The start date for filtering customers by creation date.</param>
        /// <param name="endDate">The end date for filtering customers by creation date.</param>
        /// <param name="orderStatus">The status of the orders to filter.</param>
        /// <param name="sortColumn">The column to sort the results by.</param>
        /// <param name="sortOrder">The direction of sorting (e.g., ascending or descending).</param>
        /// <param name="webRootPath">The root path of the web application for file storage.</param>
        /// <returns>A task that returns a file result containing the exported Excel file.</returns>
        Task<FileResult> ExportCustomersToExcel(string searchText, DateTime? startDate, DateTime? endDate, int? orderStatus, string sortColumn, string sortOrder, string webRootPath);
    }
}


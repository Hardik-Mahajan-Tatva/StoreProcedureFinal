using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces
{
    public interface IOrderTaxRepository
    {
        /// <summary>
        /// Adds a tax mapping to an order asynchronously.
        /// </summary>
        /// <param name="orderTax">The tax mapping to be added to the order.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddTaxMappingToOrderAsync(Ordertaxmapping orderTax);

        /// <summary>
        /// Deletes all tax mappings associated with a specific order ID asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order whose tax mappings are to be deleted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteTaxMappingsByOrderIdAsync(int orderId);

        /// <summary>
        /// Retrieves all tax mappings associated with a specific order ID asynchronously.
        /// </summary>
        /// <param name="orderId">The ID of the order whose tax mappings are to be retrieved.</param>
        /// <returns>A task that returns a list of tax mappings for the specified order.</returns>
        Task<List<Ordertaxmapping>> GetTaxMappingsByOrderIdAsync(int orderId);
    }
}

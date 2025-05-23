using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces
{
    public interface IOrderTableMappingRepository
    {
        /// <summary>
        /// Creates a new mapping between an order and a table asynchronously.
        /// </summary>
        /// <param name="mapping">The order-table mapping to create.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CreateCustomerOrderToTableMappingAsync(Ordertable mapping);
    }
}

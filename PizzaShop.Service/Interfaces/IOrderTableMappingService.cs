using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces
{
    public interface IOrderTableMappingService
    {
        /// <summary>
        /// Creates a new mapping between an order and a table asynchronously.
        /// </summary>
        /// <param name="mapping">The view model containing the order-table mapping details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CreateMappingAsync(OrderTableMappingViewModel mapping);
    }
}

using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces
{
    public interface IAssignService
    {
        /// <summary>
        /// Assigns a customer order and creates the necessary mappings asynchronously.
        /// </summary>
        /// <param name="model">The customer order view model containing order and mapping details.</param>
        /// <returns>A task that returns a tuple indicating success and a message.</returns>
        Task<(bool IsSuccess, string Message, int OrderId)> AssignCustomerOrderAndMappingWithOrderIdAsync(CustomerOrderViewModel model);
        Task<(bool IsSuccess, string Message, int orderId)> AssignWaitinCustomerSelectedTableAndSectionAsync(AssignTableViewModel model);
    }
}


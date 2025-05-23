using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces
{
    public interface ICustomerReviewService
    {
        /// <summary>
        /// Adds a customer review asynchronously.
        /// </summary>
        /// <param name="model">The customer review model containing review details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddReviewAsync(CustomerReviewViewModel model);
    }
}
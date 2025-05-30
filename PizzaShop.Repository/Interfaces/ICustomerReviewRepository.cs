using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces
{
    public interface ICustomerReviewRepository
    {
        /// <summary>
        /// Adds a new customer review to the repository asynchronously.
        /// </summary>
        /// <param name="customerReview">The customer review to be added.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddCustomerReviewAsync(Customerreview customerReview);
        Task AddCustomerReviewAsyncSP(Customerreview customerReview);   
    }
}

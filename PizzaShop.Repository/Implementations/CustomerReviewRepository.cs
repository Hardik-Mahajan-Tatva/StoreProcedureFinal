using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations
{
    public class CustomerReviewRepository : ICustomerReviewRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public CustomerReviewRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region AddCustomerReviewAsync
        public async Task AddCustomerReviewAsync(Customerreview customerReview)
        {
            await _context.Customerreviews.AddAsync(customerReview);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}

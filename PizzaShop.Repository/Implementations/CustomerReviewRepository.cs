using Microsoft.EntityFrameworkCore;
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
        #region AddCustomerReviewAsyncSP
        public async Task AddCustomerReviewAsyncSP(Customerreview customerReview)
        {
            var avgRating = Math.Round(
        ((customerReview.Foodrating ?? 0) + (customerReview.Servicerating ?? 0) + (customerReview.Ambiencerating ?? 0)) / 3.0m,
        2
    );

            await _context.Database.ExecuteSqlRawAsync(
                "CALL add_customer_review({0}, {1}, {2}, {3}, {4}, {5})",
                customerReview.Orderid,
                customerReview.Foodrating ?? 0,
                customerReview.Servicerating ?? 0,
                customerReview.Ambiencerating ?? 0,
                avgRating,
                customerReview.Comments ?? ""
            );
        }
        #endregion
    }
}

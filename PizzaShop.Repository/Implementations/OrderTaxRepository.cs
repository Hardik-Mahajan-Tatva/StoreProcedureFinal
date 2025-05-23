using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations
{
    public class OrderTaxRepository : IOrderTaxRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public OrderTaxRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region AddTaxMappingToOrderAsync
        public async Task AddTaxMappingToOrderAsync(Ordertaxmapping ordertaxmapping)
        {
            await _context.Ordertaxmappings.AddAsync(ordertaxmapping);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region DeleteTaxMappingsByOrderIdAsync
        public async Task DeleteTaxMappingsByOrderIdAsync(int orderId)
        {
            var taxes = await _context.Ordertaxmappings
                .Where(t => t.Orderid == orderId)
                .ToListAsync();

            _context.Ordertaxmappings.RemoveRange(taxes);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region GetTaxMappingsByOrderIdAsync
        public async Task<List<Ordertaxmapping>> GetTaxMappingsByOrderIdAsync(int orderId)
        {
            return await _context.Ordertaxmappings
                .Where(tm => tm.Orderid == orderId)
                .Include(tm => tm.Tax)
                .ToListAsync();
        }
        #endregion

    }
}

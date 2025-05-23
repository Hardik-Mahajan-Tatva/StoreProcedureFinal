using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations
{
    public class OrderTableMappingRepository : IOrderTableMappingRepository
    {
        private readonly PizzaShopContext _context;
        #region Constructor
        public OrderTableMappingRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region CreateCustomerOrderToTableMappingAsync
        public async Task CreateCustomerOrderToTableMappingAsync(Ordertable mapping)
        {
            await _context.Ordertables.AddAsync(mapping);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}

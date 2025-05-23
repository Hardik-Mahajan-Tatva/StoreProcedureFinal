using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations
{
    public class UnitRepository : IUnitRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public UnitRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region GetUnitsAsync
        public async Task<IEnumerable<Unit>> GetUnitsAsync()
        {
            return await _context.Units.ToListAsync();
        }
        #endregion
    }
}

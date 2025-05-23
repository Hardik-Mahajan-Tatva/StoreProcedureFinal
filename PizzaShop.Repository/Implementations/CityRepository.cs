using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace PizzaShop.Repository.Implementations
{
    public class CityRepository : ICityRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public CityRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region GetCitiesByStateIdAsyce
        public async Task<List<City>> GetCitiesByStateIdAsyce(int stateId)
        {
            return await _context.Cities
                .Where(c => c.Stateid == stateId)
                .AsNoTracking()
                .ToListAsync();
        }
        #endregion
    }
}

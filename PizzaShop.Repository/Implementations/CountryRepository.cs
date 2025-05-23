using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations
{
    public class CountryRepository : ICountryRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public CountryRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region GetAllCountriesAsync
        public async Task<List<Country>> GetAllCountriesAsync()
        {
            return await _context.Countries.AsNoTracking().ToListAsync();
        }
        #endregion
    }
}

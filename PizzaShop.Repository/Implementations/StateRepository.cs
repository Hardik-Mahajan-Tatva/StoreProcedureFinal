using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace PizzaShop.Repository.Implementations
{
    public class StateRepository : IStateRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public StateRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region GetStatesByCountryId
        public async Task<IEnumerable<State>> GetStatesByCountryId(int countryId)
        {
            return await _context.States.Where(s => s.Countryid == countryId).ToListAsync();
        }
        #endregion
    }
}

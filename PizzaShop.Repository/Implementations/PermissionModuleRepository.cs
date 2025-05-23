using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations
{
    public class PermissionModuleRepository : IPermissionModuleRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor

        public PermissionModuleRepository(PizzaShopContext context)
        {
            _context = context;
        }

        #endregion

        #region GetAllPermissionModuleAsync
        public async Task<List<Permissionmodule>> GetAllPermissionModuleAsync()
        {
            return await _context.Permissionmodules.ToListAsync();
        }
        #endregion
    }
}


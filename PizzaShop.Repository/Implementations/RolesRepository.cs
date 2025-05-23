using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations
{
    public class RolesRepository : IRolesRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public RolesRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region GetAllRoleAsync
        public async Task<List<Role>> GetAllRoleAsync()
        {
            return await _context.Roles.ToListAsync();
        }
        #endregion

        #region GetRoleByIdAsync
        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Roleid == id);
        }
        #endregion

        #region GetRoleNameByIdAsync
        public async Task<string?> GetRoleNameByIdAsync(int id)
        {
            var roleName = await _context.Roles
                .Where(r => r.Roleid == id)
                .Select(r => r.Rolename)
                .FirstOrDefaultAsync();

            return roleName;
        }
        #endregion
    }
}


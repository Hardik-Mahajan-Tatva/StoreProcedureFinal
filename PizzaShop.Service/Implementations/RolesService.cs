using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class RolesService : IRolesService
    {
        private readonly IRolesRepository _rolesRepository;
        public RolesService(IRolesRepository rolesRepository)
        {
            _rolesRepository = rolesRepository;
        }

        public async Task<List<Role>> GetAllRoles()
        {
            return await _rolesRepository.GetAllRoleAsync();
        }

        public async Task<Role?> GetRoleById(int id)
        {
            return await _rolesRepository.GetRoleByIdAsync(id);
        }

        public Task<string?> GetRoleNameById(int id)
        {
            var rolename = _rolesRepository.GetRoleNameByIdAsync(id);
            return rolename;
        }
    }
}





using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class PermissionModuleService : IPermissionModuleService
    {
        private readonly IPermissionModuleRepository _permissionModuleRepository;
        public PermissionModuleService(IPermissionModuleRepository permissionModuleRepository)
        {
            _permissionModuleRepository = permissionModuleRepository;
        }

        public async Task<List<Permissionmodule>> GetAllPermissionModule()
        {
            return await _permissionModuleRepository.GetAllPermissionModuleAsync();
        }
    }
}

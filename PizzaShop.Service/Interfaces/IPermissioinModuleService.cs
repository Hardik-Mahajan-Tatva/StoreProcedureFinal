using PizzaShop.Repository.Models;

namespace PizzaShop.Service.Interfaces
{
    public interface IPermissionModuleService
    {
        /// <summary>
        /// Retrieves a list of all permission modules.
        /// </summary>
        /// <returns>A task that returns a list of all permission modules.</returns>
        Task<List<Permissionmodule>> GetAllPermissionModule();
    }
}

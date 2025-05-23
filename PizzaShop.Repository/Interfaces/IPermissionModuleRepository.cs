using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces
{
    public interface IPermissionModuleRepository
    {
        /// <summary>
        /// Retrieves a list of all permission modules asynchronously.
        /// </summary>
        /// <returns>A task that returns a list of all permission modules.</returns>
        Task<List<Permissionmodule>> GetAllPermissionModuleAsync();
    }
}



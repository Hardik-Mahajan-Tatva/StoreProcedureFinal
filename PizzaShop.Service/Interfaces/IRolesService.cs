using PizzaShop.Repository.Models;

namespace PizzaShop.Service.Interfaces
{
    public interface IRolesService
    {
        /// <summary>
        /// Retrieves a list of all roles asynchronously.
        /// </summary>
        /// <returns>A task that returns a list of all roles.</returns>
        Task<List<Role>> GetAllRoles();

        /// <summary>
        /// Retrieves a role by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the role to retrieve.</param>
        /// <returns>A task that returns the role if found, otherwise null.</returns>
        Task<Role?> GetRoleById(int id);

        /// <summary>
        /// Retrieves the name of a role by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the role to retrieve the name for.</param>
        /// <returns>A task that returns the name of the role if found, otherwise null.</returns>
        Task<string?> GetRoleNameById(int id);
    }
}

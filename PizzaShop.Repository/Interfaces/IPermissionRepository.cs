using pizzashop.Repository.ViewModels;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces
{
    public interface IPermissionRepository
    {
        /// <summary>
        /// Retrieves a list of permissions associated with a specific role ID.
        /// </summary>
        /// <param name="roleId">The ID of the role to retrieve permissions for.</param>
        /// <returns>A task that returns a list of permissions.</returns>
        Task<List<Permission>> GetPermissionsByRoleId(int roleId);

        /// <summary>
        /// Retrieves a specific permission by role ID and module ID.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <param name="moduleId">The ID of the module.</param>
        /// <returns>A task that returns the permission if found, otherwise null.</returns>
        Task<Permission?> GetPermissionByRoleAndModule(int roleId, int moduleId);

        /// <summary>
        /// Adds a new permission to the database.
        /// </summary>
        /// <param name="permission">The permission to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddPermission(Permission permission);

        /// <summary>
        /// Updates an existing permission in the database.
        /// </summary>
        /// <param name="permission">The permission to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdatePermission(Permission permission);

        /// <summary>
        /// Retrieves all permissions associated with a specific role ID asynchronously.
        /// </summary>
        /// <param name="roleid">The ID of the role to retrieve permissions for.</param>
        /// <returns>A task that returns a list of permissions.</returns>
        Task<List<Permissions>> GetAllPermissionsByIdAsync(int roleid);

        /// <summary>
        /// Updates all permissions for a specific role asynchronously.
        /// </summary>
        /// <param name="roleNPermission">The role and permission data to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAllPermissionsAsync(RoleNPermission roleNPermission);

        /// <summary>
        /// Retrieves all permissions associated with a specific role ID as a list asynchronously.
        /// </summary>
        /// <param name="roleid">The ID of the role to retrieve permissions for.</param>
        /// <returns>A task that returns a list of permissions.</returns>
        Task<List<Permission>> GetAllPermissionsByIdAsyncList(int roleid);

        /// <summary>
        /// Checks if a role has a specific permission for a given module.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <param name="permissionName">The name of the permission.</param>
        /// <param name="moduleId">The ID of the module.</param>
        /// <returns>A task that returns true if the role has the permission, otherwise false.</returns>
        Task<bool> RoleHasPermissionAsync(string roleName, string permissionName, int moduleId);
    }
}


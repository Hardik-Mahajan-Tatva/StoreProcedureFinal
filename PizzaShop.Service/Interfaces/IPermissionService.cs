using pizzashop.Repository.ViewModels;
using PizzaShop.Repository.Models;

namespace PizzaShop.Service.Interfaces
{
    public interface IPermissionService
    {
        /// <summary>
        /// Retrieves a list of permissions associated with a specific role ID.
        /// </summary>
        /// <param name="roleId">The ID of the role to retrieve permissions for.</param>
        /// <returns>A task that returns a list of permissions.</returns>
        Task<List<Permission>> GetPermissionsByRoleId(int roleId);

        /// <summary>
        /// Saves a list of permissions for a specific role.
        /// </summary>
        /// <param name="roleId">The ID of the role to save permissions for.</param>
        /// <param name="permissions">The list of permissions to save.</param>
        /// <returns>A task that returns true if the save operation was successful, otherwise false.</returns>
        Task<bool> SavePermissions(int roleId, List<Permission> permissions);

        /// <summary>
        /// Updates all permissions for a specific role asynchronously.
        /// </summary>
        /// <param name="roleNPermission">The role and permission data to update.</param>
        /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
        Task<bool> UpdateAllPermissionsAsync(RoleNPermission roleNPermission);

        /// <summary>
        /// Retrieves all permissions for a specific role ID asynchronously.
        /// </summary>
        /// <param name="roleid">The ID of the role to retrieve permissions for.</param>
        /// <returns>A task that returns the role and its permissions.</returns>
        Task<RoleNPermission> GetAllPermissionsAsync(int roleid);

        /// <summary>
        /// Retrieves all permissions for a specific role ID as a list asynchronously.
        /// </summary>
        /// <param name="roleid">The ID of the role to retrieve permissions for.</param>
        /// <returns>A task that returns a list of permissions.</returns>
        Task<List<Permission>> GetAllPermissionsAsyncList(int roleid);

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

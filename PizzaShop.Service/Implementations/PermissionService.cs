using pizzashop.Repository.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRolesRepository _rolesRepository;
        private readonly Dictionary<string, List<string>> _rolePermissions = new()
        {
            { "Admin", new List<string> { "CanView", "CanEdit", "CanDelete" } },
            { "Manager", new List<string> { "CanView", "CanEdit" } },
            { "Chef", new List<string> { "CanView" } }
        };
        public PermissionService(IPermissionRepository permissionRepository, IRolesRepository rolesRepository)
        {
            _permissionRepository = permissionRepository;
            _rolesRepository = rolesRepository;
        }

        public async Task<List<Permission>> GetPermissionsByRoleId(int roleId)
        {
            return await _permissionRepository.GetPermissionsByRoleId(roleId);
        }

        public async Task<bool> SavePermissions(int roleId, List<Permission> permissions)
        {
            foreach (var permission in permissions)
            {
                var existingPermission = await _permissionRepository.GetPermissionByRoleAndModule(roleId, permission.Moduleid);

                if (existingPermission != null)
                {
                    existingPermission.Canview = permission.Canview;
                    existingPermission.Canaddedit = permission.Canaddedit;
                    existingPermission.Candelete = permission.Candelete;
                    await _permissionRepository.UpdatePermission(existingPermission);

                }
                else
                {
                    permission.Roleid = roleId;
                    await _permissionRepository.AddPermission(permission);
                }
            }
            return true;
        }
        public async Task<RoleNPermission> GetAllPermissionsAsync(int roleid)
        {
            var role = await _rolesRepository.GetRoleByIdAsync(roleid);
            var Permission = await _permissionRepository.GetAllPermissionsByIdAsync(roleid);

            RoleNPermission roleNPermission = new RoleNPermission
            {
                Roleid = role!.Roleid,
                Rolename = role.Rolename,
                Permissions = Permission
            };
            return roleNPermission;
        }
        public async Task<List<Permission>> GetAllPermissionsAsyncList(int roleid)
        {

            var Permission = await _permissionRepository.GetAllPermissionsByIdAsyncList(roleid);

            return Permission;
        }

        public async Task<bool> UpdateAllPermissionsAsync(RoleNPermission roleNPermission)
        {
            try
            {
                await _permissionRepository.UpdateAllPermissionsAsync(roleNPermission);

            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> RoleHasPermissionAsync(string roleName, string permissionName, int moduleId)
        {
            return await _permissionRepository.RoleHasPermissionAsync(roleName, permissionName, moduleId);
        }
    }
}


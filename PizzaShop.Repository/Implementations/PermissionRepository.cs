using Microsoft.EntityFrameworkCore;
using pizzashop.Repository.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public PermissionRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region GetPermissionsByRoleId
        public async Task<List<Permission>> GetPermissionsByRoleId(int roleId)
        {
            return await _context.Permissions.Where(p => p.Roleid == roleId).ToListAsync();
        }
        #endregion

        #region   GetPermissionByRoleAndModule
        public async Task<Permission?> GetPermissionByRoleAndModule(int roleId, int moduleId)
        {
            return await _context.Permissions.FirstOrDefaultAsync(
                p => p.Roleid == roleId && p.Moduleid == moduleId
            );
        }
        #endregion

        #region AddPermission
        public async Task AddPermission(Permission permission)
        {
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region UpdatePermission
        public async Task UpdatePermission(Permission permission)
        {
            _context.Permissions.Update(permission);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region GetAllPermissionsByIdAsync
        public async Task<List<Permissions>> GetAllPermissionsByIdAsync(int roleid)
        {
            return await _context.Permissions
                .Include(p => p.Module)
                .Where(p => p.Roleid == roleid)
                .Select(
                    p =>
                        new Permissions
                        {
                            Permissionid = p.Permissionid,
                            Moduleid = p.Moduleid,
                            Modulename = p.Module.Modulename,
                            Canview = p.Canview ?? false,
                            Canaddedit = p.Canaddedit ?? false,
                            Candelete = p.Candelete ?? false
                        }
                )
                .ToListAsync();
        }
        #endregion

        #region GetAllPermissionsByIdAsyncList
        public async Task<List<Permission>> GetAllPermissionsByIdAsyncList(int roleid)
        {
            return await _context.Permissions.Where(p => p.Roleid == roleid).ToListAsync();
        }
        #endregion

        #region UpdateAllPermissionsAsync
        public async Task UpdateAllPermissionsAsync(RoleNPermission roleNPermission)
        {
            var role = roleNPermission.Roleid;
            var updatedPermissions = roleNPermission.Permissions;
            var rolesPermission = _context.Permissions;
            for (int i = 0; i < updatedPermissions!.Count; i++)
            {
                var pagePermissionAnd = rolesPermission
                    .Where(
                        p =>
                            p.Roleid == role && p.Permissionid == updatedPermissions[i].Permissionid
                    )
                    .FirstOrDefault();
                pagePermissionAnd!.Canview = updatedPermissions[i].Canview;
                pagePermissionAnd.Canaddedit = updatedPermissions[i].Canaddedit;
                pagePermissionAnd.Candelete = updatedPermissions[i].Candelete;
                _context.Update(pagePermissionAnd);
            }
            await _context.SaveChangesAsync();
        }
        #endregion

        #region RoleHasPermissionAsync
        public async Task<bool> RoleHasPermissionAsync(
            string roleName,
            string permissionName,
            int moduleId
        )
        {
            return await _context.Permissions
                .Include(p => p.Role)
                .Include(p => p.Module)
                .Where(p => p.Role.Rolename == roleName && p.Moduleid == moduleId)
                .AnyAsync(
                    p =>
                        (permissionName == "CanView" && p.Canview == true)
                        || (permissionName == "CanAddEdit" && p.Canaddedit == true)
                        || (permissionName == "CanDelete" && p.Candelete == true)
                );
        }
        #endregion
    }
}


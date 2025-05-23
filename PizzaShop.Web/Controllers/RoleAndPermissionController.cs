using Microsoft.AspNetCore.Mvc;
using pizzashop.Repository.ViewModels;
using PizzaShop.Repository.Models;
using PizzaShop.Service.Attributes;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers
{
  public class RoleAndPermissionController : Controller
  {
    private readonly IRolesService _rolesService;
    private readonly IPermissionService _permissionService;

    #region Constructor
    public RoleAndPermissionController(IRolesService rolesService, IPermissionService permissionService)
    {
      _rolesService = rolesService;
      _permissionService = permissionService;
    }
    #endregion

    #region RoleAndPermission Index
    [CustomAuthorize("CanView", "Admin", "Manager", "Chef")]
    [HttpGet]
    public async Task<IActionResult> RoleAndPermission()
    {
      try
      {
        List<Role> roles = await _rolesService.GetAllRoles();
        return View(roles);
      }
      catch
      {
        TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
        return View();
      }
    }
    #endregion

    #region Permission
    [CustomAuthorize("CanView", "Admin", "Manager", "Chef")]
    [HttpGet]
    public async Task<IActionResult> Permission(int id)
    {
      try
      {
        RoleNPermission roleNPermission = await _permissionService.GetAllPermissionsAsync(id);
        return View(roleNPermission);
      }
      catch
      {
        TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
        return View();
      }
    }
    #endregion

    #region UpdatePermission
    [CustomAuthorize("CanView", "Admin", "Manager", "Chef")]
    [HttpPost]
    public IActionResult UpdatePermission(RoleNPermission roleNPermission)
    {
      try
      {
        var id = roleNPermission.Roleid;
        if (_permissionService.UpdateAllPermissionsAsync(roleNPermission).Result)
        {
          TempData["SuccessMessage"] = "Permissions Updated Successfully";
          return RedirectToAction("Permission", new { id = id });
        }
        else
        {
          TempData["ErrorMessage"] = "Permissions Updated Failed";
        }
        return RedirectToAction("Permission", new { id = id });
      }
      catch
      {
        TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
        return View();
      }
    }
    #endregion
  }
}
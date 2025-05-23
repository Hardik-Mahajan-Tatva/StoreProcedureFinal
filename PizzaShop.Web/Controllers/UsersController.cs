using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Attributes;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IAuthService _authService;
        private readonly ICountryService _countryService;
        private readonly IStateService _stateService;
        private readonly ICityService _cityService;
        private readonly IEmailSender _emailSender;
        private readonly IRolesService _rolesService;

        #region Constructor
        public UsersController(IUsersService usersService, ICountryService countryService, IStateService stateService, ICityService cityService, IEmailSender emailSender, IRolesService rolesService, IAuthService authService)
        {
            _usersService = usersService;
            _countryService = countryService;
            _stateService = stateService;
            _cityService = cityService;
            _emailSender = emailSender;
            _rolesService = rolesService;
            _authService = authService;
        }
        #endregion

        #region Users
        [CustomAuthorize("CanView", "Admin", "Manager", "Chef")]
        [HttpGet]
        public async Task<IActionResult> Users(int pageNumber = 1, int pageSize = 5, string query = "", string sortOrder = "asc", string sortColumn = "Firstname", string search = "")
        {
            try
            {
                IQueryable<User> usersQuery = _usersService.GetAllUsers();

                int? currentUserId = await GetCurrentUserIdAsync();

                if (!string.IsNullOrEmpty(search))
                {
                    usersQuery = usersQuery.Where(u =>
                        (u.Firstname != null && (u.Firstname.Contains(search) || u.Firstname.ToLower().Contains(search))) ||
                        (u.Email != null && (u.Email.Contains(search) || u.Email.ToLower().Contains(search))) ||
                        (u.Phone != null && (u.Phone.Contains(search) || u.Phone.ToLower().Contains(search)))
                    );
                }

                if (!string.IsNullOrEmpty(sortColumn))
                {
                    var parameter = Expression.Parameter(typeof(User), "u");
                    var property = Expression.Property(parameter, sortColumn);
                    var lambda = Expression.Lambda<Func<User,
                      object>>(Expression.Convert(property, typeof(object)), parameter);

                    usersQuery = sortOrder.ToLower() == "desc" ?
                      usersQuery.OrderByDescending(lambda) :
                      usersQuery.OrderBy(lambda);
                }
                var paginatedUsers = await PaginatedList<User>.CreateAsync(usersQuery, pageNumber, pageSize);

                ViewBag.FromRec = paginatedUsers.FromRec;
                ViewBag.ToRec = paginatedUsers.ToRec;
                ViewBag.TotalItems = paginatedUsers.TotalItems;
                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = pageNumber;
                ViewBag.TotalPages = paginatedUsers.TotalPages;
                ViewBag.CurrentUserId = currentUserId;

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_UserTablePartialView", paginatedUsers);
                }
                return View(paginatedUsers);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return View();
            }
        }
        #endregion

        #region AddNewUser
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> AddNewUser()
        {
            try
            {
                var model = new CreateUserViewModel
                {
                    Countries = (List<Country>)await _countryService.GetAllCountries(),
                    Roles = await _rolesService.GetAllRoles()
                };

                return View(model);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return View();
            }
        }
        #endregion

        #region GetStatesByCountry
        [HttpGet]
        public IActionResult GetStatesByCountry(int Countryid)
        {
            try
            {
                var states = _stateService.GetStatesByCountryId(Countryid).Result;
                return Json(states);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetCitiesByState
        [HttpGet]
        public IActionResult GetCitiesByState(int Stateid)
        {
            try
            {
                var cities = _cityService.GetCitiesByStateId(Stateid).Result;
                return Json(cities);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region AddNewUser
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> AddNewUser(CreateUserViewModel createUserViewModel, IFormFile? itemImage)
        {
            try
            {
                var uniqueErrors = await _usersService.ValidateUniqueFieldsAsync(createUserViewModel);
                foreach (var error in uniqueErrors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
                if (!ModelState.IsValid)
                {
                    foreach (var key in ModelState.Keys)
                    {
                        var errors = ModelState[key]?.Errors;
                        if (errors != null)
                            foreach (var error in errors)
                            {
                                Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                            }
                    }
                }

                if (!ModelState.IsValid)
                {
                    createUserViewModel.Countries = (List<Country>?)await _countryService.GetAllCountries();
                    createUserViewModel.States = (List<State>?)(createUserViewModel.Countryid != 0
              ? await _stateService.GetStatesByCountryId(createUserViewModel.Countryid)
              : new List<State>());

                    createUserViewModel.Cities = (List<City>?)(createUserViewModel.Stateid != 0
                        ? await _cityService.GetCitiesByStateId(createUserViewModel.Stateid)
                        : new List<City>());



                    createUserViewModel.Roles = await _rolesService.GetAllRoles();
                    TempData["WarningMessage"] = "Please submit the valid form";
                    return View(createUserViewModel);
                }

                var msg = _usersService.CreateNewUser(createUserViewModel, itemImage!).Result;
                if (msg)
                {
                    string resetToken = await _authService.GeneratePasswordResetToken(createUserViewModel.Email ?? string.Empty);

                    string resetLink = Url.Action("ResetPassword", "Auth", new
                    {
                        token = resetToken
                    }, Request.Scheme!)!;

                    await _emailSender.SendResetPasswordEmail(createUserViewModel.Email ?? string.Empty, resetLink);
                    TempData["SuccessMessage"] = "User Created Successfull";
                    return RedirectToAction("Users", "Users");
                }
                else
                {
                    TempData["ErrorMessage"] = "User Can't created";
                    return RedirectToAction("Users", "Users");
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return View();
            }
        }
        #endregion

        #region UpdateUser
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> UpdateUser(int id)
        {
            try
            {
                var user = await _usersService.GetUserById(id);
                var model = new UpdateUserViewModel
                {
                    Id = user!.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Email = user.Email,
                    Status = user.Status,
                    Phone = user.Phone,
                    Address = user.Address,
                    Zipcode = user.Zipcode,
                    RoleId = user.RoleId,
                    Countryid = user.Countryid,
                    Stateidid = user.Stateidid,
                    Cityidid = user.Cityidid,
                    ProfileImage = user.ProfileImage,

                    Countries = (List<Country>)await _countryService.GetAllCountries(),
                    States = (List<State>)await _stateService.GetStatesByCountryId(user.Countryid),
                    Cities = (List<City>)await _cityService.GetCitiesByStateId(user.Stateidid),
                    Roles = await _rolesService.GetAllRoles()
                };

                return View(model);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return View();
            }
        }
        #endregion

        #region Updateuser
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> Updateuser(UpdateUserViewModel updateUserViewModel, int id, IFormFile? itemImage)
        {
            try
            {
                var uniqueErrors = await _usersService.ValidateUniqueFieldsAsync(updateUserViewModel, id);
                foreach (var error in uniqueErrors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
                if (!ModelState.IsValid)
                {
                    foreach (var key in ModelState.Keys)
                    {
                        var errors = ModelState[key]?.Errors;
                        if (errors != null)
                            foreach (var error in errors)
                            {
                                Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                            }
                    }
                }

                if (!ModelState.IsValid)
                {

                    var model = updateUserViewModel;
                    var profile = await _usersService.GetUserById(id);
                    if (profile == null)
                        throw new Exception("Profile Not found");
                    var email = profile.Email;
                    model.Countries = (List<Country>?)await _countryService.GetAllCountries();
                    model.States = (List<State>?)await _stateService.GetStatesByCountryId(model.Countryid);
                    model.Cities = (List<City>?)await _cityService.GetCitiesByStateId(model.Stateidid);
                    model.Roles = await _rolesService.GetAllRoles();
                    model.Email = profile.Email;
                    if (itemImage == null && !string.IsNullOrEmpty(updateUserViewModel.ProfileImage))
                    {
                        updateUserViewModel.ProfileImage = profile.ProfileImage;
                    }
                    TempData["WarningMessage"] = "Please submit the valid form";
                    return View(updateUserViewModel);
                }
                var updatedmodel = updateUserViewModel;
                updatedmodel.Email = updateUserViewModel.Email;
                updatedmodel.Role = await _rolesService.GetRoleNameById(updatedmodel.RoleId);

                var result = await _usersService.UpdateExixtingUser(updatedmodel, id, itemImage!);
                if (result)
                {
                    TempData["SuccessMessage"] = "User Edited Successfull";
                    return RedirectToAction("Users", "Users");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return View();
            }
        }
        #endregion

        #region DeleteUser
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _usersService.DeleteExistingUser(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "User Deleted Successfull";
                    return RedirectToAction("Users", "Users");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return View();
            }
        }
        #endregion

        #region GetCurrentUserIdAsync
        private async Task<int?> GetCurrentUserIdAsync()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }
            return await _usersService.GetUserIdByEmailAsync(email);
        }
        #endregion

        #region DeleteProfileImage
        [HttpPost]
        public IActionResult DeleteProfileImage(string imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/uploads", imageName);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }
        #endregion
    }
}
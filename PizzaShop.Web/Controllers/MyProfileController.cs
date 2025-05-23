using Microsoft.AspNetCore.Mvc;
using PizzaShop.Service.Interfaces;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Attributes;

namespace PizzaShop.Web.Controllers
{

    public class MyProfileController : Controller
    {
        private readonly IMyProfileService _profileService;
        private readonly IAuthService _authenticateUserService;
        private readonly IAccountService _accountService;
        private readonly ICountryService _countryService;
        private readonly IStateService _stateService;
        private readonly ICityService _cityService;

        #region Constructor
        public MyProfileController(IMyProfileService profileService, IAuthService authenticateUserService, IAccountService accountService, ICountryService countryService, IStateService stateService, ICityService cityService)
        {
            _profileService = profileService;
            _authenticateUserService = authenticateUserService;
            _accountService = accountService;
            _countryService = countryService;
            _stateService = stateService;
            _cityService = cityService;
        }
        #endregion

        #region MyProfile - GET
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> MyProfile(string from)
        {
            try
            {
                if (from == "OrderApp")
                {
                    ViewData["Layout"] = "~/Views/Shared/_OrderAppLayout.cshtml";
                    ViewData["From"] = "OrderApp";
                }
                else
                {
                    ViewData["Layout"] = "~/Views/Shared/_Layout.cshtml";
                    ViewData["From"] = "Default";
                }

                var userEmail = GetEmailFromToken();
                if (string.IsNullOrEmpty(userEmail))
                {
                    return RedirectToAction("Login", "Auth");
                }

                var userId = await _profileService.GetProfileUserIdAsync(userEmail);
                var userProfile = await _profileService.GetProfileAsync(userId);

                if (userProfile == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                userProfile.Countries = (List<Repository.Models.Country>?)await _countryService.GetAllCountries();
                userProfile.States = (List<Repository.Models.State>?)await _stateService.GetStatesByCountryId(userProfile.Countryid);
                userProfile.Cities = (List<Repository.Models.City>?)await _cityService.GetCitiesByStateId(userProfile.Stateid);

                userProfile.ProfileImage = await _profileService.GetProfileImagePathAsync(userId);
                return View(userProfile);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return RedirectToAction("Login", "Auth");
            }
        }
        #endregion

        #region MyProfile - POST
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> MyProfile(MyProfileViewModel model, string from)
        {
            try
            {
                if (from == "OrderApp")
                {
                    ViewBag.Layout = "~/Views/Shared/_OrderAppLayout.cshtml";
                }
                else
                {
                    ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
                }

                var email = GetEmailFromToken();
                if (string.IsNullOrEmpty(email))
                {
                    return RedirectToAction("Dashboard", "Dashboard");
                }

                var userId = await _profileService.GetProfileUserIdAsync(email);

                bool isDuplicateUsername = await _profileService.IsUsernameTakenAsync(model.Username, userId);
                if (isDuplicateUsername)
                {
                    ModelState.AddModelError("Username", "Username already exists. Please choose a different one.");
                }

                if (!ModelState.IsValid)
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Invalid Field: {state.Key}, Error: {error.ErrorMessage}");
                        }
                    }
                    model.Countries = (List<Repository.Models.Country>?)await _countryService.GetAllCountries();
                    model.States = (List<Repository.Models.State>?)await _stateService.GetStatesByCountryId(model.Countryid);
                    model.Cities = (List<Repository.Models.City>?)await _cityService.GetCitiesByStateId(model.Stateid);

                    model.ProfileImage = await _profileService.GetProfileImagePathAsync(userId);

                    return View(model);
                }

                var result = await _profileService.UpdateProfileAsync(userId, model);

                if (result != null)
                {
                    TempData["SuccessMessage"] = "Profile Updated Successfully";
                    return RedirectToAction(nameof(MyProfile), new { from = from });
                }

                TempData["ErrorMessage"] = "Profile update failed!";
                return RedirectToAction(nameof(MyProfile), new { from = from });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return RedirectToAction(nameof(MyProfile), new { from = from });
            }
        }
        #endregion

        #region Change Password - GET
        [CustomAuthorize]
        [HttpGet]
        public IActionResult MyProfileChangePassword(string from)
        {
            try
            {
                if (from == "OrderApp")
                {
                    ViewBag.Layout = "~/Views/Shared/_OrderAppLayout.cshtml";
                }
                else
                {
                    ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
                }
                var email = GetEmailFromToken();
                if (string.IsNullOrEmpty(email))
                {
                    return RedirectToAction("Dashboard", "Dashboard");
                }
                var model = new ChangePasswordViewModel
                {
                    Email = email
                };
                return View(model);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return RedirectToAction("Login", "Auth");
            }
        }
        #endregion

        #region Change Password - POST
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> MyProfileChangePassword(ChangePasswordViewModel model, string? from)
        {
            try
            {
                if (from == "OrderApp")
                {
                    ViewBag.Layout = "~/Views/Shared/_OrderAppLayout.cshtml";
                }
                else
                {
                    ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
                }
                if (!ModelState.IsValid)
                {
                    foreach (var key in ModelState.Keys)
                    {
                        var state = ModelState[key];
                        if (state != null)
                        {
                            foreach (var error in state.Errors)
                            {
                                Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                            }
                        }
                    }

                    return View(model);
                }
                var email = model.Email ?? throw new ArgumentNullException(nameof(model.Email));
                var result = await _accountService.ChangePasswordAsync(email, model.CurrentPassword, model.NewPassword, model.ConfirmPassword);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Password changed successfully!";
                    return RedirectToAction("MyProfileChangePassword");
                }
                else
                {
                    TempData["ErrorMessage"] = result.ErrorMessage;
                    return View(model);
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return RedirectToAction("Login", "Auth");
            }
        }
        #endregion

        #region Upload Profile Image - POST
        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile profileImage)
        {
            try
            {
                if (profileImage == null || profileImage.Length == 0)
                {
                    TempData["ErrorMessage"] = "No image selected.";
                    return RedirectToAction("MyProfile");
                }

                var email = GetEmailFromToken();
                if (string.IsNullOrEmpty(email))
                {
                    return RedirectToAction("Login", "Auth");
                }

                var userId = await _profileService.GetProfileUserIdAsync(email);
                var result = await _profileService.UploadProfileImageAsync(userId, profileImage);

                if (result)
                {
                    TempData["SuccessMessage"] = "Profile image updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Image upload failed.";
                }

                return RedirectToAction("MyProfile");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return RedirectToAction("MyProfile");
            }
        }
        #endregion

        #region Delete Profile Image - POST
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> DeleteProfileImage()
        {
            try
            {
                var email = GetEmailFromToken();
                if (string.IsNullOrEmpty(email))
                {
                    return Json(new { success = false, message = "Unauthorized request." });
                }

                var userId = await _profileService.GetProfileUserIdAsync(email);
                var result = await _profileService.DeleteProfileImageAsync(userId);

                if (result)
                {
                    return Json(new { success = true, message = "Profile image removed successfully." });
                }

                return Json(new { success = false, message = "Failed to delete profile image. Please try again." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while processing your request. Please try again." });
            }
        }
        #endregion

        #region GetStatesByCountry - AJAX 
        public async Task<IActionResult> GetStatesByCountry(int countryId)
        {
            try
            {
                var states = await _stateService.GetStatesByCountryId(countryId);
                return Json(states);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while fetching states." });
            }
        }
        #endregion

        #region GetCitiesByState - AJAX
        public async Task<IActionResult> GetCitiesByState(int stateId)
        {
            try
            {
                var cities = await _cityService.GetCitiesByStateId(stateId);
                return Json(cities);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while fetching cities." });
            }
        }
        #endregion

        #region Helper Methods
        private string? GetEmailFromToken()
        {
            var token = Request.Cookies["AuthToken"];
            var decoded = _authenticateUserService.DecodeJwtToken(token ?? "");
            return decoded?.Email;
        }
        private async Task PopulateDropdowns(int countryId, int stateId)
        {
            ViewBag.Countries = await _countryService.GetAllCountries();
            ViewBag.States = await _stateService.GetStatesByCountryId(countryId);
            ViewBag.Cities = await _cityService.GetCitiesByStateId(stateId);
        }
        private async Task<int?> GetUserIdFromToken()
        {
            var email = GetEmailFromToken();
            if (string.IsNullOrEmpty(email)) return null;
            return await _profileService.GetProfileUserIdAsync(email);
        }
        #endregion
    }
}
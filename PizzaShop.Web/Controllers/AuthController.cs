using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;
using PizzaShop.Service.Utils;

namespace PizzaShop.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;

        #region  Constructor
        public AuthController(IEmailSender emailSender, IAuthService authService, IJwtService jwtService)
        {
            _emailSender = emailSender;
            _authService = authService;
            _jwtService = jwtService;
        }
        #endregion

        #region Login GET/POST
        public IActionResult Login()
        {
            try
            {
                var user = SessionUtils.GetUser(HttpContext);
                var principal = (ClaimsPrincipal?)null;

                if (user == null)
                    return View();

                var token = Request.Cookies["AuthToken"];
                if (token != null)
                {
                    principal = _jwtService.ValidateToken(token);
                }

                if (principal == null)
                {
                    Response.Cookies.Delete("AuthToken");
                    CookieUtils.ClearCookies(HttpContext);
                    SessionUtils.ClearSession(HttpContext);
                    return View();
                }

                return RedirectToAction("Dashboard", "Dashboard");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _authService.AuthenticateUser(
                    model.Email!.ToLower(),
                    model.Password!
                );

                if (user == null)
                {
                    bool userExists = await _authService.CheckIfUserExists(model.Email.ToLower());
                    var userReset = await _authService.GetUser(model.Email.ToLower());
                    if (userReset != null && userReset.Isfirstlogin)
                    {
                        string resetToken = await _authService.GeneratePasswordResetToken(
                            userReset.Email
                        );

                        return RedirectToAction(
                            "ResetPassword",
                            "Auth",
                            new { token = resetToken }
                        );
                    }
                    if (userExists)
                    {
                        ModelState.AddModelError(
                            "Password",
                            "Invalid password. Try again or reset your password."
                        );
                    }
                    return View(model);
                }

                var token = await _jwtService.GenerateJwtToken(user.Email, model.RememberMe);

                CookieUtils.SaveJWTToken(Response, token);

                if (model.RememberMe)
                {
                    CookieUtils.SaveUserData(Response, user);
                }
                HttpContext.Session.SetString("username", user.Username);
                return RedirectToAction("Dashboard", "Dashboard");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return RedirectToAction("GenericError", "Error");
            }
        }
        #endregion

        #region ForgotPassword GET/POST
        [HttpGet]
        public IActionResult ForgotPassword(string email = "")
        {
            try
            {
                return View(new ForgotPasswordViewModel { Email = email });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                bool userExists = await _authService.CheckIfUserExists(model.Email);
                if (!userExists)
                {
                    ModelState.AddModelError("Email", "No account found with this email.");
                    return View(model);
                }

                string resetToken = await _authService.GeneratePasswordResetToken(model.Email);
                string? resetLink = Url.Action(
                    "ResetPassword",
                    "Auth",
                    new { token = resetToken },
                    Request.Scheme
                );

                await _emailSender.SendResetPasswordEmail(model.Email, resetLink);

                TempData["SuccessMessage"] = "A password reset link has been sent to your email.";
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to send reset email.";
                return View(model);
            }
        }
        #endregion

        #region ResetPassword GET/POST
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    TempData["ErrorMessage"] = "An error occurred. Please try again.";
                    return RedirectToAction("Login");
                }

                var isValid = await _authService.ValidatePasswordResetToken(token);
                if (!isValid)
                {
                    TempData["ErrorMessage"] = "Invalid or expired reset link.";
                    return RedirectToAction("Login");
                }

                var model = new ResetPasswordViewModel { Token = token };
                TempData["InfoMessage"] = "Please reset you password";

                return View(model);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var isValid = await _authService.ValidatePasswordResetToken(model.Token ?? string.Empty);
                if (!isValid)
                {
                    ModelState.AddModelError("", "Invalid or expired reset link.");
                    return View(model);
                }

                var result = await _authService.UpdateUserPassword(model.Token ?? string.Empty, model.NewPassword!);
                if (!result)
                {
                    ModelState.AddModelError("", "Failed to reset password.");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Your password has been successfully reset. Please log in with your new password.";
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred. Please try again.";
                return View(model);
            }
        }
        #endregion

        #region LogOut 
        public IActionResult Logout()
        {
            try
            {
                CookieUtils.ClearCookies(HttpContext);
                SessionUtils.ClearSession(HttpContext);
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return RedirectToAction("Login", "Auth");
            }
        }
        #endregion

        #region RefreshToken 
        [HttpPost]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var oldToken = Request.Cookies["AuthToken"];
                if (string.IsNullOrEmpty(oldToken))
                {
                    return RedirectToAction("Login", "Auth");
                }

                var principal = _jwtService.ValidateToken(oldToken);

                if (principal == null)
                    return RedirectToAction("Login", "Auth");

                var email = principal.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                    return RedirectToAction("Login", "Auth");

                var newToken = await _jwtService.GenerateJwtToken(email);
                CookieUtils.SaveJWTToken(Response, newToken);

                return Ok(new { success = true });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while refreshing your session. Please try again.";
                return RedirectToAction("Login", "Auth");
            }
        }
        #endregion
    }
}

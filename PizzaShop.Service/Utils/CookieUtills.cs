using Microsoft.AspNetCore.Http;
using PizzaShop.Repository.Models;
using System.Text.Json;

namespace PizzaShop.Service.Utils
{
    public static class CookieUtils
    {
        /// <summary>
        /// Saves a JWT token in the response cookies.
        /// </summary>
        /// <param name="response">The HTTP response to append the cookie to.</param>
        /// <param name="token">The JWT token to save.</param>
        public static void SaveJWTToken(HttpResponse response, string token)
        {
            response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(15)
            });
        }

        /// <summary>
        /// Retrieves the JWT token from the request cookies.
        /// </summary>
        /// <param name="request">The HTTP request to retrieve the cookie from.</param>
        /// <returns>The JWT token if found, otherwise null.</returns>
        public static string? GetJWTToken(HttpRequest request)
        {
            _ = request.Cookies.TryGetValue("AuthToken", out string? token);
            return token;
        }

        /// <summary>
        /// Saves user data (email and username) in the response cookies.
        /// </summary>
        /// <param name="response">The HTTP response to append the cookie to.</param>
        /// <param name="user">The user object containing email and username.</param>
        public static void SaveUserData(HttpResponse response, Userslogin user)
        {
            string userData = JsonSerializer.Serialize(new { user.Email, user.Username });

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(3),
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            };
            response.Cookies.Append("UserData", userData, cookieOptions);
        }

        /// <summary>
        /// Clears all cookies related to authentication and user data.
        /// </summary>
        /// <param name="httpContext">The HTTP context to delete the cookies from.</param>
        public static void ClearCookies(HttpContext httpContext)
        {
            httpContext.Response.Cookies.Delete("AuthToken");
            httpContext.Response.Cookies.Delete("UserData");
        }
    }
}
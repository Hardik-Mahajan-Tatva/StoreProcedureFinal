using Microsoft.AspNetCore.Http;
using PizzaShop.Repository.Models;
using System.Text.Json;

namespace PizzaShop.Service.Utils
{
    public static class SessionUtils
    {
        /// <summary>
        /// Stores user data (email and username) in the session.
        /// </summary>
        /// <param name="httpContext">The HTTP context to store the session data in.</param>
        /// <param name="user">The user object containing email and username.</param>
        public static void SetUser(HttpContext httpContext, User user)
        {
            if (user != null)
            {
                string userData = JsonSerializer.Serialize(user);
                httpContext.Session.SetString("UserData", userData);
            }
        }

        /// <summary>
        /// Retrieves user data (email and username) from the session or cookies.
        /// </summary>
        /// <param name="httpContext">The HTTP context to retrieve the session or cookie data from.</param>
        /// <returns>A tuple containing the user's email and username, or null if not found.</returns>
        public static (string? Email, string? Username)? GetUser(HttpContext httpContext)
        {
            string? userData = httpContext.Session.GetString("UserData");

            if (string.IsNullOrEmpty(userData))
            {
                httpContext.Request.Cookies.TryGetValue("UserData", out userData);
            }


            if (string.IsNullOrEmpty(userData))
                return null;

            try
            {
                var user = JsonSerializer.Deserialize<User>(userData);
                return user is not null ? (user.Email, user.Username) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Clears all session data.
        /// </summary>
        /// <param name="httpContext">The HTTP context to clear the session data from.</param>
        public static void ClearSession(HttpContext httpContext) => httpContext.Session.Clear();
    }
}
using System.Security.Claims;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user using their email and password.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A task that returns the authenticated user if found, otherwise null.</returns>
        Task<Userslogin?> AuthenticateUser(string email, string password);

        /// <summary>
        /// Retrieves a user by their email.
        /// </summary>
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns>A task that returns the user if found, otherwise null.</returns>
        Task<Userslogin?> GetUser(string email);

        /// <summary>
        /// Checks if a user exists by their email.
        /// </summary>
        /// <param name="email">The email of the user to check.</param>
        /// <returns>A task that returns true if the user exists, otherwise false.</returns>
        Task<bool> CheckIfUserExists(string email);

        /// <summary>
        /// Generates a password reset token for a user.
        /// </summary>
        /// <param name="email">The email of the user to generate the token for.</param>
        /// <returns>A task that returns the generated password reset token.</returns>
        Task<string> GeneratePasswordResetToken(string email);

        /// <summary>
        /// Validates a password reset token.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <returns>A task that returns true if the token is valid, otherwise false.</returns>
        Task<bool> ValidatePasswordResetToken(string token);

        /// <summary>
        /// Updates a user's password using a password reset token.
        /// </summary>
        /// <param name="token">The password reset token.</param>
        /// <param name="newPassword">The new password to set.</param>
        /// <returns>A task that returns true if the password was updated successfully, otherwise false.</returns>
        Task<bool> UpdateUserPassword(string token, string newPassword);

        /// <summary>
        /// Retrieves the permissions of a user from their JWT token.
        /// </summary>
        /// <param name="user">The claims principal representing the user.</param>
        /// <returns>A list of permissions associated with the user.</returns>
        List<Permission> GetUserPermissionsFromToken(ClaimsPrincipal user);

        /// <summary>
        /// Decodes a JWT token and retrieves the login view model.
        /// </summary>
        /// <param name="token">The JWT token to decode.</param>
        /// <returns>The decoded login view model.</returns>
        LoginViewModel DecodeJwtToken(string token);
    }
}

using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces
{
    public interface IUsersloginRepository
    {
        /// <summary>
        /// Retrieves a user login by email and password asynchronously.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A task that returns the user login if found, otherwise null.</returns>
        Task<Userslogin?> GetUserLoginAsync(string userEmail, string userPassword);

        /// <summary>
        /// Retrieves a user by their email asynchronously.
        /// </summary>
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns>A task that returns the user if found, otherwise null.</returns>
        Task<Userslogin?> GetUserAsync(string userEmail);

        /// <summary>
        /// Retrieves a user by their email asynchronously.
        /// </summary>
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns>A task that returns the user if found, otherwise null.</returns>
        Task<Userslogin?> GetUserByEmailAsync(string userEmail);

        /// <summary>
        /// Retrieves a user by their ID asynchronously.
        /// </summary>
        /// <param name="Userid">The ID of the user to retrieve.</param>
        /// <returns>A task that returns the user if found, otherwise null.</returns>
        Task<Userslogin?> GetUserByIdAsync(string userId);

        /// <summary>
        /// Saves a password reset token for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="token">The reset token to save.</param>
        /// <param name="expiration">The expiration date of the token.</param>
        /// <param name="used">Indicates whether the token has been used.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SavePasswordResetToken(
            int userId,
            string passwordResetToken,
            DateTime expiration,
            bool isUsed
        );

        /// <summary>
        /// Retrieves a password reset token by its value.
        /// </summary>
        /// <param name="token">The token to retrieve.</param>
        /// <returns>A task that returns the password reset token if found, otherwise null.</returns>
        Task<PasswordResetToken?> GetPasswordResetToken(string passwordResetToken);

        /// <summary>
        /// Deletes a password reset token by its value.
        /// </summary>
        /// <param name="token">The token to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeletePasswordResetToken(string passwordResetToken);

        /// <summary>
        /// Updates a user's information asynchronously.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateUserAsync(Userslogin userslogin);

        /// <summary>
        /// Retrieves a user by their password reset token.
        /// </summary>
        /// <param name="token">The reset token to retrieve the user for.</param>
        /// <returns>A task that returns the user if found.</returns>
        Task<Userslogin?> GetUserByResetToken(string passwordResetToken);

        /// <summary>
        /// Sets a new password for a user.
        /// </summary>
        /// <param name="userLoginId">The ID of the user login to update.</param>
        /// <param name="newPassword">The new password to set.</param>
        /// <returns>A task that returns true if the password was updated successfully, otherwise false.</returns>
        Task<bool> SetUserPassword(int userLoginId, string newPassword);

        /// <summary>
        /// Invalidates a password reset token for a user.
        /// </summary>
        /// <param name="user">The user whose reset token should be invalidated.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task InvalidateResetToken(Userslogin userslogin);

        /// <summary>
        /// Creates a new user login asynchronously.
        /// </summary>
        /// <param name="login">The user login to create.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CreateUserLoginAsync(Userslogin userslogin);
    }
}

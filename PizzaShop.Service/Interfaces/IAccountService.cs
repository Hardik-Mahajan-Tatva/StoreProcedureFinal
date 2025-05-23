using PizzaShop.Service.Implementations;

namespace PizzaShop.Service.Interfaces
{
    public interface IAccountService
    {
        /// <summary>
        /// Changes the password for a user asynchronously.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="currentPassword">The current password of the user.</param>
        /// <param name="newPassword">The new password to set.</param>
        /// <param name="confirmPassword">The confirmation of the new password.</param>
        /// <returns>A task that returns the result of the password change operation.</returns>
        Task<PasswordChangeResult> ChangePasswordAsync(string email, string currentPassword, string newPassword, string confirmPassword);
    }
}

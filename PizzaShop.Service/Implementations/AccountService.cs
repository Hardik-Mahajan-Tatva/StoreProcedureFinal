using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IUsersRepository _usersRepository;
        public AccountService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        public async Task<PasswordChangeResult> ChangePasswordAsync(string email, string currentPassword, string newPassword, string confirmPassword)
        {
            var result = new PasswordChangeResult();
            if (newPassword != confirmPassword)
            {
                result.Success = false;
                result.ErrorMessage = "New password and confirmation password do not match.";
                return result;
            }
            var user = await _usersRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                result.Success = false;
                result.ErrorMessage = "User not found.";
                return result;
            }
            if (string.IsNullOrEmpty(user.Email))
            {
                result.Success = false;
                result.ErrorMessage = "Email is required.";
                return result;
            }
            var userWithValidPassword = await _usersRepository.GetUsersAsync(user.Email, currentPassword);
            if (userWithValidPassword == null)
            {
                result.Success = false;
                result.ErrorMessage = "Current password is incorrect.";
                return result;
            }
            var passwordChangeSuccess = await _usersRepository.UpdatePasswordAsync(user.Userid, newPassword);
            if (passwordChangeSuccess)
            {
                result.Success = true;
            }
            else
            {
                result.Success = false;
                result.ErrorMessage = "Failed to change password. Please try again later.";
            }
            return result;
        }
    }
    public class PasswordChangeResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

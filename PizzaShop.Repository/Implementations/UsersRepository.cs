using System.Text;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations
{
    public class UsersRepository : IUsersRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public UsersRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region GetUsersAsync
        public async Task<User?> GetUsersAsync(string userEmail, string userPassword)
        {
            return await _context.Users.FirstOrDefaultAsync(
                u => u.Email == userEmail && u.Password == HashPassword(userPassword)
            );
        }
        #endregion

        #region GetUserByEmailAsync
        public async Task<User?> GetUserByEmailAsync(string userEmail)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        }
        #endregion

        #region GetUserByIdAsync
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Userid == userId);
        }
        #endregion

        #region SavePasswordResetToken
        public async Task SavePasswordResetToken(
            int userId,
            string passwordResetToken,
            DateTime expiryTime
        )
        {
            var resetToken = new PasswordResetToken
            {
                UserId = userId,
                Token = passwordResetToken,
                ExpiryTime = expiryTime
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region GetPasswordResetToken
        public async Task<PasswordResetToken?> GetPasswordResetToken(string passwordResetToken)
        {
            return await _context.PasswordResetTokens.FirstOrDefaultAsync(
                t => t.Token == passwordResetToken && t.ExpiryTime > DateTime.UtcNow
            );
        }
        #endregion

        #region DeletePasswordResetToken
        public async Task DeletePasswordResetToken(string passwordResetToken)
        {
            var resetToken = await _context.PasswordResetTokens.FirstOrDefaultAsync(
                t => t.Token == passwordResetToken
            );

            if (resetToken != null)
            {
                _context.PasswordResetTokens.Remove(resetToken);
                await _context.SaveChangesAsync();
            }
        }
        #endregion

        #region UpdateUserAsync
        public async Task<bool> UpdateUserAsync(User user)
        {
            if (user == null)
            {
                return false;
            }

            _context.Users.Update(user);
            var userLogin = await _context.Userslogins.FirstOrDefaultAsync(
                u => u.Email == user.Email
            );
            if (userLogin != null)
            {
                userLogin.Roleid = user.Roleid;
                _context.Userslogins.Update(userLogin);
            }
            var rowAffected = await _context.SaveChangesAsync();
            return rowAffected > 0;
        }
        #endregion

        #region SoftDeleteUserAsync
        public async Task<bool> SoftDeleteUserAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Userid == userId);
            if (user != null)
            {
                user.Isdeleted = true;
                var updatedUser = user;
                _context.Users.Update(updatedUser);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region UpdatePasswordAsync
        public async Task<bool> UpdatePasswordAsync(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            var userLogin = await _context.Userslogins.FindAsync(userId);

            if (user != null && userLogin != null)
            {
                user.Password = HashPassword(newPassword);
                userLogin.Passwordhash = HashPassword(newPassword);
                _context.Users.Update(user);
                _context.Userslogins.Update(userLogin);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        #endregion

        #region GetAll
        public IQueryable<User> GetAll()
        {
            return _context.Users.Where(u => u.Isdeleted == false);
        }
        #endregion

        #region CreateUser
        public async Task<bool> CreateUser(User user)
        {
            if (user != null)
            {
                var newPassword = HashPassword(user.Password!);
                user.Password = newPassword;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region DeleteUser
        public async Task<bool> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;
            var newUser = user;
            if (user.Isdeleted == false)
            {
                user.Isdeleted = true;
            }
            _context.Users.Update(user);

            return await _context.SaveChangesAsync() > 0;
        }
        #endregion

        #region GetPaginatedUsersAsync
        public async Task<List<Userslogin>> GetPaginatedUsersAsync(
            int page,
            int pageSize,
            string search
        )
        {
            return await _context.Userslogins
                .Include(u => u.User)
                .Include(u => u.Role)
                .Where(
                    u =>
                        u.User != null
                        && u.User.Isdeleted == false
                        && (
                            string.IsNullOrEmpty(search)
                            || u.User.Firstname.Contains(search)
                            || u.User.Lastname.Contains(search)
                            || u.Email.Contains(search)
                            || u.User.Phone.Contains(search)
                            || u.Role.Rolename.Contains(search)
                        )
                )
                .OrderBy(u => u.Userloginid)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        #endregion

        #region GetTotalUsersCountAsync
        public async Task<int> GetTotalUsersCountAsync(string search)
        {
            return await _context.Userslogins
                .Include(u => u.User)
                .Include(u => u.Role)
                .Where(
                    u =>
                        u.User != null
                        && u.User.Isdeleted == false
                        && (
                            string.IsNullOrEmpty(search)
                            || u.User.Firstname.Contains(search)
                            || u.User.Lastname.Contains(search)
                            || u.Email.Contains(search)
                            || u.User.Phone.Contains(search)
                            || u.Role.Rolename.Contains(search)
                        )
                )
                .CountAsync();
        }
        #endregion

        #region AddUserlogin
        public async Task AddUserlogin(Userslogin userslogin)
        {
            await _context.Userslogins.AddAsync(userslogin);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region GetPaginatedUsersAsync
        Task<List<User>> IUsersRepository.GetPaginatedUsersAsync(
            int page,
            int pageSize,
            string search
        )
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetUserIdByEmailAsync
        public async Task<int?> GetUserIdByEmailAsync(string userEmail)
        {
            var user = await _context.Users
                .Where(u => u.Email == userEmail)
                .Select(u => u.Userid)
                .FirstOrDefaultAsync();

            return user == 0 ? null : user;
        }
        #endregion

        #region GetUserByUsernameAsync
        public async Task<User?> GetUserByUsernameAsync(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(
                u => u.Username != null && u.Username.ToLower() == userName.ToLower()
            );
        }
        #endregion

        #region UpdateProfileImageAsync
        public async Task UpdateProfileImageAsync(int userId, string imagePath)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Profileimg = imagePath;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }
        #endregion

        #region DeleteProfileImageAsync
        public async Task<bool> DeleteProfileImageAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Userid == userId);
            if (user == null)
                return false;

            user.Profileimg = null;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion

        #region HashPassword
        private static string HashPassword(string password)
        {
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(password))
            );
        }
        #endregion

        #region IsEmailExistsAsync
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        #endregion

        #region IsUsernameExistsAsync
        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
        #endregion

        #region IsPhoneExistsAsync
        public async Task<bool> IsPhoneExistsAsync(string phone)
        {
            return await _context.Users.AnyAsync(u => u.Phone == phone);
        }
        #endregion

        #region IsEmailExistsAsync
        public async Task<bool> IsEmailExistsAsync(string email, int userId)
        {
            return await _context.Users.AnyAsync(u => u.Email == email && u.Userid != userId);
        }
        #endregion

        #region IsUsernameExistsAsync
        public async Task<bool> IsUsernameExistsAsync(string username, int userId)
        {
            return await _context.Users.AnyAsync(u => u.Username == username && u.Userid != userId);
        }
        #endregion

        #region IsPhoneExistsAsync
        public async Task<bool> IsPhoneExistsAsync(string phone, int userId)
        {
            return await _context.Users.AnyAsync(u => u.Phone == phone && u.Userid != userId);
        }
        #endregion
    }
}

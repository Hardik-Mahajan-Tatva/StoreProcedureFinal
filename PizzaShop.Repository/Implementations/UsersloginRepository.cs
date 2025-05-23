using System.Text;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations
{
    public class UsersloginRepository : IUsersloginRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public UsersloginRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region GetUserLoginAsync
        public async Task<Userslogin?> GetUserLoginAsync(string userEmail, string userPassword)
        {
            var hashedPassword = HashPassword(userPassword);
            var user = await _context.Userslogins
                .Include(u => u.User)
                .FirstOrDefaultAsync(
                    u =>
                        u.Email == userEmail
                        && u.Passwordhash == hashedPassword
                        && u.Isfirstlogin == false
                        && u.User!.Isdeleted == false
                        && u.User.Status == 1
                );
            return user;
        }
        #endregion

        #region GetUserAsync
        public async Task<Userslogin?> GetUserAsync(string userEmail)
        {
            var user = await _context.Userslogins
                .Include(u => u.User)
                .FirstOrDefaultAsync(
                    u => u.Email == userEmail && u.User!.Isdeleted == false && u.User.Status == 1
                );
            return user;
        }
        #endregion

        #region GetUserByEmailAsync
        public async Task<Userslogin?> GetUserByEmailAsync(string userEmail)
        {
            return await _context.Userslogins.FirstOrDefaultAsync(u => u.Email == userEmail);
        }
        #endregion

        #region GetUserByIdAsync
        public async Task<Userslogin?> GetUserByIdAsync(string userId)
        {
            return await _context.Userslogins.FirstOrDefaultAsync(
                u => u.Userid.ToString() == userId
            );
        }
        #endregion

        #region SavePasswordResetToken
        public async Task SavePasswordResetToken(
            int userId,
            string passwordResetToken,
            DateTime expiration,
            bool isUsed
        )
        {
            var user = await _context.Userslogins.FindAsync(userId);
            if (user != null)
            {
                user.Resettoken = passwordResetToken;
                user.Resettokenexpiration = expiration;
                user.Resettokenused = isUsed;
                await _context.SaveChangesAsync();
            }
        }
        #endregion

        #region GetPasswordResetToken
        public async Task<PasswordResetToken?> GetPasswordResetToken(string passwordResetToken)
        {
            return await _context.PasswordResetTokens.FirstOrDefaultAsync(
                t => t.Token == passwordResetToken
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
        public async Task UpdateUserAsync(Userslogin userslogin)
        {
            _context.Userslogins.Update(userslogin);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region GetUserByResetToken
        public async Task<Userslogin?> GetUserByResetToken(string resetToken)
        {
            return await _context.Userslogins.FirstOrDefaultAsync(u => u.Resettoken == resetToken);
        }
        #endregion

        #region SetUserPassword
        public async Task<bool> SetUserPassword(int userLoginId, string newPassword)
        {
            var user = await _context.Userslogins.FindAsync(userLoginId);
            if (user == null)
            {
                return false;
            }
            var userTable = await _context.Users.FindAsync(user.Userid);
            if (userTable == null)
            {
                return false;
            }
            user.Passwordhash = HashPassword(newPassword);
            userTable.Password = HashPassword(newPassword);

            _context.Userslogins.Update(user);
            _context.Users.Update(userTable);
            await _context.SaveChangesAsync();

            return true;
        }
        #endregion

        #region InvalidateResetToken
        public async Task InvalidateResetToken(Userslogin userslogin)
        {
            userslogin.Resettokenused = true;
            _context.Userslogins.Update(userslogin);
            await _context.SaveChangesAsync();
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

        #region CreateUserLoginAsync
        public async Task CreateUserLoginAsync(Userslogin userslogin)
        {
            await _context.Userslogins.AddAsync(userslogin);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}

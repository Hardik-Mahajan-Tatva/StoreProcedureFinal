using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace PizzaShop.Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public UserRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region GetAllUsers
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }
        #endregion

        #region GetUserById
        public async Task<User?> GetUserById(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }
        #endregion

        #region GetUserByEmail
        public async Task<User?> GetUserByEmail(string userEmail)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        }
        #endregion

        #region CreateUser
        public async Task<bool> CreateUser(User user)
        {
            await _context.Users.AddAsync(user);
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion

        #region UpdateUser
        public async Task<bool> UpdateUser(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion

        #region DeleteUser
        public async Task<bool> DeleteUser(int userId)
        {
            User? user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
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
    }
}

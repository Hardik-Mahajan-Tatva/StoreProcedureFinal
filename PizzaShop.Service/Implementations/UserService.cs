using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        #region Constructor
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        #endregion
        #region Get All Users
        public async Task<IEnumerable<UserViewModel>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();
            return users.Select(u => new UserViewModel
            {
                Id = u.Userid, 
                Firstname = u.Firstname, 
                Lastname = u.Lastname,
                Email = u.Email ?? "test@gmail.com",
                Username = u.Username ?? "DefaultUser",
                Phone = u.Phone,
                CountryId = u.Countryid,
                StateId = u.Stateid,
                CityId = u.Cityid,
                Address = u.Address,
                Zipcode = u.Zipcode,
                Roles = (Roles)u.Roleid,
                ProfilePicture = null 
            });
        }
        #endregion
        #region Get User By Id
        public async Task<UserViewModel?> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null) return null;

            return new UserViewModel
            {
                Id = user.Userid,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email ?? "test@gmail.com",
                Username = user.Username ?? "DefaultUser",
                Phone = user.Phone,
                CountryId = user.Countryid,
                StateId = user.Stateid,
                CityId = user.Cityid,
                Address = user.Address,
                Zipcode = user.Zipcode,
                Roles = (Roles)user.Roleid,
                ProfilePicture = null 
            };
        }
        #endregion
        #region Create User
        public async Task<bool> CreateUser(UserViewModel user)
        {
            var newUser = new User
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Username = user.Username,
                Password = user.Password, 
                Phone = user.Phone ?? "null",
                Countryid = user.CountryId,
                Stateid = user.StateId,
                Cityid = user.CityId,
                Address = user.Address,
                Zipcode = user.Zipcode,
                Roleid = user.Roleid,
                Createdat = DateTime.UtcNow,
                Createdby = 1, 
                Isdeleted = false
            };

            return await _userRepository.CreateUser(newUser);
        }
        #endregion
        #region Update User
        public async Task<bool> UpdateExistingUser(UserViewModel user)
        {
            var existingUser = await _userRepository.GetUserById(user.Id);
            if (existingUser == null) return false;

            existingUser.Firstname = user.Firstname;
            existingUser.Lastname = user.Lastname;
            existingUser.Email = user.Email;
            existingUser.Username = user.Username;
            existingUser.Phone = user.Phone ?? "null";
            existingUser.Countryid = user.CountryId;
            existingUser.Stateid = user.StateId;
            existingUser.Cityid = user.CityId;
            existingUser.Address = user.Address;
            existingUser.Zipcode = user.Zipcode;
            existingUser.Roleid = user.Roleid;
            existingUser.Modifiedat = DateTime.UtcNow;
            existingUser.Modifiedby = 1; 

            return await _userRepository.UpdateUser(existingUser);
        }
        #endregion
        #region Delete User
        public async Task<bool> DeleteUser(int id)
        {
            return await _userRepository.DeleteUser(id);
        }
        #endregion
    }
}

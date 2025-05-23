using Microsoft.AspNetCore.Http;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IUsersLoginService _usersLoginRepository;
        private readonly IRolesRepository _roleRepository;
        private readonly IImageService _imageService;

        public UsersService(IUsersRepository userRepository, IRolesRepository roleRepository, IUsersLoginService usersLoginRepository, IImageService imageService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _usersLoginRepository = usersLoginRepository;
            _imageService = imageService;
        }

        public async Task<bool> CreateNewUser(CreateUserViewModel createUserViewModel, IFormFile itemImage)
        {

            var rolename = _roleRepository.GetRoleNameByIdAsync(createUserViewModel.Roleid).Result;

            if (createUserViewModel.Role == null && rolename != null)
            {
                createUserViewModel.Role = rolename.ToString();
            }
            var itemImgPath = await _imageService.ImgPath(itemImage);
            var user = new User
            {
                Firstname = createUserViewModel.FirstName ?? string.Empty,
                Lastname = createUserViewModel.LastName ?? string.Empty,
                Username = createUserViewModel.Username,
                Roleid = createUserViewModel.Roleid,
                Status = 1,
                Email = createUserViewModel.Email,
                Password = createUserViewModel.Password,
                Zipcode = createUserViewModel.Zipcode,
                Address = createUserViewModel.Address,
                Phone = createUserViewModel.Phone ?? string.Empty,
                Countryid = createUserViewModel.Countryid,
                Stateid = createUserViewModel.Stateid,
                Cityid = createUserViewModel.Cityid,
                Profileimg = itemImgPath
            };

            if (user != null)
            {
                var created = await _userRepository.CreateUser(user);
                if (created)
                {
                    var userId = user.Userid;
                    var login = new UsersLoginViewModel
                    {
                        Email = user.Email ?? string.Empty,
                        Userid = userId,
                        Username = user.Username ?? string.Empty,
                        Passwordhash = user.Password ?? string.Empty,
                        Roleid = user.Roleid,
                        Status = (Status)1,
                    };

                    await _usersLoginRepository.CreateUserLoginAsync(login);
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteExistingUser(int id)
        {
            var result = await _userRepository.SoftDeleteUserAsync(id);
            return result;
        }

        public async Task<bool> DeleteUser(int id)
        {
            return await _userRepository.DeleteUser(id);
        }

        public IQueryable<User> GetAllUsers()
        {
            return _userRepository.GetAll();
        }

        public async Task<UpdateUserViewModel?> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user != null)
            {
                var updateview = new UpdateUserViewModel
                {
                    FirstName = user.Firstname,
                    LastName = user.Lastname,
                    Username = user.Username ?? string.Empty,
                    RoleId = user.Roleid,
                    Email = user.Email,
                    Password = user.Password,
                    Countryid = user.Countryid,
                    Stateidid = user.Stateid,
                    Cityidid = user.Cityid,
                    Address = user.Address ?? string.Empty,
                    Zipcode = user.Zipcode,
                    Phone = user.Phone,
                    Status = user.Status.HasValue ? (UserStatus)user.Status : UserStatus.Active,
                    ProfileImage = user.Profileimg
                };
                return updateview;
            }
            else
            {
                return null;

            }
        }
        public async Task<bool> UpdateExixtingUser(UpdateUserViewModel updateUserViewModel, int id, IFormFile itemImage)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (itemImage != null)
            {
                user!.Profileimg = await _imageService.ImgPath(itemImage);
            }
            else
            {
                user!.Profileimg = updateUserViewModel.ProfileImage;
            }


            if (user != null)
            {
                user.Firstname = updateUserViewModel.FirstName;
                user.Lastname = updateUserViewModel.LastName;
                user.Username = updateUserViewModel.Username;
                user.Roleid = updateUserViewModel.RoleId;
                user.Email = user.Email;
                user.Status = (int?)updateUserViewModel.Status;
                user.Countryid = updateUserViewModel.Countryid;
                user.Stateid = updateUserViewModel.Stateidid;
                user.Cityid = updateUserViewModel.Cityidid;
                user.Address = updateUserViewModel.Address;
                user.Zipcode = updateUserViewModel.Zipcode;
                user.Phone = updateUserViewModel.Phone;
                await _userRepository.UpdateUserAsync(user);
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<(List<User>, int)> GetUsersWithPaginationAsync(int page, int pageSize, string search)
        {
            int totalUsers = await _userRepository.GetTotalUsersCountAsync(search);
            var users = await _userRepository.GetPaginatedUsersAsync(page, pageSize, search);
            return (users, totalUsers);
        }
        public async Task<int?> GetUserIdByEmailAsync(string email)
        {
            return await _userRepository.GetUserIdByEmailAsync(email);
        }
        public async Task<Dictionary<string, string>> ValidateUniqueFieldsAsync(CreateUserViewModel model)
        {
            var errors = new Dictionary<string, string>();
            if (model.Email != null)
                if (await _userRepository.IsEmailExistsAsync(model.Email))
                    errors.Add(nameof(model.Email), "Email already exists.");

            if (await _userRepository.IsUsernameExistsAsync(model.Username ?? string.Empty))
                errors.Add(nameof(model.Username), "Username already exists.");
            if (model.Phone != null)
                if (await _userRepository.IsPhoneExistsAsync(model.Phone))
                    errors.Add(nameof(model.Phone), "Phone number already exists.");

            return errors;
        }
        public async Task<Dictionary<string, string>> ValidateUniqueFieldsAsync(UpdateUserViewModel model, int userId)
        {
            var errors = new Dictionary<string, string>();

            if (await _userRepository.IsEmailExistsAsync(model.Email ?? string.Empty, userId))
                errors.Add(nameof(model.Email), "Email already exists.");

            if (await _userRepository.IsUsernameExistsAsync(model.Username, userId))
                errors.Add(nameof(model.Username), "Username already exists.");

            if (await _userRepository.IsPhoneExistsAsync(model.Phone, userId))
                errors.Add(nameof(model.Phone), "Phone number already exists.");

            return errors;
        }

    }
}


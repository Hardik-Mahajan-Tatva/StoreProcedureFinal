using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class MyProfileService : IMyProfileService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IImageService _imageService;
        public MyProfileService(IUsersRepository usersRepository, IWebHostEnvironment hostingEnvironment, IImageService imageService)
        {
            _usersRepository = usersRepository;
            _hostingEnvironment = hostingEnvironment;
            _imageService = imageService;
        }

        public async Task<int> GetProfileUserIdAsync(string email)
        {
            var user = await _usersRepository.GetUserByEmailAsync(email);
            var userid = user!.Userid;
            if (userid == 0) return 0;
            return userid;
        }
        public async Task<MyProfileViewModel?> GetProfileAsync(int userId)
        {
            var user = await _usersRepository.GetUserByIdAsync(userId);
            if (user == null) return null;

            return new MyProfileViewModel
            {
                Id = user.Userid,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Username = user.Username ?? string.Empty,
                Phone = user.Phone,
                Countryid = user.Countryid,
                Stateid = user.Stateid,
                Cityid = user.Cityid,
                Address = user.Address ?? string.Empty,
                Zipcode = user.Zipcode,
                ProfileImage = user.Profileimg,
                Role = (Roles)user.Roleid,
                Email = user.Email
            };
        }
        public async Task<MyProfileViewModel?> UpdateProfileAsync(int userId, MyProfileViewModel updateProfileViewModel)
        {
            var user = await _usersRepository.GetUserByIdAsync(userId);
            if (user == null) return null;

            user.Firstname = updateProfileViewModel.FirstName;
            user.Lastname = updateProfileViewModel.LastName;
            user.Username = updateProfileViewModel.Username;
            user.Phone = updateProfileViewModel.Phone;
            user.Countryid = updateProfileViewModel.Countryid;
            user.Stateid = updateProfileViewModel.Stateid;
            user.Cityid = updateProfileViewModel.Cityid;
            user.Address = updateProfileViewModel.Address;
            user.Zipcode = updateProfileViewModel.Zipcode;
            await _usersRepository.UpdateUserAsync(user);
            return await GetProfileAsync(userId);
        }
        public async Task<bool> ChangePasswordAsyn(int userId, ChangePasswordViewModel changePasswordViewModel)
        {
            var user = await _usersRepository.GetUserByIdAsync(userId);
            if (user == null) return false;


            if (user.Password != changePasswordViewModel.CurrentPassword)
                return false;

            user.Password = changePasswordViewModel.NewPassword;

            await _usersRepository.UpdateUserAsync(user);

            return true;
        }
        public async Task<bool> IsUsernameTakenAsync(string username, int currentUserId)
        {
            var existingUser = await _usersRepository.GetUserByUsernameAsync(username);
            return existingUser != null && existingUser.Userid != currentUserId;
        }

        public async Task<bool> UploadProfileImageAsync(int userId, IFormFile file)
        {
            try
            {
                var itemImgPath = await _imageService.ImgPath(file);
                var user = await _usersRepository.GetUserByIdAsync(userId);
                if (user == null || file == null || file.Length == 0)
                    return false;

                var email = user.Email;
                var userName = user.Username;


                await _usersRepository.UpdateProfileImageAsync(userId, itemImgPath);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteProfileImageAsync(int userId)
        {
            return await _usersRepository.DeleteProfileImageAsync(userId);
        }
        public async Task<string?> GetProfileImagePathAsync(int userId)
        {
            var user = await _usersRepository.GetUserByIdAsync(userId);
            return user?.Profileimg;
        }

    }
}

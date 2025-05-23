using Microsoft.AspNetCore.Http;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces
{
    public interface IMyProfileService
    {
        /// <summary>
        /// Retrieves the profile details of a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve the profile for.</param>
        /// <returns>A task that returns the user's profile view model.</returns>
        Task<MyProfileViewModel?> GetProfileAsync(int userId);

        /// <summary>
        /// Updates the profile details of a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to update the profile for.</param>
        /// <param name="myProfileViewModel">The view model containing updated profile details.</param>
        /// <returns>A task that returns the updated profile view model.</returns>
        Task<MyProfileViewModel?> UpdateProfileAsync(int userId, MyProfileViewModel myProfileViewModel);

        /// <summary>
        /// Changes the password of a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to change the password for.</param>
        /// <param name="changePasswordViewModel">The view model containing the current and new password details.</param>
        /// <returns>A task that returns true if the password change was successful, otherwise false.</returns>
        Task<bool> ChangePasswordAsyn(int userId, ChangePasswordViewModel changePasswordViewModel);

        /// <summary>
        /// Retrieves the user ID associated with a specific email asynchronously.
        /// </summary>
        /// <param name="email">The email of the user to retrieve the ID for.</param>
        /// <returns>A task that returns the user ID.</returns>
        Task<int> GetProfileUserIdAsync(string email);

        /// <summary>
        /// Checks if a username is already taken by another user asynchronously.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <param name="currentUserId">The ID of the current user to exclude from the check.</param>
        /// <returns>A task that returns true if the username is taken, otherwise false.</returns>
        Task<bool> IsUsernameTakenAsync(string username, int currentUserId);

        /// <summary>
        /// Uploads a profile image for a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to upload the profile image for.</param>
        /// <param name="file">The image file to upload.</param>
        /// <returns>A task that returns true if the upload was successful, otherwise false.</returns>
        Task<bool> UploadProfileImageAsync(int userId, IFormFile file);

        /// <summary>
        /// Deletes the profile image of a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to delete the profile image for.</param>
        /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
        Task<bool> DeleteProfileImageAsync(int userId);

        /// <summary>
        /// Retrieves the file path of a user's profile image asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve the profile image path for.</param>
        /// <returns>A task that returns the file path of the profile image, or null if not found.</returns>
        Task<string?> GetProfileImagePathAsync(int userId);
    }
}



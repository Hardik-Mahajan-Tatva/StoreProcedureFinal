using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces
{
    public interface IUsersRepository
    {
        /// <summary>
        /// Retrieves a user by their email and password asynchronously.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A task that returns the user if found, otherwise null.</returns>
        Task<User?> GetUsersAsync(string email, string password);

        /// <summary>
        /// Retrieves a user by their email asynchronously.
        /// </summary>
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns>A task that returns the user if found, otherwise null.</returns>
        Task<User?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Saves a password reset token for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="token">The reset token to save.</param>
        /// <param name="expiryTime">The expiration time of the token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SavePasswordResetToken(int userId, string token, DateTime expiryTime);

        /// <summary>
        /// Retrieves a password reset token by its value.
        /// </summary>
        /// <param name="token">The token to retrieve.</param>
        /// <returns>A task that returns the password reset token if found, otherwise null.</returns>
        Task<PasswordResetToken?> GetPasswordResetToken(string token);

        /// <summary>
        /// Deletes a password reset token by its value.
        /// </summary>
        /// <param name="token">The token to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeletePasswordResetToken(string token);

        /// <summary>
        /// Updates a user's information asynchronously.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
        Task<bool> UpdateUserAsync(User user);

        /// <summary>
        /// Retrieves a user by their ID asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>A task that returns the user if found, otherwise null.</returns>
        Task<User?> GetUserByIdAsync(int userId);

        /// <summary>
        /// Updates a user's password asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="newPassword">The new password to set.</param>
        /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
        Task<bool> UpdatePasswordAsync(int userId, string newPassword);

        /// <summary>
        /// Retrieves all users as an IQueryable for further filtering or querying.
        /// </summary>
        /// <returns>An IQueryable of all users.</returns>
        IQueryable<User> GetAll();

        /// <summary>
        /// Creates a new user asynchronously.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <returns>A task that returns true if the creation was successful, otherwise false.</returns>
        Task<bool> CreateUser(User user);

        /// <summary>
        /// Soft deletes a user by their ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the user to soft delete.</param>
        /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
        Task<bool> SoftDeleteUserAsync(int id);

        /// <summary>
        /// Deletes a user by their ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
        Task<bool> DeleteUser(int id);

        /// <summary>
        /// Retrieves a paginated list of users based on search criteria.
        /// </summary>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of users per page.</param>
        /// <param name="search">The search term to filter users.</param>
        /// <returns>A task that returns a list of users.</returns>
        Task<List<User>> GetPaginatedUsersAsync(int page, int pageSize, string search);

        /// <summary>
        /// Retrieves the total count of users based on search criteria.
        /// </summary>
        /// <param name="search">The search term to filter users.</param>
        /// <returns>A task that returns the total count of users.</returns>
        Task<int> GetTotalUsersCountAsync(string search);

        /// <summary>
        /// Adds a user login asynchronously.
        /// </summary>
        /// <param name="userslogin">The user login to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddUserlogin(Userslogin userslogin);

        /// <summary>
        /// Retrieves the ID of a user by their email asynchronously.
        /// </summary>
        /// <param name="email">The email of the user to retrieve the ID for.</param>
        /// <returns>A task that returns the user ID if found, otherwise null.</returns>
        Task<int?> GetUserIdByEmailAsync(string email);

        /// <summary>
        /// Retrieves a user by their username asynchronously.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>A task that returns the user if found, otherwise null.</returns>
        Task<User?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Updates the profile image of a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="imagePath">The path of the new profile image.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateProfileImageAsync(int userId, string imagePath);

        /// <summary>
        /// Deletes the profile image of a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to delete the profile image for.</param>
        /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
        Task<bool> DeleteProfileImageAsync(int userId);

        /// <summary>
        /// Checks if an email exists in the database.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns>A task that returns true if the email exists, otherwise false.</returns>
        Task<bool> IsEmailExistsAsync(string email);

        /// <summary>
        /// Checks if a username exists in the database.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>A task that returns true if the username exists, otherwise false.</returns>
        Task<bool> IsUsernameExistsAsync(string username);

        /// <summary>
        /// Checks if a phone number exists in the database.
        /// </summary>
        /// <param name="phone">The phone number to check.</param>
        /// <returns>A task that returns true if the phone number exists, otherwise false.</returns>
        Task<bool> IsPhoneExistsAsync(string phone);

        /// <summary>
        /// Checks if an email exists in the database, excluding a specific user by ID.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <param name="userId">The ID of the user to exclude from the check.</param>
        /// <returns>A task that returns true if the email exists, otherwise false.</returns>
        Task<bool> IsEmailExistsAsync(string email, int userId);

        /// <summary>
        /// Checks if a username exists in the database, excluding a specific user by ID.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <param name="userId">The ID of the user to exclude from the check.</param>
        /// <returns>A task that returns true if the username exists, otherwise false.</returns>
        Task<bool> IsUsernameExistsAsync(string username, int userId);

        /// <summary>
        /// Checks if a phone number exists in the database, excluding a specific user by ID.
        /// </summary>
        /// <param name="phone">The phone number to check.</param>
        /// <param name="userId">The ID of the user to exclude from the check.</param>
        /// <returns>A task that returns true if the phone number exists, otherwise false.</returns>
        Task<bool> IsPhoneExistsAsync(string phone, int userId);
    }
}

using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a list of all users.
        /// </summary>
        /// <returns>A task that returns a collection of all users.</returns>
        Task<IEnumerable<User>> GetAllUsers();

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>A task that returns the user if found, otherwise null.</returns>
        Task<User?> GetUserById(int userId);

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>A task that returns the user if found, otherwise null.</returns>
        Task<User?> GetUserByEmail(string userEmail);

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <returns>A task that returns true if the creation was successful, otherwise false.</returns>
        Task<bool> CreateUser(User user);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
        Task<bool> UpdateUser(User user);

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
        Task<bool> DeleteUser(int userId);

        /// <summary>
        /// Retrieves the ID of a user by their email address asynchronously.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve the ID for.</param>
        /// <returns>A task that returns the user ID if found, otherwise null.</returns>
        Task<int?> GetUserIdByEmailAsync(string userEmail);
    }
}

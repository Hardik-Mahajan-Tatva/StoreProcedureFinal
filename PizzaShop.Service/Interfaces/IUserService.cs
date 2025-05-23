using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Retrieves a list of all users asynchronously.
        /// </summary>
        /// <returns>A task that returns a collection of user view models.</returns>
        Task<IEnumerable<UserViewModel>> GetAllUsers();

        /// <summary>
        /// Retrieves a user by their ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>A task that returns the user view model if found, otherwise null.</returns>
        Task<UserViewModel?> GetUserById(int id);

        /// <summary>
        /// Creates a new user asynchronously.
        /// </summary>
        /// <param name="user">The view model containing user details.</param>
        /// <returns>A task that returns true if the creation was successful, otherwise false.</returns>
        Task<bool> CreateUser(UserViewModel user);

        /// <summary>
        /// Updates an existing user asynchronously.
        /// </summary>
        /// <param name="user">The view model containing updated user details.</param>
        /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
        Task<bool> UpdateExistingUser(UserViewModel user);

        /// <summary>
        /// Deletes a user by their ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
        Task<bool> DeleteUser(int id);
    }
}



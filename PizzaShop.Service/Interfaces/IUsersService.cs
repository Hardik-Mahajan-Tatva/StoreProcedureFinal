using Microsoft.AspNetCore.Http;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces
{
    public interface IUsersService
    {
        /// <summary>
        /// Retrieves all users as an IQueryable for further filtering or querying.
        /// </summary>
        /// <returns>An IQueryable of all users.</returns>
        IQueryable<User> GetAllUsers();

        /// <summary>
        /// Creates a new user asynchronously.
        /// </summary>
        /// <param name="createUserViewModel">The view model containing user details.</param>
        /// <returns>A task that returns true if the creation was successful, otherwise false.</returns>
        Task<bool> CreateNewUser(CreateUserViewModel createUserViewModel, IFormFile itemImage);

        /// <summary>
        /// Retrieves a user by their ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>A task that returns the user view model if found, otherwise null.</returns>
        Task<UpdateUserViewModel?> GetUserById(int id);

        /// <summary>
        /// Updates an existing user asynchronously.
        /// </summary>
        /// <param name="updateUserViewModel">The view model containing updated user details.</param>
        /// <param name="id">The ID of the user to update.</param>
        /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
        Task<bool> UpdateExixtingUser(UpdateUserViewModel updateUserViewModel, int id, IFormFile itemImage);

        /// <summary>
        /// Deletes an existing user by their ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
        Task<bool> DeleteExistingUser(int id);

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
        /// <returns>A task that returns a tuple containing the list of users and the total count.</returns>
        Task<(List<User>, int)> GetUsersWithPaginationAsync(int page, int pageSize, string search);

        /// <summary>
        /// Retrieves the ID of a user by their email asynchronously.
        /// </summary>
        /// <param name="email">The email of the user to retrieve the ID for.</param>
        /// <returns>A task that returns the user ID if found, otherwise null.</returns>
        Task<int?> GetUserIdByEmailAsync(string email);

        /// <summary>
        /// Validates the uniqueness of fields for a new user asynchronously.
        /// </summary>
        /// <param name="model">The view model containing user details.</param>
        /// <returns>A task that returns a dictionary of validation errors, if any.</returns>
        Task<Dictionary<string, string>> ValidateUniqueFieldsAsync(CreateUserViewModel model);

        /// <summary>
        /// Validates the uniqueness of fields for an existing user asynchronously.
        /// </summary>
        /// <param name="model">The view model containing updated user details.</param>
        /// <param name="userId">The ID of the user to exclude from validation.</param>
        /// <returns>A task that returns a dictionary of validation errors, if any.</returns>
        Task<Dictionary<string, string>> ValidateUniqueFieldsAsync(UpdateUserViewModel model, int userId);
    }
}


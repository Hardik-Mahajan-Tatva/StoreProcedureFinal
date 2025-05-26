using PizzaShop.Repository.Models;

namespace Pizzashop.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        /// <summary>
        /// Retrieves all categories from the database.
        /// </summary>
        /// <returns>A collection of all categories.</returns>
        Task<List<Category>> GetAllCategoriesAsync();

        /// <summary>
        /// Adds a new category asynchronously to the database.
        /// </summary>
        /// <param name="category">The category entity to add.</param>
        /// <returns>A task the categoryId of the new created category the asynchronous operation.</returns>
        Task<int> AddCategoryAsync(Category category);

        /// <summary>
        /// Checks if a category with the specified name exists in the database.
        /// </summary>
        /// <param name="categoryName">The name of the category to check.</param>
        /// <returns>A task that returns true if the category exists, otherwise false.</returns>
        Task<bool> IsCategoryNameExistsAsync(string categoryName);

        /// <summary>
        /// Checks if a category with the specified name exists, excluding a specific categoryID.
        /// </summary>
        /// <param name="categoryName">The name of the category to check.</param>
        /// <param name="excludeCategoryId">The CategoryId of the category to exclude from the check.</param>
        /// <returns>A task that returns true if the category name and categoryId exists in the database, otherwise false.</returns>
        Task<bool> DoesCategoryExistAsync(string categoryName, int excludeCategoryId);

        /// <summary>
        /// Retrieves a category by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the category to retrieve.</param>
        /// <returns>A task that returns the category if found in the database, otherwise null.</returns>
        Task<Category?> GetCategoryByIdAsync(int categoryId);

        /// <summary>
        /// Updates an existing category asynchronously in the database.
        /// </summary>
        /// <param name="category">The category to update.</param>
        /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
        Task<bool> UpdateCategoryAsync(Category category);

        /// <summary>
        /// Deletes a category by its CategoryId asynchronously in the database(Soft Delete).
        /// </summary>
        /// <param name="categoryId">The ID of the category to delete.</param>
        /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
        Task<bool> SoftDeleteCategoryAsync(int categoryId);

        /// <summary>
        /// Saves changes to the data base asynchronously(For the category list view "Order" change feature).
        /// </summary>
        /// <returns>A task representing the asynchronous save operation.</returns>
        Task SaveChangesAsync();

        /// <summary>
        /// Retrieves the name of a category by its categoryId.
        /// </summary>
        /// <param name="categoryId">The ID of the category.</param>
        /// <returns>The name of the category if found in the database, otherwise null.</returns>
        Task<string?> GetCategoryNameByCategoryId(int categoryId);

        Task<List<Category>> SPGetAllCategoriesAsync();
        Task<string?> SPGetCategoryNameByCategoryId(int categoryId);
    }
}


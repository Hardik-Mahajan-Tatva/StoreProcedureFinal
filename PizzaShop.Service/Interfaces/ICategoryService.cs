using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces
{
    public interface ICategoryService
    {
        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        /// <returns>A collection of category view models.</returns>
        Task<List<CategoryViewModel>> GetAll();

        /// <summary>
        /// Adds a new category asynchronously.
        /// </summary>
        /// <param name="categoryViewModel">The category view model to add.</param>
        /// <returns>A task the category of the new created category   asynchronous operation.</returns>
        Task<int> AddAsync(CategoryViewModel categoryViewModel);

        /// <summary>
        /// Retrieves a category by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the category to retrieve.</param>
        /// <returns>A task that returns the category view model if found, otherwise null.</returns>
        Task<CategoryViewModel> GetCategoryByIdAsync(int id);

        /// <summary>
        /// Updates an existing category asynchronously.
        /// </summary>
        /// <param name="category">The category view model to update.</param>
        /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
        Task<bool> UpdateAsync(CategoryViewModel category);

        /// <summary>
        /// Deletes a category by its ID asynchronously.
        /// </summary>
        /// <param name="categoryId">The ID of the category to delete.</param>
        /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
        Task<bool> DeleteCategory(int categoryId);

        /// <summary>
        /// Updates the order of categories asynchronously.
        /// </summary>
        /// <param name="orderedCategoryIds">The list of category IDs in the desired order.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateCategoryOrderAsync(List<int> orderedCategoryIds);

        /// <summary>
        /// Retrieves the name of a category by its ID.
        /// </summary>
        /// <param name="categoryId">The ID of the category to retrieve the name for.</param>
        /// <returns>The name of the category if found, otherwise null.</returns>
        Task<string?> GetCategoryNameByCategoryId(int categoryId);
    }
}


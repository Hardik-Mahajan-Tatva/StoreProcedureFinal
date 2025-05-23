using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Repository.Interfaces
{
    public interface ITaxesFeesRepository
    {
        /// <summary>
        /// Adds a new tax asynchronously.
        /// </summary>
        /// <param name="tax">The tax to add.</param>
        /// <returns>A task that returns true if the addition was successful, otherwise false.</returns>
        Task<bool> AddTaxAsync(Taxis tax);

        /// <summary>
        /// Checks if a tax name exists, optionally excluding a specific tax by ID.
        /// </summary>
        /// <param name="taxName">The name of the tax to check.</param>
        /// <param name="taxId">The ID of the tax to exclude from the check (optional).</param>
        /// <returns>A task that returns true if the tax name exists, otherwise false.</returns>
        Task<bool> IsTaxNameExistsAsync(string taxName, int? taxId = null);

        /// <summary>
        /// Retrieves all taxes.
        /// </summary>
        /// <returns>A list of all taxes.</returns>
        List<Taxis> GetAll();

        /// <summary>
        /// Retrieves a tax by its ID.
        /// </summary>
        /// <param name="id">The ID of the tax to retrieve.</param>
        /// <returns>The tax if found, otherwise null.</returns>
        Taxis? GetById(int taxId);

        /// <summary>
        /// Retrieves a tax by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the tax to retrieve.</param>
        /// <returns>A task that returns the tax if found, otherwise null.</returns>
        Task<Taxis?> GetTaxByIdAsync(int taxId);

        /// <summary>
        /// Updates an existing tax.
        /// </summary>
        /// <param name="tax">The tax to update.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        bool Update(Taxis tax);

        /// <summary>
        /// Deletes a tax by its ID asynchronously.
        /// </summary>
        /// <param name="taxId">The ID of the tax to delete.</param>
        /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
        Task<bool> DeleteTax(int taxId);

        /// <summary>
        /// Retrieves a paginated list of taxes and fees based on search criteria and sorting options.
        /// </summary>
        /// <param name="search">The search term to filter taxes and fees.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of taxes and fees per page.</param>
        /// <param name="sortColumn">The column to sort the results by.</param>
        /// <param name="sortDirection">The direction of sorting (e.g., ascending or descending).</param>
        /// <returns>A task that returns a paginated list of taxes and fees.</returns>
        Task<PaginatedList<Taxis>> GetTaxesAndFeesAsync(
            string search,
            int page,
            int pageSize,
            string sortColumn,
            string sortDirection
        );

        /// <summary>
        /// Updates a specific field of a tax asynchronously.
        /// </summary>
        /// <param name="taxId">The ID of the tax to update.</param>
        /// <param name="isChecked">The new value for the field.</param>
        /// <param name="field">The name of the field to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateTaxFieldAsync(int taxId, bool isChecked, string field);

        /// <summary>
        /// Retrieves a list of enabled taxes asynchronously.
        /// </summary>
        /// <returns>A task that returns a list of enabled taxes.</returns>
        Task<List<Taxis>> GetEnabledTaxesAsync();

        /// <summary>
        /// Retrieves the default taxes for a list of items.
        /// </summary>
        /// <param name="itemIds">The list of item IDs to retrieve default taxes for.</param>
        /// <returns>A list of item-specific tax view models.</returns>
        List<ItemSpecificTaxViewModel> GetDefaultTaxesForItems(List<int> itemIds);
    }
}

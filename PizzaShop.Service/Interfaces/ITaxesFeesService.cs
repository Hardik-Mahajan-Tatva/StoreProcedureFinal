using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces
{
    public interface ITaxesFeesService
    {
        /// <summary>
        /// Retrieves a list of all taxes and fees.
        /// </summary>
        /// <returns>A list of taxes and fees view models.</returns>
        List<TaxesFeesViewModel> GetAllTaxes();

        /// <summary>
        /// Adds a new tax asynchronously.
        /// </summary>
        /// <param name="model">The view model containing tax details.</param>
        /// <returns>A task that returns true if the addition was successful, otherwise false.</returns>
        Task<bool> AddTaxAsync(TaxesFeesViewModel model);

        /// <summary>
        /// Checks if a tax name already exists, optionally excluding a specific tax by ID.
        /// </summary>
        /// <param name="taxName">The name of the tax to check.</param>
        /// <param name="taxId">The ID of the tax to exclude from the check (optional).</param>
        /// <returns>A task that returns true if the tax name exists, otherwise false.</returns>
        Task<bool> IsDuplicateTaxNameAsync(string taxName, int? taxId = null);

        /// <summary>
        /// Retrieves a tax by its ID.
        /// </summary>
        /// <param name="id">The ID of the tax to retrieve.</param>
        /// <returns>The view model containing tax details if found, otherwise null.</returns>
        TaxesFeesViewModel GetTaxById(int id);

        /// <summary>
        /// Retrieves a tax by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the tax to retrieve.</param>
        /// <returns>A task that returns the view model containing tax details if found, otherwise null.</returns>
        Task<TaxesFeesViewModel> GetTaxByIdAsync(int id);

        /// <summary>
        /// Updates an existing tax.
        /// </summary>
        /// <param name="model">The view model containing updated tax details.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        bool UpdateTax(TaxesFeesViewModel model);

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
        /// <returns>A task that returns a paginated list of taxes and fees view models.</returns>
        Task<PaginatedList<TaxesFeesViewModel>> GetTaxesAndFeesAsync(
            string search,
            int page,
            int pageSize,
            string sortColumn,
            string sortDirection);

        /// <summary>
        /// Toggles the value of a specific field for a tax asynchronously.
        /// </summary>
        /// <param name="taxId">The ID of the tax to update.</param>
        /// <param name="isChecked">The new value for the field.</param>
        /// <param name="field">The name of the field to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ToggleTaxFieldAsync(int taxId, bool isChecked, string field);
        Task<List<TaxViewModel>> GetEnabledTaxesAsync();
        List<ItemSpecificTaxViewModel> GetDefaultItemTaxes(List<int> itemIds);
    }
}

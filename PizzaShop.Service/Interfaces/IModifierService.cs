using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces
{
    public interface IModifierService
    {
        /// <summary>
        /// Retrieves a list of modifiers by the specified modifier group ID.
        /// </summary>
        /// <param name="modifiergroupId">The ID of the modifier group to retrieve modifiers for.</param>
        /// <returns>A task that returns a collection of modifier view models.</returns>
        Task<IEnumerable<ModifierViewModel>> GetModifiersByModifiergroupId(int modifiergroupId);

        /// <summary>
        /// Retrieves all modifier groups.
        /// </summary>
        /// <returns>A list of all modifier groups.</returns>
        List<Modifiergroup> GetAllModifierGroups();

        /// <summary>
        /// Adds a new modifier.
        /// </summary>
        /// <param name="model">The view model containing modifier details.</param>
        /// <returns>True if the addition was successful, otherwise false.</returns>
        bool AddModifier(AddModifierViewModel model);

        /// <summary>
        /// Retrieves a modifier by its ID.
        /// </summary>
        /// <param name="id">The ID of the modifier to retrieve.</param>
        /// <returns>The view model containing modifier details if found, otherwise null.</returns>
        AddModifierViewModel GetModifierById(int id);

        /// <summary>
        /// Updates an existing modifier asynchronously.
        /// </summary>
        /// <param name="model">The view model containing updated modifier details.</param>
        /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
        Task<bool> UpdateModifierAsync(AddModifierViewModel model);

        /// <summary>
        /// Deletes a modifier by its ID asynchronously.
        /// </summary>
        /// <param name="modifierId">The ID of the modifier to delete.</param>
        /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
        Task<bool> DeleteModifierAsync(int modifierId);

        /// <summary>
        /// Retrieves a list of modifiers by a specific modifier group ID.
        /// </summary>
        /// <param name="modifierGroupId">The ID of the modifier group to retrieve modifiers for.</param>
        /// <returns>A list of modifiers belonging to the specified group.</returns>
        List<Modifier> GetModifiersByGroupId(int modifierGroupId);

        /// <summary>
        /// Retrieves a list of modifiers by multiple modifier group IDs.
        /// </summary>
        /// <param name="modifierGroupIds">The list of modifier group IDs to retrieve modifiers for.</param>
        /// <returns>A list of modifiers belonging to the specified groups.</returns>
        List<Modifier> GetModifiersByGroupIds(List<int> modifierGroupIds);

        /// <summary>
        /// Soft deletes multiple modifiers by their IDs.
        /// </summary>
        /// <param name="modifierIds">The list of modifier IDs to soft delete.</param>
        void SoftDeleteModifiers(List<int> modifierIds);

        /// <summary>
        /// Retrieves a paginated list of modifiers by group ID with an optional search query.
        /// </summary>
        /// <param name="categoryId">The ID of the category to retrieve modifiers for.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of modifiers per page.</param>
        /// <param name="searchQuery">An optional search query to filter modifiers.</param>
        /// <returns>A task that returns a paginated list of modifier view models.</returns>
        Task<PaginatedList<ModifierViewModel>> GetPaginatedModifiersByGroupIdAsync(int categoryId, int pageNumber, int pageSize, string searchQuery = "");

        /// <summary>
        /// Retrieves a paginated list of modifiers with an optional search query.
        /// </summary>
        /// <param name="pageIndex">The page index for pagination.</param>
        /// <param name="pageSize">The number of modifiers per page.</param>
        /// <param name="searchQuery">An optional search query to filter modifiers.</param>
        /// <returns>A task that returns a paginated list of modifier view models.</returns>
        Task<PaginatedList<ModifierViewModel>> GetModifiersAsync(int pageIndex, int pageSize, string searchQuery = "");
        Task<List<int>> GetAllModifierIds(int modifierGroupId);
        Task<List<int>> GetAllModifierIdsExisting();

        Task<List<ModifierViewModel>> GetAllModifierIdsWithNamesAsync();
    }
}


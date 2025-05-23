using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace Pizzashop.Repository.Interfaces
{
    public interface IModifierRepository
    {
        /// <summary>
        /// Retrieves a list of modifiers by the specified modifier group ID.
        /// </summary>
        /// <param name="modifiergroupId">The ID of the modifier group to retrieve modifiers for.</param>
        /// <returns>A task that returns a collection of modifiers belonging to the specified group.</returns>
        Task<IEnumerable<Modifier>> GetModifiersByModifiergroupId(int modifiergroupId);

        /// <summary>
        /// Retrieves a paginated list of modifiers by group ID with an optional search query.
        /// </summary>
        /// <param name="ModifiergroupId">The ID of the modifier group to retrieve modifiers for.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of modifiers per page.</param>
        /// <param name="searchQuery">An optional search query to filter modifiers.</param>
        /// <returns>A task that returns a tuple containing the modifiers and the total count.</returns>
        Task<(IEnumerable<Modifier>, int)> GetPaginatedModifiersByGroupIdAsync(
            int ModifiergroupId,
            int pageNumber,
            int pageSize,
            string searchQuery = ""
        );

        /// <summary>
        /// Retrieves all modifier groups.
        /// </summary>
        /// <returns>A list of all modifier groups.</returns>
        List<Modifiergroup> GetAllModifierGroups();

        /// <summary>
        /// Adds a new modifier and associates it with specified modifier groups.
        /// </summary>
        /// <param name="modifier">The modifier to add.</param>
        /// <param name="modifierGroupIds">The list of modifier group IDs to associate with the modifier.</param>
        /// <returns>True if the addition was successful, otherwise false.</returns>
        bool AddModifier(Modifier modifier, List<int> modifierGroupIds);

        /// <summary>
        /// Retrieves a modifier by its ID.
        /// </summary>
        /// <param name="id">The ID of the modifier to retrieve.</param>
        /// <returns>The modifier if found, otherwise null.</returns>
        Modifier GetModifierById(int modifierId);

        /// <summary>
        /// Updates an existing modifier and its associated modifier groups asynchronously.
        /// </summary>
        /// <param name="modifier">The modifier to update.</param>
        /// <param name="modifierGroupIds">The list of modifier group IDs to associate with the modifier.</param>
        /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
        Task<bool> UpdateModifierAsync(Modifier modifier, List<int> modifierGroupIds);

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
        public void SoftDeleteModifiers(List<int> modifierIds);

        /// <summary>
        /// Retrieves a paginated list of modifiers with an optional search query.
        /// </summary>
        /// <param name="pageIndex">The page index for pagination.</param>
        /// <param name="pageSize">The number of modifiers per page.</param>
        /// <param name="searchQuery">An optional search query to filter modifiers.</param>
        /// <returns>A task that returns a paginated list of modifiers.</returns>
        Task<PaginatedList<Modifier>> GetModifiersAsync(
            int pageIndex,
            int pageSize,
            string searchQuery = ""
        );

        /// <summary>
        /// Adds a mapping between a modifier group and a modifier.
        /// </summary>
        /// <param name="mapping">The mapping to add.</param>
        /// <returns>True if the mapping was added successfully, otherwise false.</returns>
        bool AddModifierGroupMapping(ModifierGroupModifierMapping mapping);

        Task<List<int>> GetAllModifierIds(int modifierGroupId);
        Task<List<int>> GetAllModifierIdsExisting();
        // IModifierService.cs
        Task<List<Modifier>> GetAllModifierIdsWithNamesAsync();

    }
}



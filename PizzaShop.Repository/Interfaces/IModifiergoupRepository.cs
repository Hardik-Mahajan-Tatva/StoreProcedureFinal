using PizzaShop.Repository.Models;

namespace Pizzashop.Repository.Interfaces
{
    public interface IModifiergoupRepository
    {
        /// <summary>
        /// Retrieves all modifier groups.
        /// </summary>
        /// <returns>A collection of all modifier groups.</returns>
        IEnumerable<Modifiergroup> GetAll();

        /// <summary>
        /// Adds a new modifier group asynchronously.
        /// </summary>
        /// <param name="modifierGroup">The modifier group to add.</param>
        /// <returns>A task that returns true if the addition was successful, otherwise false.</returns>
        Task<bool> AddAsync(Modifiergroup modifierGroup);

        /// <summary>
        /// Retrieves a modifier group by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the modifier group to retrieve.</param>
        /// <returns>A task that returns the modifier group if found, otherwise null.</returns>
        Task<Modifiergroup?> GetByIdAsync(int modifierGroupId);

        /// <summary>
        /// Updates an existing modifier group asynchronously.
        /// </summary>
        /// <param name="modifierGroup">The modifier group to update.</param>
        /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
        Task<bool> UpdateAsync(Modifiergroup modifierGroup);

        /// <summary>
        /// Soft deletes a modifier group by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the modifier group to soft delete.</param>
        /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
        Task<bool> SoftDeleteAsync(int modifierGroupId);

        /// <summary>
        /// Retrieves a modifier group by its ID.
        /// </summary>
        /// <param name="modifierGroupId">The ID of the modifier group to retrieve.</param>
        /// <returns>The modifier group if found, otherwise null.</returns>
        Modifiergroup? GetModifierGroupById(int modifierGroupId);

        /// <summary>
        /// Retrieves a list of modifier groups by their IDs.
        /// </summary>
        /// <param name="modifierGroupIds">The list of modifier group IDs to retrieve.</param>
        /// <returns>A list of modifier groups.</returns>
        List<Modifiergroup> GetModifierGroupsByIds(List<int> modifierGroupIds);

        /// <summary>
        /// Saves changes to the data source asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous save operation.</returns>
        Task SaveAsync();

        /// <summary>
        /// Adds mappings between a modifier group and modifiers asynchronously.
        /// </summary>
        /// <param name="modifierGroupId">The ID of the modifier group.</param>
        /// <param name="modifierIds">The list of modifier IDs to map to the group.</param>
        /// <returns>A task that returns true if the mappings were added successfully, otherwise false.</returns>
        Task<bool> AddModifierMappingsAsync(int modifierGroupId, List<int> modifierIds);

        /// <summary>
        /// Updates the mappings between a modifier group and modifiers asynchronously.
        /// </summary>
        /// <param name="modifierGroupId">The ID of the modifier group.</param>
        /// <param name="modifierIds">The list of modifier IDs to update the mappings for.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateModifierGroupMappingsAsync(int modifierGroupId, List<int> modifierIds);

        /// <summary>
        /// Checks if a modifier group with the specified name exists.
        /// </summary>
        /// <param name="name">The name of the modifier group to check.</param>
        /// <returns>A task that returns true if the name exists, otherwise false.</returns>
        Task<bool> ModifierGroupNameExistsAsync(string modifierGroupName);

        /// <summary>
        /// Checks if a modifier group with the specified name exists, excluding a specific modifier group by ID.
        /// </summary>
        /// <param name="name">The name of the modifier group to check.</param>
        /// <param name="excludeId">The ID of the modifier group to exclude from the check.</param>
        /// <returns>A task that returns true if the name exists, otherwise false.</returns>
        Task<bool> ModifierGroupNameExistsAsync(string modifierGroupName, int excludeId);
    }
}



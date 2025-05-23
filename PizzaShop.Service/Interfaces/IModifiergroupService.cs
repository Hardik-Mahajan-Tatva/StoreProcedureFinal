using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces
{
  public interface IModifiergroupService
  {
    /// <summary>
    /// Retrieves all modifier groups.
    /// </summary>
    /// <returns>A collection of modifier group view models.</returns>
    IEnumerable<ModifiergroupViewModel> GetAll();

    /// <summary>
    /// Creates a new modifier group asynchronously.
    /// </summary>
    /// <param name="viewModel">The view model containing modifier group details.</param>
    /// <returns>A task that returns true if the creation was successful, otherwise false.</returns>
    Task<bool> CreateModifierGroupAsync(ModifiergroupViewModel viewModel);

    /// <summary>
    /// Retrieves a modifier group by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the modifier group to retrieve.</param>
    /// <returns>A task that returns the modifier group view model if found, otherwise null.</returns>
    Task<ModifiergroupViewModel> GetModifierGroupByIdAsync(int id);

    /// <summary>
    /// Updates an existing modifier group asynchronously.
    /// </summary>
    /// <param name="model">The view model containing updated modifier group details.</param>
    /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
    Task<bool> UpdateModifierGroupAsync(ModifiergroupViewModel model);

    /// <summary>
    /// Soft deletes a modifier group by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the modifier group to soft delete.</param>
    /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
    Task<bool> SoftDeleteModifierGroupAsync(int id);

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
    /// Updates the order of modifier groups asynchronously.
    /// </summary>
    /// <param name="orderedModifierGroupIds">The list of modifier group IDs in the desired order.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateModifierGroupOrderAsync(List<int> orderedModifierGroupIds);

    /// <summary>
    /// Checks if a modifier group name exists.
    /// </summary>
    /// <param name="name">The name of the modifier group to check.</param>
    /// <returns>A task that returns true if the name exists, otherwise false.</returns>
    Task<bool> ModifierGroupNameExistsAsync(string name);
  }
}



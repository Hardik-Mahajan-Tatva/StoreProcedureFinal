using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IOrderedItemService
{
    /// <summary>
    /// Updates the ready quantities of multiple ordered items asynchronously.
    /// </summary>
    /// <param name="updates">A list of view models containing the ordered item IDs and their updated ready quantities.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateReadyQuantitiesAsync(List<ReadyQuantityUpdateViewModel> updates);

    /// <summary>
    /// Updates the quantities of multiple ordered items asynchronously.
    /// </summary>
    /// <param name="updates">A list of view models containing the ordered item IDs and their updated quantities.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateQuantitiesAsync(List<ReadyQuantityUpdateViewModel> updates);

    /// <summary>
    /// Checks that if the item is available to delete or not.
    /// </summary>
    /// <param name="itemId">A itemId for which the deletion to apply.</param>
    /// <returns>A bool true or flase that if the item is available to delete or not.</returns>
    Task<bool> IsItemAvailableToDelete(int itemId);
}

using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class OrderedItemService : IOrderedItemService
{
    private readonly IOrderedItemRepository _repository;

    public OrderedItemService(IOrderedItemRepository repository)
    {
        _repository = repository;
    }

    // public async Task UpdateReadyQuantitiesAsync(List<ReadyQuantityUpdateViewModel> updates)
    // {
    //     foreach (var item in updates)
    //     {
    //         await _repository.UpdateReadyQuantityAsync(item.OrderedItemId, item.ReadyQuantity);
    //     }
    // }
    public async Task UpdateReadyQuantitiesAsync(List<ReadyQuantityUpdateViewModel> updates)
    {
        await _repository.UpdateReadyQuantitiesAsync(updates);
    }
    // public async Task UpdateQuantitiesAsync(List<ReadyQuantityUpdateViewModel> updates)
    // {
    //     foreach (var item in updates)
    //     {
    //         await _repository.UpdateQuantityAsync(item.OrderedItemId, item.ReadyQuantity);
    //     }
    // }
    public async Task UpdateQuantitiesAsync(List<ReadyQuantityUpdateViewModel> updates)
    {
        await _repository.UpdateQuantitiesAsync(updates);
    }

    public async Task<bool> IsItemAvailableToDelete(int itemId)
    {
        return await _repository.IsItemAvailableToDeleteAsync(itemId);
    }
}

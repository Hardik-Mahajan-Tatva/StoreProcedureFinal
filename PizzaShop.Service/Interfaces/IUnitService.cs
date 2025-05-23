using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IUnitService
{
    /// <summary>
    /// Retrieves a list of all units asynchronously.
    /// </summary>
    /// <returns>A task that returns a collection of unit view models.</returns>
    Task<IEnumerable<UnitViewModel>> GetUnitsAsync();
}

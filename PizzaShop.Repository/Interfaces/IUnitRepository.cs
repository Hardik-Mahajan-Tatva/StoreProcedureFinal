using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces
{
    public interface IUnitRepository
    {
        /// <summary>
        /// Retrieves a list of all units asynchronously.
        /// </summary>
        /// <returns>A task that returns a collection of all units.</returns>
        Task<IEnumerable<Unit>> GetUnitsAsync();
    }
}

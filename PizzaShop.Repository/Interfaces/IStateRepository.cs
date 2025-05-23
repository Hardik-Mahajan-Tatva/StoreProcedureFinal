using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces
{
    public interface IStateRepository
    {
        /// <summary>
        /// Retrieves a list of states by the specified country ID.
        /// </summary>
        /// <param name="countryId">The ID of the country to retrieve states for.</param>
        /// <returns>A task that returns a collection of states belonging to the specified country.</returns>
        Task<IEnumerable<State>> GetStatesByCountryId(int countryId);
    }
}

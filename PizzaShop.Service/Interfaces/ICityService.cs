using PizzaShop.Repository.Models;

namespace PizzaShop.Service.Interfaces
{
    public interface ICityService
    {
        /// <summary>
        /// Retrieves a list of cities by the specified state ID.
        /// </summary>
        /// <param name="stateId">The ID of the state to retrieve cities for.</param>
        /// <returns>A task that returns a collection of cities belonging to the specified state.</returns>
        Task<IEnumerable<City>> GetCitiesByStateId(int stateId);
    }
}

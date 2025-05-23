using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces
{
    public interface ICityRepository
    {
        /// <summary>
        /// Retrieves a list of cities by the specified state ID from the database.
        /// </summary>
        /// <param name="stateId">The ID of the state to retrieve cities for.</param>
        /// <returns>A task that returns a collection of cities belonging to the specified state.</returns>
        Task<List<City>> GetCitiesByStateIdAsyce(int stateId);
    }
}

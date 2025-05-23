using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces
{
    public interface ICountryRepository
    {
        /// <summary>
        /// Retrieves a list of all countries from the database.
        /// </summary>
        /// <returns>A task that returns a collection of all countries.</returns>
        Task<List<Country>> GetAllCountriesAsync();
    }
}

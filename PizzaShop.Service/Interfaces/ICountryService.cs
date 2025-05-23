using PizzaShop.Repository.Models;

namespace PizzaShop.Service.Interfaces
{
    public interface ICountryService
    {
        /// <summary>
        /// Retrieves a list of all countries asynchronously.
        /// </summary>
        /// <returns>A task that returns a collection of all countries.</returns>
        Task<IEnumerable<Country>> GetAllCountries();
    }
}

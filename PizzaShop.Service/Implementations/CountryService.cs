using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;

        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<IEnumerable<Country>> GetAllCountries()
        {
            var test = await _countryRepository.GetAllCountriesAsync();
            return test;
        }
    }
}

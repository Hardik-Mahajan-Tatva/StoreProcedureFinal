using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<IEnumerable<City>> GetCitiesByStateId(int stateId)
        {
            var test = await _cityRepository.GetCitiesByStateIdAsyce(stateId);
            return test;
        }
    }
}

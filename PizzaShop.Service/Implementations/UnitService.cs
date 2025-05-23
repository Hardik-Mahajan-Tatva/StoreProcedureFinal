using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class UnitService : IUnitService
{
    private readonly IUnitRepository _unitRepository;
    public UnitService(IUnitRepository unitRepository)
    {
        _unitRepository = unitRepository;
    }

    public async Task<IEnumerable<UnitViewModel>> GetUnitsAsync()
    {
        var units = await _unitRepository.GetUnitsAsync();
        return units.Select(u => new UnitViewModel
        {
            Unitid = u.Unitid,
            Unitname = u.Unitname,
            ShortName = u.Shortname
        }).ToList();
    }

}

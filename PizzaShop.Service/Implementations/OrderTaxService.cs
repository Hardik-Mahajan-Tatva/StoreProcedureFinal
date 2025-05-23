using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class OrderTaxService : IOrderTaxService
{
    private readonly IOrderTaxRepository _taxMappingRepository;

    public OrderTaxService(IOrderTaxRepository taxMappingRepository)
    {
        _taxMappingRepository = taxMappingRepository;
    }

    public async Task<List<TaxMappingViewModel>> GetTaxMappingsByOrderIdAsync(int orderId)
    {
        var taxMappings = await _taxMappingRepository.GetTaxMappingsByOrderIdAsync(orderId);

        var result = taxMappings.Select(tm => new TaxMappingViewModel
        {
            TaxId = tm.Taxid,
            TaxName = tm.Tax.Taxname,
            TaxValue = (decimal)(tm.Taxvalue ?? 0),
            TaxType = tm.Tax.Taxtype,
            IsInclusive = tm.Tax.Isinclusive ?? false,
            Amount = (decimal)tm.Tax.Taxvalue
        }).ToList();

        return result;
    }



}
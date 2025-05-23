using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IOrderTaxService
{
    Task<List<TaxMappingViewModel>> GetTaxMappingsByOrderIdAsync(int orderId);
}

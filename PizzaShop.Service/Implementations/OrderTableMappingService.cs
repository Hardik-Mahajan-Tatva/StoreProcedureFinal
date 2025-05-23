using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class OrderTableMappingService : IOrderTableMappingService
    {
        private readonly IOrderTableMappingRepository _mappingRepo;

        public OrderTableMappingService(IOrderTableMappingRepository mappingRepo)
        {
            _mappingRepo = mappingRepo;
        }

        public async Task CreateMappingAsync(OrderTableMappingViewModel model)
        {
            try
            {
                var mapping = new Ordertable
                {
                    Orderid = model.OrderId,
                    Tableid = model.TableId
                };

                await _mappingRepo.CreateCustomerOrderToTableMappingAsync(mapping);
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                throw new Exception("An error occurred while mapping table to order. Details: " + error, ex);
            }
        }

    }

}

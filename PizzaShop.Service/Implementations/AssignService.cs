using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;
namespace PizzaShop.Service.Implementations;

public class AssignService : IAssignService
{
    private readonly ICustomerService _customerService;
    private readonly IOrderService _orderService;
    private readonly IOrderTableMappingService _mappingService;
    private readonly ITableService _tableService;
    private readonly IWaitingTokenService _waitingTokenService;
    private readonly ICustomerRepository _customerRepository;
    public AssignService(
        IWaitingTokenService waitingTokenService,
        ICustomerService customerService,
        IOrderService orderService,
        IOrderTableMappingService mappingService,
        ITableService tableService,
        ICustomerRepository customerRepository
        )
    {
        _customerService = customerService;
        _orderService = orderService;
        _mappingService = mappingService;
        _tableService = tableService;
        _waitingTokenService = waitingTokenService;
        _customerRepository = customerRepository;
    }

    public async Task<(bool IsSuccess, string Message, int OrderId)> AssignCustomerOrderAndMappingWithOrderIdAsync(CustomerOrderViewModel model)
    {
        int customerId = model.CustomerId ?? 0;

        if (string.IsNullOrEmpty(model.Email))
        {
            return (false, "Email is required", 0);
        }
        var existingCustomer = await _customerRepository.GetCustomerByNamePhoneAndEmailAsync(model.Email);


        if (existingCustomer != null)
        {
            customerId = existingCustomer.Customerid;
            if (existingCustomer != null)
            {
                existingCustomer.Customername = model.Name ?? string.Empty;
                existingCustomer.Email = model.Email;
                existingCustomer.Phoneno = model.MobileNumber ?? string.Empty;

                var updated = await _customerRepository.UpdateCustomerDataAsync(existingCustomer);
                if (!updated)
                    return (false, "Customer update failed.", 0);
                var waitingToken = await _waitingTokenService.GetWaitingTokenByCustomerIdAsync(customerId);
                if (waitingToken != null)
                {
                    waitingToken.Isassigned = true;
                    await _waitingTokenService.UpdateWaitingTokenStatusAsync(customerId, true);
                }
            }
            else
            {
                customerId = await _customerService.CreateCustomerAsync(new CustomerViewModel
                {
                    CustomerName = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.MobileNumber
                });

                if (customerId <= 0)
                    return (false, "Customer creation failed.", 0);
            }
        }
        else
        {
            customerId = await _customerService.CreateCustomerAsync(new CustomerViewModel
            {
                CustomerName = model.Name,
                Email = model.Email,
                PhoneNumber = model.MobileNumber
            });

            if (customerId <= 0)
                return (false, "Customer creation failed.", 0);
        }

        var order = new OrderViewModel
        {
            CustomerId = customerId,
            NoOfPersons = model.NoOfPersons,
            Status = (OrderStatus)0,
        };

        int orderId = await _orderService.CreateOrderAsync(order);
        if (orderId <= 0)
            return (false, "Order creation failed.", 0);

        if (model.TableIds == null || !model.TableIds.Any())
            return (false, "No tables were selected.", 0);

        foreach (var tableId in model.TableIds)
        {
            var mapping = new OrderTableMappingViewModel
            {
                OrderId = orderId,
                TableId = tableId
            };
            await _mappingService.CreateMappingAsync(mapping);
            await _tableService.UpdateTableStatusAsync(tableId, 2);
        }

        return (true, "Customer, Order, and Mapping created or updated.", orderId);
    }

    public async Task<(bool IsSuccess, string Message, int orderId)> AssignWaitinCustomerSelectedTableAndSectionAsync(AssignTableViewModel model)
    {
        int customerId = model.CustomerId ?? 0;
        if (customerId <= 0)
            return (false, "Customer not found.", 0);

        var order = new OrderViewModel
        {
            CustomerId = customerId,
            Status = (OrderStatus)0,
        };

        int orderId = await _orderService.CreateOrderAsync(order);
        if (orderId <= 0)
            return (false, "Order creation failed.", 0);

        if (model.TableIds == null || !model.TableIds.Any())
            return (false, "No tables were selected.", 0);

        foreach (var tableId in model.TableIds)
        {
            var mapping = new OrderTableMappingViewModel
            {
                OrderId = orderId,
                TableId = tableId
            };
            await _mappingService.CreateMappingAsync(mapping);
            await _tableService.UpdateTableStatusAsync(tableId, 2);
        }

        var waitingToken = await _waitingTokenService.GetWaitingTokenByCustomerIdAsync(customerId);
        if (waitingToken != null)
        {
            waitingToken.Isassigned = true;
            await _waitingTokenService.UpdateWaitingTokenStatusAsync(customerId, true);
        }

        return (true, "Customer, Order, and Mapping created. Waiting token status updated to 'Assigned'.", orderId);
    }

}

using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;
namespace PizzaShop.Service.Implementations;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<CustomerService> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IWaitingTokenRepository _waitingTokenRepository;

    public CustomerService(ICustomerRepository customerRepository, ILogger<CustomerService> logger,
    IOrderRepository orderRepository, IWaitingTokenRepository waitingTokenRepository
    )
    {
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
        _waitingTokenRepository = waitingTokenRepository;
        _logger = logger;
    }

    public async Task<PaginatedList<CustomerViewModel>> GetCustomerAsync(string search, string status, DateTime? startDate, DateTime? endDate, int page, int pageSize, string sortColumn, string sortDirection)
    {
        var customers = await _customerRepository.GetPaginatedCustomersAsync(
             search, status, startDate, endDate, page, pageSize, sortColumn, sortDirection);

        var customerViewModel = customers.Select(c => new CustomerViewModel
        {
            CustomerId = c.Customerid,
            CustomerName = c.Customername,
            CreatedDate = c.Createdat ?? DateTime.Now,
            PhoneNumber = c.Phoneno,
            Email = c.Email,
            TotalOrder = c.Totalorder ?? 0,
        }).ToList();

        return new PaginatedList<CustomerViewModel>(customerViewModel, customers.TotalItems, page, pageSize);
    }


    public async Task<IEnumerable<Customer>> GetFilteredOrders(string searchText, DateTime? startDate, DateTime? endDate, int? orderStatus, string sortColumn, string sortOrder)
    {
        var query = _customerRepository.GetAllCustomersQueryable();
        var startUtc = startDate?.ToUniversalTime();
        var endUtc = endDate?.ToUniversalTime();
        if (!string.IsNullOrEmpty(searchText))
        {
            query = query.Where(o =>
                EF.Functions.ILike(o.Customername, $"%{searchText}%") ||
                EF.Functions.ILike(o.Email!, $"%{searchText}%") ||
                EF.Functions.ILike(o.Customerid.ToString(), $"%{searchText}%"));
        }

        if (startUtc.HasValue)
        {
            query = query.Where(o => o.Createdat.HasValue && o.Createdat.Value >= startUtc.Value);
        }


        if (endUtc.HasValue)
        {
            DateTime endOfDay = endUtc.Value.Date.AddDays(1).AddTicks(-1); // End of day as max ticks on that date
            query = query.Where(o => o.Createdat.HasValue && o.Createdat.Value <= endOfDay);
        }


        switch (sortColumn)
        {
            case "CustomerName":
                query = sortOrder == "asc" ? query.OrderBy(o => o.Customername) : query.OrderByDescending(o => o.Customername);
                break;
            case "OrderDate":
                query = sortOrder == "asc" ? query.OrderBy(o => o.Createdat) : query.OrderByDescending(o => o.Createdat);
                break;
            case "TotalAmount":
                query = sortOrder == "asc" ? query.OrderBy(o => o.Totalorder) : query.OrderByDescending(o => o.Totalorder);
                break;
            default:
                query = sortOrder == "asc" ? query.OrderBy(o => o.Customerid) : query.OrderByDescending(o => o.Customerid);
                break;
        }
        var restult = query.ToListAsync();

        return await restult;
    }



    public async Task<CustomerHistoryViewModel> GetCustomerHistory(int customerId)
    {
        var customer = await _customerRepository.GetCustomerWithOrdersAsync(customerId);
        if (customer == null) return new CustomerHistoryViewModel();

        return new CustomerHistoryViewModel
        {
            Name = customer.Customername,
            PhoneNumber = customer.Phoneno,
            MaxOrder = customer.Orders.Any() ? customer.Orders.Max(o => o.Totalamount) : 0,
            AvgBill = customer.Orders.Any() ? Math.Round(customer.Orders.Average(o => o.Totalamount), 2) : 0,
            ComingSince = customer.Createdat ?? DateTime.Now,
            Visits = customer.Orders.Count,
            Orders = customer.Orders.Select(o => new OrderViewModel
            {
                OrderDate = o.Orderdate ?? DateTime.Now,
                OrderType = o.Ordertype ?? "Dining",
                PaymentMode = o.Paymentmode ?? "NA",
                ItemsCount = o.Ordereditems.Count,
                TotalAmount = o.Totalamount
            }).ToList()
        };
    }
    public async Task<int> CreateCustomerAsync(CustomerViewModel customerViewModel)
    {
        try
        {
            var customer = new Customer
            {
                Customername = customerViewModel.CustomerName!,
                Email = customerViewModel.Email,
                Phoneno = customerViewModel.PhoneNumber!,
            };

            return await _customerRepository.AddCustomerAsync(customer);
        }
        catch (Exception ex)
        {
            _ = ex.InnerException?.Message ?? ex.Message;
            throw new Exception("An error occurred while creating the customer.", ex);
        }
    }
    public async Task<CustomerUpdateViewModal?> GetCustomerDetails(int customerId)
    {
        var customer = await _customerRepository.GetCustomerWithLatestOrderAsync(customerId);

        if (customer == null) return null;

        var latestOrder = customer.Orders.FirstOrDefault();

        return new CustomerUpdateViewModal
        {
            CustomerName = customer.Customername,
            PhoneNumber = customer.Phoneno,
            NumberOfPersons = latestOrder?.Noofperson ?? 0,
            Email = customer.Email
        };
    }

    public async Task<bool> UpdateAsync(CustomerUpdateViewModal customer)
    {
        var entity = await _customerRepository.GetCustomerByCustomerIdAsync(customer.CustomerId);
        if (entity == null)
        {
            throw new KeyNotFoundException("Customer not found.");
        }
        if (customer.Email == null)
        {
            throw new KeyNotFoundException("Customer not found.");
        }
        bool isDuplicate = await _customerRepository.DoesCustomerExistAsync(customer.Email, customer.CustomerId);
        if (isDuplicate)
        {
            throw new InvalidOperationException("A customer with the same email already exists.");
        }
        if (customer.CustomerName == null || customer.PhoneNumber == null)
        {
            throw new KeyNotFoundException("Customer not found.");
        }
        entity.Customername = customer.CustomerName;
        entity.Phoneno = customer.PhoneNumber;
        entity.Email = customer.Email;

        var orders = await _customerRepository.GetOrdersByCustomerIdAsync(customer.CustomerId);
        foreach (var order in orders)
        {
            order.Noofperson = (short?)customer.NumberOfPersons;
        }

        await _customerRepository.UpdateCustomerAsync(entity);
        bool customerUpdated = true;
        bool ordersUpdated = await _orderRepository.UpdateOrdersAsync(orders);

        return customerUpdated && ordersUpdated;
    }

    public async Task<bool> UpdateCustomerAsync(CustomerViewModel customer)
    {

        if (customer == null)
            return false;
        if (customer.CustomerName == null || customer.PhoneNumber == null)
        {
            throw new KeyNotFoundException("Customer not found.");
        }
        var entity = new Customer
        {
            Customerid = customer.CustomerId,
            Customername = customer.CustomerName,
            Email = customer.Email,
            Phoneno = customer.PhoneNumber
        };

        return await _customerRepository.UpdateCustomerDataAsync(entity);
    }

    public async Task<CustomerViewModel?> GetCustomerByIdAsync(int customerId)
    {
        var customer = await _customerRepository.GetCustomerByCustomerIdAsync(customerId);
        if (customer == null) return null;

        return new CustomerViewModel
        {
            CustomerId = customer.Customerid,
            CustomerName = customer.Customername,
            Email = customer.Email,
            PhoneNumber = customer.Phoneno
        };
    }
    public async Task<WaitingTokenViewModel?> GetCustomerWithLatestOrderAsync(string email)
    {
        if (await _waitingTokenRepository.IsEmailAlreadyInWaitingListAsync(email))
        {
            throw new Exception("Customer Already waiting ");
        }
        var customer = await _customerRepository.GetCustomerByEmailAsync(email);
        if (customer == null) return null;

        var latestOrder = await _orderRepository.GetLatestOrderByCustomerIdAsync(customer.Customerid);

        return new WaitingTokenViewModel
        {
            Name = customer.Customername,
            Email = customer.Email,
            MobileNumber = customer.Phoneno,
            NoOfPersons = latestOrder?.Noofperson ?? 0,
        };
    }
    public async Task<bool> HasCustomerOrderWithStatusAsync(string email, int[] statuses)
    {
        var customer = await _customerRepository.GetCustomerByEmailAsync(email);
        if (customer == null) return false;

        return await _orderRepository.HasOrderWithStatusAsync(customer.Customerid, statuses);
    }
    public async Task<FileResult> ExportCustomersToExcel(string searchText, DateTime? startDate, DateTime? endDate, int? orderStatus, string sortColumn, string sortOrder, string webRootPath)
    {
        var customers = await GetFilteredOrders(searchText, startDate, endDate, orderStatus, sortColumn, sortOrder);

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Customers");

            var imagePath = Path.Combine(webRootPath, "images/logos/pizzashop_logo.png");

            if (System.IO.File.Exists(imagePath))
            {
                var mergedRange = worksheet.Range("O2:P6").Merge();
                mergedRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                mergedRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                double mergedWidth = worksheet.Columns("O:P").Sum(c => c.Width) * 7;
                double mergedHeight = worksheet.Rows(2, 6).Sum(r => r.Height);

                var picture = worksheet.AddPicture(imagePath)
                    .MoveTo(worksheet.Cell("O2"))
                    .WithSize((int)mergedWidth, (int)mergedHeight);
            }

            var statusRange = worksheet.Range("A2:B3");
            statusRange.Merge().Value = "Account:";
            statusRange.Style.Font.Bold = true;
            statusRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
            statusRange.Style.Font.FontColor = XLColor.White;
            statusRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            statusRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            statusRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            string statusTextorder = orderStatus switch
            {
                0 => "Account Manager",
                1 => "Chef",
                _ => "Admin"
            };

            var allStatusRange = worksheet.Range("C2:F3");
            allStatusRange.Merge().Value = statusTextorder;
            allStatusRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            allStatusRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            allStatusRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            var searchLabelRange = worksheet.Range("H2:I3");
            searchLabelRange.Merge().Value = "Search Text:";
            searchLabelRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
            searchLabelRange.Style.Font.FontColor = XLColor.White;
            searchLabelRange.Style.Font.Bold = true;
            searchLabelRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            searchLabelRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            searchLabelRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            var searchValueRange = worksheet.Range("J2:M3");
            searchValueRange.Merge().Value = string.IsNullOrEmpty(searchText) ? "" : searchText;
            searchValueRange.Style.Fill.BackgroundColor = XLColor.White;
            searchValueRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            searchValueRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            searchValueRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            var dateLabelRange = worksheet.Range("A5:B6");
            dateLabelRange.Merge().Value = "Date:";
            dateLabelRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
            dateLabelRange.Style.Font.FontColor = XLColor.White;
            dateLabelRange.Style.Font.Bold = true;
            dateLabelRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dateLabelRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            dateLabelRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            var dateValueRange = worksheet.Range("C5:F6");
            string dateValueText;
            if (startDate.HasValue && endDate.HasValue)
                dateValueText = $"{startDate.Value:dd-MM-yyyy} to {endDate.Value:dd-MM-yyyy}";
            else if (startDate.HasValue)
                dateValueText = startDate.Value.ToString("dd-MM-yyyy");
            else if (endDate.HasValue)
                dateValueText = endDate.Value.ToString("dd-MM-yyyy");
            else
                dateValueText = "All Time";
            dateValueRange.Merge().Value = dateValueText;
            dateValueRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dateValueRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            dateValueRange.Style.Fill.BackgroundColor = XLColor.White;
            dateValueRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            var recordsLabelRange = worksheet.Range("H5:I6");
            recordsLabelRange.Merge().Value = "No. Of Records:";
            recordsLabelRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
            recordsLabelRange.Style.Font.FontColor = XLColor.White;
            recordsLabelRange.Style.Font.Bold = true;
            recordsLabelRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            recordsLabelRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            recordsLabelRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            var recordsValueRange = worksheet.Range("J5:M6");
            recordsValueRange.Merge().Value = customers.Count();
            recordsValueRange.Style.Fill.BackgroundColor = XLColor.White;
            recordsValueRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            recordsValueRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            recordsValueRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            var summaryRange = worksheet.Range("A2:M6");
            summaryRange.Style.Font.Bold = true;
            summaryRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            summaryRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            summaryRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Range("A9:A9").Merge().Value = "Customer ID";
            worksheet.Range("B9:D9").Merge().Value = "Customer Name";
            worksheet.Range("E9:H9").Merge().Value = "Email";
            worksheet.Range("I9:K9").Merge().Value = "Date";
            worksheet.Range("L9:N9").Merge().Value = "Mobile Number";
            worksheet.Range("O9:P9").Merge().Value = "Total Order";

            var headerRange = worksheet.Range("A9:P9");
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            int row = 10;
            foreach (var customer in customers)
            {
                worksheet.Cell(row, 1).Value = customer.Customerid;
                worksheet.Range(row, 2, row, 4).Merge().Value = customer.Customername;
                worksheet.Range(row, 5, row, 8).Merge().Value = customer.Email;
                worksheet.Range(row, 9, row, 11).Merge().Value = customer.Createdat?.ToString("dd-MM-yyyy HH:mm:ss") ?? "";
                worksheet.Range(row, 12, row, 14).Merge().Value = customer.Phoneno;
                worksheet.Range(row, 15, row, 16).Merge().Value = customer.Totalorder;
                row++;
            }

            var dataRange = worksheet.Range("A9:P" + (row - 1));
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Columns().AdjustToContents();
            worksheet.Column(1).Width = 15;

            worksheet.Cells().Style.IncludeQuotePrefix = true;

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var fileBytes = stream.ToArray();
                return new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"Orders_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx"
                };
            }
        }
    }

    public async Task<(bool IsSuccess, string Message, int OrderId)> AssignCustomerOrderUsingStoredProcedureAsync(CustomerOrderViewModel model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.Email))
            return (false, "Email is required.", 0);

        if (model.TableIds == null || !model.TableIds.Any())
            return (false, "No tables selected.", 0);

        // Call to Repository which executes the stored procedure
        var (success, message, orderId) = await _customerRepository.AssignCustomerToOrderSPAsync(
            model.Name ?? string.Empty,
            model.Email,
            model.MobileNumber ?? string.Empty,
            model.NoOfPersons,
            model.TableIds.ToArray()
        );

        return (success, message, orderId);
    }



}
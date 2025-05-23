using Microsoft.AspNetCore.Mvc;
using PizzaShop.Service.Attributes;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        #region Constructor
        public CustomersController(ICustomerService customerService, IWebHostEnvironment hostingEnvironment)
        {

            _customerService = customerService;
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion

        #region  Index
        [CustomAuthorize("CanView", "Admin", "Manager", "Chef")]
        [HttpGet]
        public IActionResult Customers()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return View();
            }
        }
        #endregion

        #region Customer's Partial View
        [CustomAuthorize("CanView", "Admin", "Manager", "Chef")]
        public async Task<IActionResult> LoadCustomers(string search, string status, string timeRange, DateTime? startDate, DateTime? endDate, string sortOrder = "asc", string sortColumn = "CustomerName", int page = 1, int pageSize = 5)
        {
            try
            {
                if (!startDate.HasValue || !endDate.HasValue)
                {
                    DateTime today = DateTime.Today;

                    switch (timeRange)
                    {
                        case "7":
                            startDate = today.AddDays(-7);
                            endDate = today;
                            break;
                        case "30":
                            startDate = today.AddDays(-30);
                            endDate = today;
                            break;
                        case "month":
                            startDate = new DateTime(today.Year, today.Month, 1);
                            endDate = today;
                            break;
                        case "year":
                            startDate = new DateTime(today.Year, 1, 1);
                            endDate = today;
                            break;
                    }
                }

                if (string.IsNullOrEmpty(sortColumn)) sortColumn = "CustomerName";
                if (string.IsNullOrEmpty(sortOrder)) sortOrder = "asc";

                ViewData["SortColumn"] = sortColumn;
                ViewData["SortDirection"] = sortOrder;

                var customers = await _customerService.GetCustomerAsync(search, status, startDate, endDate, page, pageSize, sortColumn, sortOrder);

                ViewBag.PageSize = pageSize;
                ViewBag.TotalItems = customers.TotalItems;
                ViewBag.Page = page;
                ViewBag.TotalPages = customers.TotalPages;
                // ViewBag.FromRec = ((page - 1) * pageSize) + 1;
                // ViewBag.ToRec = Math.Min(page * pageSize, orders.TotalItems);
                if (customers.TotalItems == 0)
                {
                    ViewBag.FromRec = 0;
                    ViewBag.ToRec = 0;
                }
                else
                {
                    ViewBag.FromRec = ((page - 1) * pageSize) + 1;
                    ViewBag.ToRec = Math.Min(page * pageSize, customers.TotalItems);
                }
                return PartialView("_CustomersTablePartialView", customers);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return PartialView("_CustomersTablePartialView", null);
            }
        }
        #endregion

        #region ExportCustomers
        [CustomAuthorize("CanAddEdit", "Admin", "Manager", "Chef")]
        public async Task<IActionResult> ExportCustomers(string searchText, DateTime? startDate, DateTime? endDate, int? orderStatus, string sortColumn, string sortOrder)
        {
            
            try
            {
                var customers = await _customerService.GetFilteredOrders(searchText, startDate, endDate, orderStatus, sortColumn, sortOrder);

                if (customers == null || !customers.Any())
                {
                    Response.ContentType = "application/json";
                    Response.StatusCode = 200;
                    return Json(new { success = false, message = "No records found to download" });
                }

                var fileResult = await _customerService.ExportCustomersToExcel(searchText, startDate, endDate, orderStatus, sortColumn, sortOrder, _hostingEnvironment.WebRootPath);

                if (fileResult == null)
                {
                    Response.ContentType = "application/json";
                    Response.StatusCode = 200;
                    return Json(new { success = false, message = "File generation failed." });
                }

                return fileResult;
            }
            catch (Exception ex)
            {


                Response.ContentType = "application/json";
                Response.StatusCode = 200; 
                return Json(new { success = false, message = "An error occurred while processing your request: " + ex.Message });
            }
        }
        #endregion

        #region  GetCustomerHistory
        [HttpGet]
        public async Task<IActionResult> GetCustomerHistory(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerHistory(id);

                if (customer == null)
                {
                    return Json(new { success = false });
                }

                var result = new
                {
                    success = true,
                    data = new
                    {
                        name = customer.Name,
                        phoneNumber = customer.PhoneNumber,
                        maxOrder = customer.MaxOrder,
                        avgBill = customer.AvgBill,
                        comingSince = customer.ComingSince.ToString("yyyy-MM-dd HH:mm:ss"),
                        visits = customer.Visits,
                        orders = customer.Orders.Select(o => new
                        {
                            orderDate = o.OrderDate.ToString("yyyy-MM-dd"),
                            PaymentMode = o.PaymentMode,
                            items = o.ItemsCount,
                            amount = o.TotalAmount,
                            orderType = o.OrderType
                        }).ToList()
                    }
                };
                return Json(result);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return Json(new { success = false });
            }
        }
        #endregion
    }
}
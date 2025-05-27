using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Attributes;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers
{

    public class OrderAppTableViewController : Controller
    {
        private readonly ITableService _tableService;
        private readonly ISectionService _sectionService;
        private readonly IAssignService _assignService;
        private readonly IWaitingTokenService _waitingTokenService;
        private readonly ICustomerService _customerService;

        #region Constructor
        public OrderAppTableViewController(ITableService tableService, ISectionService sectionService, IAssignService assignService, IWaitingTokenService waitingTokenService, ICustomerService customerService)
        {
            _tableService = tableService;
            _sectionService = sectionService;
            _assignService = assignService;
            _waitingTokenService = waitingTokenService;
            _customerService = customerService;
        }
        #endregion

        #region Index
        [CustomAuthorize]
        [HttpGet]
        public IActionResult Index()
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

        #region GetTableView
        [CustomAuthorize]
        public async Task<IActionResult> GetTableView()
        {
            try
            {
                var tables = await _tableService.GetTablesBySectionAsync();
                return PartialView("_TableViewPartial", tables);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetSections
        [CustomAuthorize]
        [HttpGet]
        public JsonResult GetSections()
        {
            try
            {
                var sections = _sectionService.GetAll();
                return Json(sections);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetSelectedSections
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetSelectedSections()
        {
            try
            {
                var sections = await _sectionService.GetAllSectionsAsync();
                return Json(sections.Select(s => new { SectionId = s.SectionId, SectionName = s.SectionName }));
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region LoadAssignCustomerModal
        [CustomAuthorize]
        [HttpGet]
        public IActionResult LoadAssignCustomerModal(string selectedTableIds)
        {
            try
            {
                List<int> selectedTables = new List<int>();
                if (!string.IsNullOrEmpty(selectedTableIds))
                {
                    selectedTables = JsonConvert.DeserializeObject<List<int>>(selectedTableIds) ?? new List<int>();
                }

                var viewModel = new CustomerOrderViewModel
                {
                    TableIds = selectedTables
                };

                return PartialView("_AssignCustomerModalPartial", viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region AssignCustomerToOrder
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> AssignCustomerToOrder([FromBody] CustomerOrderViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Invalid request: No data received." });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToList()
                );

                return Json(new { success = false, errors });
            }
            try
            {
                var selectedTables = await _tableService.GetTablesByIdsAsync(model.TableIds);

                if (selectedTables == null || !selectedTables.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "No tables selected or tables not found.",
                        errors = new Dictionary<string, List<string>> {
                    { "TableSelection", new List<string> { "No tables selected or invalid table IDs." } }
                }
                    });
                }

                var totalCapacity = selectedTables.Sum(t => t.Capacity);
                int noOfPersons = model.NoOfPersons;

                if (totalCapacity < noOfPersons)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Selected tables provide only {totalCapacity} capacity and you are {noOfPersons} people",
                        errors = new Dictionary<string, List<string>> {
                    { "Capacity", new List<string> { "Total table capacity is not enough." } }
                }
                    });
                }

                // var sorted = selectedTables.OrderByDescending(t => t.Capacity).ToList();
                // int capacitySum = 0;
                // var requiredTables = new List<PizzaShop.Repository.Models.Table>();

                // foreach (var table in sorted)
                // {
                //     if (capacitySum >= noOfPersons) break;
                //     capacitySum += (int)table.Capacity;
                //     requiredTables.Add(table);
                // }

                // if (requiredTables.Count == 1 && selectedTables.Count > 1)
                // {
                //     return Json(new
                //     {
                //         success = false,
                //         message = $"Only one table with {requiredTables[0].Capacity} capacity is enough. No need to merge {selectedTables.Count} tables.",
                //         errors = new Dictionary<string, List<string>> {
                //     { "Merge", new List<string> { "Avoid unnecessary table merge." } }
                // }
                //     });
                // }

                // if (requiredTables.Count < selectedTables.Count)
                // {
                //     return Json(new
                //     {
                //         success = false,
                //         message = $"Only {requiredTables.Count} table(s) are required. You selected more than needed.",
                //         errors = new Dictionary<string, List<string>> {
                //     { "MergeOptimization", new List<string> { "Optimize your table selection." } }
                // }
                //     });
                // }

                var (isSuccess, message, orderId) = await _assignService.AssignCustomerOrderAndMappingWithOrderIdAsync(model);

                return Json(new
                {
                    success = isSuccess,
                    message = message,
                    orderId = orderId
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    errors = new Dictionary<string, List<string>> {
                { "General", new List<string> { ex.Message } }
            }
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "An unexpected error occurred while assigning the customer."
                });
            }
        }
        #endregion

        #region SaveWaitingToken
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> SaveWaitingToken(WaitingTokenViewModel model)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(ms => ms.Value?.Errors.Count > 0)
                                       .ToDictionary(
                                           kvp => kvp.Key,
                                           kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                                       );

                return Json(new { success = false, message = "Validation failed.", errors });
            }
            try
            {
                var result = await _waitingTokenService.AddNewWaitingTokenAsync(model);
                if (result)
                    return Json(new { success = true, message = "Waiting token added successfully." });

                return Json(new { success = false, message = "Failed to add waiting token." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetWaitingListForSections
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetWaitingListForSections([FromQuery] List<int> sectionIds)
        {
            try
            {
                var waitingTokens = await _waitingTokenService.GetWaitingTokensBySectionsAsync(sectionIds);
                return Json(waitingTokens);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error fetching waiting list" });
            }
        }
        #endregion

        #region GetCustomerDetailsById
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetCustomerDetailsById(int customerId)
        {
            try
            {
                var customer = await _waitingTokenService.GetCustomerDetailsByIdAsync(customerId);
                if (customer == null)
                {
                    return Json(new { success = false, message = "No customer found" });
                }
                return Json(new
                {
                    email = customer.Email,
                    name = customer.Name,
                    mobileNumber = customer.MobileNumber,
                    noOfPersons = customer.NoOfPersons,
                    sectionId = customer.SectionId
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetCustomerDetailsByEmail
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetCustomerDetailsByEmail(string email)
        {
            try
            {
                var hasRestrictedOrder = await _customerService.HasCustomerOrderWithStatusAsync(email, new[] { 0, 1 });
                if (hasRestrictedOrder)
                {
                    return Json(new
                    {
                        success = false,
                        message = "This customer already has an order in progress or running. Please complete the existing order first."
                    });
                }

                var customerData = await _customerService.GetCustomerWithLatestOrderAsync(email);

                if (customerData == null)
                    return Json(null);

                return Json(new
                {
                    success = true,
                    name = customerData.Name,
                    mobileNumber = customerData.MobileNumber,
                    noOfPersons = customerData.NoOfPersons
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> GetCustomerDetailsByEmailSP(string email)
        {
            try
            {
                var customerData = await _customerService.GetCustomerWithLatestOrderAsync(email);

                if (customerData == null)
                {
                    return Json(null);
                }

                return Json(new
                {
                    success = true,
                    name = customerData.Name,
                    mobileNumber = customerData.MobileNumber,
                    noOfPersons = customerData.NoOfPersons
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

    }
}
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Attributes;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers
{
    public class OrderAppWaitingListController : Controller
    {
        private readonly ITableService _tableService;
        private readonly ISectionService _sectionService;
        private readonly IWaitingTokenService _waitingTokenService;
        private readonly IAssignService _assignService;

        #region Constructor
        public OrderAppWaitingListController(ITableService tableService, ISectionService sectionService, IWaitingTokenService waitingTokenService, IAssignService assignService)
        {
            _tableService = tableService;
            _sectionService = sectionService;
            _waitingTokenService = waitingTokenService;
            _assignService = assignService;
        }
        #endregion

        #region Index
        [CustomAuthorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region GetSectionTabs
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetSectionTabs(int selectedSectionId = 0)
        {
            try
            {
                var sections = await _sectionService.GetAllSectionsAsyncSP();

                var model = new WaitingListPageViewModel
                {
                    Sections = sections,
                    SelectedSectionId = selectedSectionId
                };

                return PartialView("_SectionTabsPartial", model);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error occurred while fetching section tabs.");
            }
        }
        #endregion

        #region GetWaitingList
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetWaitingList(int sectionId = 0)
        {
            try
            {
                var waitingList = await _waitingTokenService.GetWaitingListBySectionAsyncSP(sectionId);
                return PartialView("_WaitingListPartial", waitingList);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region OpenAddWaitingTokenModal
        public async Task<IActionResult> OpenAddWaitingTokenModal()
        {
            try
            {
                var sections = await _sectionService.GetAllSectionsAsync();
                var viewModel = new WaitingTokenViewModel
                {
                    Sections = sections,
                };
                return PartialView("_AddWaitingTokenPartialModal", viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region AddWaitingTokenToDatabase
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> AddWaitingTokenToDatabase(WaitingTokenViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await _waitingTokenService.AddNewWaitingTokenAsyncSP(model);
                return Json(new { success = true, message = "Waiting token added successfully!" });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region OpenEditWaitingTokenModal
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> OpenEditWaitingTokenModal(int id)
        {
            try
            {
                var model = await _waitingTokenService.GetWaitingTokenByIdAsync(id);
                if (model == null)
                    return Json(new { success = false, message = "Modal not found" });

                return PartialView("_EditWaitingTokenPartialModal", model);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion
        #region OpenEditWaitingTokenModal
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> OpenEditWaitingTokenModalSP(int id)
        {
            try
            {
                var model = await _waitingTokenService.GetWaitingTokenByIdAsyncsp(id);
                if (model == null)
                    return Json(new { success = false, message = "Modal not found" });

                return PartialView("_EditWaitingTokenPartialModal", model);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region SaveEditWaitingTokenToDatabase
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> SaveEditWaitingTokenToDatabase(WaitingTokenViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Sections = await _sectionService.GetAllSectionsAsync();
                    return PartialView("_EditWaitingTokenPartialModal", model);
                }
                var result = await _waitingTokenService.UpdateWaitingTokenAsync(model);
                if (!result)
                    return Json(new { success = false });


                return Json(new
                {
                    success = true,
                    message = "Waiting token updated successfully.",
                    updatedSectionId = model.SectionId
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region DeleteWaitingToken
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> DeleteWaitingToken(int id)
        {
            try
            {
                var result = await _waitingTokenService.DeleteWaitingTokenAsync(id);
                if (!result)
                    return Json(new { success = false });

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        #endregion

        #region LoadAssignTableModal
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> LoadAssignTableModal(int customerId)
        {
            try
            {
                var sections = await _sectionService.GetAllSectionsAsync();
                var viewModel = new AssignTableViewModel
                {
                    CustomerId = customerId,
                    Sections = sections.Select(s => new SectionViewModel
                    {
                        SectionId = s.SectionId,
                        SectionName = s.SectionName
                    }).ToList()
                };

                return PartialView("_AssignTableModalPartial", viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion


        #region GetTablesBySection
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetTablesBySection([FromQuery] List<int> sectionIds)
        {
            try
            {
                var tables = await _tableService.GetTablesBySectionsAsync(sectionIds);
                var result = tables.Select(t => new
                {
                    TableId = t.Tableid,
                    TableName = t.Tablename,
                    TableStatus = t.Tablestatus,
                    SectionName = t.Section?.Sectionname
                });
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region AssignWaitingCustomerToSelectedSectionAndTables
        [CustomAuthorize]
        public async Task<IActionResult> AssignWaitingCustomerToSelectedSectionAndTables([FromBody] CustomerOrderViewModel model)
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
                    errors = new Dictionary<string, List<string>>
                {
                    { "General", new List<string> { ex.Message } }
                }
                });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An unexpected error occurred while assigning the customer." });
            }
        }
        #endregion

        #region AssignSelectedTablesToWaitingCustomer
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> AssignSelectedTablesToWaitingCustomer(AssignTableViewModel model)
        {
            try
            {
                var (IsSuccess, Message, orderId) = await _assignService.AssignWaitinCustomerSelectedTableAndSectionAsync(model);
                return Json(new { isSuccess = IsSuccess, message = Message, orderId = orderId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }
}
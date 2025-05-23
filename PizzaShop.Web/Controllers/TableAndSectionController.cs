using Microsoft.AspNetCore.Mvc;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Attributes;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers
{
    public class TableAndSectionController : Controller
    {
        private readonly ITableService _tableService;
        private readonly ISectionService _sectionService;

        #region Constructor
        public TableAndSectionController(ITableService tableService, ISectionService sectionService)
        {
            _tableService = tableService;
            _sectionService = sectionService;
        }
        #endregion

        #region TableAndSection
        [CustomAuthorize("CanView", "Admin", "Manager", "Chef")]
        [HttpGet]
        public IActionResult TableAndSection()
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

        #region LoadTableSection Partial View
        [CustomAuthorize("CanView", "Admin", "Manager", "Chef")]
        [HttpGet]
        public IActionResult LoadTableSection()
        {
            try
            {
                var model = new TableSectionViewModel
                {
                    Tables = _tableService.GetAll(),
                    Sections = _sectionService.GetAll()
                };

                return PartialView("_SectionPartial", model);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region LoadTablesPaginated
        [CustomAuthorize("CanView", "Admin", "Manager", "Chef")]
        public async Task<IActionResult> LoadTablesPaginated(int sectionId, int pageNumber, int pageSize, string searchQuery = "")
        {
            try
            {
                var paginatedTables = await _tableService.GetPaginatedTablesBySectionIdAsync(sectionId, pageNumber, pageSize, searchQuery);

                ViewBag.TotalItems = paginatedTables.TotalItems;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = paginatedTables.TotalPages;
                if (paginatedTables.TotalItems == 0)
                {
                    ViewBag.FromRec = 0;
                    ViewBag.ToRec = 0;
                }
                else
                {
                    ViewBag.FromRec = paginatedTables.FromRec;
                    ViewBag.ToRec = paginatedTables.ToRec;
                }
                var model = new TableViewModel
                {
                    Sections = _sectionService.GetAll(),
                    TablesPaginated = paginatedTables
                };

                return PartialView("_TablesPartial", model);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetAddSectionModal
        public IActionResult GetAddSectionModal()
        {
            try
            {
                return PartialView("_AddSectionModalPartial");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region AddSection
        [HttpPost]
        public async Task<IActionResult> AddSection([FromBody] SectionViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value?.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                        );

                    return Json(new { success = false, validationErrors = errors });
                }

                bool isDuplicate = await _sectionService.CheckDuplicateSectionNameAsync(model.SectionName);
                if (isDuplicate)
                {
                    return Json(new
                    {
                        success = false,
                        validationErrors = new Dictionary<string, string[]>
            {
                { "SectionName", new[] { "A section with this name already exists." } }
            }
                    });
                }

                await _sectionService.AddAsync(model);
                var updatedSections = _sectionService.GetAll().ToList();

                return Json(new
                {
                    success = true,
                    sections = updatedSections
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetSectionById
        public async Task<IActionResult> GetSectionById(int id)
        {
            try
            {
                var section = await _sectionService.GetSectionByIdAsync(id);
                if (section == null)
                {
                    return Json(new { success = false, message = "Section not found" });
                }
                return PartialView("_EditSectionModalPartial", section);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion



        #region Edit Category POST
        [CustomAuthorize("CanAddEdit", "Admin", "Manager", "Chef")]
        [HttpPost]
        public async Task<IActionResult> EditSection(SectionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToList()
                );

                return Json(new
                {
                    success = false,
                    message = "Validation failed. Please fix the highlighted errors.",
                    errors
                });
            }

            try
            {

                var updated = await _sectionService.UpdateAsync(model);
                if (!updated)
                {
                    return Json(new { success = false, message = "Failed to update section." });
                }
                return Json(new { success = true, message = "Section updated successfully!" });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    errors = new Dictionary<string, List<string>>
            {
                { "SectionName", new List<string> { ex.Message } }
            }
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An unexpected error occurred while updating the category." });
            }
        }
        #endregion

        #region DeleteSection
        [HttpPost]
        public async Task<IActionResult> DeleteSection(int sectionId)
        {
            try
            {
                bool deleted = await _sectionService.DeleteSection(sectionId);
                if (deleted)
                {
                    return Json(new
                    {
                        success = true
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region LoadTablesBySection
        [HttpGet]
        public IActionResult LoadTablesBySection(int sectionId)
        {
            try
            {
                var tables = _tableService.GetTablesBySection(sectionId);
                return PartialView("_TablesPartial", tables);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetTableData
        public IActionResult GetTableData(int selectedSectionId)
        {
            try
            {
                var sections = _sectionService.GetAll();
                var selectedSection = sections.FirstOrDefault(s => s.SectionId == selectedSectionId);

                var viewModel = new TableViewModel
                {
                    Sections = sections,
                    SelectedSectionId = selectedSectionId,
                    SelectedSectionName = selectedSection?.SectionName
                };

                return PartialView("_AddTableModalPartial", viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region AddTable
        [HttpPost]
        public IActionResult AddTable(TableViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var allErrors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return Json(new
                    {
                        success = false,
                        validationErrors = allErrors
                    });
                }

                if (_tableService.IsDuplicateTableName(model.TableName ?? "", model.SectionId ?? 0))
                {
                    return Json(new
                    {
                        success = false,
                        validationErrors = new Dictionary<string, string[]>
            {
                { "TableName", new[] { "A table with this name already exists in the selected section." } }
            }
                    });
                }

                var result = _tableService.AddTable(model);

                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Table added successfully!"
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "Failed to add table."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetTableById
        public IActionResult GetTableById(int tableId)
        {
            try
            {
                var table = _tableService.GetById(tableId);
                if (table == null)
                {
                    return Json(new { success = false, message = "No Table found" });
                }

                var sections = _sectionService.GetAll();
                var selectedSection = sections.FirstOrDefault(s => s.SectionId == table.Sectionid);

                var viewModel = new TableViewModel
                {
                    TableId = table.Tableid,
                    TableName = table.Tablename,
                    Capacity = (int)table.Capacity,
                    Status = table.Tablestatus.HasValue ? (TableStatus)table.Tablestatus.Value : TableStatus.Available,
                    Sections = sections,
                    SelectedSectionId = table.Sectionid,
                    SelectedSectionName = selectedSection?.SectionName
                };

                return PartialView("_EditTableModalPartial", viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region EditTable
        [HttpPost]
        public IActionResult EditTable([FromForm] TableViewModel model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Model is null. Check AJAX request."
                    });
                }

                if (!ModelState.IsValid)
                {
                    var allErrors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return Json(new
                    {
                        success = false,
                        validationErrors = allErrors
                    });
                }
                if (_tableService.IsDuplicateTableName(model.TableName ?? "", model.SectionId ?? 0, model.TableId > 0 ? model.TableId : null))
                {
                    return Json(new
                    {
                        success = false,
                        validationErrors = new Dictionary<string, string[]>
        {
            { "TableName", new[] { "A table with this name already exists in the selected section." } }
        }
                    });
                }

                var result = _tableService.UpdateTable(model);

                return Json(new
                {
                    success = result
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region DeleteTable
        [HttpPost]
        public IActionResult DeleteTable([FromBody] int tableId)
        {
            try
            {
                if (tableId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid Table ID."
                    });
                }

                bool result = _tableService.SoftDeleteTable(tableId);

                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Table deleted successfully."
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "Failed to delete table."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region UpdateSectionOrder
        [HttpPost]
        public async Task<IActionResult> UpdateSectionOrder([FromBody] List<int> sortedSectionIds)
        {
            try
            {
                await _sectionService.UpdateSectionOrderAsync(sortedSectionIds);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region CheckSectionTablesAvailability
        [HttpGet]
        public IActionResult CheckSectionTablesAvailability(int sectionId)
        {
            try
            {
                var tables = _tableService.GetTablesBySectionId(sectionId);

                bool allAvailable = tables.All(t => t.Tablestatus == 1);

                if (allAvailable)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region DeleteMultipleItems POST
        [HttpPost]
        public IActionResult DeleteMultiple([FromBody] List<int> tableids)
        {
            try
            {
                if (tableids == null || !tableids.Any())
                {
                    return Json(new { success = false, message = "No items selected." });
                }

                _tableService.SoftDeleteItems(tableids);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetAllTableIds(int sectionId)
        {
            try
            {
                var itemIds = await _tableService.GetAllTableIds(sectionId);
                return Json(itemIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #region DeleteMultipleTable POST
        [HttpPost]
        public IActionResult DeleteMultipleTable([FromBody] List<int> tableIds)
        {
            if (tableIds == null || !tableIds.Any())
            {
                return Json(new { success = false, message = "No items selected." });
            }

            _tableService.DeleteMultipleTableAsync(tableIds);
            return Json(new { success = true });
        }
        #endregion
    }
}
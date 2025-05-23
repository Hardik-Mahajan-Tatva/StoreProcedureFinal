
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Attributes;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers
{
    public class TaxesAndFeesController : Controller
    {
        private readonly ITaxesFeesService _taxesFeesService;

        #region Constructor
        public TaxesAndFeesController(ITaxesFeesService taxesFeesService)
        {
            _taxesFeesService = taxesFeesService;
        }
        #endregion

        #region TaxesAndFees Index
        [CustomAuthorize("CanView", "Admin", "Manager", "Chef")]
        [HttpGet]
        public IActionResult TaxesAndFees()
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

        #region GetTaxesPartial
        [CustomAuthorize("CanView", "Admin", "Manager", "Chef")]
        [HttpGet]
        public IActionResult GetTaxesPartial()
        {
            try
            {
                var taxes = _taxesFeesService.GetAllTaxes();
                return PartialView("_TaxFeesPartialView", taxes);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error loading taxes.");
            }
        }
        #endregion

        #region AddTax
        [CustomAuthorize("CanAddEdit", "Admin", "Manager", "Chef")]
        [HttpPost]
        public async Task<IActionResult> AddTax(TaxesFeesViewModel model)
        {
            try
            {
                if (await _taxesFeesService.IsDuplicateTaxNameAsync(model.TaxName ?? string.Empty, model.TaxId))
                {
                    ModelState.AddModelError("TaxName", "Tax name already exists.");
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return Json(new
                    {
                        success = false,
                        errors
                    });
                }

                var result = await _taxesFeesService.AddTaxAsync(model);
                return Json(new
                {
                    success = result,
                    message = result ? "Tax added successfully!" : "Failed to add tax."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion


        #region GetTaxById
        public async Task<JsonResult> GetTaxById(int id)
        {
            try
            {
                var tax = await _taxesFeesService.GetTaxByIdAsync(id);
                if (tax == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Tax not found."
                    });
                }

                return Json(new
                {
                    success = true,
                    taxId = tax.TaxId,
                    taxName = tax.TaxName,
                    taxType = tax.TaxType,
                    taxValue = tax.TaxValue,
                    isEnabled = tax.IsEnabled,
                    isDefault = tax.IsDefault
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region UpdateTax
        [HttpPost]
        public async Task<IActionResult> UpdateTax(TaxesFeesViewModel model)
        {
            try
            {
                if (model.TaxName == "Other")
                {
                    ModelState.AddModelError("TaxName", "Tax name 'Other' cannot be used.");
                }
                if (await _taxesFeesService.IsDuplicateTaxNameAsync(model.TaxName ?? string.Empty, model.TaxId))
                {
                    ModelState.AddModelError("TaxName", "Tax name already exists.");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return BadRequest(new { success = false, errors });
                }

                var result = _taxesFeesService.UpdateTax(model);
                if (!result)
                {
                    return StatusCode(500, "Error updating tax.");
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region DeleteTax 
        [HttpPost]
        public async Task<IActionResult> DeleteTax(int taxId)
        {
            try
            {
                bool deleted = await _taxesFeesService.DeleteTax(taxId);
                return Json(new { success = deleted });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetTaxesAndFeesTable
        public async Task<IActionResult> GetTaxesAndFeesTable(string search = "", int page = 1, int pageSize = 5, string sortColumn = "TaxName", string sortDirection = "asc")
        {
            try
            {
                var taxesAndFees = await _taxesFeesService.GetTaxesAndFeesAsync(search, page, pageSize, sortColumn, sortDirection);

                ViewBag.PageNumber = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalItems = taxesAndFees.TotalItems;
                ViewBag.TotalPages = taxesAndFees.TotalPages;
                if (taxesAndFees.TotalItems == 0)
                {
                    ViewBag.FromRec = 0;
                    ViewBag.ToRec = 0;
                }
                else
                {
                    ViewBag.FromRec = ((page - 1) * pageSize) + 1;
                    ViewBag.ToRec = Math.Min(page * pageSize, taxesAndFees.TotalItems);
                }

                return PartialView("_TaxFeesPartialView", taxesAndFees);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region ToggleTaxField
        [CustomAuthorize("CanAddEdit", "Manager", "Admin", "Chef")]
        [HttpPost]
        public async Task<IActionResult> ToggleTaxField(int taxId, bool isChecked, string field)
        {
            try
            {
                await _taxesFeesService.ToggleTaxFieldAsync(taxId, isChecked, field);
                return Json(new { success = true, message = $"{field} updated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetEnabledTaxes
        public async Task<IActionResult> GetEnabledTaxes()
        {
            try
            {
                var taxes = await _taxesFeesService.GetEnabledTaxesAsync();
                return Json(taxes);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetItemSpecificTaxes
        public IActionResult GetItemSpecificTaxes([FromBody] List<int> itemIds)
        {
            try
            {
                if (itemIds == null || !itemIds.Any())
                    return BadRequest("Item ID list is empty.");

                var taxes = _taxesFeesService.GetDefaultItemTaxes(itemIds);
                return Json(taxes);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }
}
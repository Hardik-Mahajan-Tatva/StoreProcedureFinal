using Microsoft.AspNetCore.Mvc;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Attributes;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers
{
    public class OrderAppKOTController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IOrderService _orderService;
        private readonly IOrderedItemService _orderedItemService;

        #region Constructor
        public OrderAppKOTController(ICategoryService categoryService, IOrderService orderService, IOrderedItemService orderedItemService)
        {
            _categoryService = categoryService;
            _orderService = orderService;
            _orderedItemService = orderedItemService;
        }
        #endregion

        #region Index
        [CustomAuthorize]
        [HttpGet("OrderAppKOT/Index/{categoryId?}/{pageNumber?}")]
        public async Task<IActionResult> Index(int categoryId = 0, int pageNumber = 1, int pageSize = 4, string orderStatus = "InProgress", string itemStatus = "InProgress")
        {
            try
            {
                var categories = await _categoryService.GetAll();
                string? categoryName = categoryId == 0 ? "All" : await _categoryService.GetCategoryNameByCategoryId(categoryId);
                var kotData = await _orderService.GetKOTDetailsAsync(pageNumber, pageSize, categoryId, orderStatus, itemStatus);

                ViewBag.Categories = categories;
                ViewBag.SelectedCategoryId = categoryId;
                ViewBag.SelectedCategoryName = categoryName;
                ViewBag.PageIndex = kotData.PageIndex;
                ViewBag.TotalPages = kotData.TotalPages;
                ViewBag.OrderStatusButton = orderStatus;

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_KOTPartial", kotData);
                }
                return View(kotData);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return View();
            }
        }
        #endregion

        #region UpdateReadyQuantities
        [HttpPost]
        public async Task<IActionResult> UpdateReadyQuantities([FromBody] List<ReadyQuantityUpdateViewModel> updates)
        {
            try
            {
                await _orderedItemService.UpdateReadyQuantitiesAsync(updates);
                return Json(new { success = true, message = "Selected items marked as prepared." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Failed to update quantities." });
            }
        }
        #endregion

        #region UpdateQuantities
        [HttpPost]
        public async Task<IActionResult> UpdateQuantities([FromBody] List<ReadyQuantityUpdateViewModel> updates)
        {
            try
            {
                await _orderedItemService.UpdateQuantitiesAsync(updates);
                return Json(new { success = true, message = "Selected items marked as InProgress" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Failed to update quantities." });
            }
        }
        #endregion
    }
}
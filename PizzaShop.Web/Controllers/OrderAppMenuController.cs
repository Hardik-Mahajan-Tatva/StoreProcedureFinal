using Microsoft.AspNetCore.Mvc;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Attributes;
using PizzaShop.Service.Interfaces;
using PizzaShop.Service.Utils;

namespace PizzaShop.Web.Controllers
{
    public class OrderAppMenuController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IItemService _itemService;
        private readonly IItemModifierGroupMapService _itemModifierGroupMapService;
        private readonly IModifiergroupService _modifiergroupService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IOrderedItemService _orderedItemService;
        private readonly IOrderTaxService _orderTaxService;
        private readonly ICustomerReviewService _reviewService;

        #region Constructor
        public OrderAppMenuController(ICategoryService categoryService, IItemService itemService, IItemModifierGroupMapService itemModifierGroupMapService, IModifiergroupService modifiergroupService, IOrderService orderService, ICustomerService customerService,
            IOrderedItemService orderedItemService, IOrderTaxService orderTaxService, ICustomerReviewService reviewService)
        {
            _categoryService = categoryService;
            _itemService = itemService;
            _itemModifierGroupMapService = itemModifierGroupMapService;
            _modifiergroupService = modifiergroupService;
            _orderService = orderService;
            _customerService = customerService;
            _orderedItemService = orderedItemService;
            _orderTaxService = orderTaxService;
            _reviewService = reviewService;
        }
        #endregion

        #region Index
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> Index(string? orderId)
        {
            try
            {
                int? validOrderId = null;
                if (!string.IsNullOrEmpty(orderId))
                {
                    string? decryptedOrderId = null;
                    try
                    {
                        decryptedOrderId = EncryptionHelper.Decrypt(orderId);
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("PageNotFound", "Error");
                    }

                    if (!int.TryParse(decryptedOrderId, out int parsedOrderId))
                    {
                        return RedirectToAction("PageNotFound", "Error");
                    }
                    validOrderId = parsedOrderId;
                }

                // var categories = await _categoryService.GetAll();
                var categories = await _categoryService.GetAllSP();
                var items = _itemService.GetAllItemsSP();

                OrderInvoiceViewModel customerSummary = new();
                if (validOrderId != null)
                {
                    customerSummary = await _orderService.GetCustomerSummary(validOrderId ?? 0);
                    ViewBag.CustomerSummary = customerSummary;
                }

                ViewBag.Items = items;
                ViewBag.OrderId = validOrderId;

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                    WriteIndented = true
                };
                ViewBag.ItemsJson = System.Text.Json.JsonSerializer.Serialize(items, options);

                return View(categories);
            }
            catch (Exception)
            {
                return RedirectToAction("GenericError", "Error");
            }
        }
        #endregion

        #region GetMenu
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetMenu(int? orderId)
        {
            try
            {
                var categories = await _categoryService.GetAll();
                var items = _itemService.GetAllItems();
                ViewBag.Items = items;
                ViewBag.OrderId = orderId;

                return PartialView("_MenuPartial", categories);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return View();
            }
        }
        #endregion

        #region GetItemModifiers
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetItemModifiers(int itemId)
        {
            try
            {
                var modifierGroupMappings = await _itemModifierGroupMapService.GetMappingByItemIdAsync(itemId);
                foreach (var groupMapping in modifierGroupMappings)
                {
                    var modifierGroup = await _modifiergroupService.GetModifierGroupByIdAsync(groupMapping.ModifierGroupId);

                    if (modifierGroup != null)
                    {
                        groupMapping.ModifierGroupName = modifierGroup.ModifierGroupName;

                        if (groupMapping.ModifierItems == null || !groupMapping.ModifierItems.Any())
                        {
                            if (modifierGroup.ModifierItems != null)
                            {
                                groupMapping.ModifierItems = modifierGroup.ModifierItems.Select(item => new ModifierItemViewModel
                                {
                                    OrderedItemId = item.OrderedItemId,
                                    ModifierItemId = item.ModifierItemId,
                                    ModifierItemName = item.ModifierItemName,
                                    Price = item.Price,
                                    ModifierType = item.ModifierType ?? default
                                }).ToList();
                            }
                        }
                    }
                }
                var item = await _itemModifierGroupMapService.GetMappingByItemIdAsync(itemId);

                if (item == null)
                    return Json(new { success = false, message = "Item not found" });

                return PartialView("_ModifierGroupsPartial", modifierGroupMappings);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region LoadCustomerSummary
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> LoadCustomerSummary(int orderId)
        {
            try
            {
                var customerSummary = await _orderService.GetCustomerSummary(orderId);
                if (customerSummary == null)
                {
                    return View("Customer summary not found.");
                }
                return PartialView("_CustomerSummaryPartial", customerSummary);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error.");
            }
        }
        #endregion

        #region EncryptOrderId
        [CustomAuthorize]
        [HttpGet]
        public IActionResult EncryptOrderId(int orderId)
        {
            try
            {
                string encryptedOrderId = EncryptionHelper.Encrypt(orderId.ToString());
                return Json(new { success = true, encryptedOrderId });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Failed to encrypt order ID" });
            }
        }
        #endregion

        #region LoadCustomerDetailsModal
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> LoadCustomerDetailsModal(int customerId)
        {
            try
            {
                var customerDetails = await _customerService.GetCustomerDetails(customerId);
                if (customerDetails == null)
                {
                    customerDetails = new CustomerUpdateViewModal();
                }
                return PartialView("_CustomerDetailModalPartial", customerDetails);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region EditCustomer
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> EditCustomer(CustomerUpdateViewModal model)
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
                var updated = await _customerService.UpdateAsync(model);
                if (!updated)
                {
                    return Json(new { success = false, message = "Failed to update customer." });
                }

                return Json(new { success = true, message = "Customer updated successfully!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An unexpected error occurred while updating the customer." });
            }
        }
        #endregion

        #region LoadOrderCommentModal
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> LoadOrderCommentModal(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderById(orderId);
                if (order == null)
                {
                    return View("Order not found.");
                }

                var model = new OrderCommentViewModel
                {
                    OrderId = orderId,
                    Comment = order.Comment
                };

                return PartialView("_OrderCommentModalPartial", model);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region SaveOrderComment
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> SaveOrderComment(OrderCommentViewModel model)
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
                var result = await _orderService.SaveOrderComment(model.OrderId, model.Comment ?? string.Empty);
                if (!result)
                {
                    return Json(new { success = false, message = "Failed to add comment." });
                }

                return Json(new { success = true, message = "Comment  added successfully!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An unexpected error occurred while updating the customer." });
            }
        }
        #endregion

        #region LoadSpecialInstructionModal
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> LoadSpecialInstructionModal(int orderId, int orderedItemId)
        {
            try
            {
                var instruction = await _orderService.GetSpecialInstruction(orderId, orderedItemId);

                var model = new SpecialInstructionViewModel
                {
                    OrderId = orderId,
                    OrderedItemId = orderedItemId,
                    Instruction = instruction
                };
                return PartialView("_SpecialInstructionModalPartial", model);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion  

        #region SaveSpecialInstruction
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> SaveSpecialInstruction(SpecialInstructionViewModel model)
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
                var result = await _orderService.SaveSpecialInstruction(model.OrderId, model.OrderedItemId, model.Instruction ?? string.Empty);
                if (!result)
                {
                    return Json(new { success = false, message = "Failed to add comment." });
                }

                return Json(new { success = true, message = "Comment  added successfully!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An unexpected error occurred while updating the customer." });
            }
        }
        #endregion  

        #region GetOrderedItems
        [HttpGet]
        public async Task<IActionResult> GetOrderedItems(int orderId)
        {
            try
            {
                var orderedItems = await _orderService.GetOrderItemsAsync(orderId);
                return Json(orderedItems);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region  MarkAsFavorite
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> MarkAsFavorite(int itemId, bool isFavorite)
        {
            try
            {
                var result = await _itemService.MarkAsFavoriteAsync(itemId, isFavorite);
                if (result)
                {
                    return Json(new { success = true, message = "Item marked as favorite successfully." });
                }
                return Json(new { success = false, message = "Failed to mark item as favorite." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region SaveOrder
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> SaveOrder([FromBody] OrderRequestModel orderRequest)
        {
            if (orderRequest == null || orderRequest.Items == null || !orderRequest.Items.Any())
            {
                return Json(new { success = false, message = "Please select atlead one item before save" });
            }
            try
            {
                await _orderService.SaveOrder(orderRequest);
                return Json(new { success = true, message = "Order saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region AvailableToDelete
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> AvailableToDelete(int itemId)
        {
            try
            {
                bool result = await _orderedItemService.IsItemAvailableToDelete(itemId);
                if (!result)
                {
                    return Json(new { success = true, message = "Item is available to delete." });
                }
                else
                {
                    return Json(new { success = false, message = "Item is not available to delete." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetTaxMapping
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetTaxMapping(int orderId)
        {
            try
            {
                var taxMappings = await _orderTaxService.GetTaxMappingsByOrderIdAsync(orderId);
                if (taxMappings == null || !taxMappings.Any())
                {
                    return Json(new { success = false, message = "No tax mapping found" });
                }

                return Ok(new { success = true, taxMappings });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region MarkOrderComplete
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> MarkOrderComplete(int orderId)
        {
            try
            {
                var result = await _orderService.MarkOrderAsCompleteAsync(orderId);
                if (result)
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Unable to complete the order." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region LoadCustomerReviewModal
        [CustomAuthorize]
        [HttpGet]
        public IActionResult LoadCustomerReviewModal(int orderId)
        {
            try
            {
                var model = new CustomerReviewViewModel { OrderId = orderId };
                return PartialView("_CustomerReviewModalPartial", model);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region SubmitReview
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> SubmitReview(CustomerReviewViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _reviewService.AddReviewAsync(model);
                    return Ok();
                }
                return View();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region CancelOrder
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                var result = await _orderService.CancelOrderAsync(orderId);

                if (result)
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Unable to cancel order." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }
}
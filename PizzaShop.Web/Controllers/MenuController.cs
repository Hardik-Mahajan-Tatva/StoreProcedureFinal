using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Attributes;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers
{
    [CustomAuthorize("CanView", "Admin", "Manager", "Chef")]
    public class MenuController : Controller
    {
        #region Dependencies
        private readonly ILogger<MenuController> _logger;
        private readonly ICategoryService _categoryService;
        private readonly IItemService _itemService;
        private readonly IModifiergroupService _modifiergroupService;
        private readonly IModifierService _modifierService;
        private readonly IUnitService _unitService;
        private readonly IItemModifierGroupMapService _iItemModifierGroupMapService;
        #endregion

        #region Constructor
        public MenuController(ICategoryService categoryService, IItemService itemService, IModifiergroupService modifiergroupService, IModifierService modifierService, IUnitService unitService, IItemModifierGroupMapService iItemModifierGroupMapService, ILogger<MenuController> logger)
        {
            _categoryService = categoryService;
            _itemService = itemService;
            _modifiergroupService = modifiergroupService;
            _modifierService = modifierService;
            _unitService = unitService;
            _iItemModifierGroupMapService = iItemModifierGroupMapService;
            _logger = logger;
        }
        #endregion

        #region Index
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region Add Category POST
        [CustomAuthorize("CanAddEdit", "Admin", "Manager", "Chef")]
        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryViewModel model)
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
                int newCategoryId = await _categoryService.AddAsync(model);

                return Json(new
                {
                    success = true,
                    message = "Category added successfully!",
                    categoryId = newCategoryId
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
                { "CategoryName", new List<string> { ex.Message } }
            }
                });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An unexpected error occurred while adding the category." });
            }
        }
        #endregion

        public IActionResult LoadAddCategoryModal()
        {
            var model = new CategoryViewModel();
            return PartialView("_AddCategoryModal", model);
        }
        public IActionResult LoadEditCategoryModal()
        {
            var model = new CategoryViewModel();
            return PartialView("_EditCategoryModal", model);
        }

        #region Edit Category POST
        [CustomAuthorize("CanAddEdit", "Admin", "Manager", "Chef")]
        [HttpPost]
        public async Task<IActionResult> EditCategory(CategoryViewModel model)
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
                var updated = await _categoryService.UpdateAsync(model);
                if (!updated)
                {
                    return Json(new { success = false, message = "Failed to update category." });
                }

                return Json(new { success = true, message = "Category updated successfully!" });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    errors = new Dictionary<string, List<string>>
            {
                { "CategoryName", new List<string> { ex.Message } }
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


        #region GetCategoryById
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return Json(new { success = false, message = "NO category found" });
            }
            return PartialView("_EditCategoryModal", category);
        }


        #endregion
        #region Delete Category POST
        [CustomAuthorize("CanAddEdit", "Admin", "Manager", "Chef")]
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            bool deleted = await _categoryService.DeleteCategory(categoryId);
            if (deleted)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        #endregion

        #region GetAllCategories
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categoryViewModel = await _categoryService.GetAll();
            return Json(categoryViewModel);
        }
        #endregion

        #region AddMenuItem POST
        [CustomAuthorize("CanAddEdit", "Admin", "Manager", "Chef")]
        [HttpPost]
        public async Task<IActionResult> AddMenuItem(ItemViewModel model, string ModifierGroupsJson, IFormFile? itemImage)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .Select(x => new
                    {
                        Key = x.Key,
                        Errors = x.Value?.Errors.Select(e => e.ErrorMessage).ToList()
                    }).ToList();

                return Json(new
                {
                    success = false,
                    validationErrors = errors
                });
            }


            try
            {
                bool isDuplicate = _itemService.IsDuplicateItem(model.Itemname);
                if (isDuplicate)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Item name already exists!"
                    });
                }

                var modifierGroups = JsonConvert.DeserializeObject<List<ItemModifierGroupMapViewModel>>(ModifierGroupsJson) ?? new List<ItemModifierGroupMapViewModel>();

                int itemId = await _itemService.AddMenuItem(model, itemImage);
                if (itemId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to add item."
                    });
                }

                foreach (var modifierGroup in modifierGroups)
                {
                    modifierGroup.ItemId = itemId;
                    await _iItemModifierGroupMapService.AddItemModifierGroupMap(modifierGroup);
                }

                return Json(new
                {
                    success = true,
                    message = "Menu item added successfully!",
                    categoryId = model.Categoryid
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        #endregion


        #region UpdateCategoryOrder POST
        [HttpPost]
        public async Task<IActionResult> UpdateCategoryOrder([FromBody] List<int> orderedCategoryIds)
        {
            try
            {
                await _categoryService.UpdateCategoryOrderAsync(orderedCategoryIds);
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return BadRequest(new { success = false, message = "You are not authorized to perform this action." });
            }
        }
        #endregion


        #region GetItemById GET
        [HttpGet]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = _itemService.GetItemById(id);
            if (item == null)
            {
                return Json(new { success = false, message = "Item not found" });
            }

            var categories = await _categoryService.GetAll();
            var units = await _unitService.GetUnitsAsync();
            var modifierGroups = _modifierService.GetAllModifierGroups();

            var modifierGroupMappings = await _iItemModifierGroupMapService.GetMappingByItemIdAsync(id);

            var modifierGroupMappingViewModels = modifierGroupMappings.Select(mapping => new ItemModifierGroupMapViewModel
            {
                ModifierGroupId = mapping.ModifierGroupId,
                MinValue = mapping.MinValue,
                MaxValue = mapping.MaxValue
            }).ToList();

            var model = new EditMenuItemViewModel
            {
                ItemId = item.Itemid,
                ItemName = item.Itemname,
                ItemType = item.ItemType,
                CategoryId = item.Categoryid,
                Rate = item.Rate,
                Quantity = item.Quantity,
                UnitId = item.Unitid,
                IsAvailable = item.Isavailable,
                Itemimg = item.Itemimg,
                IsDefaultTax = item.Isdefaulttax,
                TaxPercentage = item.Taxpercentage,
                ShortCode = item.Shortcode,
                Description = item.Description,
                Categories = categories,
                Units = units,
                ModifierGroups = modifierGroups,
                ModifierGroupMappings = modifierGroupMappingViewModels
            };

            return PartialView("_EditItemModalPartialView", model);
        }
        #endregion

        #region GetModifierMappingsByItemId GET
        [HttpGet]
        public async Task<IActionResult> GetModifierMappingsByItemId(int id)
        {
            var modifierGroupMappings = await _iItemModifierGroupMapService.GetMappingByItemIdAsync(id);



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
                                ModifierItemId = item.ModifierItemId,
                                ModifierItemName = item.ModifierItemName,
                                Price = item.Price
                            }).ToList();
                        }
                    }
                }
            }

            return Json(modifierGroupMappings);
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> UpdateMenuItem(EditMenuItemViewModel model, IFormFile? itemImage, string ModifierGroupsJson)
        {


            model.IsAvailable = Request.Form["Isavailable"].ToString().ToLower() == "true";
            model.IsDefaultTax = Request.Form["Isdefaulttax"].ToString().ToLower() == "true";

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value?.Errors.Select(x => x.ErrorMessage).FirstOrDefault()
                    );

                return Json(new { success = false, message = "Invalid input data!", errors = errors });
            }

            try
            {
                bool isDuplicate = _itemService.IsDuplicateItem(model.ItemName, model.ItemId);
                if (isDuplicate)
                {
                    return Json(new { success = false, message = "An item with the same name already exists!" });
                }

                var modifierGroups = JsonConvert.DeserializeObject<List<ItemModifierGroupMapViewModel>>(ModifierGroupsJson) ?? new List<ItemModifierGroupMapViewModel>();


                bool isUpdated = await _itemService.UpdateMenuItem(model, itemImage);
                if (!isUpdated)
                {
                    return Json(new { success = false, message = "Failed to update item." });
                }

                await _iItemModifierGroupMapService.DeleteItemModifierGroupMapsByItemId(model.ItemId);

                foreach (var modifierGroup in modifierGroups)
                {
                    modifierGroup.ItemId = model.ItemId;
                    await _iItemModifierGroupMapService.AddItemModifierGroupMap(modifierGroup);
                }

                return Json(new { success = true, message = "Menu item updated successfully!", categoryId = model.CategoryId });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during item update: {Error}", ex.Message);
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        #region DeleteMenuItem POST
        [HttpPost]
        public IActionResult DeleteMenuItem(int itemId)
        {
            var result = _itemService.SoftDeleteItem(itemId);
            if (result)
            {
                return Json(new
                {
                    success = true,
                    message = "Item deleted successfully!"
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Error deleting item!"
                });
            }
        }
        #endregion
        #region LoadItems
        public async Task<IActionResult> LoadItems(int pageNumber, int pageSize)
        {
            var paginatedItems = await _itemService.GetPaginatedItemsByGroupIdAsync(1, pageNumber, pageSize);

            ViewBag.FromRec = paginatedItems.FromRec;
            ViewBag.ToRec = paginatedItems.ToRec;
            ViewBag.TotalItems = paginatedItems.TotalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = paginatedItems.TotalPages;

            var model = new MenuViewModel
            {
                Categories = await _categoryService.GetAll(),
                ItemsPaginated = paginatedItems
            };

            return PartialView("_ItemSectionPartial", model);
        }
        #endregion

        #region LoadItemsByCategory
        public async Task<IActionResult> LoadItemsByCategory(int categoryId, int pageNumber, int pageSize, string searchQuery = "")
        {
            var paginatedItems = await _itemService.GetPaginatedItemsByGroupIdAsync(categoryId, pageNumber, pageSize, searchQuery);

            ViewBag.FromRec = paginatedItems.FromRec;
            ViewBag.ToRec = paginatedItems.ToRec;
            ViewBag.TotalItems = paginatedItems.TotalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = paginatedItems.TotalPages;

            return PartialView("_ItemsPartial", paginatedItems);
        }
        #endregion

        #region DeleteMultipleItems POST
        [HttpPost]
        public IActionResult DeleteMultiple([FromBody] List<int> itemIds)
        {
            if (itemIds == null || !itemIds.Any())
            {
                return Json(new { success = false, message = "No items selected." });
            }

            _itemService.SoftDeleteItems(itemIds);
            return Json(new { success = true });
        }
        #endregion

        #region GetAllModifiers GET
        [HttpGet]
        public async Task<IActionResult> GetAllModifiers(int pageIndex = 1, int pageSize = 5, string searchQuery = "")
        {
            var paginatedModifiers = await _modifierService.GetModifiersAsync(pageIndex, pageSize, searchQuery);

            if (paginatedModifiers == null || !paginatedModifiers.Any())
            {
                paginatedModifiers = new PaginatedList<ModifierViewModel>(new List<ModifierViewModel>(), 0, pageIndex, pageSize);
            }
            return PartialView("_ModifierListPartial", paginatedModifiers);
        }
        #endregion
        #region GetAllModifiers GET
        [HttpGet]
        public async Task<IActionResult> GetAllModifiersEdit(int pageIndex = 1, int pageSize = 5, string searchQuery = "")
        {
            var paginatedModifiers = await _modifierService.GetModifiersAsync(pageIndex, pageSize, searchQuery);

            if (paginatedModifiers == null || !paginatedModifiers.Any())
            {
                paginatedModifiers = new PaginatedList<ModifierViewModel>(new List<ModifierViewModel>(), 0, pageIndex, pageSize);
            }
            return PartialView("_ModifierListPartialEdit", paginatedModifiers);
        }
        #endregion

        #region GetAllModifierGroups GET
        [CustomAuthorize("CanView", "Manager", "Admin", "Chef")]
        [HttpGet]
        public IActionResult GetAllModifierGroups()
        {
            var modifierGroups = _modifiergroupService.GetAll();

            if (modifierGroups == null || !modifierGroups.Any())
            {
                return Json(new { message = "No modifier groups found" });
            }

            return Json(modifierGroups);
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> AddModifierGroup([FromBody] ModifiergroupViewModel model)
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
                var success = await _modifiergroupService.CreateModifierGroupAsync(model);
                return Json(new
                {
                    success = success,
                    message = success ? "Modifier Group added successfully!" : "Error adding modifier group."
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
                { "ModifierGroupName", new List<string> { ex.Message } }
            }
                });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An unexpected error occurred while adding the modifier group." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetModifierGroupByIdEdit(int id)
        {
            var modifierGroup = await _modifiergroupService.GetModifierGroupByIdAsync(id);

            if (modifierGroup == null)
            {
                return Json(new { success = false, message = "Modifier Group not found" });
            }

            return Json(new { success = true, modifierGroup = modifierGroup });
        }


        #region EditModifierGroup POST
        [HttpPost]
        public async Task<IActionResult> EditModifierGroup([FromBody] ModifiergroupViewModel model)
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
                var success = await _modifiergroupService.UpdateModifierGroupAsync(model);
                return Json(new
                {
                    success = success,
                    message = success ? "Modifier Group updated successfully!" : "Error updating modifier group.",
                    modifierGroupId = model.ModifierGroupId
                });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("ModifierGroupName", ex.Message);

                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToList()
                );

                return Json(new { success = false, errors });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An unexpected error occurred while updating the modifier group." });
            }
        }
        #endregion



        #region DeleteModifierGroup POST
        public async Task<IActionResult> DeleteModifierGroup(int modifierGroupId)
        {
            var deleted = await _modifiergroupService.SoftDeleteModifierGroupAsync(modifierGroupId);
            if (deleted)
            {
                return Json(new
                {
                    success = true
                });
            }
            return Json(new
            {
                success = false
            });
        }
        #endregion

        #region UpdateModifierGroupOrder
        [HttpPost]
        public async Task<IActionResult> UpdateModifierGroupOrder([FromBody] List<int> orderedModifierGroupIds)
        {
            try
            {
                await _modifiergroupService.UpdateModifierGroupOrderAsync(orderedModifierGroupIds);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region LoadModifiers 
        public async Task<IActionResult> LoadModifiers(int pageNumber, int pageSize)
        {
            var paginatedModifiers = await _modifierService.GetPaginatedModifiersByGroupIdAsync(1, pageNumber, pageSize);

            ViewBag.FromRec = paginatedModifiers.FromRec;
            ViewBag.ToRec = paginatedModifiers.ToRec;
            ViewBag.TotalItems = paginatedModifiers.TotalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = paginatedModifiers.TotalPages;

            var model = new MenuViewModel
            {
                Modifiergroups = _modifiergroupService.GetAll(),
                ModifiersPaginated = paginatedModifiers
            };
            return PartialView("_ModifierSectionPartial", model);
        }
        #endregion

        #region LoadModifiersByGroup
        public async Task<IActionResult> LoadModifiersByGroup(int modifierGroupId, int pageNumber, int pageSize, string searchQuery = "")
        {
            var paginatedModifiers = await _modifierService.GetPaginatedModifiersByGroupIdAsync(modifierGroupId, pageNumber, pageSize, searchQuery);

            ViewBag.FromRec = paginatedModifiers.FromRec;
            ViewBag.ToRec = paginatedModifiers.ToRec;
            ViewBag.TotalItems = paginatedModifiers.TotalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = paginatedModifiers.TotalPages;
            var model = new MenuViewModel
            {
                ModifiersPaginated = paginatedModifiers
            };
            return PartialView("_ModifiersPartial", model.ModifiersPaginated);
        }
        #endregion

        #region AddModifier
        [HttpGet]
        public async Task<IActionResult> AddModifier()
        {
            var modifierGroups = _modifiergroupService.GetAll();
            var units = await _unitService.GetUnitsAsync();
            var viewModel = new ModifierSectionViewModel

            {
                Modifiergroups = modifierGroups,
                Units = units,
            };
            return PartialView("_AddNewModifierModalPartial", viewModel);
        }
        #endregion
        public IActionResult AddModifierGroupPartial()
        {
            return PartialView("_AddModifierGroupPartial");
        }

        #region AddModifier POST
        [HttpPost]
        public JsonResult AddModifier(AddModifierViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                return Json(new
                {
                    success = false,
                    validationErrors = errors,
                    modifierGroupIds = model.ModifierGroupIds
                });
            }

            try
            {
                var result = _modifierService.AddModifier(model);
                if (result)
                {
                    return Json(new { success = true });
                }

                return Json(new
                {
                    success = false,
                    message = "Failed to add modifier."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred: " + ex.Message
                });
            }
        }

        #endregion

        #region GetModifierByIdEdit
        [HttpGet]
        public async Task<ActionResult> GetModifierByIdEdit(int id)
        {
            var modifier = _modifierService.GetModifierById(id);
            var units = await _unitService.GetUnitsAsync();
            var modifierGroups = _modifiergroupService.GetAll();

            var model = new AddModifierViewModel
            {
                ModifierName = modifier.ModifierName,
                Units = units,
                ModifierId = modifier.ModifierId,
                Modifiergroups = modifierGroups,
                Rate = modifier.Rate,
                Quantity = modifier.Quantity,
                Description = modifier.Description,
                ModifierGroupIds = modifier.ModifierGroupIds,
                Unitid = modifier.Unitid
            };
            return PartialView("_EditNewModifierModalPartial", model);
        }
        #endregion

        #region UpdateModifier
        [HttpPost]
        public async Task<IActionResult> UpdateModifier(AddModifierViewModel model)
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

            var result = await _modifierService.UpdateModifierAsync(model);

            if (result)
            {
                return Json(new { success = true, modifierGroupIds = model.ModifierGroupIds });
            }
            else
            {
                return Json(new { success = false, message = "Failed to update modifier." });
            }
        }
        #endregion

        #region DeleteModifier POST
        [HttpPost]
        public async Task<IActionResult> DeleteModifier(int modifierId)
        {
            var result = await _modifierService.DeleteModifierAsync(modifierId);
            return Json(new
            {
                success = result
            });
        }
        #endregion

        #region DeleteMultipleModifier POST
        [HttpPost]
        public IActionResult DeleteMultipleModifier([FromBody] List<int> modifierIds)
        {
            if (modifierIds == null || !modifierIds.Any())
            {
                return Json(new { success = false, message = "No items selected." });
            }

            _modifierService.SoftDeleteModifiers(modifierIds);
            return Json(new { success = true });
        }
        #endregion

        #region DropDowns
        public async Task<IActionResult> GetMenuData()
        {
            var categories = await _categoryService.GetAll();
            var modifierGroups = _modifiergroupService.GetAll();
            var units = await _unitService.GetUnitsAsync();
            var itemTypes = Enum.GetValues(typeof(Repository.ViewModels.ItemType))
              .Cast<Repository.ViewModels.ItemType>()
              .Select(it => new KeyValuePair<int, string>((int)it, it.ToString()))
              .ToList();

            var viewModel = new ItemViewModel

            {
                Categories = categories,
                Modifiergroups = modifierGroups,
                Units = units,
                ItemTypes = itemTypes
            };

            return PartialView("_AddNewItemModalPartial", viewModel);
        }
        #endregion

        #region GetModifierGroupsByIds
        [HttpGet]
        public JsonResult GetModifiersByGroup([FromQuery] List<int> modifierGroupIds)
        {
            if (modifierGroupIds == null || !modifierGroupIds.Any())
            {
                return Json(new
                {
                    Error = "No modifier groups selected."
                });
            }
            var groups = _modifiergroupService.GetModifierGroupsByIds(modifierGroupIds);
            var modifiers = _modifierService.GetModifiersByGroupIds(modifierGroupIds);
            var response = new
            {
                Groups = groups.Select(g => new
                {
                    GroupId = g.Modifiergroupid,
                    GroupName = g.Modifiergroupname
                }).ToList(),

                ModifierItems = modifiers.Select(m => new
                {
                    ModifierId = m.Modifierid,
                    ModifierName = m.Modifiername,
                    Price = m.Rate,
                    GroupId = m.ModifierGroupModifierMappings.Select(mgm => mgm.ModifierGroupId).ToList()
                }).ToList()
            };
            return Json(response);
        }
        #endregion

        [CustomAuthorize("CanAddEdit", "Manager", "Admin", "Chef")]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string field, bool value)
        {
            try
            {
                await _itemService.UpdateStatusAsync(id, field, value);
                return Json(new { success = true, message = $"{field} updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetAllItemIds(int categoryId)
        {
            try
            {
                var itemIds = await _itemService.GetAllItemIds(categoryId);
                return Json(itemIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetAllModifierIds(int modifierGroupId)
        {
            try
            {
                var modifierIds = await _modifierService.GetAllModifierIds(modifierGroupId);
                return Json(modifierIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllModifierIdsExisting()
        {
            try
            {
                var modifierIds = await _modifierService.GetAllModifierIdsExisting();
                return Json(modifierIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllModifierIdsWithNames()
        {
            var modifiers = await _modifierService.GetAllModifierIdsWithNamesAsync();

            var result = modifiers.Select(m => new
            {
                id = m.ModifierId.ToString(),
                name = m.Modifiername
            }).ToList();

            return Json(result);
        }

    }
}
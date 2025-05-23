using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pizzashop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IImageService _imageService;



        public ItemService(IItemRepository itemRepository, IImageService imageService)
        {
            _itemRepository = itemRepository;
            _imageService = imageService;
        }

        public async Task<int> AddMenuItem(ItemViewModel model, IFormFile? itemImage)
        {
            if (itemImage == null)
                throw new ArgumentNullException(nameof(itemImage), "Item image cannot be null.");
            var itemImgPath = await _imageService.ImgPath(itemImage);
            int? ModifierId = model.ModifierId;
            var menuItem = new Item
            {
                Itemname = model.Itemname,
                Categoryid = model.Categoryid,
                Rate = model.Rate,
                Quantity = model.Quantity,
                Unitid = model.Unitid,
                Isavailable = model.Isavailable,
                Taxpercentage = model.Taxpercentage,
                Shortcode = model.Shortcode,
                Isdefaulttax = model.Isdefaulttax,
                Description = model.Description,
                Isdeleted = false,
                Itemimg = itemImgPath,
            };
            return await _itemRepository.AddMenuItem(menuItem, ModifierId ?? 0);
        }

        public ItemViewModel GetItemById(int id)
        {
            var item = _itemRepository.GetItemById(id);
            if (item == null) return new ItemViewModel();

            return new ItemViewModel
            {
                Itemid = item.Itemid,
                Categoryid = item.Categoryid,
                Itemname = item.Itemname,
                ItemType = item.Itemtype,
                Rate = item.Rate,
                Quantity = item.Quantity,
                Unitid = item.Unitid,
                Isavailable = item.Isavailable ?? false,
                Isdefaulttax = item.Isdefaulttax ?? false,
                Taxpercentage = item.Taxpercentage,
                Shortcode = item.Shortcode,
                Description = item.Description,
                Itemimg = item.Itemimg,
            };
        }


        public async Task<IEnumerable<ItemViewModel>> GetItemsByCategoryId(int categoryId)
        {
            var items = await _itemRepository.GetItemsByCategoryId(categoryId);

            return items.Select(item => new ItemViewModel
            {
                Itemid = item.Itemid,
                Itemname = item.Itemname,
                Categoryid = item.Categoryid,
                Rate = item.Rate,
                Quantity = item.Quantity,
                Unitid = item.Unitid,
                Isavailable = item.Isavailable ?? false,
                Taxpercentage = item.Taxpercentage,
                Shortcode = item.Shortcode,
                Isfavourite = item.Isfavourite,
                Isdefaulttax = item.Isdefaulttax ?? false,
                Itemimg = item.Itemimg,
                Description = item.Description,
                Isdeleted = item.Isdeleted,
                ItemType = item.Itemtype
            }).ToList();
        }

        public bool SoftDeleteItem(int itemId)
        {
            return _itemRepository.SoftDeleteItem(itemId);
        }

        public async Task<bool> UpdateMenuItem(EditMenuItemViewModel model, IFormFile? itemImage)
        {
            var itemImgPath = await _imageService.ImgPath(itemImage);
            var menuItem = _itemRepository.GetItemById(model.ItemId);
            if (menuItem == null) return false;

            menuItem.Itemname = model.ItemName;
            menuItem.Categoryid = model.CategoryId;
            menuItem.Itemtype = model.ItemType ?? "Veg";
            menuItem.Rate = model.Rate;
            menuItem.Quantity = model.Quantity;
            menuItem.Unitid = model.UnitId;
            menuItem.Isavailable = model.IsAvailable;
            menuItem.Isdefaulttax = model.IsDefaultTax;
            menuItem.Taxpercentage = model.TaxPercentage;
            menuItem.Shortcode = model.ShortCode;
            menuItem.Description = model.Description;
            menuItem.Itemimg = model.Itemimg;
            menuItem.Itemimg = itemImgPath;

            return _itemRepository.UpdateMenuItem(menuItem);
        }
        public IQueryable<Item> GetAllItems()
        {
            return _itemRepository.GetAll();
        }

        public IQueryable<Item> GetItemsByCategoryIdPaginated(int categoryId)
        {
            IQueryable<Item>? item = _itemRepository.GetItemsByCategoryIdPaginated(categoryId);
            return item;
        }


        public async Task<PaginatedList<Item>> GetPaginatedItemsByCategory(int categoryId, int pageNumber, int pageSize)
        {
            return await _itemRepository.GetItemsByCategoryPaginated(categoryId, pageNumber, pageSize);
        }

        public async Task<PaginatedList<ItemViewModel>> GetPaginatedItemsByGroupIdAsync(int categoryId, int pageNumber, int pageSize, string searchQuery = "")
        {
            var (items, totalCount) = await _itemRepository.GetPaginatedItemsByGroupIdAsync(categoryId, pageNumber, pageSize, searchQuery);
            var mappedItems = items.Select(item => new ItemViewModel
            {
                Itemid = item.Itemid,
                Itemname = item.Itemname,
                ItemType = item.Itemtype,
                Rate = item.Rate,
                Quantity = item.Quantity,
                Isavailable = item.Isavailable ?? false,
                Itemimg = item.Itemimg,
            }).ToList();

            return new PaginatedList<ItemViewModel>(mappedItems, totalCount, pageNumber, pageSize);
        }

        public bool IsDuplicateItem(string itemName, int? itemId = null)
        {
            return _itemRepository.IsDuplicateItem(itemName, itemId);
        }

        public void SoftDeleteItems(List<int> itemIds)
        {
            _itemRepository.SoftDeleteItems(itemIds);
        }
        public async Task<ItemWithModifiersViewModel> GetItemWithModifiersAsync(int itemId)
        {
            return await _itemRepository.GetItemWithModifiersAsync(itemId);
        }
        public async Task UpdateStatusAsync(int id, string field, bool value)
        {
            await _itemRepository.UpdateStatusAsync(id, field, value);
        }
        public async Task<bool> MarkAsFavoriteAsync(int itemId, bool isFavorite)
        {
            return await _itemRepository.MarkAsFavoriteAsync(itemId, isFavorite);
        }
        public async Task<List<int>> GetAllItemIds(int categoryId)
        {
            return await _itemRepository.GetAllItemIds(categoryId);
        }


    }
}

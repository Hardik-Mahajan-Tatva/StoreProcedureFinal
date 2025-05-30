using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Models;
using Pizzashop.Repository.Interfaces;
using PizzaShop.Repository.ViewModels;
using System.Threading.Tasks;

namespace Pizzashop.Repository.Implementations
{
    public class ItemRepository : IItemRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor

        public ItemRepository(PizzaShopContext context)
        {
            _context = context;
        }

        #endregion

        #region AddMenuItem
        public async Task<int> AddMenuItem(Item menuItem, int modifierGroupId)
        {
            await _context.Items.AddAsync(menuItem);
            await _context.SaveChangesAsync();
            return menuItem.Itemid;
        }
        #endregion

        #region GetItemById
        public Item GetItemById(int itemId)
        {
            Item? item =
                _context.Items.FirstOrDefault(m => m.Itemid == itemId)
                ?? throw new InvalidOperationException($"Item with ID {itemId} not found.");
            return item;
        }
        #endregion

        #region GetItemsByCategoryId
        public async Task<IEnumerable<Item>> GetItemsByCategoryId(int categoryId)
        {
            return await _context.Items
                .Where(i => i.Categoryid == categoryId && i.Isdeleted == false)
                .ToListAsync();
        }
        #endregion

        #region SoftDeleteItem
        public bool SoftDeleteItem(int itemId)
        {
            var item = _context.Items.FirstOrDefault(i => i.Itemid == itemId);
            if (item != null)
            {
                item.Isdeleted = true;
                item.Modifiedat = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region UpdateMenuItem
        public bool UpdateMenuItem(Item menuItem)
        {
            if (menuItem != null)
            {
                _context.Items.Update(menuItem);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region GetAll
        public IQueryable<Item> GetAll()
        {
            return _context.Items.Where(u => u.Isdeleted == false && u.Isavailable == true);
        }
        #endregion

        #region GetItemsByCategoryIdPaginated
        public IQueryable<Item> GetItemsByCategoryIdPaginated(int categoryId)
        {
            return _context.Items.Where(i => i.Categoryid == categoryId && i.Isdeleted == false);
        }
        #endregion

        #region GetItemsByCategoryPaginated
        public async Task<PaginatedList<Item>> GetItemsByCategoryPaginated(
            int categoryId,
            int pageNumber,
            int pageSize
        )
        {
            var query = _context.Items.Where(
                i => i.Categoryid == categoryId && i.Isdeleted == false
            );
            return await PaginatedList<Item>.CreateAsync(query, pageNumber, pageSize);
        }
        #endregion

        #region GetPaginatedItemsByGroupIdAsync
        public async Task<(IEnumerable<Item>, int)> GetPaginatedItemsByGroupIdAsync(
            int CategoryId,
            int pageNumber,
            int pageSize,
            string searchQuery = ""
        )
        {
            var query = _context.Items
                .Where(item => item.Categoryid == CategoryId && item.Isdeleted == false)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                string trimmedSearch = searchQuery.Trim().ToLower();
                query = query.Where(
                    item =>
                        (item.Itemname != null && item.Itemname.ToLower().Contains(trimmedSearch))
                        || (
                            item.Shortcode != null
                            && item.Shortcode.ToLower().Contains(trimmedSearch)
                        )
                );
            }

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }
        #endregion

        #region IsDuplicateItem 
        public bool IsDuplicateItem(string itemName, int? itemId = null)
        {
            return _context.Items.Any(
                i =>
                    i.Itemname.ToLower() == itemName.ToLower()
                    && (itemId == null || i.Itemid != itemId)
            );
        }
        #endregion

        #region SoftDeleteItems
        public void SoftDeleteItems(List<int> itemIds)
        {
            var items = _context.Items.Where(i => itemIds.Contains(i.Itemid)).ToList();
            if (items.Any())
            {
                foreach (var item in items)
                {
                    item.Isdeleted = true;
                }
                _context.SaveChanges();
            }
        }
        #endregion

        #region GetItemWithModifiersAsync
        public async Task<ItemWithModifiersViewModel> GetItemWithModifiersAsync(int itemId)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
                return new ItemWithModifiersViewModel();

            var modifierGroups = await (
                from map in _context.Itemmodifiergroupmaps
                join mg in _context.Modifiergroups on map.Modifiergroupid equals mg.Modifiergroupid
                where map.Itemid == itemId
                select new ModifierGroupViewModel
                {
                    GroupId = mg.Modifiergroupid,
                    GroupName = mg.Modifiergroupname,
                    MinSelection = map.Minselectionrequired,
                    MaxSelection = map.Maxselectionallowed,
                    Modifiers = _context.Modifiers
                        .Where(m => m.Modifierid == mg.Modifiergroupid)
                        .Select(
                            m =>
                                new ModifierViewModel
                                {
                                    ModifierId = m.Modifierid,
                                    Modifiername = m.Modifiername,
                                    Rate = m.Rate
                                }
                        )
                        .ToList()
                }
            ).ToListAsync();

            return new ItemWithModifiersViewModel
            {
                ItemId = item.Itemid,
                ItemName = item.Itemname,
                BasePrice = item.Rate,
                ModifierGroups = modifierGroups
            };
        }
        #endregion

        #region UpdateStatusAsync
        public async Task UpdateStatusAsync(int itemId, string field, bool value)
        {
            if (field == "IsAvailable")
            {
                var item = await _context.Items.FirstOrDefaultAsync(i => i.Itemid == itemId);
                if (item == null)
                    throw new Exception("Item not found");

                item.Isavailable = value;
                _context.Items.Update(item);
            }
            else
            {
                throw new Exception("Invalid field");
            }

            await _context.SaveChangesAsync();
        }
        #endregion

        #region MarkAsFavoriteAsync
        public async Task<bool> MarkAsFavoriteAsync(int itemId, bool isFavorite)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
                return false;

            item.Isfavourite = isFavorite;
            _context.Items.Update(item);
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion

        #region GetItemNameByIdAsync
        public async Task<Item> GetItemNameByIdAsync(int id)
        {
            Item? item =
                await _context.Items.FirstAsync(m => m.Itemid == id)
                ?? throw new InvalidOperationException($"Item with ID {id} not found.");
            return item;
        }
        #endregion

        #region GetAllItemIds
        public async Task<List<int>> GetAllItemIds(int categoryId)
        {
            return await _context.Items
                .Where(item => item.Isdeleted == false && item.Categoryid == categoryId)
                .Select(item => item.Itemid)
                .ToListAsync();
        }
        #endregion

        #region GetAllSP
        public async Task<IQueryable<Item>> GetAllSP()
        {

            var listItems = await _context.Items
        .FromSqlRaw("SELECT * FROM get_all_items()")
        .ToListAsync();
            return listItems.AsQueryable();
        }
        #endregion

        #region MarkAsFavoriteAsyncSP
        public async Task<bool> MarkAsFavoriteAsyncSP(int itemId, bool isFavorite)
        {
            var result = await _context.Database.ExecuteSqlRawAsync(
        "CALL mark_as_favorite({0}, {1})",
        itemId, isFavorite
    );
            return true;
        }
        #endregion
    }
}


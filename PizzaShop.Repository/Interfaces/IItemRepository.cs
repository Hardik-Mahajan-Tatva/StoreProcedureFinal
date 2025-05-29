using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace Pizzashop.Repository.Interfaces
{
    public interface IItemRepository
    {
        /// <summary>
        /// Retrieves a list of items by the specified category ID.
        /// </summary>
        /// <param name="categoryId">The ID of the category to retrieve items for.</param>
        /// <returns>A task that returns a collection of items belonging to the specified category.</returns>
        Task<IEnumerable<Item>> GetItemsByCategoryId(int categoryId);

        /// <summary>
        /// Adds a new menu item along with its modifier group.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="modifierGroupId">The ID of the modifier group to associate with the item.</param>
        /// <returns>A task that returns the ID of the newly added item.</returns>
        Task<int> AddMenuItem(Item item, int modifierGroupId);

        /// <summary>
        /// Retrieves an item by its ID.
        /// </summary>
        /// <param name="id">The ID of the item to retrieve.</param>
        /// <returns>The item if found, otherwise null.</returns>
        Item GetItemById(int itemId);
        Task<Item> GetItemNameByIdAsync(int itemId);

        /// <summary>
        /// Updates an existing menu item.
        /// </summary>
        /// <param name="menuItem">The menu item to update.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        bool UpdateMenuItem(Item menuItem);

        /// <summary>
        /// Soft deletes an item by its ID.
        /// </summary>
        /// <param name="itemId">The ID of the item to soft delete.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        bool SoftDeleteItem(int itemId);

        /// <summary>
        /// Retrieves all items as an IQueryable for further filtering or querying.
        /// </summary>
        /// <returns>An IQueryable of all items.</returns>
        IQueryable<Item> GetAll();

        /// <summary>
        /// Retrieves items by category ID as an IQueryable for pagination.
        /// </summary>
        /// <param name="categoryId">The ID of the category to retrieve items for.</param>
        /// <returns>An IQueryable of items belonging to the specified category.</returns>
        IQueryable<Item> GetItemsByCategoryIdPaginated(int categoryId);

        /// <summary>
        /// Retrieves a paginated list of items by category ID.
        /// </summary>
        /// <param name="categoryId">The ID of the category to retrieve items for.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A task that returns a paginated list of items.</returns>
        Task<PaginatedList<Item>> GetItemsByCategoryPaginated(
            int categoryId,
            int pageNumber,
            int pageSize
        );

        /// <summary>
        /// Retrieves a paginated list of items by group ID with an optional search query.
        /// </summary>
        /// <param name="CategoryId">The ID of the category to retrieve items for.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="searchQuery">An optional search query to filter items.</param>
        /// <returns>A task that returns a tuple containing the items and the total count.</returns>
        Task<(IEnumerable<Item>, int)> GetPaginatedItemsByGroupIdAsync(
            int CategoryId,
            int pageNumber,
            int pageSize,
            string searchQuery = ""
        );

        /// <summary>
        /// Checks if an item with the specified name already exists, optionally excluding a specific item by ID.
        /// </summary>
        /// <param name="itemName">The name of the item to check.</param>
        /// <param name="itemId">The ID of the item to exclude from the check (optional).</param>
        /// <returns>True if a duplicate item exists, otherwise false.</returns>
        bool IsDuplicateItem(string itemName, int? itemId = null);

        /// <summary>
        /// Soft deletes multiple items by their IDs.
        /// </summary>
        /// <param name="itemIds">The list of item IDs to soft delete.</param>
        void SoftDeleteItems(List<int> itemIds);

        /// <summary>
        /// Retrieves an item along with its associated modifiers asynchronously.
        /// </summary>
        /// <param name="itemId">The ID of the item to retrieve.</param>
        /// <returns>A task that returns the item with its modifiers if found, otherwise null.</returns>
        Task<ItemWithModifiersViewModel> GetItemWithModifiersAsync(int itemId);

        /// <summary>
        /// Updates the status of a specific field for an item asynchronously.
        /// </summary>
        /// <param name="id">The ID of the item to update.</param>
        /// <param name="field">The field to update.</param>
        /// <param name="value">The new value for the field.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateStatusAsync(int id, string field, bool value);

        /// <summary>
        /// Updates the status of a item mark it as favorite asynchronously.
        /// </summary>
        /// <param name="id">The ID of the item to update.</param>
        /// <param name="isFavorite">The field to update bool.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<bool> MarkAsFavoriteAsync(int itemId, bool isFavorite);

        /// <summary>
        /// Gives the all items in as list having the given categoryId.
        /// </summary>
        /// <param name="categoryId">The ID of the category who's items needed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<List<int>> GetAllItemIds(int categoryId);
        Task<IQueryable<Item>> GetAllSP();
    }
}



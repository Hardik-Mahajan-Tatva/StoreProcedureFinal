using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces
{
    public interface ITableService
    {
        /// <summary>
        /// Retrieves all tables.
        /// </summary>
        /// <returns>A collection of table view models.</returns>
        IEnumerable<TableViewModel> GetAll();

        /// <summary>
        /// Retrieves tables by the specified section ID asynchronously.
        /// </summary>
        /// <param name="sectionId">The ID of the section to retrieve tables for.</param>
        /// <returns>A task that returns a collection of table view models.</returns>
        Task<IEnumerable<TableViewModel>> GetTablesBySection(int sectionId);

        /// <summary>
        /// Adds a new table asynchronously.
        /// </summary>
        /// <param name="model">The view model containing table details.</param>
        /// <returns>A task that returns true if the addition was successful, otherwise false.</returns>
        Task<bool> AddTableAsync(TableViewModel model);

        /// <summary>
        /// Adds a new table.
        /// </summary>
        /// <param name="model">The view model containing table details.</param>
        /// <returns>True if the addition was successful, otherwise false.</returns>
        bool AddTable(TableViewModel model);

        /// <summary>
        /// Retrieves a table by its ID.
        /// </summary>
        /// <param name="tableId">The ID of the table to retrieve.</param>
        /// <returns>The table if found, otherwise null.</returns>
        PizzaShop.Repository.Models.Table? GetById(int tableId);

        /// <summary>
        /// Updates an existing table.
        /// </summary>
        /// <param name="table">The table to update.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        bool Update(PizzaShop.Repository.Models.Table table);

        /// <summary>
        /// Updates an existing table using a view model.
        /// </summary>
        /// <param name="model">The view model containing updated table details.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        bool UpdateTable(TableViewModel model);

        /// <summary>
        /// Soft deletes a table by its ID.
        /// </summary>
        /// <param name="tableId">The ID of the table to soft delete.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        bool SoftDeleteTable(int tableId);

        /// <summary>
        /// Retrieves all tables grouped by their sections asynchronously.
        /// </summary>
        /// <returns>A task that returns a collection of table view models grouped by sections.</returns>
        Task<IEnumerable<TableViewModel>> GetTablesBySectionAsync();

        /// <summary>
        /// Retrieves a paginated list of tables by section ID with an optional search query.
        /// </summary>
        /// <param name="sectionId">The ID of the section to retrieve tables for.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of tables per page.</param>
        /// <param name="searchQuery">An optional search query to filter tables.</param>
        /// <returns>A task that returns a paginated list of table view models.</returns>
        Task<PaginatedList<TableViewModel>> GetPaginatedTablesBySectionIdAsync(int sectionId, int pageNumber, int pageSize, string searchQuery = "");

        /// <summary>
        /// Checks if a table name exists in a specific section, optionally excluding a specific table by ID.
        /// </summary>
        /// <param name="tableName">The name of the table to check.</param>
        /// <param name="sectionId">The ID of the section to check in.</param>
        /// <param name="excludeTableId">The ID of the table to exclude from the check (optional).</param>
        /// <returns>True if the table name exists, otherwise false.</returns>
        bool IsDuplicateTableName(string tableName, int sectionId, int? excludeTableId = null);

        /// <summary>
        /// Retrieves tables by the specified section ID.
        /// </summary>
        /// <param name="sectionId">The ID of the section to retrieve tables for.</param>
        /// <returns>A collection of tables belonging to the specified section.</returns>
        IEnumerable<PizzaShop.Repository.Models.Table> GetTablesBySectionId(int sectionId);

        /// <summary>
        /// Soft deletes multiple tables by their IDs.
        /// </summary>
        /// <param name="tableids">The list of table IDs to soft delete.</param>
        void SoftDeleteItems(List<int> tableids);
        Task UpdateTableStatusAsync(int tableId, int tableStatus);
        Task<IEnumerable<PizzaShop.Repository.Models.Table>> GetTablesBySectionsAsync(List<int> sectionIds);
        Task<List<PizzaShop.Repository.Models.Table>> GetTablesByIdsAsync(List<int> tableIds);
        /// <summary>
        /// Retrieves all table IDs for a specific section.
        /// </summary>
        /// <param name="sectionId">The ID of the section to retrieve table IDs for.</param>
        /// <returns>A task that returns a list of table IDs.</returns>
        Task<List<int>> GetAllTableIds(int sectionId);

        /// <summary>
        /// Soft deletes multiple tables by their IDs.
        /// </summary>
        /// <param name="itemIds">The list of table IDs to soft delete.</param>
        void DeleteMultipleTableAsync(List<int> tableIds);

        Task<List<TableViewModel>> GetTablesBySectionsUsingFunctionAsync(List<int> sectionIds);
    }
}
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces
{
    public interface ITableRepository
    {
        /// <summary>
        /// Retrieves all tables.
        /// </summary>
        /// <returns>A collection of all tables.</returns>
        IEnumerable<PizzaShop.Repository.Models.Table> GetAll();

        /// <summary>
        /// Retrieves tables by the specified section ID asynchronously.
        /// </summary>
        /// <param name="sectionId">The ID of the section to retrieve tables for.</param>
        /// <returns>A task that returns a collection of tables belonging to the specified section.</returns>
        Task<IEnumerable<PizzaShop.Repository.Models.Table>> GetTablesBySection(int sectionId);

        /// <summary>
        /// Adds a new table asynchronously.
        /// </summary>
        /// <param name="table">The table to add.</param>
        /// <returns>A task that returns true if the addition was successful, otherwise false.</returns>
        Task<bool> AddTableAsync(PizzaShop.Repository.Models.Table table);

        /// <summary>
        /// Adds a new table.
        /// </summary>
        /// <param name="table">The table to add.</param>
        /// <returns>True if the addition was successful, otherwise false.</returns>
        bool Add(PizzaShop.Repository.Models.Table table);

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
        /// Soft deletes a table by its ID.
        /// </summary>
        /// <param name="tableId">The ID of the table to soft delete.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        bool SoftDeleteTable(int tableId);

        /// <summary>
        /// Retrieves tables grouped by their sections asynchronously.
        /// </summary>
        /// <returns>A task that returns a collection of tables grouped by sections.</returns>
        Task<List<PizzaShop.Repository.Models.Table>> GetTablesGroupedBySectionAsync();

        /// <summary>
        /// Retrieves a paginated list of tables by section ID with an optional search query.
        /// </summary>
        /// <param name="sectionId">The ID of the section to retrieve tables for.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of tables per page.</param>
        /// <param name="searchQuery">An optional search query to filter tables.</param>
        /// <returns>A task that returns a tuple containing the tables and the total count.</returns>
        Task<(IEnumerable<PizzaShop.Repository.Models.Table>, int)> GetPaginatedTablesBySectionIdAsync(
            int sectionId,
            int pageNumber,
            int pageSize,
            string searchQuery = ""
        );

        /// <summary>
        /// Checks if a table name exists in a specific section, optionally excluding a specific table by ID.
        /// </summary>
        /// <param name="tableName">The name of the table to check.</param>
        /// <param name="sectionId">The ID of the section to check in.</param>
        /// <param name="excludeTableId">The ID of the table to exclude from the check (optional).</param>
        /// <returns>True if the table name exists, otherwise false.</returns>
        bool IsTableNameExists(string tableName, int sectionId, int? excludeTableId = null);

        /// <summary>
        /// Retrieves tables by the specified section ID.
        /// </summary>
        /// <param name="sectionId">The ID of the section to retrieve tables for.</param>
        /// <returns>A collection of tables belonging to the specified section.</returns>
        IEnumerable<PizzaShop.Repository.Models.Table> GetTablesBySectionId(int sectionId);

        /// <summary>
        /// Soft deletes multiple tables by their IDs.
        /// </summary>
        /// <param name="tableIds">The list of table IDs to soft delete.</param>
        void SoftDeleteItems(List<int> tableIds);

        /// <summary>
        /// Retrieves a table by its ID asynchronously.
        /// </summary>
        /// <param name="tableId">The ID of the table to retrieve.</param>
        /// <returns>The table if found, otherwise null.</returns>
        Task<Table?> GetTableByIdAsync(int tableId);

        /// <summary>
        /// Updates an existing table asynchronously.
        /// </summary>
        /// <param name="table">The table to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateTableAsync(Table table);

        /// <summary>
        /// Retrieves a list of sections along with their tables asynchronously.
        /// </summary>
        /// <returns>A task that returns a list of sections with their tables.</returns>
        Task<List<Section>> GetSectionsWithTablesAsync();

        /// <summary>
        /// Retrieves tables by a list of section IDs asynchronously.
        /// </summary>
        /// <param name="sectionIds">The list of section IDs to retrieve tables for.</param>
        /// <returns>A task that returns a collection of tables belonging to the specified sections.</returns>
        Task<IEnumerable<PizzaShop.Repository.Models.Table>> GetTablesBySectionsAsync(
            List<int> sectionIds
        );

        /// <summary>
        /// Retrieves tables by their IDs asynchronously.
        /// </summary>
        /// <param name="tableIds">The list of table IDs to retrieve.</param>
        /// <returns>A task that returns a list of tables with the specified IDs.</returns>
        Task<List<Table>> GetTablesByIdsAsync(List<int> tableIds);

        /// <summary>
        /// Gives the all tables in as list having the given sectionId.
        /// </summary>
        /// <param name="sectionId">The ID of the section who's tables needed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<List<int>> GetAllTableIds(int sectionId);

        /// <summary>
        /// Soft deletes multiple tables by their IDs.
        /// </summary>
        /// <param name="itemIds">The list of table IDs to soft delete.</param>
        void DeleteMultipleTableAsync(List<int> itemIds);
    }
}

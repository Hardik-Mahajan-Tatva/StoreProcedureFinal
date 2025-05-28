using System.Data;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using Table = PizzaShop.Repository.Models.Table;

namespace PizzaShop.Repository.Implementations
{
    public class TableRepository : ITableRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public TableRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region GetAll
        public IEnumerable<PizzaShop.Repository.Models.Table> GetAll()
        {
            return _context.Tables.Where(t => !t.Isdeleted ?? false).ToList();
        }
        #endregion

        #region GetPaginatedTablesBySectionIdAsync
        public async Task<(IEnumerable<PizzaShop.Repository.Models.Table>, int)> GetPaginatedTablesBySectionIdAsync(
            int sectionId,
            int pageNumber,
            int pageSize,
            string searchQuery
        )
        {
            var query = _context.Tables
                .Where(table => table.Sectionid == sectionId && table.Isdeleted == false)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                string trimmedSearch = searchQuery.Trim().ToLower();
                query = query.Where(
                    table =>
                        table.Tablename != null && table.Tablename.ToLower().Contains(trimmedSearch)
                );
            }

            var totalCount = await query.CountAsync();
            var tables = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (tables, totalCount);
        }
        #endregion

        #region GetTablesBySection
        public async Task<IEnumerable<Table>> GetTablesBySection(int sectionId)
        {
            return await _context.Tables
                .Where(
                    t => t.Sectionid == sectionId && (t.Isdeleted == false || t.Isdeleted == null)
                )
                .ToListAsync();
        }
        #endregion

        #region AddTableAsync
        public async Task<bool> AddTableAsync(Table table)
        {
            await _context.Tables.AddAsync(table);
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion

        #region Add
        public bool Add(PizzaShop.Repository.Models.Table table)
        {
            _context.Tables.Add(table);
            return _context.SaveChanges() > 0;
        }
        #endregion

        #region GetById
        public Table? GetById(int tableId)
        {
            return _context.Tables.FirstOrDefault(t => t.Tableid == tableId);
        }
        #endregion

        #region Update
        public bool Update(Table table)
        {
            _context.Tables.Update(table);
            return _context.SaveChanges() > 0;
        }
        #endregion

        #region SoftDeleteTable
        public bool SoftDeleteTable(int tableId)
        {
            var table = _context.Tables.FirstOrDefault(t => t.Tableid == tableId);
            if (table != null)
            {
                table.Isdeleted = true;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region GetTablesGroupedBySectionAsync
        public async Task<List<Table>> GetTablesGroupedBySectionAsync()
        {
            return await _context.Tables
                .Where(t => t.Isdeleted == false)
                .Include(ot => ot.Ordertables)
                .ThenInclude(ot => ot.Order)
                .Include(t => t.Section)
                .ToListAsync();
        }
        #endregion

        #region GetSectionsWithTablesAsync
        public async Task<List<Section>> GetSectionsWithTablesAsync()
        {
            return await _context.Sections
                .Where(s => s.Isdeleted == false)
                .Include(s => s.Tables)
                .ThenInclude(t => t.Ordertables)
                .ThenInclude(ot => ot.Order)
                .ToListAsync();
        }
        #endregion

        #region IsTableNameExists
        public bool IsTableNameExists(string tableName, int sectionId, int? excludeTableId = null)
        {
            return _context.Tables.Any(
                t =>
                    t.Tablename.ToLower() == tableName.ToLower()
                    && t.Sectionid == sectionId
                    && (excludeTableId == null || t.Tableid != excludeTableId)
            );
        }
        #endregion

        #region GetTablesBySectionId
        public IEnumerable<Table> GetTablesBySectionId(int sectionId)
        {
            return _context.Tables
                .Where(t => t.Sectionid == sectionId && t.Isdeleted == false)
                .ToList();
        }
        #endregion

        #region SoftDeleteItems
        public void SoftDeleteItems(List<int> tableids)
        {
            var tables = _context.Tables.Where(i => tableids.Contains(i.Tableid)).ToList();
            if (tables.Any())
            {
                foreach (var table in tables)
                {
                    table.Isdeleted = true;
                }
                _context.SaveChanges();
            }
        }
        #endregion

        #region GetTableByIdAsync
        public async Task<Table?> GetTableByIdAsync(int tableId)
        {
            return await _context.Tables.FirstOrDefaultAsync(t => t.Tableid == tableId);
        }
        #endregion

        #region UpdateTableAsync
        public async Task UpdateTableAsync(Table table)
        {
            if (table.Modifiedat.HasValue)
                table.Modifiedat = ConvertToISTUnspecified(table.Modifiedat.Value);

            _context.Tables.Update(table);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region ConvertToISTUnspecified
        private static DateTime ConvertToISTUnspecified(DateTime dt)
        {
            TimeZoneInfo indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime istTime = TimeZoneInfo.ConvertTimeFromUtc(dt.ToUniversalTime(), indiaTimeZone);
            return DateTime.SpecifyKind(istTime, DateTimeKind.Unspecified);
        }
        #endregion

        #region GetTablesBySectionsAsync
        public async Task<IEnumerable<Table>> GetTablesBySectionsAsync(List<int> sectionIds)
        {
            return await _context.Tables
                .Include(t => t.Section)
                .Where(t => sectionIds.Contains(t.Sectionid ?? 0) && t.Tablestatus == 1)
                .ToListAsync();
        }
        #endregion


        #region GetTablesByIdsAsync
        public async Task<List<Table>> GetTablesByIdsAsync(List<int> tableIds)
        {
            return await _context.Tables
                .Where(t => tableIds.Contains(t.Tableid) && t.Isdeleted == false)
                .ToListAsync();
        }
        #endregion

        #region GetAllTableIds
        public async Task<List<int>> GetAllTableIds(int sectionId)
        {
            return await _context.Tables
                .Where(table => table.Isdeleted == false && table.Sectionid == sectionId)
                .Select(table => table.Tableid)
                .ToListAsync();
        }
        #endregion

        #region DeleteMultipleTableAsync
        public void DeleteMultipleTableAsync(List<int> tableIds)
        {
            var tables = _context.Tables.Where(i => tableIds.Contains(i.Tableid)).ToList();
            if (tables.Any())
            {
                foreach (var item in tables)
                {
                    item.Isdeleted = true;
                }
                _context.SaveChanges();
            }
        }
        #endregion

        #region GetTablesBySectionsUsingFunctionAsync
        public async Task<List<TableViewModel>> GetTablesBySectionsUsingFunctionAsync(List<int> sectionIds)
        {
            var tables = new List<TableViewModel>();
            var connection = _context.Database.GetDbConnection();
            var sectionIdArray = sectionIds.ToArray();

            await using (connection)
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();

                command.CommandText = "SELECT * FROM get_tables_by_section_ids(@section_ids)";
                command.CommandType = CommandType.Text;

                var param = new Npgsql.NpgsqlParameter("@section_ids", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer)
                {
                    Value = sectionIdArray
                };
                command.Parameters.Add(param);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    tables.Add(new TableViewModel
                    {
                        TableId = reader.GetInt32(0),
                        TableName = reader.GetString(1),
                        Status = (TableStatus)reader.GetInt32(2),
                        SelectedSectionName = reader.GetString(3)
                    });
                }
            }
            return tables;
        }
        #endregion


    }
}


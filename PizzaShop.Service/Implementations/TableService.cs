using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;
namespace PizzaShop.Service.Implementations
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _tableRepository;
        private readonly IOrderRepository _orderRepository;
        public TableService(ITableRepository tableRepository, IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
        }

        public IEnumerable<TableViewModel> GetAll()
        {
            var tables = _tableRepository.GetAll();
            return tables.Select(t => new TableViewModel
            {
                TableId = t.Tableid,
                TableName = t.Tablename,
                SectionId = t.Sectionid,
                IsDeleted = t.Isdeleted
            });
        }
        public async Task<PaginatedList<TableViewModel>> GetPaginatedTablesBySectionIdAsync(int sectionId, int pageNumber, int pageSize, string searchQuery)
        {
            var (tables, totalCount) = await _tableRepository.GetPaginatedTablesBySectionIdAsync(sectionId, pageNumber, pageSize, searchQuery);
            var mappedTables = tables.Select(table => new TableViewModel
            {
                TableId = table.Tableid,
                TableName = table.Tablename,
                Status = (TableStatus)(table.Tablestatus ?? 0),
                Capacity = (int)table.Capacity
            }).ToList();

            return new PaginatedList<TableViewModel>(mappedTables, totalCount, pageNumber, pageSize);
        }
        public async Task<IEnumerable<TableViewModel>> GetTablesBySection(int sectionId)
        {
            var tables = await _tableRepository.GetTablesBySection(sectionId);

            return tables.Select(t => new TableViewModel
            {
                TableId = t.Tableid,
                TableName = t.Tablename,
                Capacity = (int)t.Capacity,
                Status = (TableStatus)(t.Tablestatus ?? 0)
            });
        }

        public async Task<bool> AddTableAsync(TableViewModel model)
        {
            var table = new Repository.Models.Table
            {
                Tablename = model.TableName!,
                Sectionid = model.SectionId,
                Capacity = model.Capacity,
                Tablestatus = (int?)model.Status
            };

            return await _tableRepository.AddTableAsync(table);
        }
        public bool AddTable(TableViewModel model)
        {
            var table = new PizzaShop.Repository.Models.Table
            {
                Tablename = model.TableName!,
                Sectionid = model.SectionId,
                Capacity = model.Capacity,
                Tablestatus = (int?)model.Status
            };

            return _tableRepository.Add(table);
        }

        public PizzaShop.Repository.Models.Table? GetById(int tableId)
        {
            return _tableRepository.GetById(tableId);
        }

        public bool Update(PizzaShop.Repository.Models.Table table)
        {
            return _tableRepository.Update(table);
        }
        public bool UpdateTable(TableViewModel model)
        {
            var table = _tableRepository.GetById(model.TableId);
            if (table == null) return false;

            table.Tablename = model.TableName!;
            table.Capacity = model.Capacity;
            table.Tablestatus = (int?)model.Status;

            return _tableRepository.Update(table);
        }

        public bool SoftDeleteTable(int tableId)
        {
            return _tableRepository.SoftDeleteTable(tableId);
        }
        public async Task<IEnumerable<TableViewModel>> GetTablesBySectionAsync()
        {
            var tables = await _tableRepository.GetTablesGroupedBySectionAsync();

            return tables.Select(t =>
            {
                var tableStatus = (TableStatus)(t.Tablestatus ?? 0);
                int? orderId = null;
                decimal totalAmount = 0;

                if (tableStatus == TableStatus.Occupied)
                {
                    var associatedOrder = t.Ordertables?.Where(otm => otm.Order != null).OrderByDescending(otm => otm.Order.Modifiedat).FirstOrDefault();
                    totalAmount = associatedOrder?.Order.Totalamount ?? 0;

                    if (associatedOrder != null)
                    {
                        orderId = associatedOrder.Order.Orderid;
                        switch (associatedOrder.Order.Status)
                        {

                            case (int)OrderStatus.Pending:
                                tableStatus = TableStatus.Occupied;
                                break;

                            case (int)OrderStatus.InProgress:
                            case (int)OrderStatus.Served:
                                tableStatus = TableStatus.Running;
                                break;

                            default:
                                tableStatus = TableStatus.Occupied;
                                break;
                        }
                    }
                }

                return new TableViewModel
                {
                    SectionId = t.Sectionid,
                    TableId = t.Tableid,
                    TableName = t.Tablename,
                    SelectedSectionName = t.Section!.Sectionname,
                    Status = tableStatus,
                    Capacity = (int)t.Capacity,
                    OccupiedStartTime = t.Modifiedat,
                    OrderId = orderId,
                    TotoaAmount = totalAmount,
                };
            }).ToList();
        }
        public bool IsDuplicateTableName(string tableName, int sectionId, int? excludeTableId = null)
        {
            return _tableRepository.IsTableNameExists(tableName, sectionId, excludeTableId);
        }

        public IEnumerable<PizzaShop.Repository.Models.Table> GetTablesBySectionId(int sectionId)
        {
            return _tableRepository.GetTablesBySectionId(sectionId);
        }
        public void SoftDeleteItems(List<int> tableids)
        {
            _tableRepository.SoftDeleteItems(tableids);
        }
        public async Task UpdateTableStatusAsync(int tableId, int tableStatus)
        {
            try
            {
                var table = await _tableRepository.GetTableByIdAsync(tableId);
                if (table == null)
                    throw new Exception($"Table with ID {tableId} not found.");

                table.Tablestatus = tableStatus;
                table.Modifiedat = DateTime.UtcNow;

                await _tableRepository.UpdateTableAsync(table);
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                throw new Exception("An error occurred while updating table status. Details: " + error, ex);
            }
        }
        public async Task<IEnumerable<PizzaShop.Repository.Models.Table>> GetTablesBySectionsAsync(List<int> sectionIds)
        {
            return await _tableRepository.GetTablesBySectionsAsync(sectionIds);
        }
        public async Task<List<PizzaShop.Repository.Models.Table>> GetTablesByIdsAsync(List<int> tableIds)
        {
            var tables = await _tableRepository.GetTablesByIdsAsync(tableIds);

            if (tables == null || !tables.Any())
            {
                throw new InvalidOperationException("No tables found with the provided IDs.");
            }

            return tables;
        }
        public async Task<List<int>> GetAllTableIds(int sectionId)
        {
            return await _tableRepository.GetAllTableIds(sectionId);
        }
        public void DeleteMultipleTableAsync(List<int> tableIds)
        {
            _tableRepository.DeleteMultipleTableAsync(tableIds);
        }
        public async Task<List<TableViewModel>> GetTablesBySectionsUsingFunctionAsync(List<int> sectionIds)
        {
            return await _tableRepository.GetTablesBySectionsUsingFunctionAsync(sectionIds);
        }
        public async Task<List<TableViewModel>> GetTablesBySectionSPAsync()
        {
            var rawTables = await _tableRepository.GetTableViewFromFunctionAsync();

            return rawTables.Select(t =>
            {
                var tableStatus = (TableStatus)(t.TableStatus);
                if (tableStatus == TableStatus.Occupied)
                {
                    if (t.OrderId.HasValue && t.OrderStatus.HasValue)
                    {
                        switch (t.OrderStatus.Value)
                        {
                            case (int)OrderStatus.Pending:
                                tableStatus = TableStatus.Occupied;
                                break;
                            case (int)OrderStatus.InProgress:
                            case (int)OrderStatus.Served:
                                tableStatus = TableStatus.Running;
                                break;
                            default:
                                tableStatus = TableStatus.Occupied;
                                break;
                        }
                    }
                }

                return new TableViewModel
                {
                    SectionId = t.SectionId,
                    TableId = t.TableId,
                    TableName = t.TableName,
                    SelectedSectionName = t.SectionName,
                    Status = tableStatus,
                    Capacity = t.Capacity,
                    OccupiedStartTime = t.ModifiedAt,
                    OrderId = t.OrderId,
                    TotoaAmount = t.TotalAmount ?? 0
                };
            }).ToList();
        }

    }
}


using Microsoft.EntityFrameworkCore;
using Npgsql;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Repository.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        #region Dependencies
        private readonly PizzaShopContext _context;

        public OrderRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region Get All Orders
        public async Task<PaginatedList<Order>> GetOrdersAsync(
            string search,
            string status,
            DateTime? startDate,
            DateTime? endDate,
            int page,
            int pageSize,
            string sortColumn,
            string sortDirection
        )
        {
            var query = _context.Orders.Include(o => o.Customer).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string trimmedSearch = search.Trim().ToLower();
                query = query.Where(
                    o =>
                        (
                            o.Customer != null
                            && o.Customer.Customername.ToLower().Contains(trimmedSearch)
                        )
                        || o.Orderid.ToString().Contains(trimmedSearch)
                        || (
                            o.Paymentmode != null && o.Paymentmode.ToLower().Contains(trimmedSearch)
                        )
                );
            }

            if (!string.IsNullOrEmpty(status) && int.TryParse(status, out int statusValue))
            {
                query = query.Where(o => o.Status == statusValue);
            }

            if (startDate.HasValue)
            {
                query = query.Where(
                    o => o.Orderdate.HasValue && o.Orderdate.Value.Date >= startDate.Value.Date
                );
            }
            if (endDate.HasValue)
            {
                DateTime endOfDay = endDate.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(o => o.Orderdate.HasValue && o.Orderdate.Value <= endOfDay);
            }

            query = sortColumn switch
            {
                "CustomerName"
                  => sortDirection == "asc"
                      ? query.OrderBy(o => o.Customer.Customername ?? string.Empty)
                      : query.OrderByDescending(o => o.Customer.Customername ?? string.Empty),
                "OrderDate"
                  => sortDirection == "asc"
                      ? query.OrderBy(o => o.Orderdate).ThenBy(o => o.Orderid)
                      : query.OrderByDescending(o => o.Orderdate).ThenByDescending(o => o.Orderid),
                "TotalAmount"
                  => sortDirection == "asc"
                      ? query.OrderBy(o => o.Totalamount).ThenBy(o => o.Orderid)
                      : query
                        .OrderByDescending(o => o.Totalamount)
                        .ThenByDescending(o => o.Orderid),
                _
                  => sortDirection == "asc"
                      ? query.OrderBy(o => o.Orderid)
                      : query.OrderByDescending(o => o.Orderid),
            };

            return await PaginatedList<Order>.CreateAsync(query, page, pageSize);
        }
        #endregion

        #region Get All Orders for Customer
        public IQueryable<Order> GetAll()
        {
            return _context.Orders.Include(o => o.Customer).AsNoTracking();
        }
        #endregion

        #region GetOrderWithDetailsAsync
        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Ordereditems)
                .Include(o => o.Invoices)
                .Include(o => o.Ordertables)
                .ThenInclude(ot => ot.Table)
                .ThenInclude(t => t.Section)
                .FirstOrDefaultAsync(o => o.Orderid == orderId);

            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "Order object is null.");
            }
            else
            {
                if (order.Ordertables.Any())
                {
                    var orderTable = order.Ordertables.FirstOrDefault();
                }
            }
            return order;
        }
        #endregion

        #region GetOrderItemsAsync
        public async Task<List<OrderItemViewModel>> GetOrderItemsAsync(int orderId)
        {
            var orderItems = await _context.Ordereditems
                .Where(oi => oi.Orderid == orderId)
                .Join(
                    _context.Items,
                    oi => oi.Itemid,
                    item => item.Itemid,
                    (oi, item) =>
                        new
                        {
                            oi.Ordereditemid,
                            oi.Itemid,
                            oi.Quantity,
                            oi.Orderid,
                            ItemName = item.Itemname,
                            UnitPrice = item.Rate
                        }
                )
                .ToListAsync();

            var orderedItemIds = orderItems.Select(oi => oi.Ordereditemid).ToList();

            var orderedModifiers = await _context.Ordereditemmodifers
                .Where(oim => orderedItemIds.Contains(oim.Ordereditemid))
                .Join(
                    _context.Modifiers,
                    oim => oim.Itemmodifierid,
                    mod => mod.Modifierid,
                    (oim, mod) =>
                        new
                        {
                            oim.Ordereditemid,
                            ModifierName = mod.Modifiername,
                            ModifierPrice = mod.Rate,
                            ModifierQuantity = oim.Quantity
                        }
                )
                .ToListAsync();

            var orderItemViewModels = orderItems
                .Select(
                    oi =>
                    {
                        var itemModifiers = orderedModifiers
                            .Where(mod => mod.Ordereditemid == oi.Ordereditemid)
                            .Select(
                                mod =>
                                    new ModifierViewModel
                                    {
                                        Modifieditemid = oi.Itemid,
                                        Modifiername = mod.ModifierName,
                                        Rate = mod.ModifierPrice,
                                        Quantity = mod.ModifierQuantity
                                    }
                            )
                            .ToList();

                        decimal modifiersTotal = itemModifiers.Sum(
                            m => ((decimal?)m.Rate ?? 0) * (m.Quantity ?? 0)
                        );

                        return new OrderItemViewModel
                        {
                            ItemName = oi.ItemName,
                            ItemId = oi.Itemid,
                            Quantity = oi.Quantity,
                            UnitPrice = oi.UnitPrice,
                            Total = oi.Quantity * oi.UnitPrice,
                            ModifierTotal = modifiersTotal,
                            Modifiers = itemModifiers
                        };
                    }
                )
                .ToList();

            return orderItemViewModels;
        }
        #endregion

        #region GetKOTOrdersAsync
        public async Task<(List<Order>, int)> GetKOTOrdersAsync(
            string itemStatus,
            string categoryName,
            int page,
            int pageSize
        )
        {
            var query = _context.Orders
                .Include(o => o.Ordereditems)
                .ThenInclude(oi => oi.Item)
                .ThenInclude(i => i.Category)
                .Include(o => o.Ordereditems)
                .ThenInclude(oi => oi.Ordereditemmodifers)
                .ThenInclude(mod => mod.Itemmodifier)
                .Include(o => o.Ordertables)
                .ThenInclude(ot => ot.Table)
                .ThenInclude(t => t.Section)
                .Where(
                    o =>
                        o.Status == 1
                        && o.Ordereditems.Any(
                            oi =>
                                categoryName == "All"
                                || oi.Item.Category.Categoryname == categoryName
                        )
                )
                .AsQueryable();

            if (itemStatus == "InProgress")
            {
                query = query.Where(o => o.Ordereditems.Any(oi => oi.Readyquantity < oi.Quantity));
            }
            else if (itemStatus == "Ready")
            {
                query = query.Where(o => o.Ordereditems.Any(oi => oi.Readyquantity > 0));
            }

            var totalOrders = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.Orderid)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orders, totalOrders);
        }
        #endregion

        #region GetElapsedTime
        private static string GetElapsedTime(DateTime orderTime)
        {
            var elapsed = DateTime.UtcNow - orderTime;
            return $"{(int)elapsed.TotalDays} days {elapsed.Hours} hours {elapsed.Minutes} min {elapsed.Seconds} sec";
        }
        #endregion

        #region CreateOrderAsync
        public async Task<int> CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order.Orderid;
        }
        #endregion

        #region KOT
        public async Task<List<Order>> GetOrdersWithDetailsAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == 1)
                .Include(o => o.Ordertables)
                .ThenInclude(ot => ot.Table)
                .ThenInclude(t => t.Section)
                .Include(o => o.Ordereditems)
                .ThenInclude(oi => oi.Ordereditemmodifers)
                .ToListAsync();
        }
        #endregion

        public async Task<
            Dictionary<int, (string Name, decimal Rate, int CategoryId)>
        > GetItemDictionaryAsync()
        {
            return await _context.Items.ToDictionaryAsync(
                i => i.Itemid,
                i => (i.Itemname, i.Rate, i.Categoryid)
            );
        }

        public async Task<Dictionary<int, (string Name, decimal Rate)>> GetModifierDictionaryAsync()
        {
            return await _context.Modifiers.ToDictionaryAsync(
                m => m.Modifierid,
                m => (m.Modifiername, m.Rate)
            );
        }

        #region GetOrderWithDetails
        public async Task<Order?> GetOrderWithDetails(int? orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Ordereditems)
                .Include(o => o.Invoices)
                .Include(o => o.Ordertables)
                .ThenInclude(ot => ot.Table)
                .ThenInclude(t => t.Section)
                .FirstOrDefaultAsync(o => o.Orderid == orderId);

            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "Order object is null.");
            }
            else
            {
                if (order.Ordertables.Any())
                {
                    var orderTable = order.Ordertables.FirstOrDefault();
                }
            }

            return order;
        }
        #endregion

        #region SaveOrderComment
        public async Task<bool> SaveOrderComment(int orderId, string comment)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Orderid == orderId);
            if (order == null)
                return false;

            order.Orderwisecomment = comment;
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion

        #region GetOrderById
        public async Task<Order> GetOrderById(int orderId)
        {
            var order =
                await _context.Orders.FirstOrDefaultAsync(o => o.Orderid == orderId)
                ?? throw new InvalidOperationException($"Order with ID {orderId} not found.");
            return order;
        }
        #endregion

        #region GetSpecialInstruction
        public async Task<string> GetSpecialInstruction(int orderId, int orderedItemId)
        {
            var orderItem = await _context.Ordereditems.FirstOrDefaultAsync(
                oi => oi.Orderid == orderId && oi.Ordereditemid == orderedItemId
            );
            return orderItem?.Itemwisecomment ?? string.Empty;
        }
        #endregion
        #region GetSpecialInstructionSP
        public async Task<string> GetSpecialInstructionSP(int orderId, int orderedItemId)
        {
            var parameters = new[]
            {
        new NpgsqlParameter("@p_order_id", orderId),
        new NpgsqlParameter("@p_ordered_item_id", orderedItemId)
    };

            var orderedItem = await _context.Ordereditems
                .FromSqlRaw("SELECT * FROM get_special_instruction(@p_order_id, @p_ordered_item_id)", parameters)
                .FirstOrDefaultAsync();

            if (orderedItem == null)
            {
                throw new InvalidOperationException($"Ordered item not found for Order ID {orderId} and Item ID {orderedItemId}.");
            }

            return orderedItem.Itemwisecomment ??
                throw new InvalidOperationException($"No special instruction found for ordered item {orderedItemId} in order {orderId}.");
        }
        #endregion


        #region SaveSpecialInstruction
        public async Task<bool> SaveSpecialInstruction(
            int orderId,
            int orderedItemId,
            string instruction
        )
        {
            var orderItem = await _context.Ordereditems.FirstOrDefaultAsync(
                oi => oi.Orderid == orderId && oi.Ordereditemid == orderedItemId
            );
            if (orderItem == null)
                return false;

            orderItem.Itemwisecomment = instruction;
            _context.Ordereditems.Update(orderItem);
            return _context.SaveChanges() > 0;
        }
        #endregion

        #region GetOrderItemsAsync
        public async Task<List<OrderItemViewModel>> GetOrderItemsAsync(int orderId, int modifierId)
        {
            var orderItems = _context.Ordereditems
                .Where(oi => oi.Orderid == orderId)
                .Join(
                    _context.Items,
                    oi => oi.Itemid,
                    item => item.Itemid,
                    (oi, item) =>
                        new
                        {
                            oi.Ordereditemid,
                            oi.Itemid,
                            oi.Quantity,
                            oi.Orderid,
                            oi.Itemwisecomment,
                            oi.Readyquantity,
                            ItemName = item.Itemname,
                            ItemId = item.Itemid,
                            UnitPrice = item.Rate
                        }
                )
                .ToList();

            var orderedModifiers = await _context.Ordereditemmodifers
                .Where(oim => oim.Orderid == orderId)
                .Join(
                    _context.Modifiers,
                    oim => oim.Itemmodifierid,
                    mod => mod.Modifierid,
                    (oim, mod) =>
                        new
                        {
                            oim.Ordereditemid,
                            oim.Orderid,
                            oim.Quantity,
                            oim.Modifieditemid,
                            oim.Itemmodifierid,
                            ModifierName = mod.Modifiername,
                            ModifierPrice = mod.Rate
                        }
                )
                .ToListAsync();

            var orderItemViewModels = orderItems
                .Select(
                    oi =>
                    {
                        var itemModifiers = orderedModifiers
                            .Where(mod => mod.Ordereditemid == oi.Ordereditemid)
                            .Select(
                                mod =>
                                    new ModifierViewModel
                                    {
                                        Modifiername = mod.ModifierName,
                                        Rate = mod.ModifierPrice,
                                        Quantity = mod.Quantity,
                                        Modifieditemid = mod.Modifieditemid,
                                        Ordereditemid = mod.Ordereditemid,
                                        Orderid = mod.Orderid,
                                        Itemmodifierid = mod.Itemmodifierid,
                                    }
                            )
                            .ToList();

                        decimal modifiersTotal = itemModifiers.Sum(
                            m => ((decimal?)m.Rate ?? 0) * (m.Quantity ?? 0)
                        );

                        return new OrderItemViewModel
                        {
                            ItemName = oi.ItemName,
                            OrderItemId = oi.Ordereditemid,
                            ItemId = oi.Itemid,
                            Quantity = oi.Quantity,
                            UnitPrice = oi.UnitPrice,
                            Total = (oi.Quantity * oi.UnitPrice) + modifiersTotal,
                            ModifierTotal = modifiersTotal,
                            Modifiers = itemModifiers,
                            ItemWiseComment = oi.Itemwisecomment,
                            ReadyQuantity = oi.Readyquantity
                        };
                    }
                )
                .ToList();

            return orderItemViewModels;
        }
        #endregion

        #region GetByIdAsync
        public async Task<Order> GetByIdAsync(int orderId)
        {
            var order =
                await _context.Orders
                    .Include(o => o.Ordertables)
                    .ThenInclude(ot => ot.Table)
                    .Include(o => o.Customer)
                    .FirstOrDefaultAsync(o => o.Orderid == orderId)
                ?? throw new InvalidOperationException($"Order with ID {orderId} not found.");
            return order;
        }
        #endregion

        #region UpdateAsync
        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region GetOrderedItemsAsync
        public async Task<List<Ordereditem>> GetOrderedItemsAsync(int orderId)
        {
            return await _context.Ordereditems.Where(oi => oi.Orderid == orderId).ToListAsync();
        }
        #endregion

        #region GetOrderedItemModifiersAsync
        public async Task<List<Ordereditemmodifer>> GetOrderedItemModifiersAsync(int orderId)
        {
            return await _context.Ordereditemmodifers
                .Where(oim => oim.Orderid == orderId)
                .ToListAsync();
        }
        #endregion

        #region GetItemsByIdsAsync
        public async Task<List<Item>> GetItemsByIdsAsync(List<int> itemIds)
        {
            return await _context.Items.Where(i => itemIds.Contains(i.Itemid)).ToListAsync();
        }
        #endregion

        #region GetModifiersByIdsAsync
        public async Task<List<Modifier>> GetModifiersByIdsAsync(List<int> modifierIds)
        {
            return await _context.Modifiers
                .Where(m => modifierIds.Contains(m.Modifierid))
                .ToListAsync();
        }
        #endregion

        #region GetOrderWithTableAsync
        public async Task<Order?> GetOrderWithTableAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Ordertables)
                .ThenInclude(ot => ot.Table)
                .ThenInclude(t => t.Section)
                .FirstOrDefaultAsync(o => o.Orderid == orderId);
        }
        #endregion

        #region SaveChangesAsync
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion

        #region GetTotalSalesByTimeRangeAsync
        public async Task<decimal> GetTotalSalesByTimeRangeAsync(DateTime? startDate, DateTime? endDate)
        {
            return await _context.Orders
                .Where(o =>
                    (!startDate.HasValue || (o.Orderdate.HasValue && o.Orderdate.Value >= startDate.Value)) &&
                    (!endDate.HasValue || (o.Orderdate.HasValue && o.Orderdate.Value < endDate.Value))
                )
                .SumAsync(o => o.Totalamount);
        }

        #endregion

        #region GetTotalOrdersByTimeRangeAsync
        public async Task<int> GetTotalOrdersByTimeRangeAsync(
            DateTime? startDate,
            DateTime? endDate
        )
        {
            return await _context.Orders
                .Where(
                    o =>
                        (
                            !startDate.HasValue
                            || (
                                o.Orderdate.HasValue
                                && o.Orderdate.Value.Date >= startDate.Value.Date
                            )
                        )
                        && (
                            !endDate.HasValue
                            || (
                                o.Orderdate.HasValue && o.Orderdate.Value.Date <= endDate.Value.Date
                            )
                        )
                )
                .CountAsync();
        }
        #endregion

        #region GetAvgOrderValueByTimeRangeAsync
        public async Task<decimal> GetAvgOrderValueByTimeRangeAsync(
            DateTime? startDate,
            DateTime? endDate
        )
        {
            var totalSales = await _context.Orders
                .Where(
                    o =>
                        (
                            !startDate.HasValue
                            || (
                                o.Orderdate.HasValue
                                && o.Orderdate.Value.Date >= startDate.Value.Date
                            )
                        )
                        && (
                            !endDate.HasValue
                            || (
                                o.Orderdate.HasValue && o.Orderdate.Value.Date <= endDate.Value.Date
                            )
                        )
                )
                .SumAsync(o => o.Totalamount);

            var totalOrders = await _context.Orders
                .Where(
                    o =>
                        (
                            !startDate.HasValue
                            || (
                                o.Orderdate.HasValue
                                && o.Orderdate.Value.Date >= startDate.Value.Date
                            )
                        )
                        && (
                            !endDate.HasValue
                            || (
                                o.Orderdate.HasValue && o.Orderdate.Value.Date <= endDate.Value.Date
                            )
                        )
                )
                .CountAsync();

            return totalOrders > 0 ? Math.Round(totalSales / totalOrders, 4) : 0;
        }
        #endregion

        #region GetTotalSalesBySpecificDayAsync
        public async Task<decimal> GetTotalSalesBySpecificDayAsync(DateTime date)
        {
            return await _context.Orders
                .Where(o => o.Orderdate.HasValue && o.Orderdate.Value.Date == date.Date)
                .SumAsync(o => o.Totalamount);
        }
        #endregion

        #region GetLatestOrderByCustomerIdAsync
        public async Task<Order?> GetLatestOrderByCustomerIdAsync(int customerId)
        {
            return await _context.Orders
                .Where(o => o.Customerid == customerId)
                .OrderByDescending(o => o.Orderdate)
                .FirstOrDefaultAsync();
        }
        #endregion

        #region GetTotalSalesGroupedByDayAsync
        public async Task<List<ChartDataPoint>> GetTotalSalesGroupedByDayAsync(
            DateTime startDate,
            DateTime endDate
        )
        {
            return await _context.Orders
                .Where(
                    o =>
                        o.Orderdate.HasValue
                        && o.Orderdate.Value.Date >= startDate.Date
                        && o.Orderdate.Value.Date <= endDate.Date
                )
                .GroupBy(o => o.Orderdate!.Value.Date)
                .Select(
                    g =>
                        new ChartDataPoint
                        {
                            Label = g.Key.ToString("yyyy-MM-dd"),
                            Value = g.Sum(o => o.Totalamount)
                        }
                )
                .ToListAsync();
        }
        #endregion

        #region UpdateOrdersAsync
        public async Task<bool> UpdateOrdersAsync(IEnumerable<Order> orders)
        {
            _context.Orders.UpdateRange(orders);
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion

        #region HasOrderWithStatusAsync
        public async Task<bool> HasOrderWithStatusAsync(int customerId, int[] statuses)
        {
            return await _context.Orders.AnyAsync(
                o => o.Customerid == customerId && statuses.Contains(o.Status)
            );
        }
        #endregion

        public async Task<List<KOTFlatData>> GetKOTDataFromProcedureAsync(int pageNumber, int pageSize, int categoryId, string orderStatus, string itemStatus)
        {
            var parameters = new[]
            {
        new NpgsqlParameter("@PageNumber", pageNumber),
        new NpgsqlParameter("@PageSize", pageSize),
        new NpgsqlParameter("@CategoryId", categoryId),
        new NpgsqlParameter("@OrderStatus", orderStatus ?? string.Empty),
        new NpgsqlParameter("@ItemStatus", itemStatus ?? string.Empty)
    };

            var result = await _context.KOTFlatData
                .FromSqlRaw("SELECT * FROM sp_get_kot_data(@PageNumber, @PageSize, @CategoryId, @OrderStatus, @ItemStatus)", parameters)
                .ToListAsync();

            return result;
        }

        #region GetOrderByIdSP
        public async Task<Order> GetOrderByIdSP(int orderId)
        {
            var order = await _context.Orders
        .FromSqlRaw("SELECT * FROM get_order_by_orderid({0})", orderId)
        .FirstOrDefaultAsync();

            return order ?? throw new InvalidOperationException($"Order with ID {orderId} not found.");
        }
        #endregion
        #region SaveOrderCommentSP
        public async Task<bool> SaveOrderCommentSP(int orderId, string comment)
        {
            var result = await _context.Database.ExecuteSqlRawAsync(
         "CALL save_order_comment({0}, {1})", orderId, comment);

            return true;
        }
        #endregion

        #region SaveSpecialInstructionSP
        public async Task<bool> SaveSpecialInstructionSP(
            int orderId,
            int orderedItemId,
            string instruction
        )
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL save_special_instruction({0}, {1}, {2})",
                    orderId, orderedItemId, instruction
                );

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region CancelOrderByStoredProcAsyncSP
        public async Task<bool> CancelOrderByStoredProcAsyncSP(int orderId)
        {
            await _context.Database.ExecuteSqlRawAsync(
                          "CALL cancel_order({0})", orderId);

            return true;
        }
        #endregion

        
        #region MarkOrderAsCompleteByStoredProcAsync
        public async Task<bool> MarkOrderAsCompleteByStoredProcAsync(int orderId)
        {
            var result = await _context.Database.ExecuteSqlRawAsync(
                "CALL mark_order_as_complete({0})", orderId);

            return true;
        }
        #endregion

    }
}

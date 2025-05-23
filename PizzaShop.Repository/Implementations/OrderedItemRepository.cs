using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Repository.Implementations
{
    public class OrderedItemRepository : IOrderedItemRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor

        public OrderedItemRepository(PizzaShopContext context)
        {
            _context = context;
        }

        #endregion

        #region UpdateReadyQuantityAsync
        public async Task UpdateReadyQuantityAsync(int orderedItemId, int readyQuantity)
        {
            var item = await _context.Ordereditems.FindAsync(orderedItemId);
            if (item != null)
            {
                item.Readyquantity += readyQuantity;
                _context.Ordereditems.Update(item);
                await _context.SaveChangesAsync();
            }
        }
        #endregion

        #region UpdateQuantityAsync
        public async Task UpdateQuantityAsync(int orderedItemId, int Quantity)
        {
            var item = await _context.Ordereditems.FindAsync(orderedItemId);
            if (item != null)
            {
                item.Readyquantity -= Quantity;
                _context.Ordereditems.Update(item);
                await _context.SaveChangesAsync();
            }
        }
        #endregion

        #region UpdateOrderStatusAsync
        public async Task UpdateOrderStatusAsync(int orderId, int status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
        }
        #endregion

        #region Add
        public async Task Add(Ordereditem orderItem)
        {
            var existingItem = await _context.Ordereditems.FirstOrDefaultAsync(
                o =>
                    o.Orderid == orderItem.Orderid
                    && o.Itemid == orderItem.Itemid
                    && o.Quantity == orderItem.Quantity
                    && o.Ordereditemid == orderItem.Ordereditemid
            );

            if (existingItem != null)
            {
                return;
            }

            await _context.Ordereditems.AddAsync(orderItem);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region GetByOrderIdAndItemIdAsync
        public async Task<Ordereditem?> GetByOrderIdAndItemIdAsync(
            int orderId,
            int itemId,
            List<OrderItemModifierRequestModel> modifiers
        )
        {
            var existingItem = await _context.Ordereditems.FirstOrDefaultAsync(
                oi => oi.Orderid == orderId && oi.Itemid == itemId
            );

            if (existingItem == null)
            {
                return null;
            }

            var existingModifiers = await _context.Ordereditemmodifers
                .Where(
                    mod => mod.Orderid == orderId && mod.Ordereditemid == existingItem.Ordereditemid
                )
                .ToListAsync();

            if (modifiers == null || !modifiers.Any())
            {
                return !existingModifiers.Any() ? existingItem : null;
            }

            bool isExactMatch =
                modifiers.All(
                    mod =>
                        existingModifiers.Any(
                            em => em.Itemmodifierid == mod.ModifierId && em.Quantity == mod.Quantity
                        )
                )
                && modifiers.Count == existingModifiers.Count;

            return isExactMatch ? existingItem : null;
        }
        #endregion

        #region UpdateAsync
        public async Task UpdateAsync(Ordereditem item)
        {
            _context.Ordereditems.Update(item);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region GetItemsByOrderIdAsync
        public async Task<IEnumerable<Ordereditem>> GetItemsByOrderIdAsync(int orderId)
        {
            return await _context.Ordereditems.Where(oi => oi.Orderid == orderId).ToListAsync();
        }
        #endregion

        #region DeleteAsync
        public async Task DeleteAsync(int orderItemId)
        {
            var orderItem = await _context.Ordereditems.FirstOrDefaultAsync(
                oi => oi.Ordereditemid == orderItemId
            );

            if (orderItem != null)
            {
                _context.Ordereditems.Remove(orderItem);

                await _context.SaveChangesAsync();

            }
            else
            {
                throw new Exception($"Order item with ID {orderItemId} not found.");
            }
        }
        #endregion

        #region GetByIdAsync
        public async Task<Ordereditem?> GetByIdAsync(int orderedItemId)
        {
            return await _context.Ordereditems.FirstOrDefaultAsync(
                oi => oi.Ordereditemid == orderedItemId
            );
        }
        #endregion

        #region IsItemAvailableToDeleteAsync
        public async Task<bool> IsItemAvailableToDeleteAsync(int itemId)
        {
            var item = await _context.Ordereditems
                .Where(i => i.Itemid == itemId)
                .FirstOrDefaultAsync();

            if (item == null)
                return false;

            return true;
        }
        #endregion

        #region GetTopSellingItemsAsync
        public async Task<List<ItemSale>> GetTopSellingItemsAsync(
            DateTime? startDate,
            DateTime? endDate,
            int count
        )
        {
            var query = _context.Ordereditems
                .Include(oi => oi.Order)
                .Include(oi => oi.Item)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(oi => oi.Order.Orderdate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(oi => oi.Order.Orderdate <= endDate.Value);
            }

            query = query.Where(oi => oi.Order.Status == 3);

            var result = await query
                .GroupBy(oi => new { oi.Item.Itemid, oi.Item.Itemname })
                .Select(
                    g =>
                        new ItemSale
                        {
                            ItemName = g.Key.Itemname,
                            ImageUrl = g.Select(i => i.Item.Itemimg).FirstOrDefault(),
                            OrderCount = g.Count()
                        }
                )
                .OrderByDescending(x => x.OrderCount)
                .Take(count)
                .ToListAsync();
            return result;
        }
        #endregion

        #region GetLeastSellingItemsAsync
        public async Task<List<ItemSale>> GetLeastSellingItemsAsync(
            DateTime? startDate,
            DateTime? endDate,
            int count
        )
        {
            var query = _context.Ordereditems
                .Include(oi => oi.Order)
                .Include(oi => oi.Item)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(oi => oi.Order.Orderdate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(oi => oi.Order.Orderdate <= endDate.Value);
            }

            query = query.Where(oi => oi.Order.Status == 3);

            var result = await query
                .GroupBy(oi => new { oi.Item.Itemid, oi.Item.Itemname })
                .Select(
                    g =>
                        new ItemSale
                        {
                            ItemName = g.Key.Itemname,
                            ImageUrl = g.Select(i => i.Item.Itemimg).FirstOrDefault(),
                            OrderCount = g.Count()
                        }
                )
                .OrderBy(x => x.OrderCount)
                .Take(count)
                .ToListAsync();
            return result;
        }
        #endregion
    }
}

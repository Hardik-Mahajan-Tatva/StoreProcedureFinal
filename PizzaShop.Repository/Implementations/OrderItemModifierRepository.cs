using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations
{
    public class OrderItemModifierRepository : IOrderItemModifierRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor

        public OrderItemModifierRepository(PizzaShopContext context)
        {
            _context = context;
        }

        #endregion

        #region AddOrderedItemModifierMappingIfNotExistsAsync
        public async Task AddOrderedItemModifierMappingIfNotExistsAsync(
            Ordereditemmodifer orderItemModifier
        )
        {
            var existingModifier = await _context.Ordereditemmodifers.FirstOrDefaultAsync(
                oim =>
                    oim.Ordereditemid == orderItemModifier.Ordereditemid
                    && oim.Itemmodifierid == orderItemModifier.Itemmodifierid
            );

            if (existingModifier != null)
            {
                return;
            }

            await _context.Ordereditemmodifers.AddAsync(orderItemModifier);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region GetModifierMappingsByOrderedItemIdAsync
        public async Task<List<Ordereditemmodifer>> GetModifierMappingsByOrderedItemIdAsync(
            int orderedItemId
        )
        {
            return await _context.Ordereditemmodifers
                .Where(mod => mod.Ordereditemid == orderedItemId)
                .ToListAsync();
        }
        #endregion

        #region DeleteAllModifierMappingsForOrderedItemIdAsync
        public async Task DeleteAllModifierMappingsForOrderedItemIdAsync(int orderedItemId)
        {
            var modifiers = await _context.Ordereditemmodifers
                .Where(mod => mod.Ordereditemid == orderedItemId)
                .ToListAsync();

            if (modifiers.Any())
            {
                _context.Ordereditemmodifers.RemoveRange(modifiers);
                await _context.SaveChangesAsync();
            }
        }
        #endregion
    }
}

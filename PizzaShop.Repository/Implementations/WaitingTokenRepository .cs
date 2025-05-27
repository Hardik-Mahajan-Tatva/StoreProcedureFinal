using Microsoft.EntityFrameworkCore;
using Npgsql;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations
{
    public class WaitingTokenRepository : IWaitingTokenRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public WaitingTokenRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region GetAllWaitingTokenAsync
        public async Task<List<Waitingtoken>> GetAllWaitingTokenAsync()
        {
            return await _context.Waitingtokens
                .Include(t => t.Customer)
                .Where(wt => wt.Isassigned == false)
                .ToListAsync();
        }
        #endregion

        #region GetByIdAsync
        public async Task<Waitingtoken?> GetByIdAsync(int id)
        {
            return await _context.Waitingtokens
                .Include(t => t.Customer)
                .OrderByDescending(t => t.Createdat)
                .FirstOrDefaultAsync(t => t.Customerid == id);
        }
        #endregion

        #region AddNewWaitingTokenAsync
        public async Task<bool> AddNewWaitingTokenAsync(Waitingtoken token)
        {
            await _context.Waitingtokens.AddAsync(token);
            var changes = await _context.SaveChangesAsync();
            return changes > 0;
        }
        #endregion

        #region UpdateAsync
        public async Task UpdateAsync(Waitingtoken token)
        {
            _context.Waitingtokens.Update(token);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region DeleteAsync
        public async Task DeleteAsync(int id)
        {
            var token = await _context.Waitingtokens.FindAsync(id);
            if (token != null)
            {
                _context.Waitingtokens.Remove(token);
                await _context.SaveChangesAsync();
            }
        }
        #endregion

        #region GetWaitingTokenByCustomerIdAsync
        public async Task<Waitingtoken?> GetWaitingTokenByCustomerIdAsync(int customerId)
        {
            return await _context.Waitingtokens
                .Where(w => w.Customerid == customerId)
                .OrderByDescending(w => w.Waitingtokenid)
                .FirstOrDefaultAsync();
        }
        #endregion

        #region UpdateWaitingTokenAsync
        public async Task<bool> UpdateWaitingTokenAsync(Waitingtoken token)
        {
            _context.Waitingtokens.Update(token);
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion

        #region DeleteWaitingTokenAsync
        public async Task<bool> DeleteWaitingTokenAsync(int waitingTokenId)
        {
            var token = await _context.Waitingtokens.FindAsync(waitingTokenId);
            if (token == null)
                return false;

            _context.Waitingtokens.Remove(token);
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion
        #region DeleteWaitingTokenAsyncSP
        public async Task<bool> DeleteWaitingTokenAsyncSP(int waitingTokenId)
        {
            try
            {
                var param = new Npgsql.NpgsqlParameter("p_waiting_token_id", waitingTokenId);

                await _context.Database.ExecuteSqlRawAsync("CALL delete_waiting_token({0});", param);

                return true;
            }
            catch (Npgsql.NpgsqlException npgsqlEx)
            {
                Console.WriteLine("PostgreSQL error:");
                Console.WriteLine($"Message: {npgsqlEx.Message}");
                Console.WriteLine($"Code: {npgsqlEx.SqlState}");
                return false;
            }
            catch (DbUpdateException dbUpdateEx)
            {
                Console.WriteLine("Database update error:");
                Console.WriteLine($"Message: {dbUpdateEx.Message}");
                if (dbUpdateEx.InnerException != null)
                    Console.WriteLine($"Inner: {dbUpdateEx.InnerException.Message}");
                return false;
            }
            catch (InvalidOperationException invalidOpEx)
            {
                Console.WriteLine("Invalid operation:");
                Console.WriteLine($"Message: {invalidOpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // General fallback
                Console.WriteLine("Unexpected error:");
                Console.WriteLine($"Message: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner: {ex.InnerException.Message}");
                return false;
            }
        }
        #endregion

        #region GetTokensBySectionsAsync
        public async Task<List<Waitingtoken>> GetTokensBySectionsAsync(List<int> sectionIds)
        {
            return await _context.Waitingtokens
                .Where(w => sectionIds.Contains(w.Sectionid) && w.Isassigned == false)
                .Include(w => w.Customer)
                .ToListAsync();
        }
        #endregion

        #region GetAvgWaitingTimeByTimeRangeAsync
        public async Task<decimal> GetAvgWaitingTimeByTimeRangeAsync(DateTime? startDate, DateTime? endDate)
        {
            if (endDate.HasValue)
            {
                endDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
            }

            var orderItems = await _context.Ordereditems
                .Where(oi => oi.Servedat != null)
                .ToListAsync();

            if (startDate.HasValue)
                orderItems = orderItems.Where(oi => oi.Createdat >= startDate.Value).ToList();

            if (endDate.HasValue)
                orderItems = orderItems.Where(oi => oi.Createdat <= endDate.Value).ToList();

            if (!orderItems.Any())
                return 0;

            var averageTicks = orderItems
                .Average(oi => oi.Servedat!.Value.Ticks - oi.Createdat.Ticks);

            var averageWaitingTimeInSeconds = averageTicks / TimeSpan.TicksPerSecond;

            var averageWaitingTimeInMinutes = averageWaitingTimeInSeconds / 60;

            return (decimal)averageWaitingTimeInMinutes;
        }
        #endregion

        #region GetWaitingListCountOfTodayAsync
        public async Task<int> GetWaitingListCountOfTodayAsync()
        {
            var todayUtc = DateTime.UtcNow.Date;
            var tomorrowUtc = todayUtc.AddDays(1);

            var waitingListCount = await _context.Waitingtokens
                .Where(
                    wt =>
                        wt.Createdat >= todayUtc
                        && wt.Createdat < tomorrowUtc
                        && wt.Isassigned == false
                )
                .CountAsync();

            return waitingListCount;
        }
        #endregion

        #region GetWaitingListCountBySectionIdAsync
        public async Task<int> GetWaitingListCountBySectionIdAsync(int sectionId)
        {
            return await _context.Waitingtokens
                .Where(w => w.Sectionid == sectionId && w.Isassigned == false)
                .CountAsync();
        }
        #endregion

        #region IsMobileNumberExistsAsync
        public async Task<bool> IsMobileNumberExistsAsync(string mobileNumber)
        {
            return await _context.Customers.AnyAsync(w => w.Phoneno == mobileNumber);
        }
        #endregion

        #region IsEmailAlreadyInWaitingListAsync
        public async Task<bool> IsEmailAlreadyInWaitingListAsync(string email)
        {
            return await _context.Waitingtokens
                .Include(c => c.Customer)
                .AnyAsync(w => w.Customer.Email == email && w.Isassigned == false);
        }
        #endregion
    }
}

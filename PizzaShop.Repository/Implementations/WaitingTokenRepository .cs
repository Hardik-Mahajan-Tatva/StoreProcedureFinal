using System.Data;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

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
            var param = new Npgsql.NpgsqlParameter("p_waiting_token_id", waitingTokenId);

            await _context.Database.ExecuteSqlRawAsync("CALL delete_waiting_token({0});", param);

            return true;
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

        public async Task<WaitingTokenViewModel?> GetCustomerWaitingDataByEmailAsyncSP(string email)
        {

            var result = await _context
                .Set<WaitingTokenViewModel>()
                .FromSqlRaw("SELECT * FROM get_customer_waiting_data({0})", email)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return result;
        }


        public async Task<bool> AddNewWaitingTokenAsyncSP(WaitingTokenViewModel model)
        {
            try
            {
                var parameters = new[]
                {
            new NpgsqlParameter("p_name", model.Name ?? (object)DBNull.Value),
            new NpgsqlParameter("p_phone", model.MobileNumber ?? (object)DBNull.Value),
            new NpgsqlParameter("p_no_of_persons", model.NoOfPersons),
            new NpgsqlParameter("p_email", model.Email ?? (object)DBNull.Value),
            new NpgsqlParameter("p_section_id", model.SectionId ?? 0),
            new NpgsqlParameter("p_is_assigned", model.IsAssigned)
        };

                await _context.Database.ExecuteSqlRawAsync(
                    "CALL add_waiting_token(@p_name, @p_email, @p_phone, @p_no_of_persons, @p_section_id, @p_is_assigned);",
                    parameters
                );

                return true;
            }
            catch (PostgresException ex) when (ex.Message.Contains("Customer Already waiting"))
            {
                throw new InvalidOperationException("Customer Already waiting");
            }

        }
        public async Task<TokenWithSectionsResult?> GetWaitingTokenWithSectionsSPAsync(int customerId)
        {

            var conn = _context.Database.GetDbConnection();

            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT get_waiting_token_with_sections(@p0)";
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.Add(new Npgsql.NpgsqlParameter("p0", customerId));

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync() && !reader.IsDBNull(0))
            {
                var json = reader.GetString(0);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return null;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<TokenWithSectionsResult>(json, options);
                return result;

            }
            return null;
        }

        #region UpdateCustomerAndWaitingTokenUsingStoredProcedureAsync
        public async Task<bool> UpdateCustomerAndWaitingTokenUsingSPAsync(WaitingTokenViewModel model)
        {
            var customerId = model.CustomerId ?? 0;
            var connection = _context.Database.GetDbConnection();
            await using (connection)
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = "CALL update_waiting_token_and_customer(@customerId, @name, @email, @phone, @noOfPeople, @sectionId)";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(new Npgsql.NpgsqlParameter("@customerId", customerId));
                command.Parameters.Add(new Npgsql.NpgsqlParameter("@name", model.Name));
                command.Parameters.Add(new Npgsql.NpgsqlParameter("@email", model.Email ?? (object)DBNull.Value));
                command.Parameters.Add(new Npgsql.NpgsqlParameter("@phone", model.MobileNumber));
                command.Parameters.Add(new Npgsql.NpgsqlParameter("@noOfPeople", model.NoOfPersons));
                command.Parameters.Add(new Npgsql.NpgsqlParameter("@sectionId", model.SectionId ?? 0));

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return true; // assume procedure does the right job
            }
        }
        #endregion







    }
}

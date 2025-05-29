using Microsoft.EntityFrameworkCore;
using Npgsql;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Repository.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public CustomerRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region GetPaginatedCustomersAsync
        public async Task<PaginatedList<Customer>> GetPaginatedCustomersAsync(
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
            var startUtc = startDate?.ToUniversalTime();
            var endUtc = endDate?.ToUniversalTime();
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string trimmedSearch = search.Trim().ToLower();
                query = query.Where(
                    o =>
                        (o.Customername != null && o.Customername.ToLower().Contains(trimmedSearch))
                        || (o.Email != null && o.Email.ToLower().Contains(trimmedSearch))
                        || (o.Phoneno != null && o.Phoneno.ToLower().Contains(trimmedSearch))
                );
            }

            if (startUtc.HasValue)
            {
                query = query.Where(o => o.Createdat.HasValue && o.Createdat.Value >= startUtc.Value);
            }


            if (endUtc.HasValue)
            {
                DateTime endOfDay = endUtc.Value.Date.AddDays(1).AddTicks(-1); // End of day as max ticks on that date
                query = query.Where(o => o.Createdat.HasValue && o.Createdat.Value <= endOfDay);
            }


            query = sortColumn switch
            {
                "CreateDate"
                  => sortDirection == "asc"
                      ? query.OrderBy(o => o.Createdat)
                      : query.OrderByDescending(o => o.Createdat),
                "TotalOrder"
                  => sortDirection == "asc"
                      ? query.OrderBy(o => o.Totalorder).ThenBy(o => o.Customerid)
                      : query
                        .OrderByDescending(o => o.Totalorder)
                        .ThenByDescending(o => o.Customerid),
                "CustomerName"
                  => sortDirection == "asc"
                      ? query.OrderBy(o => o.Customername)
                      : query.OrderByDescending(o => o.Customername),
                _ => query.OrderByDescending(o => o.Createdat)
            };

            return await PaginatedList<Customer>.CreateAsync(query, page, pageSize);
        }
        #endregion

        #region GetAllCustomersAsync
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.AsNoTracking().ToListAsync();
        }
        #endregion
        public IQueryable<Customer> GetAllCustomersQueryable()
        {
            return _context.Customers.AsQueryable();
        }
        #region GetCustomerWithOrdersAsync
        public async Task<Customer?> GetCustomerWithOrdersAsync(int customerId)
        {
            return await _context.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.Ordereditems)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Customerid == customerId);
        }
        #endregion

        #region GetCustomerWithLatestOrderAsync
        public async Task<Customer?> GetCustomerWithLatestOrderAsync(int customerId)
        {
            return await _context.Customers
                .Include(c => c.Orders.OrderByDescending(o => o.Orderdate))
                .FirstOrDefaultAsync(c => c.Customerid == customerId);
        }
        #endregion

        #region GetCustomerByCustomerIdAsync
        public async Task<Customer?> GetCustomerByCustomerIdAsync(int customerId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Customerid == customerId);
        }
        #endregion

        #region DoesCustomerExistAsync
        public async Task<bool> DoesCustomerExistAsync(string customerEmail, int excludeCustomerId)
        {
            return await _context.Customers.AnyAsync(
                c =>
                    c.Email!.ToLower() == customerEmail.ToLower()
                    && c.Customerid != excludeCustomerId
            );
        }
        #endregion

        #region GetOrdersByCustomerIdAsync
        public async Task<List<Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            return await _context.Orders.Where(o => o.Customerid == customerId).ToListAsync();
        }
        #endregion

        #region GetCustomerByNamePhoneAndEmailAsync
        public async Task<Customer?> GetCustomerByNamePhoneAndEmailAsync(string customerEmail)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == customerEmail);
        }
        #endregion

        #region UpdateCustomerAsync
        public async Task UpdateCustomerAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region AddCustomerAsync
        public async Task<int> AddCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer.Customerid;
        }
        #endregion

        #region UpdateCustomerDataAsync
        public async Task<bool> UpdateCustomerDataAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion

        #region GetNewCustomerCountByTimeRange
        public async Task<int> GetNewCustomerCountByTimeRange(DateTime? startDate, DateTime? endDate)
        {
            var startUtc = startDate?.ToUniversalTime();
            var endUtc = endDate?.ToUniversalTime();

            var newCustomerCount = await _context.Customers
                .Where(c =>
                    (!startUtc.HasValue || (c.Createdat.HasValue && c.Createdat.Value >= startUtc.Value)) &&
                    (!endUtc.HasValue || (c.Createdat.HasValue && c.Createdat.Value < endUtc.Value))
                )
                .CountAsync();

            return newCustomerCount;
        }

        #endregion

        #region GetCustomerByEmailAsync
        public async Task<Customer?> GetCustomerByEmailAsync(string customerEmail)
        {
            return await _context.Customers.FirstOrDefaultAsync(
                c =>
                    c.Email != null
                    && customerEmail != null
                    && c.Email.ToLower() == customerEmail.ToLower()
            );
        }
        #endregion

        public async Task<(bool Success, string Message, int OrderId)> AssignCustomerToOrderSPAsync(
     string customerName,
     string email,
     string mobileNumber,
     int noOfPersons,
     int[] tableIds)
        {
            await using var conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "assign_customer_to_order";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            // Input parameters
            var p_customer_name = new Npgsql.NpgsqlParameter("p_customer_name", customerName);
            var p_email = new Npgsql.NpgsqlParameter("p_email", email);
            var p_mobile_number = new Npgsql.NpgsqlParameter("p_mobile_number", mobileNumber);
            var p_no_of_persons = new Npgsql.NpgsqlParameter("p_no_of_persons", noOfPersons);
            var p_table_ids = new Npgsql.NpgsqlParameter("p_table_ids", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = tableIds
            };

            // Output parameters
            var p_success = new Npgsql.NpgsqlParameter("p_success", System.Data.DbType.Boolean)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            var p_message = new Npgsql.NpgsqlParameter("p_message", System.Data.DbType.String, 4000)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            var p_order_id = new Npgsql.NpgsqlParameter("p_order_id", System.Data.DbType.Int32)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            cmd.Parameters.Add(p_customer_name);
            cmd.Parameters.Add(p_email);
            cmd.Parameters.Add(p_mobile_number);
            cmd.Parameters.Add(p_no_of_persons);
            cmd.Parameters.Add(p_table_ids);
            cmd.Parameters.Add(p_success);
            cmd.Parameters.Add(p_message);
            cmd.Parameters.Add(p_order_id);

            await cmd.ExecuteNonQueryAsync();

            bool success = (bool)p_success.Value!;
            string message = p_message.Value!.ToString() ?? "";
            int orderId = p_order_id.Value != DBNull.Value ? (int)p_order_id.Value! : 0;

            return (success, message, orderId);
        }

        #region GetCustomerWithLatestOrderSPAsync
        public async Task<CustomerUpdateRaw?> GetCustomerWithLatestOrderSPAsync(int customerId)
        {

            var param = new NpgsqlParameter("customer_id", customerId);

            CustomerUpdateRaw? customer = await _context.CustomerUpdateRaw
                .FromSqlRaw("SELECT * FROM get_customer_with_latest_order(@customer_id)", param)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return customer;

        }
        #endregion


    }
}

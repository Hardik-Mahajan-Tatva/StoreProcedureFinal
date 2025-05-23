using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Repository.Implementations
{
    public class TaxesFeesRepository : ITaxesFeesRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public TaxesFeesRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region GetAll
        public List<Taxis> GetAll()
        {
            return _context.Taxes.Where(t => !t.Isdeleted ?? false).ToList();
        }
        #endregion

        #region AddTaxAsync
        public async Task<bool> AddTaxAsync(Taxis tax)
        {
            _context.Taxes.Add(tax);
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion

        #region IsTaxNameExistsAsync
        public async Task<bool> IsTaxNameExistsAsync(string taxName, int? taxId = null)
        {
            return await _context.Taxes.AnyAsync(
                t => t.Taxname == taxName && (!taxId.HasValue || t.Taxid != taxId)
            );
        }
        #endregion

        #region GetById
        public Taxis? GetById(int taxId)
        {
            return _context.Taxes.FirstOrDefault(t => t.Taxid == taxId);
        }
        #endregion

        #region Update
        public bool Update(Taxis tax)
        {
            _context.Taxes.Update(tax);
            return _context.SaveChanges() > 0;
        }
        #endregion

        #region GetTaxByIdAsync
        public async Task<Taxis?> GetTaxByIdAsync(int taxId)
        {
            return await _context.Taxes.FirstOrDefaultAsync(t => t.Taxid == taxId);
        }
        #endregion

        #region DeleteTax
        public async Task<bool> DeleteTax(int taxId)
        {
            var tax = await _context.Taxes.FirstOrDefaultAsync(t => t.Taxid == taxId);
            if (tax != null)
            {
                tax.Isdeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        #endregion

        #region GetTaxesAndFeesAsync
        public async Task<PaginatedList<Taxis>> GetTaxesAndFeesAsync(
            string search,
            int page,
            int pageSize,
            string sortColumn,
            string sortDirection
        )
        {
            var query = _context.Taxes.Where(t => !(t.Isdeleted ?? false)).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string trimmedSearch = search.Trim().ToLower();
                query = query.Where(
                    t =>
                        t.Taxname.ToLower().Contains(trimmedSearch)
                        || t.Taxtype.ToLower().Contains(trimmedSearch)
                );
            }

            query = sortColumn switch
            {
                "TaxName"
                  => sortDirection == "asc"
                      ? query.OrderBy(t => t.Taxname)
                      : query.OrderByDescending(t => t.Taxname),
                "Value"
                  => sortDirection == "asc"
                      ? query.OrderBy(t => t.Taxvalue)
                      : query.OrderByDescending(t => t.Taxvalue),
                _
                  => sortDirection == "asc"
                      ? query.OrderBy(t => t.Taxid)
                      : query.OrderByDescending(t => t.Taxid),
            };

            return await PaginatedList<Taxis>.CreateAsync(query, page, pageSize);
        }
        #endregion

        #region UpdateTaxFieldAsync
        public async Task UpdateTaxFieldAsync(int taxId, bool isChecked, string field)
        {
            var tax =
                await _context.Taxes.FirstOrDefaultAsync(t => t.Taxid == taxId)
                ?? throw new Exception("Tax not found");
            switch (field)
            {
                case "IsEnabled":
                    tax.Isenabled = isChecked;
                    break;
                case "IsDefault":
                    tax.Isdefault = isChecked;
                    break;
                case "IsInclusive":
                    tax.Isinclusive = isChecked;
                    break;
                default:
                    throw new Exception("Invalid field type.");
            }

            _context.Taxes.Update(tax);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region GetEnabledTaxesAsync
        public async Task<List<Taxis>> GetEnabledTaxesAsync()
        {
            return await _context.Taxes
                .Where(t => t.Isenabled == true && !(t.Isdeleted ?? false))
                .OrderByDescending(t => t.Taxid)
                .ToListAsync();
        }
        #endregion

        #region GetDefaultTaxesForItems
        public List<ItemSpecificTaxViewModel> GetDefaultTaxesForItems(List<int> itemIds)
        {
            return _context.Items
                .Where(i => itemIds.Contains(i.Itemid) && i.Isdefaulttax == true)
                .Select(
                    i =>
                        new ItemSpecificTaxViewModel
                        {
                            ItemId = i.Itemid,
                            Percentage = i.Taxpercentage,
                            TaxName = "Other"
                        }
                )
                .ToList();
        }
        #endregion
    }
}

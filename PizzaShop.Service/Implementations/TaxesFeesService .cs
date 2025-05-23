using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;
namespace PizzaShop.Service.Implementations
{

    public class TaxesFeesService : ITaxesFeesService
    {
        private readonly ITaxesFeesRepository _taxesFeesRepository;

        public TaxesFeesService(ITaxesFeesRepository taxesFeesRepository)
        {
            _taxesFeesRepository = taxesFeesRepository;
        }

        public List<TaxesFeesViewModel> GetAllTaxes()
        {
            var taxes = _taxesFeesRepository.GetAll();
            return taxes.Select(t => new TaxesFeesViewModel
            {
                TaxId = t.Taxid,
                TaxName = t.Taxname,
                TaxType = t.Taxtype,
                IsEnabled = t.Isenabled ?? false,
                IsDefault = t.Isdefault ?? false,
                IsInclusive = t.Isinclusive ?? false,
                TaxValue = (decimal?)t.Taxvalue
            }).ToList();
        }


        public async Task<bool> AddTaxAsync(TaxesFeesViewModel model)
        {
            var tax = new Taxis
            {
                Taxname = model.TaxName ?? string.Empty,
                Taxtype = model.TaxType ?? string.Empty,
                Taxvalue = (int)(model.TaxValue ?? 0),
                Isenabled = model.IsEnabled,
                Isdefault = model.IsDefault
            };

            return await _taxesFeesRepository.AddTaxAsync(tax);
        }
        public async Task<bool> IsDuplicateTaxNameAsync(string taxName, int? taxId = null)
        {
            return await _taxesFeesRepository.IsTaxNameExistsAsync(taxName, taxId);
        }


        public TaxesFeesViewModel GetTaxById(int id)
        {
            var tax = _taxesFeesRepository.GetById(id);
            if (tax == null) return new TaxesFeesViewModel();

            return new TaxesFeesViewModel
            {
                TaxId = tax.Taxid,
                TaxName = tax.Taxname,
                TaxType = tax.Taxtype,
                TaxValue = (decimal?)tax.Taxvalue,
                IsEnabled = tax.Isenabled ?? false,
                IsDefault = tax.Isdefault ?? false
            };
        }

        public bool UpdateTax(TaxesFeesViewModel model)
        {
            var tax = _taxesFeesRepository.GetById(model.TaxId);
            if (tax == null) return false;

            tax.Taxname = model.TaxName ?? string.Empty;
            tax.Taxtype = model.TaxType ?? string.Empty;
            tax.Taxvalue = (int)(model.TaxValue ?? 0);
            tax.Isenabled = model.IsEnabled;
            tax.Isdefault = model.IsDefault;

            return _taxesFeesRepository.Update(tax);
        }

        public async Task<TaxesFeesViewModel> GetTaxByIdAsync(int id)
        {
            var tax = await _taxesFeesRepository.GetTaxByIdAsync(id);
            if (tax == null) return new TaxesFeesViewModel();

            return new TaxesFeesViewModel
            {
                TaxId = tax.Taxid,
                TaxName = tax.Taxname,
                TaxType = tax.Taxtype,
                TaxValue = (decimal?)tax.Taxvalue,
                IsEnabled = tax.Isenabled ?? false,
                IsDefault = tax.Isdefault ?? false
            };
        }


        public async Task<bool> DeleteTax(int taxId)
        {
            if (taxId == 0)
            {
                return false;
            }
            return await _taxesFeesRepository.DeleteTax(taxId);
        }
        public async Task<PaginatedList<TaxesFeesViewModel>> GetTaxesAndFeesAsync(
     string search,
     int page,
     int pageSize,
     string sortColumn,
     string sortDirection)
        {
            var taxes = await _taxesFeesRepository.GetTaxesAndFeesAsync(search, page, pageSize, sortColumn, sortDirection);

            var taxesViewModel = taxes.Select(t => new TaxesFeesViewModel
            {
                TaxId = t.Taxid,
                TaxName = t.Taxname,
                TaxType = t.Taxtype,
                IsEnabled = t.Isenabled ?? false,
                IsDefault = t.Isdefault ?? false,
                IsInclusive = t.Isinclusive ?? false,
                TaxValue = (decimal?)t.Taxvalue
            }).ToList();

            return new PaginatedList<TaxesFeesViewModel>(taxesViewModel, taxes.TotalItems, page, pageSize);
        }
        public async Task ToggleTaxFieldAsync(int taxId, bool isChecked, string field)
        {
            await _taxesFeesRepository.UpdateTaxFieldAsync(taxId, isChecked, field);
        }
        public async Task<List<TaxViewModel>> GetEnabledTaxesAsync()
        {
            var taxes = await _taxesFeesRepository.GetEnabledTaxesAsync();

            return taxes.Select(t => new TaxViewModel
            {
                TaxId = t.Taxid,
                TaxName = t.Taxname,
                Amount = (decimal)t.Taxvalue,
                TaxType = t.Taxtype
            }).ToList();
        }
        public List<ItemSpecificTaxViewModel> GetDefaultItemTaxes(List<int> itemIds)
        {
            if (itemIds == null || !itemIds.Any())
                return new List<ItemSpecificTaxViewModel>();

            return _taxesFeesRepository.GetDefaultTaxesForItems(itemIds);
        }

    }
}

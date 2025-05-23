namespace PizzaShop.Repository.ViewModels
{
    public class TaxMappingViewModel
    {
        public int TaxId { get; set; }

        public string? TaxName { get; set; }

        public decimal TaxValue { get; set; }

        public string? TaxType { get; set; }

        public decimal Amount { get; set; }

        public bool IsInclusive { get; set; }
    }
}
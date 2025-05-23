namespace PizzaShop.Repository.ViewModels
{
    public class ItemSpecificTaxViewModel
    {
        public int ItemId { get; set; }

        public string? TaxName { get; set; }

        public decimal? Percentage { get; set; }
    }
}
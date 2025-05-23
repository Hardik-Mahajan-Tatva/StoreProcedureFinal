namespace PizzaShop.Repository.ViewModels
{
    public class ItemWithModifiersViewModel
    {
        public int ItemId { get; set; }

        public string? ItemName { get; set; }

        public decimal BasePrice { get; set; }

        public List<ModifierGroupViewModel>? ModifierGroups { get; set; }
    }

    public class ModifierGroupViewModel
    {
        public int GroupId { get; set; }

        public string? GroupName { get; set; }

        public int? MinSelection { get; set; }

        public int? MaxSelection { get; set; }

        public List<ModifierViewModel>? Modifiers { get; set; }
    }
}
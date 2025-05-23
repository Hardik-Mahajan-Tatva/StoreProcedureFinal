namespace PizzaShop.Repository.ViewModels
{
    public class ItemModifierGroupMapViewModel
    {
        public int ItemModifierGroupMapId { get; set; }

        public int ItemId { get; set; }

        public int ModifierGroupId { get; set; }

        public string? ModifierGroupName { get; set; }

        public int? MinValue { get; set; }

        public int? MaxValue { get; set; }

        public List<ModifierItemViewModel> ModifierItems { get; set; } = new List<ModifierItemViewModel>();

        public ModifierType? ModifierType { get; set; }
    }
    public class ModifierItemViewModel
    {
        public int OrderedItemId { get; set; }

        public int? ModifierItemId { get; set; }

        public string? ModifierItemName { get; set; }

        public decimal? Price { get; set; }

        public ModifierType? ModifierType { get; set; }
    }
}

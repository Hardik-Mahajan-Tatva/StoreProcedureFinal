namespace PizzaShop.Repository.ViewModels
{
    public class MenuViewModel
    {
        public IEnumerable<CategoryViewModel>? Categories { get; set; }

        public PaginatedList<ItemViewModel>? ItemsPaginated { get; set; }

        public PaginatedList<ModifierViewModel>? ModifiersPaginated { get; set; }

        public IEnumerable<ModifiergroupViewModel>? Modifiergroups { get; set; }
    }
    public enum ItemType
    {
        Veg,
        NonVeg,
        Vegan
    }
}

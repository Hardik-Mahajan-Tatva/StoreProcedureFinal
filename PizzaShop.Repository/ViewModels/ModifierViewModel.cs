namespace PizzaShop.Repository.ViewModels
{
    public class ModifierViewModel
    {
        public int ModifierId { get; set; }

        public int Modifieditemid { get; set; }

        public int Ordereditemid { get; set; }

        public int Orderid { get; set; }

        public int Itemmodifierid { get; set; }

        public string Modifiername { get; set; } = null!;

        public int Modifiergroupid { get; set; }

        public decimal Rate { get; set; }

        public int? Quantity { get; set; }

        public int Unitid { get; set; }

        public string? Description { get; set; }

        public bool? Isdeleted { get; set; }

        public ModifierType? ModifierType { get; set; }
    }
    public enum ModifierType
    {
        Veg = 0,
        NonVeg = 1,
        Vegan = 2
    }
}
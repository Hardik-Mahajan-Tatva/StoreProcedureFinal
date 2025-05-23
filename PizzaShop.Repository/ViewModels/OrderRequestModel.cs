namespace PizzaShop.Repository.ViewModels
{
    public class OrderRequestModel
    {
        public int OrderId { get; set; }

        public List<OrderItemRequestModel>? Items { get; set; }

        public List<OrderTaxModel>? Taxes { get; set; }

        public float SubAmount { get; set; }

        public float Discount { get; set; }

        public float TotalTax { get; set; }

        public float TotalAmount { get; set; }

        public string? PaymentMethod { get; set; }
    }

    public class OrderItemRequestModel
    {
        public int ItemId { get; set; }

        public int Quantity { get; set; }

        public List<OrderItemModifierRequestModel>? Modifiers { get; set; }
    }

    public class OrderItemModifierRequestModel
    {
        public int ModifierId { get; set; }

        public int Quantity { get; set; }
    }
    public class OrderTaxModel
    {
        public int TaxId { get; set; }

        public float TaxValue { get; set; }
    }
}
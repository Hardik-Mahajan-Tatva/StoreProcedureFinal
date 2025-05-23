namespace PizzaShop.Repository.Models
{
    public class KOTFlatData
    {
        public int OrderId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? OrderwiseComment { get; set; }

        public string? TableName { get; set; }
        public string? SectionName { get; set; }

        public int OrderItemId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int ReadyQuantity { get; set; }
        public string? ItemwiseComment { get; set; }
        public decimal ItemRate { get; set; }

        public string? ModifierName { get; set; }
        public decimal? ModifierRate { get; set; }
    }

}
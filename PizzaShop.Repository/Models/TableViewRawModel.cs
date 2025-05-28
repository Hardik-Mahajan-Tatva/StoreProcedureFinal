namespace PizzaShop.Repository.Models
{
    public class TableViewRawModel
    {
        public int TableId { get; set; }
        public string? TableName { get; set; }
        public int TableStatus { get; set; }
        public int Capacity { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int SectionId { get; set; }
        public string? SectionName { get; set; }
        public int? OrderId { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? OrderStatus { get; set; }
        public DateTime? OrderModifiedAt { get; set; }
    }

}
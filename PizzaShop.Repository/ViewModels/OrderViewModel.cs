namespace PizzaShop.Repository.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }

        public int? CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public string? CustomerName { get; set; }

        public string? OrderType { get; set; }

        public OrderStatus Status { get; set; }

        public string? PaymentMode { get; set; }

        public decimal? Rating { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal? ItemsCount { get; set; }

        public int? NoOfPersons { get; set; } = 1;
    }

    public enum OrderStatus
    {
        Pending = 0,
        InProgress = 1,
        Served = 2,
        Completed = 3,
        Cancelled = 4,
        OnHold = 5,
        Failed = 6
    }
}




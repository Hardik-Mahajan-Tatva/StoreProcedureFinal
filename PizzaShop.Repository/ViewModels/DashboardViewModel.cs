namespace PizzaShop.Repository.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalSales { get; set; }

        public int TotalOrders { get; set; }

        public decimal AvgOrderValue { get; set; }

        public double AvgWaitingTime { get; set; }

        public List<ChartDataPoint>? RevenueChartData { get; set; }

        public List<ChartDataPoint>? CustomerGrowthData { get; set; }

        public List<ItemSale>? TopSellingItems { get; set; }

        public List<ItemSale>? LeastSellingItems { get; set; }

        public int WaitingListCount { get; set; }

        public int NewCustomerCount { get; set; }

        public string? UserRole { get; set; }
    }

    public class ChartDataPoint
    {
        public string? Label { get; set; }

        public decimal Value { get; set; }
    }

    public class ItemSale
    {
        public string? ItemName { get; set; }

        public int OrderCount { get; set; }

        public string? ImageUrl { get; set; }
    }
}
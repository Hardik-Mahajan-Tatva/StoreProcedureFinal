using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IWaitingTokenRepository _waitingTokenRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderedItemRepository _orderedItemRepository;
        public DashboardService(
            IOrderedItemRepository orderedItemRepository,
            ICustomerRepository customerRepository,
            IOrderRepository orderRepository,
            IWaitingTokenRepository waitingTokenRepository)
        {
            _orderedItemRepository = orderedItemRepository;
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _waitingTokenRepository = waitingTokenRepository;
        }
        public async Task<DashboardViewModel> GetDashboardDataAsync(DateTime? startDate, DateTime? endDate)
        {
            var totalSales = await _orderRepository.GetTotalSalesByTimeRangeAsync(startDate, endDate);
            var orderCount = await _orderRepository.GetTotalOrdersByTimeRangeAsync(startDate, endDate);
            var avgOrderValue = await _orderRepository.GetAvgOrderValueByTimeRangeAsync(startDate, endDate);
            var waitingTokenCount = await _waitingTokenRepository.GetAvgWaitingTimeByTimeRangeAsync(startDate, endDate);
            var waitingListCount = await _waitingTokenRepository.GetWaitingListCountOfTodayAsync();
            var newCustomerCount = await _customerRepository.GetNewCustomerCountByTimeRange(startDate, endDate);
            var topSelling = await _orderedItemRepository.GetTopSellingItemsAsync(startDate, endDate, 5);
            var leastSelling = await _orderedItemRepository.GetLeastSellingItemsAsync(startDate, endDate, 5);

            return new DashboardViewModel
            {
                TotalSales = totalSales,
                TotalOrders = orderCount,
                AvgOrderValue = avgOrderValue,
                AvgWaitingTime = (double)waitingTokenCount,
                WaitingListCount = waitingListCount,
                NewCustomerCount = newCustomerCount,
                TopSellingItems = topSelling,
                LeastSellingItems = leastSelling
            };
        }
        public async Task<List<ChartDataPoint>> GetRevenueChartDataAsync(DateTime? startDate, DateTime? endDate)
        {
            var result = new List<ChartDataPoint>();

            DateTime start = startDate ?? DateTime.Today;
            DateTime end = endDate ?? DateTime.Today;

            if (start.Date == end.Date && start.Date == DateTime.Today)
            {
                int currentHour = DateTime.Now.Hour;

                for (int hour = 0; hour <= currentHour; hour++)
                {
                    var from = start.AddHours(hour);
                    var to = from.AddHours(1);

                    decimal hourlySales = await _orderRepository.GetTotalSalesByTimeRangeAsync(from, to);

                    result.Add(new ChartDataPoint
                    {
                        Label = from.ToString("HH:mm"),
                        Value = hourlySales
                    });
                }
            }
            else
            {
                for (DateTime date = start.Date; date <= end.Date; date = date.AddDays(1))
                {
                    var dailySales = await _orderRepository.GetTotalSalesBySpecificDayAsync(date);

                    result.Add(new ChartDataPoint
                    {
                        Label = date.Day.ToString(),
                        Value = dailySales
                    });
                }
            }

            return result;
        }



        public async Task<List<ChartDataPoint>> GetCustomerGrowthDataAsync(DateTime? startDate, DateTime? endDate)
        {
            var result = new List<ChartDataPoint>();

            DateTime start = startDate ?? DateTime.Today;
            DateTime end = endDate ?? DateTime.Today;

            if (start.Date == end.Date && start.Date == DateTime.Today)
            {
                int currentHour = DateTime.Now.Hour;

                for (int hour = 0; hour <= currentHour; hour++)
                {
                    var from = start.AddHours(hour);
                    var to = from.AddHours(1);

                    var hourlyCustomerCount = await _customerRepository.GetNewCustomerCountByTimeRange(from, to);

                    result.Add(new ChartDataPoint
                    {
                        Label = from.ToString("HH:mm"),
                        Value = hourlyCustomerCount
                    });
                }
            }
            else
            {
                for (DateTime date = start.Date; date <= end.Date; date = date.AddDays(1))
                {
                    var dailyCustomerCount = await _customerRepository.GetNewCustomerCountByTimeRange(date, date.AddDays(1));

                    result.Add(new ChartDataPoint
                    {
                        Label = date.ToString("dd MMM"),
                        Value = dailyCustomerCount
                    });
                }
            }

            return result;
        }

    }
}
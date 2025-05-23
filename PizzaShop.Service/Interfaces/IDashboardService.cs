using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces
{
    public interface IDashboardService
    {
        /// <summary>
        /// Retrieves dashboard data asynchronously for the specified date range.
        /// </summary>
        /// <param name="startDate">The start date for the data range.</param>
        /// <param name="endDate">The end date for the data range.</param>
        /// <returns>A task that returns the dashboard view model containing the data.</returns>
        Task<DashboardViewModel> GetDashboardDataAsync(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Retrieves revenue chart data asynchronously for the specified date range.
        /// </summary>
        /// <param name="startDate">The start date for the data range.</param>
        /// <param name="endDate">The end date for the data range.</param>
        /// <returns>A task that returns a list of chart data points representing revenue data.</returns>
        Task<List<ChartDataPoint>> GetRevenueChartDataAsync(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Retrieves customer growth chart data asynchronously for the specified date range.
        /// </summary>
        /// <param name="startDate">The start date for the data range.</param>
        /// <param name="endDate">The end date for the data range.</param>
        /// <returns>A task that returns a list of chart data points representing customer growth data.</returns>
        Task<List<ChartDataPoint>> GetCustomerGrowthDataAsync(DateTime? startDate, DateTime? endDate);
    }
}
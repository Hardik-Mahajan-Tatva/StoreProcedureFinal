using Microsoft.AspNetCore.Mvc;
using PizzaShop.Service.Attributes;
using PizzaShop.Service.Interfaces;
namespace PizzaShop.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        #region Constructor
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        #endregion

        #region  Dashboard
        [CustomAuthorize("CanView", "Admin", "Manager", "Chef")]
        [HttpGet]
        public IActionResult Dashboard(string from)
        {
            try
            {
                if (from == "OrderApp")
                {
                    ViewBag.Layout = "~/Views/Shared/_OrderAppLayout.cshtml";
                    return RedirectToAction("Index", "OrderApp");
                }
                else
                {
                    ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
                    return View();
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request for dashboard data. Please try again.If the problem persists, contact support.";
                return View();
            }
        }
        #endregion

        #region Dashboard's Partial View
        public async Task<IActionResult> LoadDashbord(string timeRange, DateTime? startDate, DateTime? endDate, string userRole)
        {
            try
            {
                if (!startDate.HasValue || !endDate.HasValue)
                {
                    DateTime today = DateTime.Today;

                    switch (timeRange)
                    {
                        case "today":
                            startDate = today;
                            endDate = today;
                            break;
                        case "7":
                            startDate = today.AddDays(-7);
                            endDate = today;
                            break;
                        case "30":
                            startDate = today.AddDays(-30);
                            endDate = today;
                            break;
                        case "month":
                            startDate = new DateTime(today.Year, today.Month, 1);
                            endDate = today;
                            break;
                        case "year":
                            startDate = new DateTime(today.Year, 1, 1);
                            endDate = today;
                            break;
                        case "all":
                            startDate = DateTime.MinValue;
                            endDate = DateTime.MaxValue;
                            break;
                    }
                }

                var dashboardData = await _dashboardService.GetDashboardDataAsync(startDate, endDate);
                dashboardData.RevenueChartData = await _dashboardService.GetRevenueChartDataAsync(startDate, endDate);
                dashboardData.CustomerGrowthData = await _dashboardService.GetCustomerGrowthDataAsync(startDate, endDate);
                dashboardData.UserRole = userRole;

                return PartialView("_DashboardPartial", dashboardData);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
                return PartialView("_DashboardPartial");
            }
        }
        #endregion
    }
}
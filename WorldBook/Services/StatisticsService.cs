using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;

namespace WorldBook.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;

        public StatisticsService(IStatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }

        public async Task<StatisticsViewModel> GetDashboardStatisticsAsync()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfYear = new DateTime(today.Year, 1, 1);

            var viewModel = new StatisticsViewModel
            {
                // Customer Statistics
                TotalCustomers = await _statisticsRepository.GetTotalCustomersAsync(),
                LoyalCustomers = await _statisticsRepository.GetLoyalCustomersAsync(),
                RegularCustomers = await _statisticsRepository.GetRegularCustomersAsync(),
                OccasionalCustomers = await _statisticsRepository.GetOccasionalCustomersAsync(),

                // Revenue Statistics
                TodayRevenue = await _statisticsRepository.GetRevenueByCriteriaAsync(today, today.AddDays(1)),
                MonthRevenue = await _statisticsRepository.GetRevenueByCriteriaAsync(startOfMonth, null),
                YearRevenue = await _statisticsRepository.GetRevenueByCriteriaAsync(startOfYear, null),
                DailyRevenue = await _statisticsRepository.GetDailyRevenueAsync(30),
                MonthlyRevenue = await _statisticsRepository.GetMonthlyRevenueAsync(12),

                // Cancelled Orders Statistics
                TodayCancelledOrders = await _statisticsRepository.GetCancelledOrdersCountAsync(today, today.AddDays(1)),
                MonthCancelledOrders = await _statisticsRepository.GetCancelledOrdersCountAsync(startOfMonth, null),
                YearCancelledOrders = await _statisticsRepository.GetCancelledOrdersCountAsync(startOfYear, null),
                DailyCancelledOrders = await _statisticsRepository.GetDailyCancelledOrdersAsync(30),
                MonthlyCancelledOrders = await _statisticsRepository.GetMonthlyCancelledOrdersAsync(12),

                // Additional Statistics
                TotalOrders = await _statisticsRepository.GetTotalOrdersAsync(),
                CompletedOrders = await _statisticsRepository.GetOrdersByStatusAsync("Completed"),
                PendingOrders = await _statisticsRepository.GetOrdersByStatusAsync("Not Approved"),
                AverageOrderValue = await _statisticsRepository.GetAverageOrderValueAsync(),
                TotalBooks = await _statisticsRepository.GetTotalBooksAsync(),
                LowStockBooks = await _statisticsRepository.GetLowStockBooksAsync(10)
            };

            return viewModel;
        }
    }
}
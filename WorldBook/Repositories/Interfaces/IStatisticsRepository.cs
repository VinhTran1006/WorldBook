using WorldBook.ViewModel;

namespace WorldBook.Repositories.Interfaces
{
    public interface IStatisticsRepository
    {
        // Customer Statistics
        Task<int> GetTotalCustomersAsync();
        Task<int> GetLoyalCustomersAsync(); // >= 5 orders
        Task<int> GetRegularCustomersAsync(); // 2-4 orders
        Task<int> GetOccasionalCustomersAsync(); // 1 order
        Task<List<CustomerLoyaltyViewModel>> GetTopCustomersAsync(int count = 10);

        // Revenue Statistics
        Task<decimal> GetRevenueByCriteriaAsync(DateTime? startDate, DateTime? endDate, string status = "Completed");
        Task<List<RevenueByDateViewModel>> GetDailyRevenueAsync(int days = 30);
        Task<List<RevenueByMonthViewModel>> GetMonthlyRevenueAsync(int months = 12);

        // Cancelled Orders Statistics
        Task<int> GetCancelledOrdersCountAsync(DateTime? startDate, DateTime? endDate);
        Task<List<CancelledOrderByDateViewModel>> GetDailyCancelledOrdersAsync(int days = 30);
        Task<List<CancelledOrderByMonthViewModel>> GetMonthlyCancelledOrdersAsync(int months = 12);

        // General Order Statistics
        Task<int> GetTotalOrdersAsync();
        Task<int> GetOrdersByStatusAsync(string status);
        Task<decimal> GetAverageOrderValueAsync();

        // Book Statistics
        Task<int> GetTotalBooksAsync();
        Task<int> GetLowStockBooksAsync(int threshold = 10);
    }
}
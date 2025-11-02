using Microsoft.EntityFrameworkCore;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;
using WorldBook.ViewModel;
using System.Globalization;

namespace WorldBook.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly WorldBookDbContext _context;

        public StatisticsRepository(WorldBookDbContext context)
        {
            _context = context;
        }

        // Customer Statistics
        public async Task<int> GetTotalCustomersAsync()
        {
            return await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Customer") && u.IsActive)
                .CountAsync();
        }

        public async Task<int> GetLoyalCustomersAsync()
        {
            return await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Customer")
                    && u.IsActive
                    && u.Orders.Count >= 5)
                .CountAsync();
        }

        public async Task<int> GetRegularCustomersAsync()
        {
            return await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Customer")
                    && u.IsActive
                    && u.Orders.Count >= 2 && u.Orders.Count < 5)
                .CountAsync();
        }

        public async Task<int> GetOccasionalCustomersAsync()
        {
            return await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Customer")
                    && u.IsActive
                    && u.Orders.Count == 1)
                .CountAsync();
        }

        public async Task<List<CustomerLoyaltyViewModel>> GetTopCustomersAsync(int count = 10)
        {
            var customers = await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Customer") && u.IsActive)
                .Select(u => new
                {
                    u.Name,
                    u.Email,
                    OrderCount = u.Orders.Count,
                    TotalSpent = u.Orders.Where(o => o.Status == "Completed").Sum(o => o.TotalAmount ?? 0),
                    LastOrderDate = u.Orders.OrderByDescending(o => o.OrderDate).Select(o => o.OrderDate).FirstOrDefault()
                })
                .OrderByDescending(x => x.OrderCount)
                .ThenByDescending(x => x.TotalSpent)
                .Take(count)
                .ToListAsync();

            return customers.Select(c => new CustomerLoyaltyViewModel
            {
                CustomerName = c.Name,
                Email = c.Email,
                OrderCount = c.OrderCount,
                TotalSpent = c.TotalSpent,
                LastOrderDate = c.LastOrderDate ?? DateTime.MinValue,
                LoyaltyLevel = c.OrderCount >= 5 ? "Very Loyal" : c.OrderCount >= 2 ? "Regular" : "Occasional"
            }).ToList();
        }

        // Revenue Statistics
        public async Task<decimal> GetRevenueByCriteriaAsync(DateTime? startDate, DateTime? endDate, string status = "Completed")
        {
            var query = _context.Orders.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate <= endDate.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            var revenue = await query.SumAsync(o => o.TotalAmount ?? 0);
            return revenue;
        }

        public async Task<List<RevenueByDateViewModel>> GetDailyRevenueAsync(int days = 30)
        {
            var startDate = DateTime.Today.AddDays(-days);

            var data = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.Status == "Completed")
                .GroupBy(o => o.OrderDate.Value.Date)
                .Select(g => new RevenueByDateViewModel
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount ?? 0),
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return data;
        }

        public async Task<List<RevenueByMonthViewModel>> GetMonthlyRevenueAsync(int months = 12)
        {
            var startDate = DateTime.Today.AddMonths(-months);

            var data = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.Status == "Completed")
                .GroupBy(o => new { o.OrderDate.Value.Year, o.OrderDate.Value.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Revenue = g.Sum(o => o.TotalAmount ?? 0),
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            return data.Select(d => new RevenueByMonthViewModel
            {
                Year = d.Year,
                Month = d.Month,
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(d.Month),
                Revenue = d.Revenue,
                OrderCount = d.OrderCount
            }).ToList();
        }

        // Cancelled Orders Statistics
        public async Task<int> GetCancelledOrdersCountAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Orders.Where(o => o.Status == "Cancelled");

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate <= endDate.Value);

            return await query.CountAsync();
        }

        public async Task<List<CancelledOrderByDateViewModel>> GetDailyCancelledOrdersAsync(int days = 30)
        {
            var startDate = DateTime.Today.AddDays(-days);

            var data = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.Status == "Cancelled")
                .GroupBy(o => o.OrderDate.Value.Date)
                .Select(g => new CancelledOrderByDateViewModel
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return data;
        }

        public async Task<List<CancelledOrderByMonthViewModel>> GetMonthlyCancelledOrdersAsync(int months = 12)
        {
            var startDate = DateTime.Today.AddMonths(-months);

            var data = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.Status == "Cancelled")
                .GroupBy(o => new { o.OrderDate.Value.Year, o.OrderDate.Value.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            return data.Select(d => new CancelledOrderByMonthViewModel
            {
                Year = d.Year,
                Month = d.Month,
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(d.Month),
                Count = d.Count
            }).ToList();
        }

        // General Order Statistics
        public async Task<int> GetTotalOrdersAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<int> GetOrdersByStatusAsync(string status)
        {
            return await _context.Orders.Where(o => o.Status == status).CountAsync();
        }

        public async Task<decimal> GetAverageOrderValueAsync()
        {
            var completedOrders = await _context.Orders
                .Where(o => o.Status == "Completed" && o.TotalAmount.HasValue)
                .ToListAsync();

            if (!completedOrders.Any())
                return 0;

            return (decimal)completedOrders.Average(o => o.TotalAmount ?? 0);
        }

        // Book Statistics
        public async Task<int> GetTotalBooksAsync()
        {
            return await _context.Books.Where(b => b.IsActive).CountAsync();
        }

        public async Task<int> GetLowStockBooksAsync(int threshold = 10)
        {
            return await _context.Books
                .Where(b => b.IsActive && b.BookQuantity < threshold)
                .CountAsync();
        }
    }
}
namespace WorldBook.ViewModel
{
    public class StatisticsViewModel
    {
        // Customer Statistics
        public int TotalCustomers { get; set; }
        public int LoyalCustomers { get; set; } // >= 5 orders
        public int RegularCustomers { get; set; } // 2-4 orders
        public int OccasionalCustomers { get; set; } // 1 order

        // Revenue Statistics
        public decimal TodayRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public decimal YearRevenue { get; set; }
        public List<RevenueByDateViewModel> DailyRevenue { get; set; } = new();
        public List<RevenueByMonthViewModel> MonthlyRevenue { get; set; } = new();

        // Cancelled Orders Statistics
        public int TodayCancelledOrders { get; set; }
        public int MonthCancelledOrders { get; set; }
        public int YearCancelledOrders { get; set; }
        public List<CancelledOrderByDateViewModel> DailyCancelledOrders { get; set; } = new();
        public List<CancelledOrderByMonthViewModel> MonthlyCancelledOrders { get; set; } = new();

        // Additional Statistics
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalBooks { get; set; }
        public int LowStockBooks { get; set; } // < 10 quantity
    }

    public class RevenueByDateViewModel
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class RevenueByMonthViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class CancelledOrderByDateViewModel
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class CancelledOrderByMonthViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int Count { get; set; }
    }

    public class CustomerLoyaltyViewModel
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime LastOrderDate { get; set; }
        public string LoyaltyLevel { get; set; } // "Very Loyal", "Regular", "Occasional"
    }
}
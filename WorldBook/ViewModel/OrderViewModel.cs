namespace WorldBook.ViewModel
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string? CustomerName { get; set; }
        public string? Address { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string? Status { get; set; }
        public long? TotalAmount { get; set; }
        public int? Discount { get; set; }

        // 🆕 Thêm danh sách sản phẩm trong đơn
        public List<OrderBookItem> Books { get; set; } = new();
    }

    public class OrderBookItem
    {
        public string? BookName { get; set; }
        public string? ImageUrl { get; set; }
    }
}

namespace WorldBook.ViewModel
{
    public class OrderDetailViewModel
    {
        public int OrderId { get; set; }
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string? Status { get; set; }
        public long TotalAmount { get; set; }
        public int Discount { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; } = new();

        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public decimal? PaymentAmount { get; set; }
        public string? TransactionId { get; set; }
    }

    public class OrderItemViewModel
    {
        public string? BookName { get; set; }
        public int Quantity { get; set; }
        public long UnitPrice { get; set; }
        public string? ImageUrl { get; set; }
    }
}

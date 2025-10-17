namespace WorldBook.ViewModel
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public bool IsEmpty => Items == null || !Items.Any();
    }

    public class CartItemViewModel
    {
        public int CartId { get; set; }
        public int BookId { get; set; }
        public string BookName { get; set; } = string.Empty;
        public string? BookDescription { get; set; }
        public decimal BookPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => BookPrice * Quantity;
        public string ImageUrl { get; set; } = string.Empty;
        public int AvailableStock { get; set; }
        public string? PublisherName { get; set; }
    }
}
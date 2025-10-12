using WorldBook.Models;

namespace WorldBook.ViewModel
{
    public class BookViewModel
    {
        public IEnumerable<BookDetailViewModel> Books { get; set; }

    }

    public class BookDetailViewModel
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string? BookDescription { get; set; }
        public decimal BookPrice { get; set; }
        public int BookQuantity { get; set; }

        public string ImageUrl1 { get; set; }
        public string? ImageUrl2 { get; set; }
        public string? ImageUrl3 { get; set; }
        public string? ImageUrl4 { get; set; }

        public DateTime AddedAt { get; set; }

        // Thông tin nhà xuất bản
        public string? PublisherName { get; set; }

        // Thông tin nhà cung cấp
        public string? SupplierName { get; set; }

        // Danh sách tác giả (có thể nhiều)
        public List<string> AuthorNames { get; set; } = new List<string>();
        public List<string> Categories { get; set; } = new List<string>();
    }

    public class BookCreateEditViewModel
    {
        public int? BookId { get; set; } // null khi Create, có giá trị khi Edit
        public string BookName { get; set; }
        public string? BookDescription { get; set; }
        public decimal BookPrice { get; set; }
        public int BookQuantity { get; set; }

        public IFormFile? ImageUrl1 { get; set; }
        public IFormFile? ImageUrl2 { get; set; }
        public IFormFile? ImageUrl3 { get; set; }
        public IFormFile? ImageUrl4 { get; set; }

        public string PublisherName { get; set; }
        public string SupplierName { get; set; }

        public List<string> AuthorNames { get; set; } = new();
        public List<string> CategoryNames { get; set; } = new();
    }

}

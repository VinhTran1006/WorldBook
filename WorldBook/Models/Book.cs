using System;
using System.Collections.Generic;

namespace WorldBook.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string BookName { get; set; }

    public string? BookDescription { get; set; }

    public decimal BookPrice { get; set; }

    public int BookQuantity { get; set; }

    public bool IsActive { get; set; }

    public int PublisherId { get; set; }

    public int SupplierId { get; set; }

    public string ImageUrl1 { get; set; }

    public string? ImageUrl2 { get; set; }

    public string? ImageUrl3 { get; set; }

    public string? ImageUrl4 { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    public virtual ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<ImportStockDetail> ImportStockDetails { get; set; } = new List<ImportStockDetail>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Publisher? Publisher { get; set; }

    public virtual Supplier? Supplier { get; set; }
}

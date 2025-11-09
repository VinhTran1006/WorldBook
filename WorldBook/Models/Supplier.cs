using System;
using System.Collections.Generic;

namespace WorldBook.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string? SupplierName { get; set; }

    public string? SupplierEmail { get; set; }

    public string? PhoneNumber { get; set; }

    public bool? IsActive { get; set; } = true;

    public string? ContactPerson { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual ICollection<ImportStock> ImportStocks { get; set; } = new List<ImportStock>();
}

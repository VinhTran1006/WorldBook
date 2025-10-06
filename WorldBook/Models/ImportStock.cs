using System;
using System.Collections.Generic;

namespace WorldBook.Models;

public partial class ImportStock
{
    public int ImportId { get; set; }

    public int? SupplierId { get; set; }

    public int? UserId { get; set; }

    public DateTime? ImportDate { get; set; }

    public long? TotalAmount { get; set; }

    public virtual ICollection<ImportStockDetail> ImportStockDetails { get; set; } = new List<ImportStockDetail>();

    public virtual Supplier? Supplier { get; set; }

    public virtual User? User { get; set; }
}

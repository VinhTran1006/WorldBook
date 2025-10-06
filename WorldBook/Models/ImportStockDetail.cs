using System;
using System.Collections.Generic;

namespace WorldBook.Models;

public partial class ImportStockDetail
{
    public int ImportStockDetailId { get; set; }

    public int? ImportId { get; set; }

    public int? BookId { get; set; }

    public int? Stock { get; set; }

    public long? UnitPrice { get; set; }

    public int? StockLeft { get; set; }

    public virtual Book? Book { get; set; }

    public virtual ImportStock? Import { get; set; }
}

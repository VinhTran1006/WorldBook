using System;
using System.Collections.Generic;

namespace WorldBook.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int? UserId { get; set; }

    public int? BookId { get; set; }

    public int? Quantity { get; set; }

    public virtual Book? Book { get; set; }

    public virtual User? User { get; set; }
}

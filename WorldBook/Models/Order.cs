using System;
using System.Collections.Generic;

namespace WorldBook.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public string? Address { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? DeliveredDate { get; set; }

    public string? Status { get; set; }

    public long? TotalAmount { get; set; }

    public int? Discount { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User? User { get; set; }
}

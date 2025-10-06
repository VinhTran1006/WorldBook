using System;
using System.Collections.Generic;

namespace WorldBook.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int? UserId { get; set; }

    public int? BookId { get; set; }

    public int? OrderId { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? Star { get; set; }

    public string? Comment { get; set; }

    public bool? IsActive { get; set; }

    public string? Reply { get; set; }

    public int? ReplyAccountId { get; set; }

    public DateTime? ReplyDate { get; set; }

    public virtual Book? Book { get; set; }

    public virtual Order? Order { get; set; }

    public virtual User? ReplyAccount { get; set; }

    public virtual User? User { get; set; }
}

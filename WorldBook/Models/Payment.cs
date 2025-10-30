using System;
using System.Collections.Generic;

namespace WorldBook.Models
{
    public partial class Payment
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        public string PaymentMethod { get; set; }

        public string PaymentStatus { get; set; }

        public string? TransactionId { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime? RefundAt { get; set; }

        // 👉 Navigation property (một Payment thuộc về một Order)
        public virtual Order? Order { get; set; }
    }
}

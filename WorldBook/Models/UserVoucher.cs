// WorldBook/Models/UserVoucher.cs
using System;

namespace WorldBook.Models
{
    public partial class UserVoucher
    {
        public int UserVoucherId { get; set; }
        public int? UserId { get; set; }
        public int? VoucherId { get; set; }
        public int? OrderId { get; set; }
        public DateTime? UsedAt { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual Voucher? Voucher { get; set; }
        public virtual Order? Order { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace WorldBook.Models;

public partial class Voucher
{
    public int VoucherId { get; set; }

    public string? VoucherCode { get; set; }

    public int? DiscountPercent { get; set; }

    public DateTime? ExpriryDate { get; set; }

    public decimal? MinOrderAmount { get; set; }

    public decimal? MaxOrderAmount { get; set; }

    public int? UsageCount { get; set; }

    public bool? IsActive { get; set; }

    public string? VoucherDescription { get; set; }
}

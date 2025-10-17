using System.ComponentModel.DataAnnotations;

namespace WorldBook.ViewModel
{
    public class VoucherViewModel
    {
        public int VoucherId { get; set; }

        [Required(ErrorMessage = "Voucher code is required")]
        [Display(Name = "Voucher Code")]
        [StringLength(50, ErrorMessage = "Voucher code cannot exceed 50 characters")]
        public string VoucherCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Discount percent is required")]
        [Display(Name = "Discount Percent (%)")]
        [Range(1, 100, ErrorMessage = "Discount percent must be between 1 and 100")]
        public int DiscountPercent { get; set; }

        [Required(ErrorMessage = "Expiry date is required")]
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime ExpriryDate { get; set; }

        [Display(Name = "Minimum Order Amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum order amount must be a positive value")]
        [DataType(DataType.Currency)]
        public decimal? MinOrderAmount { get; set; }

        [Display(Name = "Maximum Order Amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Maximum order amount must be a positive value")]
        [DataType(DataType.Currency)]
        public decimal? MaxOrderAmount { get; set; }

        [Display(Name = "Usage Limit")]
        [Range(0, int.MaxValue, ErrorMessage = "Usage limit must be zero or a positive number")]
        public int? UsageCount { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Voucher Description")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        public string? VoucherDescription { get; set; }
    }
}
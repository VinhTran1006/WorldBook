// WorldBook/ViewModel/CheckoutViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorldBook.ViewModel
{
    /// <summary>
    /// ViewModel chính cho trang Checkout
    /// </summary>
    public class CheckoutViewModel
    {
        // User Information
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Delivery address is required")]
        public string Address { get; set; } = string.Empty;

        // Cart Items
        public List<CheckoutItemViewModel> Items { get; set; } = new List<CheckoutItemViewModel>();

        // Available Vouchers
        public List<AvailableVoucherViewModel> AvailableVouchers { get; set; } = new List<AvailableVoucherViewModel>();

        // Selected Voucher
        public int? SelectedVoucherId { get; set; }

        // Price Calculation
        public decimal Subtotal { get; set; }
        public int DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    /// <summary>
    /// ViewModel cho từng item trong giỏ hàng được chọn
    /// </summary>
    public class CheckoutItemViewModel
    {
        public int BookId { get; set; }
        public string BookName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => Price * Quantity;
    }

    /// <summary>
    /// ViewModel cho voucher khả dụng
    /// </summary>
    public class AvailableVoucherViewModel
    {
        public int VoucherId { get; set; }
        public string VoucherCode { get; set; } = string.Empty;
        public int DiscountPercent { get; set; }
        public DateTime ExpriryDate { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxOrderAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsApplicable { get; set; } // Check điều kiện amount
        public string NotApplicableReason { get; set; } = string.Empty; // Lý do không áp dụng được
    }
}
// WorldBook/Services/CheckoutService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;

namespace WorldBook.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IUserVoucherRepository _userVoucherRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<CheckoutService> _logger;
        private readonly WorldBookDbContext _context;

        public CheckoutService(
            IUserRepository userRepository,
            ICartRepository cartRepository,
            IVoucherRepository voucherRepository,
            IUserVoucherRepository userVoucherRepository,
            IOrderRepository orderRepository,
            IBookRepository bookRepository,
            ILogger<CheckoutService> logger,
            WorldBookDbContext context)
        {
            _userRepository = userRepository;
            _cartRepository = cartRepository;
            _voucherRepository = voucherRepository;
            _userVoucherRepository = userVoucherRepository;
            _orderRepository = orderRepository;
            _bookRepository = bookRepository;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Lấy dữ liệu cho trang Checkout
        /// </summary>
        public async Task<CheckoutViewModel> GetCheckoutDataAsync(int userId, List<int> selectedBookIds)
        {
            // Lấy thông tin user
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Lấy cart items đã chọn
            var cartItems = await _cartRepository.GetCartByUserAsync(userId);
            var selectedItems = cartItems
                .Where(c => selectedBookIds.Contains(c.BookId.Value))
                .Select(c => new CheckoutItemViewModel
                {
                    BookId = c.BookId.Value,
                    BookName = c.Book.BookName,
                    ImageUrl = c.Book.ImageUrl1,
                    Price = c.Book.BookPrice,
                    Quantity = c.Quantity.Value
                })
                .ToList();

            if (!selectedItems.Any())
            {
                throw new Exception("No items selected for checkout");
            }

            // Tính tổng tiền
            decimal subtotal = selectedItems.Sum(i => i.Subtotal);

            // Lấy danh sách voucher khả dụng
            var availableVouchers = await GetAvailableVouchersAsync(userId, subtotal);

            var model = new CheckoutViewModel
            {
                UserId = userId,
                UserName = user.Name,
                Email = user.Email,
                Phone = user.Phone ?? string.Empty,
                Address = user.Address ?? string.Empty,
                Items = selectedItems,
                AvailableVouchers = availableVouchers,
                Subtotal = subtotal,
                DiscountPercent = 0,
                DiscountAmount = 0,
                TotalAmount = subtotal
            };

            return model;
        }

        /// <summary>
        /// Lấy danh sách voucher khả dụng
        /// </summary>
        public async Task<List<AvailableVoucherViewModel>> GetAvailableVouchersAsync(int userId, decimal totalAmount)
        {
            // Lấy tất cả voucher active và còn hạn
            var activeVouchers = await _voucherRepository.GetActiveVouchersAsync();

            var availableVouchers = new List<AvailableVoucherViewModel>();

            foreach (var voucher in activeVouchers)
            {
                // Kiểm tra user đã dùng voucher này chưa
                bool hasUsed = await _userVoucherRepository.HasUserUsedVoucherAsync(userId, voucher.VoucherId);
                if (hasUsed)
                {
                    continue; // Skip voucher đã dùng
                }

                // Kiểm tra điều kiện MinOrderAmount và MaxOrderAmount
                bool isApplicable = true;
                string notApplicableReason = string.Empty;

                if (voucher.MinOrderAmount.HasValue && totalAmount < voucher.MinOrderAmount.Value)
                {
                    isApplicable = false;
                    notApplicableReason = $"Minimum order amount: {voucher.MinOrderAmount.Value:N0} $";
                }

                if (voucher.MaxOrderAmount.HasValue && totalAmount > voucher.MaxOrderAmount.Value)
                {
                    isApplicable = false;
                    notApplicableReason = $"Maximum order amount: {voucher.MaxOrderAmount.Value:N0} $";
                }

                availableVouchers.Add(new AvailableVoucherViewModel
                {
                    VoucherId = voucher.VoucherId,
                    VoucherCode = voucher.VoucherCode ?? string.Empty,
                    DiscountPercent = voucher.DiscountPercent ?? 0,
                    ExpriryDate = voucher.ExpriryDate ?? DateTime.Now,
                    MinOrderAmount = voucher.MinOrderAmount,
                    MaxOrderAmount = voucher.MaxOrderAmount,
                    Description = voucher.VoucherDescription ?? string.Empty,
                    IsApplicable = isApplicable,
                    NotApplicableReason = notApplicableReason
                });
            }

            return availableVouchers;
        }

        /// <summary>
        /// Áp dụng voucher vào model
        /// </summary>
        public async Task<CheckoutViewModel> ApplyVoucherAsync(CheckoutViewModel model, int voucherId)
        {
            // Validate voucher
            var voucher = await _voucherRepository.GetByIdAsync(voucherId);
            if (voucher == null || voucher.IsActive != true)
            {
                throw new Exception("Voucher not found or inactive");
            }

            // Kiểm tra hết hạn
            if (voucher.ExpriryDate.HasValue && voucher.ExpriryDate.Value < DateTime.Now)
            {
                throw new Exception("Voucher has expired");
            }

            // Kiểm tra user đã dùng chưa
            bool hasUsed = await _userVoucherRepository.HasUserUsedVoucherAsync(model.UserId, voucherId);
            if (hasUsed)
            {
                throw new Exception("You have already used this voucher");
            }

            // Validate MinOrderAmount
            if (voucher.MinOrderAmount.HasValue && model.Subtotal < voucher.MinOrderAmount.Value)
            {
                throw new Exception($"Order amount must be at least {voucher.MinOrderAmount.Value:N0} $");
            }

            // Validate MaxOrderAmount
            if (voucher.MaxOrderAmount.HasValue && model.Subtotal > voucher.MaxOrderAmount.Value)
            {
                throw new Exception($"Order amount must not exceed {voucher.MaxOrderAmount.Value:N0} $");
            }

            // Áp dụng discount
            model.SelectedVoucherId = voucherId;
            model.DiscountPercent = voucher.DiscountPercent ?? 0;
            model.DiscountAmount = model.Subtotal * model.DiscountPercent / 100;
            model.TotalAmount = model.Subtotal - model.DiscountAmount;

            return model;
        }

        /// <summary>
        /// Tạo Order từ CheckoutViewModel
        /// Sử dụng Transaction để đảm bảo data consistency
        /// </summary>
        public async Task<int> CreateOrderAsync(CheckoutViewModel model)
        {
            // Validate model
            if (model.Items == null || !model.Items.Any())
            {
                throw new Exception("No items in order");
            }

            if (string.IsNullOrWhiteSpace(model.Address))
            {
                throw new Exception("Delivery address is required");
            }

            // Bắt đầu transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Tạo Order
                var order = new Order
                {
                    UserId = model.UserId,
                    Address = model.Address,
                    OrderDate = DateTime.Now,
                    DeliveredDate = null,
                    Status = "Not Approved", // Pending approval từ Admin
                    TotalAmount = (long)model.TotalAmount,
                    Discount = model.DiscountPercent,
                    UpdateAt = DateTime.Now
                };

                var createdOrder = await _orderRepository.CreateOrderAsync(order);
                _logger.LogInformation($"Created Order {createdOrder.OrderId} for User {model.UserId}");

                // 2. Tạo OrderDetails
                var orderDetails = model.Items.Select(item => new OrderDetail
                {
                    OrderId = createdOrder.OrderId,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    Price = (long)item.Price
                }).ToList();

                await _orderRepository.AddOrderDetailsAsync(orderDetails);
                _logger.LogInformation($"Added {orderDetails.Count} OrderDetails for Order {createdOrder.OrderId}");

                // 3. Xử lý Voucher (nếu có)
                if (model.SelectedVoucherId.HasValue)
                {
                    // Tạo UserVoucher record
                    var userVoucher = new UserVoucher
                    {
                        UserId = model.UserId,
                        VoucherId = model.SelectedVoucherId.Value,
                        OrderId = createdOrder.OrderId,
                        UsedAt = DateTime.Now
                    };

                    await _userVoucherRepository.AddUserVoucherAsync(userVoucher);

                    // Tăng UsageCount của Voucher
                    await _voucherRepository.IncrementUsageCountAsync(model.SelectedVoucherId.Value);

                    _logger.LogInformation($"Applied Voucher {model.SelectedVoucherId.Value} to Order {createdOrder.OrderId}");
                }

                // 4. Xóa items khỏi Cart
                var bookIds = model.Items.Select(i => i.BookId).ToList();
                await _orderRepository.RemoveCartItemsAsync(model.UserId, bookIds);
                _logger.LogInformation($"Removed {bookIds.Count} items from Cart for User {model.UserId}");

                // Commit transaction
                await transaction.CommitAsync();

                _logger.LogInformation($"Order {createdOrder.OrderId} created successfully");
                return createdOrder.OrderId;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating order");
                throw new Exception($"Failed to create order: {ex.Message}");
            }
        }
    }
}
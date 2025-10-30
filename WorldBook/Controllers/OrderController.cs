using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WorldBook.Config;
using WorldBook.Models;
using WorldBook.Repositories;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;

namespace WorldBook.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICheckoutService _checkoutService;
        private readonly MomoPaymentService _momoPaymentService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly MomoOptions _momoOptions;

        public OrderController(IOrderService orderService, ICheckoutService checkoutService,
                       MomoPaymentService momoPaymentService,
                       IPaymentRepository paymentRepository,
                       IOrderRepository orderRepository, 
                       ICartRepository cartRepository,
                       IOptions<MomoOptions> momoOptions)
        {
            _orderService = orderService;
            _checkoutService = checkoutService;
            _momoPaymentService = momoPaymentService;
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _momoOptions = momoOptions.Value;
        }

        // ============ EXISTING METHODS (Admin) ============
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View("~/Views/AdminViews/ManageOrder/Index.cshtml", orders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return View("~/Views/AdminViews/ManageOrder/Details.cshtml", order);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await _orderService.ApproveNextStatusAsync(id);
                TempData["SuccessMessage"] = "Duyệt đơn hàng thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            await _orderService.CancelOrderAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ApproveOrderDetail(int id)
        {
            try
            {
                await _orderService.ApproveNextStatusAsync(id);
                TempData["SuccessMessage"] = "Duyệt đơn hàng thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrderDetai(int id)
        {
            await _orderService.CancelOrderAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        // ============ NEW METHODS (Customer Checkout) ============

        /// <summary>
        /// GET: Trang Checkout - Hiển thị thông tin đặt hàng
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Checkout(string selectedItems)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    TempData["Error"] = "Please log in to checkout";
                    return RedirectToAction("Login", "Logins");
                }

                // Parse selected book IDs từ query string
                if (string.IsNullOrWhiteSpace(selectedItems))
                {
                    TempData["Error"] = "No items selected for checkout";
                    return RedirectToAction("Index", "Cart");
                }

                var bookIds = selectedItems
                    .Split(',')
                    .Select(id => int.TryParse(id, out var bookId) ? bookId : 0)
                    .Where(id => id > 0)
                    .ToList();

                if (!bookIds.Any())
                {
                    TempData["Error"] = "Invalid items selected";
                    return RedirectToAction("Index", "Cart");
                }

                // Lấy dữ liệu checkout
                var model = await _checkoutService.GetCheckoutDataAsync(userId, bookIds);

                return View("~/Views/UserViews/Order/Checkout.cshtml", model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Cart");
            }
        }

        /// <summary>
        /// POST: Apply Voucher - AJAX call để áp dụng voucher
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ApplyVoucher([FromBody] ApplyVoucherRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return Json(new { success = false, message = "Please log in" });
                }

                // Recreate CheckoutViewModel from request
                var model = new CheckoutViewModel
                {
                    UserId = userId,
                    Address = request.Address,
                    Items = request.Items,
                    Subtotal = request.Subtotal
                };

                // Apply voucher
                var updatedModel = await _checkoutService.ApplyVoucherAsync(model, request.VoucherId);

                return Json(new
                {
                    success = true,
                    discountPercent = updatedModel.DiscountPercent,
                    discountAmount = updatedModel.DiscountAmount,
                    totalAmount = updatedModel.TotalAmount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// POST: Place Order - Tạo đơn hàng
        /// </summary>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    TempData["Error"] = "Please log in to place order";
                    return RedirectToAction("Login", "Logins");
                }

                // Set UserId from current user
                model.UserId = userId;

                // Validate address
                if (string.IsNullOrWhiteSpace(model.Address))
                {
                    TempData["Error"] = "Delivery address is required";
                    return RedirectToAction("Checkout", new { selectedItems = string.Join(",", model.Items.Select(i => i.BookId)) });
                }

                // Create order
                var orderId = await _checkoutService.CreateOrderAsync(model);

                TempData["Success"] = "Order placed successfully!";
                return RedirectToAction("OrderSuccess", new { orderId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                // Redirect back to checkout with selected items
                var selectedItems = string.Join(",", model.Items.Select(i => i.BookId));
                return RedirectToAction("Checkout", new { selectedItems });
            }
        }

        /// <summary>
        /// GET: Order Success Page
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> OrderSuccess(int orderId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var order = await _orderService.GetOrderByIdAsync(orderId);

                if (order == null)
                {
                    return NotFound();
                }

                return View("~/Views/UserViews/Order/OrderSuccess.cshtml", order);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // ============ HELPER METHODS ============

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> OrderHistory()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                TempData["Error"] = "Please log in to view your orders";
                return RedirectToAction("Login", "Logins");
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return View("~/Views/UserViews/Order/OrderHistory.cshtml", orders);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserOrderDetails(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                TempData["Error"] = "Please log in to view your order details";
                return RedirectToAction("Login", "Logins");
            }

            var order = await _orderService.GetOrderByIdAsync(id);

            // Bảo mật: chỉ cho phép xem đơn của chính user đó
            if (order == null)
                return NotFound();

            if (order.CustomerName == null)
                return Forbid();

            return View("~/Views/UserViews/Order/OrderHistoryDetail.cshtml", order);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CancelUserOrder(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    TempData["Error"] = "Please log in to cancel your order";
                    return RedirectToAction("Login", "Logins");
                }

                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    TempData["Error"] = "Order not found!";
                    return RedirectToAction("OrderHistory");
                }

                // Chỉ cho phép user hủy đơn của chính họ khi đang ở trạng thái "Not Approved"
                if (order.Status != "Not Approved")
                {
                    TempData["Error"] = "You can only cancel orders that are not yet approved.";
                    return RedirectToAction("OrderHistory");
                }

                await _orderService.CancelOrderAsync(id);
                TempData["Success"] = "Order has been cancelled successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("OrderHistory");
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrderWithMomo([FromBody] CheckoutViewModel request)
        {
            try
            {
                // 1️⃣ Lấy thông tin user
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    TempData["Error"] = "Please log in to place order";
                    return RedirectToAction("Login", "Logins");
                }

                // Set UserId from current user
                var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userId);

                if (!cartItems.Any())
                    return BadRequest(new { message = "Giỏ hàng trống" });

                // 2️⃣ Tạo Order mới
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    Address = request.Address,
                    Status = "Not Approved",
                    TotalAmount = (long)cartItems.Sum(c => c.Quantity * (c.Book.BookPrice)),
                    UpdateAt = DateTime.Now
                };

                await _orderRepository.CreateOrderAsync(order);

                // 3️⃣ Tạo OrderDetail
                var orderDetails = cartItems.Select(c => new OrderDetail
                {
                    OrderId = order.OrderId,
                    BookId = c.BookId.Value,
                    Quantity = c.Quantity,
                    Price = (long)(c.Book.BookPrice)
                }).ToList();

                await _orderRepository.AddOrderDetailsAsync(orderDetails);

                // 4️⃣ Xóa khỏi giỏ
                await _orderRepository.RemoveCartItemsAsync(userId, cartItems.Select(c => c.BookId.Value).ToList());

                // 5️⃣ Trả về orderId cho JS
                return Ok(new { orderId = order.OrderId, amount = order.TotalAmount });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ PlaceOrderWithMomo error: " + ex.Message);
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> MomoReturn(
    string partnerCode,
    string orderId,
    string requestId,
    string amount,
    string orderInfo,
    string orderType,
    string transId,
    int resultCode,
    string message,
    string payType,
    long responseTime,
    string extraData,
    string signature)
        {
            try
            {
                var orderIdStr = orderId.Split('_')[0];

                if (!int.TryParse(orderIdStr, out int parsedOrderId))
                {
                    return Content($"Error: Invalid order ID format - {orderId}");
                }

                Order order;
                order = await _orderRepository.GetByIdAsync(parsedOrderId);

                if (order == null)
                {
                    return Content($"Error: Order {parsedOrderId} not found");
                }

                Payment payment;
                payment = await _paymentRepository.GetByOrderIdAsync(parsedOrderId);

                if (payment == null)
                {
                        payment = new Payment
                        {
                            OrderId = order.OrderId,
                            PaymentMethod = "ONLINE",
                            Amount = Convert.ToDecimal(amount),
                            CreatedAt = DateTime.Now,
                            PaymentStatus = "Pending"
                        };
                        await _paymentRepository.CreateAsync(payment);

                    order.PaymentId = payment.PaymentId;
                    await _orderRepository.UpdateAsync(order);
                }

                if (resultCode == 0)
                {
                        // Update payment
                        payment.PaymentStatus = "Paid";
                        payment.TransactionId = transId;
                        payment.PaidAt = DateTime.Now;

                        await _paymentRepository.UpdateAsync(payment);

                    try
                    {
                        // DÙNG service để chuyển trạng thái và trừ tồn kho
                        // (service sẽ kiểm tra trạng thái cũ và thực hiện trừ khi từ "Not Approved" -> "Preparing")
                        await _orderService.ApproveNextStatusAsync(parsedOrderId);

                        TempData["Success"] = "Payment successful! Your order is being prepared.";
                        return RedirectToAction("OrderSuccess", new { orderId = parsedOrderId });
                    }
                    catch (Exception ex)
                    {
                        // Nếu trừ hàng thất bại (vd: thiếu tồn kho), rollback/mark payment và order phù hợp
                        // Tùy business: ta có thể cancel order và ghi log, hoặc để admin xử lý + refund
                        payment.PaymentStatus = "Failed";
                        payment.RefundAt = DateTime.Now;
                        await _paymentRepository.UpdateAsync(payment);

                        order.Status = "Cancelled";
                        order.UpdateAt = DateTime.Now;
                        if (order.PaymentId == null) order.PaymentId = payment.PaymentId;
                        await _orderRepository.UpdateAsync(order);

                        // log
                        Console.WriteLine($"MomoReturn: failed to process order {parsedOrderId} after payment. Reason: {ex.Message}");

                        TempData["Error"] = $"Payment received but processing order failed: {ex.Message}. Admin will contact you.";

                        // Redirect user to history or a custom page explaining issue
                        return RedirectToAction("OrderFailed", new { orderId = parsedOrderId });
                    }
                }
                else
                {
                        // Update payment
                        payment.PaymentStatus = "Failed";
                        payment.RefundAt = DateTime.Now;
                        await _paymentRepository.UpdateAsync(payment);

                        // Update order
                        order.Status = "Cancelled";
                        order.UpdateAt = DateTime.Now;

                    if (order.PaymentId == null)
                        order.PaymentId = payment.PaymentId;

                    await _orderRepository.UpdateAsync(order);

                        TempData["Error"] = $"Payment failed: {message}";

                    return RedirectToAction("OrderFailed", new { orderId = parsedOrderId });
                }
            }
            catch (Exception ex)
            {
                return Content($"Critical Error: {ex.Message}\n\nStackTrace:\n{ex.StackTrace}");
            }
        }

        // ================== Helper ==================
        private static string SignSHA256(string rawData, string secretKey)
        {
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secretKey);
            byte[] messageBytes = encoding.GetBytes(rawData);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderFailed(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound();

            var orderVm = new OrderDetailViewModel
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Address = order.Address,
                TotalAmount = order.TotalAmount ?? 0, // tránh null
                Discount = order.Discount ?? 0,
                OrderItems = order.OrderDetails.Select(d => new OrderItemViewModel
                {
                    BookName = d.Book != null ? d.Book.BookName : "Unknown Book",
                    Quantity = d.Quantity ?? 0,
                    UnitPrice = d.Price ?? 0,
                    ImageUrl = d.Book != null ? d.Book.ImageUrl1 : null
                }).ToList()
            };

            return View("/Views/UserViews/Order/OrderFailed.cshtml", orderVm);
        }

        [HttpGet]
        public async Task<IActionResult> FilterOrders(string? status, string? search)
        {
            // Lấy userId từ Claims thay vì Session
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Auth");

            int userId = int.Parse(userIdClaim);

            var orders = await _orderService.FilterOrdersAsync(userId, status, search);
            return View("~/Views/UserViews/Order/OrderHistory.cshtml", orders);
        }

        [HttpGet]
        public async Task<IActionResult> FilterOrdersByAdmin(string? status, string? search)
        {
            var orders = await _orderService.FilterOrdersForAdminAsync(status, search);
            return View("~/Views/AdminViews/ManageOrder/Index.cshtml", orders);
        }

    }

    // ============ REQUEST MODELS ============

    /// <summary>
    /// Request model cho ApplyVoucher AJAX call
    /// </summary>
    public class ApplyVoucherRequest
    {
        public int VoucherId { get; set; }
        public string Address { get; set; } = string.Empty;
        public List<CheckoutItemViewModel> Items { get; set; } = new List<CheckoutItemViewModel>();
        public decimal Subtotal { get; set; }
    }
}
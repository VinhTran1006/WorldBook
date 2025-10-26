using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;

namespace WorldBook.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICheckoutService _checkoutService;

        public OrderController(IOrderService orderService, ICheckoutService checkoutService)
        {
            _orderService = orderService;
            _checkoutService = checkoutService;
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
        [Authorize]
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
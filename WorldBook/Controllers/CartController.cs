using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorldBook.Services.Interfaces;

namespace WorldBook.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: Cart/Index
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem giỏ hàng";
                return RedirectToAction("Login", "Logins");
            }

            var cart = await _cartService.GetCartAsync(userId);

            if (cart.IsEmpty)
            {
                return View("~/Views/UserViews/Cart/Empty.cshtml");
            }

            return View("~/Views/UserViews/Cart/Index.cshtml", cart);
        }

        // POST: Cart/AddToCart - Cập nhật để trả về JSON
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng",
                    requireLogin = true
                });
            }

            var result = await _cartService.AddToCartAsync(userId, id, quantity);

            if (result)
            {
                return Json(new
                {
                    success = true,
                    message = "Đã thêm sách vào giỏ hàng thành công!"
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Không thể thêm sách vào giỏ hàng. Vui lòng kiểm tra lại số lượng tồn kho."
                });
            }
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int bookId, int quantity)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" });
            }

            var result = await _cartService.UpdateQuantityAsync(userId, bookId, quantity);

            if (result)
            {
                var cart = await _cartService.GetCartAsync(userId);
                return Json(new
                {
                    success = true,
                    message = "Cập nhật số lượng thành công",
                    totalPrice = cart.TotalPrice,
                    totalItems = cart.TotalItems
                });
            }

            return Json(new { success = false, message = "Không thể cập nhật số lượng" });
        }

        // POST: Cart/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int bookId)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" });
            }

            var result = await _cartService.RemoveItemAsync(userId, bookId);

            if (result)
            {
                TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng";
                return Json(new { success = true, message = "Đã xóa sản phẩm" });
            }

            return Json(new { success = false, message = "Không thể xóa sản phẩm" });
        }

        // POST: Cart/Clear
        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" });
            }

            var result = await _cartService.ClearCartAsync(userId);

            if (result)
            {
                TempData["Success"] = "Đã xóa toàn bộ giỏ hàng";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Không thể xóa giỏ hàng";
            return RedirectToAction("Index");
        }

        // API: Get cart item count for badge
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { count = 0 });
            }

            var count = await _cartService.GetCartItemCountAsync(userId);
            return Json(new { count });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }
    }
}
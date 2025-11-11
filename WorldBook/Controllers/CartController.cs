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
                TempData["Error"] = "Please log in to view your cart!";
                return RedirectToAction("Login", "Logins");
            }

            var cart = await _cartService.GetCartAsync(userId);

            if (cart.IsEmpty)
            {
                return View("~/Views/UserViews/Cart/Empty.cshtml");
            }

            return View("~/Views/UserViews/Cart/Index.cshtml", cart);
        }

        // POST: Cart/AddToCart - Updated to return JSON
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Please log in to add items to your cart",
                    requireLogin = true
                });
            }

            var result = await _cartService.AddToCartAsync(userId, id, quantity);

            if (result)
            {
                return Json(new
                {
                    success = true,
                    message = "Book added to cart successfully!"
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Unable to add the book to the cart. Please check stock availability."
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
                return Json(new { success = false, message = "Please log in" });
            }

            var result = await _cartService.UpdateQuantityAsync(userId, bookId, quantity);

            if (result)
            {
                var cart = await _cartService.GetCartAsync(userId);
                return Json(new
                {
                    success = true,
                    message = "Quantity updated successfully",
                    totalPrice = cart.TotalPrice,
                    totalItems = cart.TotalItems
                });
            }

            return Json(new { success = false, message = "Unable to update quantity" });
        }

        // POST: Cart/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int bookId)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, message = "Please log in" });
            }

            var result = await _cartService.RemoveItemAsync(userId, bookId);

            if (result)
            {
                TempData["Success"] = "Item removed from cart";
                return Json(new { success = true, message = "Item removed" });
            }

            return Json(new { success = false, message = "Unable to remove item" });
        }

        // POST: Cart/Clear
        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, message = "Please log in" });
            }

            var result = await _cartService.ClearCartAsync(userId);

            if (result)
            {
                TempData["Success"] = "All items cleared from your cart";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Unable to clear cart";
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

        // Thêm các methods này vào CartController hiện tại của bạn

        // POST: Cart/GetSelectedItems
        [HttpPost]
        public async Task<IActionResult> GetSelectedItems([FromBody] List<int> selectedBookIds)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, message = "Please log in" });
            }

            var cart = await _cartService.GetCartAsync(userId);
            var selectedItems = cart.Items
                .Where(item => selectedBookIds.Contains(item.BookId))
                .Select(item => new
                {
                    bookId = item.BookId,
                    bookName = item.BookName,
                    price = item.BookPrice,
                    quantity = item.Quantity,
                    subtotal = item.Subtotal,
                    imageUrl = item.ImageUrl
                })
                .ToList();

            var totalPrice = selectedItems.Sum(i => i.subtotal);

            return Json(new
            {
                success = true,
                items = selectedItems,
                totalPrice = totalPrice,
                itemCount = selectedItems.Count
            });
        }

        // POST: Cart/ValidateSelectedItems - Kiểm tra tồn kho trước khi checkout
        [HttpPost]
        public async Task<IActionResult> ValidateSelectedItems([FromBody] List<int> selectedBookIds)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, message = "Please log in" });
            }

            var cart = await _cartService.GetCartAsync(userId);
            var invalidItems = new List<string>();

            foreach (var bookId in selectedBookIds)
            {
                var cartItem = cart.Items.FirstOrDefault(i => i.BookId == bookId);
                if (cartItem == null)
                {
                    invalidItems.Add($"Item not found in cart");
                    continue;
                }

                if (cartItem.AvailableStock < cartItem.Quantity)
                {
                    invalidItems.Add($"{cartItem.BookName}: Only {cartItem.AvailableStock} left in stock");
                }
            }

            if (invalidItems.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "Some items are out of stock",
                    invalidItems = invalidItems
                });
            }

            return Json(new { success = true });
        }
    }
}

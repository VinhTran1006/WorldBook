using WorldBook.Models;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;

namespace WorldBook.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IBookRepository _bookRepository;

        public CartService(ICartRepository cartRepository, IBookRepository bookRepository)
        {
            _cartRepository = cartRepository;
            _bookRepository = bookRepository;
        }

        public async Task<CartViewModel> GetCartAsync(int userId)
        {
            var cartItems = await _cartRepository.GetCartByUserAsync(userId);

            var cartViewModel = new CartViewModel
            {
                Items = cartItems.Select(c => new CartItemViewModel
                {
                    CartId = c.CartId,
                    BookId = c.Book.BookId,
                    BookName = c.Book.BookName,
                    BookDescription = c.Book.BookDescription,
                    BookPrice = c.Book.BookPrice,
                    Quantity = c.Quantity ?? 1,
                    ImageUrl = c.Book.ImageUrl1,
                    AvailableStock = c.Book.BookQuantity,
                    PublisherName = c.Book.Publisher?.PublisherName
                }).ToList()
            };

            cartViewModel.TotalPrice = cartViewModel.Items.Sum(i => i.Subtotal);
            cartViewModel.TotalItems = cartViewModel.Items.Sum(i => i.Quantity);

            return cartViewModel;
        }

        public async Task<bool> AddToCartAsync(int userId, int bookId, int quantity = 1)
        {
            try
            {
                // Kiểm tra sách có tồn tại không
                var book = await _bookRepository.GetBookByIdAsync(bookId);
                if (book == null || !book.IsActive)
                    return false;

                // Kiểm tra số lượng tồn kho
                if (book.BookQuantity < quantity)
                    return false;

                // Kiểm tra item đã tồn tại trong giỏ chưa
                var existingItem = await _cartRepository.GetCartItemAsync(userId, bookId);

                if (existingItem != null)
                {
                    // Cập nhật số lượng nếu đã tồn tại
                    var newQuantity = (existingItem.Quantity ?? 0) + quantity;

                    // Kiểm tra không vượt quá tồn kho
                    if (newQuantity > book.BookQuantity)
                        return false;

                    existingItem.Quantity = newQuantity;
                    await _cartRepository.UpdateItemAsync(existingItem);
                }
                else
                {
                    // Thêm mới nếu chưa tồn tại
                    var cartItem = new Cart
                    {
                        UserId = userId,
                        BookId = bookId,
                        Quantity = quantity
                    };
                    await _cartRepository.AddItemAsync(cartItem);
                }

                await _cartRepository.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateQuantityAsync(int userId, int bookId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                    return false;

                var cartItem = await _cartRepository.GetCartItemAsync(userId, bookId);
                if (cartItem == null)
                    return false;

                // Kiểm tra tồn kho
                var book = await _bookRepository.GetBookByIdAsync(bookId);
                if (book == null || quantity > book.BookQuantity)
                    return false;

                cartItem.Quantity = quantity;
                await _cartRepository.UpdateItemAsync(cartItem);
                await _cartRepository.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveItemAsync(int userId, int bookId)
        {
            try
            {
                await _cartRepository.RemoveItemAsync(userId, bookId);
                await _cartRepository.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            try
            {
                await _cartRepository.ClearCartAsync(userId);
                await _cartRepository.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            return await _cartRepository.GetCartItemCountAsync(userId);
        }
    }
}
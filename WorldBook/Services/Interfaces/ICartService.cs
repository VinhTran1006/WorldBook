using WorldBook.ViewModel;

namespace WorldBook.Services.Interfaces
{
    public interface ICartService
    {
        // Lấy giỏ hàng của user
        Task<CartViewModel> GetCartAsync(int userId);

        // Thêm sách vào giỏ hàng
        Task<bool> AddToCartAsync(int userId, int bookId, int quantity = 1);

        // Cập nhật số lượng
        Task<bool> UpdateQuantityAsync(int userId, int bookId, int quantity);

        // Xóa một item
        Task<bool> RemoveItemAsync(int userId, int bookId);

        // Xóa toàn bộ giỏ hàng
        Task<bool> ClearCartAsync(int userId);

        // Đếm số lượng items trong giỏ
        Task<int> GetCartItemCountAsync(int userId);
    }
}
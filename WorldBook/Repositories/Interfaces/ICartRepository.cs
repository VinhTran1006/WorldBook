using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface ICartRepository
    {
        // Lấy tất cả items trong giỏ hàng của user
        Task<List<Cart>> GetCartByUserAsync(int userId);

        // Lấy một item cụ thể trong giỏ hàng
        Task<Cart?> GetCartItemAsync(int userId, int bookId);

        // Thêm item vào giỏ hàng
        Task AddItemAsync(Cart cart);

        // Cập nhật số lượng
        Task UpdateItemAsync(Cart cart);

        // Xóa một item
        Task RemoveItemAsync(int userId, int bookId);

        // Xóa toàn bộ giỏ hàng
        Task ClearCartAsync(int userId);

        // Kiểm tra item có tồn tại không
        Task<bool> ItemExistsAsync(int userId, int bookId);

        // Lấy số lượng items trong giỏ
        Task<int> GetCartItemCountAsync(int userId);

        // Save changes
        Task SaveChangesAsync();
    }
}
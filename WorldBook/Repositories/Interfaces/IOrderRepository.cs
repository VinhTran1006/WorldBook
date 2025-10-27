using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<Order> CreateOrderAsync(Order order);
        Task AddOrderDetailsAsync(List<OrderDetail> orderDetails);

        /// Xóa items khỏi Cart sau khi đặt hàng thành công
        Task RemoveCartItemsAsync(int userId, List<int> bookIds);

        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
    }
}

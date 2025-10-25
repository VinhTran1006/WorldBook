using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task UpdateOrderStatusAsync(int orderId, string newStatus);
    }
}

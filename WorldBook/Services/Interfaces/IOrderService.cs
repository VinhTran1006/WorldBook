using WorldBook.ViewModel;

namespace WorldBook.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderViewModel>> GetAllOrdersAsync();
        Task<OrderDetailViewModel> GetOrderByIdAsync(int id);
        Task ApproveNextStatusAsync(int orderId);
        Task CancelOrderAsync(int orderId);
        Task<IEnumerable<OrderViewModel>> GetOrdersByUserIdAsync(int userId);

        Task<IEnumerable<OrderViewModel>> FilterOrdersAsync(int userId, string? status, string? search);

        Task<IEnumerable<OrderViewModel>> FilterOrdersForAdminAsync(string? status, string? search);
    }
}

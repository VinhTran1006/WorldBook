using Microsoft.EntityFrameworkCore;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;

namespace WorldBook.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly WorldBookDbContext _context;

        public OrderRepository(WorldBookDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.User) // lấy luôn thông tin user
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null) return;

            order.Status = newStatus;
            if (newStatus == "Completed")
                order.DeliveredDate = DateTime.Now;

            order.UpdateAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }

    }
}

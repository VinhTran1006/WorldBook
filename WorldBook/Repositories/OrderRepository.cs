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
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                .Include(o => o.Payment)
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                 .Include(o => o.Payment)
                 .Include(o => o.User)
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
        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task AddOrderDetailsAsync(List<OrderDetail> orderDetails)
        {
            _context.OrderDetails.AddRange(orderDetails);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveCartItemsAsync(int userId, List<int> bookIds)
        {
            var cartItems = await _context.Carts
                .Where(c => c.UserId == userId && bookIds.Contains(c.BookId.Value))
                .ToListAsync();

            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                .OrderByDescending(o => o.OrderDate)
                .Include(o => o.Payment)
                .Include(o => o.User)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                .Include(o => o.Payment)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetAllOrdersPaginatedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                .Include(o => o.Payment)
                .OrderByDescending(o => o.OrderDate);

            var totalCount = await query.CountAsync();
            var orders = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orders, totalCount);
        }

        // METHOD 2 - Thêm vào OrderRepository.cs
        public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetOrdersByUserIdPaginatedAsync(
            int userId, int pageNumber, int pageSize)
        {
            var query = _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                .Include(o => o.Payment)
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate);

            var totalCount = await query.CountAsync();
            var orders = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orders, totalCount);
        }
    }
}

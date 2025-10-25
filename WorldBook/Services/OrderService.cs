using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;

namespace WorldBook.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderViewModel>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return orders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                CustomerName = o.User != null ? o.User.Name : "Unknown",
                Address = o.Address,
                OrderDate = o.OrderDate,
                DeliveredDate = o.DeliveredDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                Discount = o.Discount
            });
        }
        public async Task<OrderDetailViewModel?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
                return null;

            return new OrderDetailViewModel
            {
                OrderId = order.OrderId,
                CustomerName = order.User?.Name,
                Email = order.User?.Email,
                Phone = order.User?.Phone,
                Address = order.Address,
                OrderDate = order.OrderDate,
                DeliveredDate = order.DeliveredDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount ?? 0,
                Discount = order.Discount ?? 0,
                OrderItems = order.OrderDetails.Select(od => new OrderItemViewModel
                {
                    BookName = od.Book?.BookName,
                    Quantity = od.Quantity ?? 0,
                    UnitPrice = od.Price ?? 0
                }).ToList()
            };
        }
        public async Task ApproveNextStatusAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null) return;

            string newStatus = order.Status switch
            {
                "Not Approved" => "Preparing",
                "Preparing" => "Delivering",
                "Delivering" => "Completed",
                _ => order.Status
            };

            // 🔸 Nếu chuyển từ Not Approved -> Preparing: kiểm tra & trừ hàng
            if (order.Status == "Not Approved" && newStatus == "Preparing")
            {
                foreach (var detail in order.OrderDetails)
                {
                    if (detail.Book == null || !detail.BookId.HasValue)
                        continue;

                    var book = detail.Book;
                    int quantityOrdered = detail.Quantity ?? 0;

                    if (book.BookQuantity < quantityOrdered)
                    {
                        throw new Exception($"Sách '{book.BookName}' không đủ tồn kho! (Còn {book.BookQuantity}, cần {quantityOrdered})");
                    }

                    book.BookQuantity -= quantityOrdered;
                }
            }

            await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus);
        }

        public async Task CancelOrderAsync(int orderId)
        {
            await _orderRepository.UpdateOrderStatusAsync(orderId, "Cancelled");
        }
    }
}

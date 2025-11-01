using System.Globalization;
using System.Text;
using Braintree;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;

namespace WorldBook.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private const int DefaultPageSize = 10;

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
                CustomerName = o.User?.Name ?? "Unknown",
                Address = o.Address,
                OrderDate = o.OrderDate,
                DeliveredDate = o.DeliveredDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                Discount = o.Discount,
                Books = o.OrderDetails.Select(od => new OrderBookItem
                {
                    BookName = od.Book?.BookName,
                    ImageUrl = od.Book?.ImageUrl1
                }).ToList(),

                PaymentMethod = o.Payment?.PaymentMethod,
                PaymentStatus = o.Payment?.PaymentStatus
            }).ToList();
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
                    UnitPrice = od.Price ?? 0,
                    ImageUrl = od.Book?.ImageUrl1
                }).ToList(),

                PaymentMethod = order.Payment?.PaymentMethod ?? "N/A",
                PaymentStatus = order.Payment?.PaymentStatus ?? "N/A",
                PaymentAmount = order.Payment?.Amount,
                TransactionId = order.Payment?.TransactionId
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

        public async Task<IEnumerable<OrderViewModel>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

            return orders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                CustomerName = o.User != null ? o.User.Name : "Unknown",
                Address = o.Address,
                OrderDate = o.OrderDate,
                DeliveredDate = o.DeliveredDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                Discount = o.Discount,
                Books = o.OrderDetails.Select(od => new OrderBookItem
                {
                    BookName = od.Book?.BookName,
                    ImageUrl = od.Book?.ImageUrl1
                }).ToList(),

                PaymentMethod = o.Payment?.PaymentMethod,
                PaymentStatus = o.Payment?.PaymentStatus
            }).ToList();
        }

        public async Task<IEnumerable<OrderViewModel>> FilterOrdersAsync(int userId, string? status, string? search)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

            if (!string.IsNullOrEmpty(status))
                orders = orders.Where(o => string.Equals(o.Status, status, StringComparison.OrdinalIgnoreCase));

            // Nếu không có search thì trả về luôn (có .ToList() ép thực thi)
            if (string.IsNullOrWhiteSpace(search))
            {
                return orders.Select(MapToViewModel).ToList();
            }

            // Normalize input: trim, lowercase, remove diacritics
            string Normalize(string input)
            {
                if (string.IsNullOrEmpty(input)) return string.Empty;
                var normalized = input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
                var sb = new StringBuilder();
                foreach (var ch in normalized)
                {
                    var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                    if (uc != UnicodeCategory.NonSpacingMark)
                        sb.Append(ch);
                }
                return sb.ToString().Normalize(NormalizationForm.FormC);
            }

            var query = search.Trim();

            // nếu người dùng nhập số, ưu tiên tìm theo orderId
            if (int.TryParse(query, out var parsedId))
            {
                orders = orders.Where(o => o.OrderId == parsedId);
                return orders.Select(MapToViewModel).ToList();
            }

            // split thành tokens (loại bỏ các khoảng trắng thừa)
            var tokens = query
                .Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => Normalize(t))
                .Where(t => !string.IsNullOrEmpty(t))
                .ToArray();

            if (!tokens.Any())
                return orders.Select(MapToViewModel).ToList();

            // filter: tìm trong mỗi Order -> có bất kỳ OrderDetail mà tên sách sau normalize chứa tất cả token
            orders = orders.Where(o =>
                o.OrderDetails != null &&
                o.OrderDetails.Any(od =>
                {
                    var bookName = od.Book?.BookName ?? string.Empty;
                    var nameNorm = Normalize(bookName);
                    // tất cả token phải xuất hiện trong 1 tên sách (AND)
                    return tokens.All(tok => nameNorm.Contains(tok));
                })
            );

            // cuối cùng ép thực thi, tạo list thật
            return orders.Select(MapToViewModel).ToList();


            // local mapper (reuse existing mapping logic)
            OrderViewModel MapToViewModel(WorldBook.Models.Order o) => new OrderViewModel
            {
                OrderId = o.OrderId,
                CustomerName = o.User?.Name ?? "Unknown",
                Address = o.Address,
                OrderDate = o.OrderDate,
                DeliveredDate = o.DeliveredDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                Discount = o.Discount,
                Books = o.OrderDetails.Select(od => new OrderBookItem
                {
                    BookName = od.Book?.BookName,
                    ImageUrl = od.Book?.ImageUrl1
                }).ToList(),

                PaymentMethod = o.Payment?.PaymentMethod,
                PaymentStatus = o.Payment?.PaymentStatus
            };
        }

        public async Task<IEnumerable<OrderViewModel>> FilterOrdersForAdminAsync(string? status, string? search)
        {
            var orders = await _orderRepository.GetAllOrdersAsync();

            if (!string.IsNullOrEmpty(status))
                orders = orders.Where(o => o.Status == status);

            if (!string.IsNullOrEmpty(search))
            {
                var lowerSearch = search.ToLower();
                orders = orders.Where(o =>
                    (o.User != null && o.User.Name.ToLower().Contains(lowerSearch)) ||
                    o.OrderDetails.Any(od => od.Book.BookName.ToLower().Contains(lowerSearch))
                );
            }

            return orders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                CustomerName = o.User?.Name ?? "Unknown",
                Address = o.Address,
                OrderDate = o.OrderDate,
                DeliveredDate = o.DeliveredDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                Discount = o.Discount,
                Books = o.OrderDetails.Select(od => new OrderBookItem
                {
                    BookName = od.Book?.BookName,
                    ImageUrl = od.Book?.ImageUrl1
                }).ToList(),
                PaymentMethod = o.Payment?.PaymentMethod,
                PaymentStatus = o.Payment?.PaymentStatus
            });
        }

        // ============ SỬA 3 METHODS TRONG ORDERSERVICE ============

        // METHOD 1 - SỬA GetAllOrdersPaginatedAsync
        public async Task<AdminPaginatedOrderViewModel<OrderViewModel>> GetAllOrdersPaginatedAsync(int pageNumber = 1)
        {
            var (orders, totalCount) = await _orderRepository.GetAllOrdersPaginatedAsync(pageNumber, DefaultPageSize);

            var viewModels = orders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                CustomerName = o.User?.Name ?? "Unknown",
                Address = o.Address,
                OrderDate = o.OrderDate,
                DeliveredDate = o.DeliveredDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                Discount = o.Discount,
                Books = o.OrderDetails.Select(od => new OrderBookItem
                {
                    BookName = od.Book?.BookName,
                    ImageUrl = od.Book?.ImageUrl1
                }).ToList(),
                PaymentMethod = o.Payment?.PaymentMethod,
                PaymentStatus = o.Payment?.PaymentStatus
            }).ToList();

            return new AdminPaginatedOrderViewModel<OrderViewModel>(
                viewModels,
                pageNumber,
                DefaultPageSize,
                totalCount
            );
        }

        // METHOD 2 - SỬA FilterOrdersForAdminPaginatedAsync
        public async Task<AdminPaginatedOrderViewModel<OrderViewModel>> FilterOrdersForAdminPaginatedAsync(
            string? status, string? search, int pageNumber = 1)
        {
            var (allOrders, _) = await _orderRepository.GetAllOrdersPaginatedAsync(1, int.MaxValue);

            var filteredOrders = allOrders.AsEnumerable();

            if (!string.IsNullOrEmpty(status))
                filteredOrders = filteredOrders.Where(o => o.Status == status);

            if (!string.IsNullOrEmpty(search))
            {
                var lowerSearch = search.ToLower();
                filteredOrders = filteredOrders.Where(o =>
                    (o.User != null && o.User.Name.ToLower().Contains(lowerSearch)) ||
                    o.OrderDetails.Any(od => od.Book.BookName.ToLower().Contains(lowerSearch))
                );
            }

            var totalCount = filteredOrders.Count();
            var pageSize = DefaultPageSize;
            var paginatedOrders = filteredOrders
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModels = paginatedOrders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                CustomerName = o.User?.Name ?? "Unknown",
                Address = o.Address,
                OrderDate = o.OrderDate,
                DeliveredDate = o.DeliveredDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                Discount = o.Discount,
                Books = o.OrderDetails.Select(od => new OrderBookItem
                {
                    BookName = od.Book?.BookName,
                    ImageUrl = od.Book?.ImageUrl1
                }).ToList(),
                PaymentMethod = o.Payment?.PaymentMethod,
                PaymentStatus = o.Payment?.PaymentStatus
            }).ToList();

            return new AdminPaginatedOrderViewModel<OrderViewModel>(
                viewModels,
                pageNumber,
                pageSize,
                totalCount
            );
        }

        // METHOD 3 - SỬA GetOrdersByUserIdPaginatedAsync
        public async Task<AdminPaginatedOrderViewModel<OrderViewModel>> GetOrdersByUserIdPaginatedAsync(int userId, int pageNumber = 1)
        {
            var (orders, totalCount) = await _orderRepository.GetOrdersByUserIdPaginatedAsync(userId, pageNumber, DefaultPageSize);

            var viewModels = orders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                CustomerName = o.User?.Name ?? "Unknown",
                Address = o.Address,
                OrderDate = o.OrderDate,
                DeliveredDate = o.DeliveredDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                Discount = o.Discount,
                Books = o.OrderDetails.Select(od => new OrderBookItem
                {
                    BookName = od.Book?.BookName,
                    ImageUrl = od.Book?.ImageUrl1
                }).ToList(),
                PaymentMethod = o.Payment?.PaymentMethod,
                PaymentStatus = o.Payment?.PaymentStatus
            }).ToList();

            return new AdminPaginatedOrderViewModel<OrderViewModel>(
                viewModels,
                pageNumber,
                DefaultPageSize,
                totalCount
            );
        }
    }
}

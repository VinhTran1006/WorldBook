using WorldBook.Models;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;

namespace WorldBook.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<IEnumerable<UserFeedbackViewModel>> GetAllFeedbacksAsync()
        {
            var feedbacks = await _feedbackRepository.GetAllAsync();
            return feedbacks.Select(f => new UserFeedbackViewModel
            {
                FeedbackId = f.FeedbackId,
                Comment = f.Comment,
                Star = f.Star,
                BookName = f.Book?.BookName,
                ImageUrl = f.Book?.ImageUrl1,
                CreateAt = f.CreateAt
            });
        }

        public async Task<IEnumerable<UserFeedbackViewModel>> GetBooksForFeedbackAsync(int orderId)
        {
            var books = await _feedbackRepository.GetBooksByOrderIdAsync(orderId);
            return books.Select(b => new UserFeedbackViewModel
            {
                BookId = b.BookId,
                BookName = b.BookName,
                ImageUrl = b.ImageUrl1,
                OrderId = orderId
            });
        }

        public async Task AddFeedbackAsync(UserFeedbackViewModel model)
        {
            var feedback = new Feedback
            {
                BookId = model.BookId,
                UserId = model.UserId,
                OrderId = model.OrderId,
                Star = model.Star,
                Comment = model.Comment,
                CreateAt = DateTime.Now,
                IsActive = true
            };
            await _feedbackRepository.AddAsync(feedback);
            await _feedbackRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserFeedbackViewModel>> GetFeedbacksByUserAsync(int userId)
        {
            var feedbacks = await _feedbackRepository.GetAllAsync();

            return feedbacks
                .Where(f => f.UserId == userId)
                .Select(f => new UserFeedbackViewModel
                {
                    FeedbackId = f.FeedbackId,
                    BookId = f.BookId,
                    BookName = f.Book?.BookName,
                    ImageUrl = f.Book?.ImageUrl1,
                    Star = f.Star,
                    Comment = f.Comment,
                    CreateAt = f.CreateAt
                });
        }


        public async Task<IEnumerable<UserFeedbackViewModel>> GetFeedbacksByOrderAsync(int userId, int orderId)
        {
            var feedbacks = await _feedbackRepository.GetAllAsync();
            return feedbacks
                .Where(f => f.UserId == userId && f.OrderId == orderId)
                .Select(f => new UserFeedbackViewModel
                {
                    FeedbackId = f.FeedbackId,
                    BookId = f.BookId,
                    BookName = f.Book?.BookName,
                    ImageUrl = f.Book?.ImageUrl1,
                    Star = f.Star,
                    Comment = f.Comment,
                    CreateAt = f.CreateAt,
                    OrderId = f.OrderId
                });
        }

        public async Task<bool> HasFeedbackAsync(int orderId, int userId)
        {
            return await _feedbackRepository.HasFeedbackAsync(orderId, userId);
        }

        public async Task<PaginatedFeedbackViewModel> GetFilteredFeedbacksByUserAsync(int userId, string filter, int page = 1, int pageSize = 6)
        {
            IEnumerable<UserFeedbackViewModel> allItems;

            if (filter == "written")
            {
                // Lọc chỉ những feedback đã viết
                var writtenFeedbacks = await _feedbackRepository.GetAllAsync();
                allItems = writtenFeedbacks
                    .Where(f => f.UserId == userId && f.Star.HasValue && f.Star > 0)
                    .Select(f => new UserFeedbackViewModel
                    {
                        FeedbackId = f.FeedbackId,
                        BookId = f.BookId,
                        BookName = f.Book?.BookName,
                        ImageUrl = f.Book?.ImageUrl1,
                        Star = f.Star,
                        Comment = f.Comment,
                        CreateAt = f.CreateAt,
                        OrderId = f.OrderId
                    }).ToList();
            }
            else if (filter == "notwritten")
            {
                // Lọc chỉ những sản phẩm chưa viết feedback
                var userOrders = await _feedbackRepository.GetUserOrdersWithoutFeedbackAsync(userId);
                allItems = userOrders
                    .SelectMany(o => o.OrderDetails)
                    .Select(od => new UserFeedbackViewModel
                    {
                        BookId = od.BookId,
                        BookName = od.Book?.BookName,
                        ImageUrl = od.Book?.ImageUrl1,
                        OrderId = od.OrderId
                    })
                    .ToList();
            }
            else // all - tất cả feedback (đã viết + chưa viết)
            {
                // Lấy tất cả feedback đã viết của user
                var allFeedbacks = await _feedbackRepository.GetAllAsync();
                var userWrittenFeedbacks = allFeedbacks
                    .Where(f => f.UserId == userId)
                    .Select(f => new UserFeedbackViewModel
                    {
                        FeedbackId = f.FeedbackId,
                        BookId = f.BookId,
                        BookName = f.Book?.BookName,
                        ImageUrl = f.Book?.ImageUrl1,
                        Star = f.Star,
                        Comment = f.Comment,
                        CreateAt = f.CreateAt,
                        OrderId = f.OrderId
                    })
                    .ToList();

                // Lấy những order completed chưa có feedback
                var notWrittenOrders = await _feedbackRepository.GetUserOrdersWithoutFeedbackAsync(userId);
                var userNotWrittenFeedbacks = notWrittenOrders
                    .SelectMany(o => o.OrderDetails)
                    .Select(od => new UserFeedbackViewModel
                    {
                        BookId = od.BookId,
                        BookName = od.Book?.BookName,
                        ImageUrl = od.Book?.ImageUrl1,
                        OrderId = od.OrderId
                    })
                    .ToList();

                // Gộp lại tất cả (đã viết + chưa viết)
                allItems = userWrittenFeedbacks.Concat(userNotWrittenFeedbacks);
            }

            // Tính toán phân trang
            int totalItems = allItems.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Đảm bảo page không vượt quá totalPages
            if (page > totalPages) page = totalPages;
            if (page < 1) page = 1;

            // Lấy items cho page hiện tại
            var pagedItems = allItems
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedFeedbackViewModel
            {
                Feedbacks = pagedItems,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentFilter = filter ?? "all",
                TotalItems = totalItems
            };
        }

        public async Task UpdateFeedbackAsync(UserFeedbackViewModel model)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(model.FeedbackId);
            if (feedback != null)
            {
                feedback.Star = model.Star;
                feedback.Comment = model.Comment;

                await _feedbackRepository.UpdateAsync(feedback);
                await _feedbackRepository.SaveChangesAsync();
            }
        }

        public async Task<UserFeedbackViewModel?> GetFeedbackByIdAsync(int feedbackId)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(feedbackId);
            if (feedback == null) return null;

            return new UserFeedbackViewModel
            {
                FeedbackId = feedback.FeedbackId,
                BookId = feedback.BookId,
                BookName = feedback.Book?.BookName,
                ImageUrl = feedback.Book?.ImageUrl1,
                Star = feedback.Star,
                Comment = feedback.Comment,
                CreateAt = feedback.CreateAt,
                OrderId = feedback.OrderId,
                UserId = feedback.UserId
            };
        }

        public async Task<IEnumerable<BookFeedbackViewModel>> GetFeedbacksByBookIdAsync(int bookId)
        {
            var feedbacks = await _feedbackRepository.GetFeedbacksByBookIdAsync(bookId);

            Console.WriteLine($"📊 BookId: {bookId}, Total Feedbacks: {feedbacks.Count()}");

            return feedbacks.Select(f => new BookFeedbackViewModel
            {
                FeedbackId = f.FeedbackId,
                UserName = f.User?.Name ?? "Anonymous",
                Star = f.Star,
                Comment = f.Comment,
                CreateAt = f.CreateAt,
                Reply = f.Reply,
                ReplyAccountName = f.ReplyAccount?.Name,
                ReplyDate = f.ReplyDate,
                OrderId = f.OrderId
            }).ToList();
        }

        public async Task<RatingStatisticsViewModel> GetRatingStatisticsAsync(int bookId)
        {
            var feedbacks = await _feedbackRepository.GetFeedbacksByBookIdAsync(bookId);
            var feedbackList = feedbacks.ToList();

            if (!feedbackList.Any())
            {
                return new RatingStatisticsViewModel
                {
                    TotalReviews = 0,
                    AverageRating = 0,
                    RatingDistribution = new()
            {
                { 5, 0 },
                { 4, 0 },
                { 3, 0 },
                { 2, 0 },
                { 1, 0 }
            }
                };
            }

            // ✅ Tính toán thống kê
            var distribution = new Dictionary<int, int>
    {
        { 5, feedbackList.Count(f => f.Star == 5) },
        { 4, feedbackList.Count(f => f.Star == 4) },
        { 3, feedbackList.Count(f => f.Star == 3) },
        { 2, feedbackList.Count(f => f.Star == 2) },
        { 1, feedbackList.Count(f => f.Star == 1) }
    };

            var averageRating = Math.Round(
        Convert.ToDecimal(feedbackList.Average(f => f.Star ?? 0)),
        1
    );

            return new RatingStatisticsViewModel
            {
                TotalReviews = feedbackList.Count,
                AverageRating = averageRating,
                RatingDistribution = distribution
            };
        }

        // =================== Admin Feedback Section ===================

        public async Task<IEnumerable<FeedbackViewModel>> GetAllFeedbacksForAdminAsync()
        {
            var feedbacks = await _feedbackRepository.GetAllAsync();

            return feedbacks.Select(f => new FeedbackViewModel
            {
                FeedbackId = f.FeedbackId,
                BookName = f.Book?.BookName,
                ImageUrl = f.Book?.ImageUrl1,
                UserName = f.User?.Name,
                Star = f.Star,
                Comment = f.Comment,
                CreateAt = f.CreateAt,
                Reply = f.Reply,
                ReplyDate = f.ReplyDate,
                ReplyAccountName = f.ReplyAccount?.Name
            });
        }


        public async Task<AdminPaginatedFeedbackViewModel> GetPagedAdminFeedbacksAsync(int page = 1, int pageSize = 10, string filter = "all")
        {
            var feedbacks = await _feedbackRepository.GetAllAsync();

            var filtered = feedbacks
                .Where(f => !string.IsNullOrEmpty(f.Comment) || (f.Star.HasValue && f.Star > 0));

            // 🟡 Filter logic
            if (filter == "noreply")
                filtered = filtered.Where(f => string.IsNullOrEmpty(f.Reply));
            else if (filter == "replied")
                filtered = filtered.Where(f => !string.IsNullOrEmpty(f.Reply));

            var list = filtered
                .Select(f => new FeedbackViewModel
                {
                    FeedbackId = f.FeedbackId,
                    BookName = f.Book?.BookName,
                    ImageUrl = f.Book?.ImageUrl1,
                    UserName = f.User?.Name,
                    Star = f.Star,
                    Comment = f.Comment,
                    CreateAt = f.CreateAt,
                    Reply = f.Reply,
                    ReplyDate = f.ReplyDate,
                    ReplyAccountName = f.ReplyAccount?.Name
                })
                .OrderByDescending(f => f.CreateAt)
                .ToList();

            int totalItems = list.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var paged = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new AdminPaginatedFeedbackViewModel
            {
                Feedbacks = paged,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }


        public async Task ReplyToFeedbackAsync(int feedbackId, int adminId, string replyContent)
        {
            await _feedbackRepository.ReplyToFeedbackAsync(feedbackId, adminId, replyContent);
        }

        public async Task<FeedbackViewModel?> AdminGetFeedbackByIdAsync(int feedbackId)
        {
            var feedback = await _feedbackRepository.GetAllAsync();
            var f = feedback.FirstOrDefault(x => x.FeedbackId == feedbackId);

            if (f == null) return null;

            return new FeedbackViewModel
            {
                FeedbackId = f.FeedbackId,
                BookName = f.Book?.BookName,
                ImageUrl = f.Book?.ImageUrl1,
                UserName = f.User?.Name,
                Star = f.Star,
                Comment = f.Comment,
                CreateAt = f.CreateAt,
                Reply = f.Reply,
                ReplyDate = f.ReplyDate,
                ReplyAccountName = f.ReplyAccount?.Name
            };
        }
    }
}

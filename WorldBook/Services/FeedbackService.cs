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
    }
}

using Braintree;
using WorldBook.Models;
using WorldBook.ViewModel;

namespace WorldBook.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<IEnumerable<UserFeedbackViewModel>> GetAllFeedbacksAsync();
        Task<IEnumerable<UserFeedbackViewModel>> GetBooksForFeedbackAsync(int orderId);
        Task AddFeedbackAsync(UserFeedbackViewModel feedback);
        Task UpdateFeedbackAsync(UserFeedbackViewModel model);
        Task<UserFeedbackViewModel?> GetFeedbackByIdAsync(int feedbackId);
        Task<IEnumerable<UserFeedbackViewModel>> GetFeedbacksByUserAsync(int userId);
        Task<bool> HasFeedbackAsync(int orderId, int userId);
        Task<IEnumerable<UserFeedbackViewModel>> GetFeedbacksByOrderAsync(int userId, int orderId);
        Task<PaginatedFeedbackViewModel> GetFilteredFeedbacksByUserAsync(int userId, string filter, int page = 1, int pageSize = 6);
        Task<IEnumerable<BookFeedbackViewModel>> GetFeedbacksByBookIdAsync(int bookId);
        Task<RatingStatisticsViewModel> GetRatingStatisticsAsync(int bookId);

        // ============== Admin Feedback ==============
        Task<IEnumerable<FeedbackViewModel>> GetAllFeedbacksForAdminAsync();
        Task<AdminPaginatedFeedbackViewModel> GetPagedAdminFeedbacksAsync(int page = 1, int pageSize = 10, string filter = "all");
        Task ReplyToFeedbackAsync(int feedbackId, int adminId, string replyContent);
        Task<FeedbackViewModel?> AdminGetFeedbackByIdAsync(int feedbackId);
    }
}

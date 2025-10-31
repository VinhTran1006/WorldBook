using WorldBook.Models;
using WorldBook.ViewModel;

namespace WorldBook.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<IEnumerable<UserFeedbackViewModel>> GetAllFeedbacksAsync();
        Task<IEnumerable<UserFeedbackViewModel>> GetBooksForFeedbackAsync(int orderId);
        Task AddFeedbackAsync(UserFeedbackViewModel feedback);
        Task<IEnumerable<UserFeedbackViewModel>> GetFeedbacksByUserAsync(int userId);
        Task<bool> HasFeedbackAsync(int orderId, int userId);

        Task<IEnumerable<UserFeedbackViewModel>> GetFeedbacksByOrderAsync(int userId, int orderId);
    }
}

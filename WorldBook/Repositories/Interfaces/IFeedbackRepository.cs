using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<Feedback?> GetByIdAsync(int id);
        Task AddAsync(Feedback feedback);
        Task UpdateAsync(Feedback feedback);
        Task<IEnumerable<Book>> GetBooksByOrderIdAsync(int orderId);
        Task SaveChangesAsync();
        Task<IEnumerable<Feedback>> GetFeedbacksByBookIdAsync(int bookId);
        Task<bool> HasFeedbackAsync(int orderId, int userId);
        Task<IEnumerable<Order>> GetUserOrdersWithoutFeedbackAsync(int userId);
        Task ReplyToFeedbackAsync(int feedbackId, int adminId, string replyContent);
    }
}

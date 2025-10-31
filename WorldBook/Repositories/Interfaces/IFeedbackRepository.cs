using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<Feedback?> GetByIdAsync(int id);
        Task AddAsync(Feedback feedback);
        Task<IEnumerable<Book>> GetBooksByOrderIdAsync(int orderId);
        Task SaveChangesAsync();
        Task<bool> HasFeedbackAsync(int orderId, int userId);
    }
}

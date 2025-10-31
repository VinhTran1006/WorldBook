using Microsoft.EntityFrameworkCore;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;

namespace WorldBook.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly WorldBookDbContext _context;

        public FeedbackRepository(WorldBookDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feedback>> GetAllAsync()
        {
            return await _context.Feedbacks.Include(f => f.Book).Include(f => f.User).ToListAsync();
        }

        public async Task<Feedback?> GetByIdAsync(int id)
        {
            return await _context.Feedbacks.FindAsync(id);
        }

        public async Task AddAsync(Feedback feedback)
        {
            await _context.Feedbacks.AddAsync(feedback);
        }

        public async Task<IEnumerable<Book>> GetBooksByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Select(od => od.Book)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasFeedbackAsync(int orderId, int userId)
        {
            return await _context.Feedbacks
        .AnyAsync(f => f.OrderId == orderId && f.UserId == userId);
        }
    }
}

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

        public async Task UpdateAsync(Feedback feedback)
        {
            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetUserOrdersWithoutFeedbackAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId &&
                            o.Status == "Completed" &&
                            !_context.Feedbacks.Any(f => f.OrderId == o.OrderId && f.UserId == userId))
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .ToListAsync();
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByBookIdAsync(int bookId)
        {
            return await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.ReplyAccount)
                .Where(f => f.BookId == bookId && f.Star.HasValue && f.IsActive == true)
                .OrderByDescending(f => f.CreateAt)
                .ToListAsync();
        }

        // ============== Admin Feedback ==============
        public async Task ReplyToFeedbackAsync(int feedbackId, int adminId, string replyContent)
        {
            var feedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);
            if (feedback == null) return;

            feedback.Reply = replyContent;
            feedback.ReplyAccountId = adminId;
            feedback.ReplyDate = DateTime.Now;

            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();
        }

    }
}

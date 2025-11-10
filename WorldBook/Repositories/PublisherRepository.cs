using Microsoft.EntityFrameworkCore;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;

namespace WorldBook.Repositories
{
    public class PublisherRepository : IPublisherRepository
    {
        private readonly WorldBookDbContext _context;

        public async Task<Publisher?> GetByNameAsync(string name)
        {
            return await _context.Publishers
                .FirstOrDefaultAsync(p => p.PublisherName.ToLower() == name.ToLower());
        }

        public async Task<Publisher> AddAsync(Publisher publisher)
        {
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();
            return publisher;
        }

        public PublisherRepository(WorldBookDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Publisher>> GetAllAsync()
        {
            return await _context.Publishers
                .Where(p => p.IsActive == true)
                .OrderByDescending(p => p.PublisherId)
                .ToListAsync();
        }

        public async Task<Publisher?> GetByIdAsync(int id)
        {
            return await _context.Publishers
                .FirstOrDefaultAsync(p => p.PublisherId == id && p.IsActive == true);
        }

        public async Task<Publisher> CreateAsync(Publisher publisher)
        {
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();
            return publisher;
        }

        public async Task<Publisher> UpdateAsync(Publisher publisher)
        {
            _context.Entry(publisher).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return publisher;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
                return false;

            publisher.IsActive = false; // Soft delete
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Publishers.AnyAsync(p => p.PublisherId == id && p.IsActive == true);
        }
    }
}
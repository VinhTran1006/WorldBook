using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface IPublisherRepository
    {
        Task<Publisher?> GetByNameAsync(string name);
        Task<Publisher> AddAsync(Publisher publisher);
        Task<IEnumerable<Publisher>> GetAllAsync();
        Task<Publisher?> GetByIdAsync(int id);
        Task<Publisher> CreateAsync(Publisher publisher);
        Task<Publisher> UpdateAsync(Publisher publisher);
        Task<bool> SoftDeleteAsync(int id); // Soft delete
        Task<bool> ExistsAsync(int id);
    }
}
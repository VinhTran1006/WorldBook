using WorldBook.Models;

namespace WorldBook.Services.Interfaces
{
    public interface IPublisherService
    {
        Task<IEnumerable<Publisher>> GetAllPublishersAsync();
        Task<Publisher?> GetPublisherByIdAsync(int id);
        Task<Publisher> CreatePublisherAsync(Publisher publisher);
        Task<Publisher> UpdatePublisherAsync(Publisher publisher);
        Task<bool> DeletePublisherAsync(int id); // Soft delete
        Task<bool> PublisherExistsAsync(int id);
    }
}
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;

namespace WorldBook.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IPublisherRepository _publisherRepository;

        public PublisherService(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }

        public async Task<IEnumerable<Publisher>> GetAllPublishersAsync()
        {
            return await _publisherRepository.GetAllAsync();
        }

        public async Task<Publisher?> GetPublisherByIdAsync(int id)
        {
            return await _publisherRepository.GetByIdAsync(id);
        }

        public async Task<Publisher> CreatePublisherAsync(Publisher publisher)
        {
            publisher.IsActive = true; // Mặc định active khi tạo mới
            return await _publisherRepository.CreateAsync(publisher);
        }

        public async Task<Publisher> UpdatePublisherAsync(Publisher publisher)
        {
            return await _publisherRepository.UpdateAsync(publisher);
        }

        public async Task<bool> DeletePublisherAsync(int id)
        {
            return await _publisherRepository.SoftDeleteAsync(id);
        }

        public async Task<bool> PublisherExistsAsync(int id)
        {
            return await _publisherRepository.ExistsAsync(id);
        }
    }
}
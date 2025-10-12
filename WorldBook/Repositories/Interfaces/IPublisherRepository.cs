using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface IPublisherRepository
    {
        Task<Publisher?> GetByNameAsync(string name);
        Task<Publisher> AddAsync(Publisher publisher);
    }

}

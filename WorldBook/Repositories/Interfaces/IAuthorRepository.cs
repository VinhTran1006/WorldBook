using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface IAuthorRepository
    {
        Task<Author?> GetByNameAsync(string name);
        Task<Author> AddAsync(Author author);
    }
}

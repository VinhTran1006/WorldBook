using WorldBook.Models;

namespace WorldBook.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<Author?> GetByNameAsync(string name);

        Task<Author?> GetByIdAsync(int authorId);

        Task<IEnumerable<Author>> GetAuthorsAsync();
        Task<Author> AddAsync(Author author);

        Task UpdateAsync(Author author);

        Task DeleteAsync(int authorId);
    }
}

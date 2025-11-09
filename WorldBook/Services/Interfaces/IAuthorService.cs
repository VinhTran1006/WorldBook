using WorldBook.Models;

namespace WorldBook.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<IEnumerable<Author>> GetAuthors();

        Task<Author> AddAsync(Author author);
        Task<Author> GetAuthorByID(int id);
        Task UpdateAuthor(Author author);
        Task DeleteAuthor(int id);
    }
}

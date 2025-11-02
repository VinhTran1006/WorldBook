using WorldBook.Models;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;

namespace WorldBook.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorService(IAuthorRepository authorRepository) => _authorRepository = authorRepository;

        public Task<Author> AddAsync(Author author)
        {
           return _authorRepository.AddAsync(author);
        }

        public Task DeleteAsync(int authorId)
        {
           return _authorRepository.DeleteAsync(authorId);
        }

        public Task<IEnumerable<Author>> GetAuthorsAsync()
        {
            return _authorRepository.GetAuthorsAsync();
        }

        public Task<Author?> GetByIdAsync(int authorId)
        {
           return _authorRepository.GetByIdAsync(authorId);
        }

        public Task<Author?> GetByNameAsync(string name)
        {
           return _authorRepository.GetByNameAsync(name);
        }

        public Task UpdateAsync(Author author)
        {
           return _authorRepository.UpdateAsync(author);
        }
    }
}

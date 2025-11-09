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

        public async Task DeleteAuthor(int id)
        {
           await _authorRepository.DeleteAuthor(id);
        }

        public async Task<Author> GetAuthorByID(int id)
        {
           return await _authorRepository.GetAuthorByID(id);
        }

        public async Task<IEnumerable<Author>> GetAuthors()
        {
           return await _authorRepository.GetAuthors();
        }

        public async Task UpdateAuthor(Author author)
        {
          await _authorRepository.UpdateAuthor(author);
        }
    }
}

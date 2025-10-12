using WorldBook.Repositories.Interfaces;

namespace WorldBook.Services
{
    public class AuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorService(IAuthorRepository authorRepository) => _authorRepository = authorRepository;
    }
}

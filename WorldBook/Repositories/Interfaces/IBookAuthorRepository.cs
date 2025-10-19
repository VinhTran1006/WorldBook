using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface IBookAuthorRepository
    {
        Task AddBookAuthorAsync(BookAuthor bookAuthor);
        Task ClearByBookIdAsync(int bookId);
    }
}

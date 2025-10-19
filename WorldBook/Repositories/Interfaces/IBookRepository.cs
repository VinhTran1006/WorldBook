using WorldBook.Models;
using WorldBook.ViewModel;

namespace WorldBook.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);

        Task<bool> CheckBookExist(string bookName, IEnumerable<string> authorNames);
    }
}

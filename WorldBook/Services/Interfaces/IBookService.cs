using WorldBook.Models;
using WorldBook.ViewModel;

namespace WorldBook.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDetailViewModel>> GetAllBooksAsync();
        Task<BookDetailViewModel> GetBookByIdAsync(int id);
        Task AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
    }
}

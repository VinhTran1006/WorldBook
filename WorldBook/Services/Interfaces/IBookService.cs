using WorldBook.Models;
using WorldBook.ViewModel;

namespace WorldBook.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDetailViewModel>> GetAllBooksAsync();
        Task<BookDetailViewModel> GetBookByIdAsync(int id);
        Task AddBookAsync(BookCreateEditViewModel book);
        Task UpdateBookAsync(BookCreateEditViewModel book);
        Task DeleteBookAsync(int id);
    }
}

using WorldBook.Models;
using WorldBook.ViewModel;

namespace WorldBook.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDetailViewModel>> GetAllBooksAsync();
        Task<BookDetailViewModel> GetBookByIdAsync(int id);
         Task<BookEditViewModel> GetBookByIdEditdAsync(int id);
        Task AddBookAsync(BookCreateEditViewModel book);
        Task UpdateBookAsync(BookEditViewModel book);
        Task DeleteBookAsync(int id);
    }
}

using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface IBookCategoryRepository
    {
        Task AddBookCategoryAsync(BookCategory bookCategory);
        Task ClearByBookIdAsync(int bookId);
    }
}

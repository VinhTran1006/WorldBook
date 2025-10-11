using WorldBook.Models;
using WorldBook.Repositories;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;

namespace WorldBook.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        public async Task AddBookAsync(Book book)
        {
            await _bookRepository.AddBookAsync(book);
        }

        public async Task DeleteBookAsync(int id)
        {
            await _bookRepository.DeleteBookAsync(id);
        }

        public async Task<IEnumerable<BookDetailViewModel>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllBooksAsync();

            // map từng Book sang ViewModel
            return books.Select(book => new BookDetailViewModel
            {
                BookId = book.BookId,
                BookName = book.BookName,
                BookDescription = book.BookDescription,
                BookPrice = book.BookPrice,
                BookQuantity = book.BookQuantity,
                ImageUrl1 = book.ImageUrl1,
                ImageUrl2 = book.ImageUrl2,
                ImageUrl3 = book.ImageUrl3,
                ImageUrl4 = book.ImageUrl4,
                AddedAt = book.AddedAt,
                PublisherName = book.Publisher?.PublisherName,
                SupplierName = book.Supplier?.SupplierName,
                AuthorNames = book.BookAuthors
                    .Where(ba => ba.Author != null)
                    .Select(ba => ba.Author.AuthorName)
                    .ToList(),
                Categories = book.BookCategories
                    .Where(bc => bc.Category != null)
                    .Select(bc => bc.Category.CategoryName)
                    .ToList()
            });
        }

        public async Task<BookDetailViewModel> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null) return null;

            return new BookDetailViewModel
            {
                BookId = book.BookId,
                BookName = book.BookName,
                BookDescription = book.BookDescription,
                BookPrice = book.BookPrice,
                BookQuantity = book.BookQuantity,
                ImageUrl1 = book.ImageUrl1,
                AddedAt = book.AddedAt,
                PublisherName = book.Publisher?.PublisherName,
                SupplierName = book.Supplier?.SupplierName,
                AuthorNames = book.BookAuthors.Select(ba => ba.Author.AuthorName).ToList(),
                 Categories = book.BookCategories.Select(ba => ba.Category.CategoryName).ToList()

            };
        }

        public async Task UpdateBookAsync(Book book)
        {
            await _bookRepository.UpdateBookAsync(book);
        }
    }
}

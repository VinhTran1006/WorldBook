using Microsoft.EntityFrameworkCore;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;
using WorldBook.ViewModel;

namespace WorldBook.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly WorldBookDbContext _db;
        public BookRepository(WorldBookDbContext db) => _db = db;
        public async Task AddBookAsync(Book book)
        {
            _db.Books.Add(book);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
           Book? b = await _db.Books.FindAsync(id);
            b!.IsActive = false;
          await  _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _db.Books
            .Include(b => b.Publisher)
            .Include(b => b.Supplier)
            .Include(b => b.BookCategories)
            .ThenInclude(bc => bc.Category)
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Where(b => b.IsActive)
            .ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _db.Books
           .Include(b => b.Publisher)             
           .Include(b => b.Supplier)
           .Include(b => b.BookCategories)
            .ThenInclude(bc => bc.Category)
           .Include(b => b.BookAuthors)
           .ThenInclude(ba => ba.Author)
           .FirstOrDefaultAsync(b => b.BookId == id && b.IsActive);
        }

        public async Task UpdateBookAsync(Book book)
        {
           _db.Books.Update(book);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> CheckBookExist(string bookName, IEnumerable<string> authorNames)
        {
            return await _db.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .AnyAsync(b =>
                    b.BookName == bookName &&
                    b.IsActive &&
                    b.BookAuthors.Any(ba => authorNames.Contains(ba.Author.AuthorName)));
        }

    }
}

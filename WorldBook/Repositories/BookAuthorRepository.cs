using System;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;

namespace WorldBook.Repositories
{
    public class BookAuthorRepository : IBookAuthorRepository
    {
        private readonly WorldBookDbContext _db;

        public BookAuthorRepository(WorldBookDbContext db)
        {
            _db = db;
        }

        public async Task AddBookAuthorAsync(BookAuthor bookAuthor)
        {
            _db.BookAuthors.Add(bookAuthor);
            await _db.SaveChangesAsync();
        }
    }
}

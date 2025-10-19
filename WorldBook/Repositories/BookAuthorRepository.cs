using Microsoft.EntityFrameworkCore;
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

        public async Task ClearByBookIdAsync(int bookId)
        {
            var bookAuthors = _db.BookAuthors.Where(ba => ba.BookId == bookId);
            _db.BookAuthors.RemoveRange(bookAuthors);
            await _db.SaveChangesAsync();
        }

    }
}

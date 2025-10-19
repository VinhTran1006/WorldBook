using Microsoft.EntityFrameworkCore;
using System;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;

namespace WorldBook.Repositories
{
    public class BookCategoryRepository : IBookCategoryRepository
    {
        private readonly WorldBookDbContext _db;

        public BookCategoryRepository(WorldBookDbContext db)
        {
            _db = db;
        }

        // ✅ Thêm quan hệ Book - Category vào bảng phụ
        public async Task AddBookCategoryAsync(BookCategory bookCategory)
        {
            _db.BookCategories.Add(bookCategory);
            await _db.SaveChangesAsync();
        }

        public async Task ClearByBookIdAsync(int bookId)
        {
            var bookCategories = _db.BookCategories.Where(bc => bc.BookId == bookId);
            _db.BookCategories.RemoveRange(bookCategories);
            await _db.SaveChangesAsync();
        }

    }
}

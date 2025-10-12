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
    }
}

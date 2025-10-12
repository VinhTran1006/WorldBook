using Microsoft.EntityFrameworkCore;
using System;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;

namespace WorldBook.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly WorldBookDbContext _db;

        public CategoryRepository(WorldBookDbContext db)
        {
            _db = db;
        }

        // ✅ Lấy Category theo tên (không phân biệt hoa thường)
        public async Task<Category?> GetByNameAsync(string categoryName)
        {
            return await _db.Categories
                .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == categoryName.ToLower());
        }

        // ✅ Tạo mới Category
        public async Task<Category> AddAsync(Category category)
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return category;
        }
    }
}

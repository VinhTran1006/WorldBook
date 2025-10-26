using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.InteropServices;
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


        public async Task<Category?> GetByNameAsync(string categoryName)
        {
            return await _db.Categories
                .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == categoryName.ToLower());
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
           return await _db.Categories.Where(c => c.IsActive == true).FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<Category> AddAsync(Category category)
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return category;
        }

        public async Task UpdateCategoryAsync(Category category)
        {
             _db.Categories.Update(category);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
           var cate = await _db.Categories.FindAsync(categoryId);
            if (cate != null)
            {
               cate.IsActive = false;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
           return await _db.Categories.Where(c => c.IsActive == true).ToListAsync();
        }

 
    }
}

using WorldBook.Models;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;

namespace WorldBook.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category> AddAsync(Category category)
        {
           return await _categoryRepository.AddAsync(category);
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            await _categoryRepository.DeleteCategoryAsync(categoryId);
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<Category?> GetByNameAsync(string categoryName)
        {
            return await _categoryRepository.GetByNameAsync(categoryName);
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
           return await _categoryRepository.GetCategoriesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
           await _categoryRepository.UpdateCategoryAsync(category);
        }
    }
}

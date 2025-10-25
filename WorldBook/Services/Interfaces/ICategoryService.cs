using WorldBook.Models;

namespace WorldBook.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<Category?> GetByNameAsync(string categoryName);
        Task<Category> AddAsync(Category category);

        Task UpdateCategoryAsync(Category category);

        Task DeleteCategoryAsync(int categoryId);

        Task<IEnumerable<Category>> GetCategoriesAsync();

        Task<Category?> GetByIdAsync(int id);
    }
}

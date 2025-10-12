using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByNameAsync(string categoryName);
        Task<Category> AddAsync(Category category);
    }
}

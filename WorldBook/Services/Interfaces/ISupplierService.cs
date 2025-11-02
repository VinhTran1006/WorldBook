using WorldBook.Models;

namespace WorldBook.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<Supplier?> GetByNameAsync(string name);
        Task<Supplier> AddAsync(Supplier supplier);

        Task UpdateAsync(Supplier supplier);

        Task<Supplier?> GetByIdAsync(int id);

        Task<List<Supplier>> GetAllAsync();

        Task DeleteAsync(int id);
    }
}

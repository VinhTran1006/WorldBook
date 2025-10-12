using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface ISupplierRepository
    {
        Task<Supplier?> GetByNameAsync(string name);
        Task<Supplier> AddAsync(Supplier supplier);
    }

}

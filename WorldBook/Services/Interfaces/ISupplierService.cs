using WorldBook.Models;

namespace WorldBook.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<Supplier> AddAsync(Supplier supplier);

        Task<IEnumerable<Supplier>> GetSuppliers();

        Task<Supplier> GetSupplierByID(int id);

        Task UpdateSupplier(Supplier sup);

        Task DeleteSupplierAsync(int id);
    }
}

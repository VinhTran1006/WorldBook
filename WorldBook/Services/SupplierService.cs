using WorldBook.Models;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;

namespace WorldBook.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }
        public async Task<Supplier> AddAsync(Supplier supplier)
        {
           return await _supplierRepository.AddAsync(supplier);
        }

        public async Task DeleteSupplierAsync(int id)
        {
            await _supplierRepository.DeleteSupplierAsync(id);
        }

        public async Task<Supplier> GetSupplierByID(int id)
        {
           return await _supplierRepository.GetSupplierByID(id);
        }

        public async Task<IEnumerable<Supplier>> GetSuppliers()
        {
            return await _supplierRepository.GetSuppliers();
        }

        public async Task UpdateSupplier(Supplier sup)
        {
           await _supplierRepository.UpdateSupplier(sup);
        }
    }
}

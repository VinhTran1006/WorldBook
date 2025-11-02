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
        public Task<Supplier> AddAsync(Supplier supplier)
        {
         
           return _supplierRepository.AddAsync(supplier);
        }

        public async Task DeleteAsync(int id)
        {
           await _supplierRepository.DeleteAsync(id);
        }

        public async Task<List<Supplier>> GetAllAsync()
        {
           return await _supplierRepository.GetAllAsync();
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
           return await _supplierRepository.GetByIdAsync(id);
        }

        public async Task<Supplier?> GetByNameAsync(string name)
        {
           return await _supplierRepository.GetByNameAsync(name);
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            await _supplierRepository.UpdateAsync(supplier);
        }
    }
}

using WorldBook.Models;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;

namespace WorldBook.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherService(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<IEnumerable<Voucher>> GetAllVouchersAsync()
        {
            return await _voucherRepository.GetAllAsync();
        }

        public async Task<Voucher?> GetVoucherByIdAsync(int id)
        {
            return await _voucherRepository.GetByIdAsync(id);
        }

        public async Task<Voucher> CreateVoucherAsync(Voucher voucher)
        {
            return await _voucherRepository.CreateAsync(voucher);
        }

        public async Task<Voucher> UpdateVoucherAsync(Voucher voucher)
        {
            return await _voucherRepository.UpdateAsync(voucher);
        }

        public async Task<bool> DeleteVoucherAsync(int id)
        {
            return await _voucherRepository.DeleteAsync(id);
        }

        public async Task<bool> VoucherExistsAsync(int id)
        {
            return await _voucherRepository.ExistsAsync(id);
        }

        public async Task<Voucher?> GetVoucherByCodeAsync(string voucherCode)
        {
            return await _voucherRepository.GetByVoucherCodeAsync(voucherCode);
        }
    }
}
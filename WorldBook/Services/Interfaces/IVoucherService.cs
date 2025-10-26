using WorldBook.Models;

namespace WorldBook.Services.Interfaces
{
    public interface IVoucherService
    {
        Task<IEnumerable<Voucher>> GetAllVouchersAsync();
        Task<Voucher?> GetVoucherByIdAsync(int id);
        Task<Voucher> CreateVoucherAsync(Voucher voucher);
        Task<Voucher> UpdateVoucherAsync(Voucher voucher);
        Task<bool> DeleteVoucherAsync(int id);
        Task<bool> VoucherExistsAsync(int id);
        Task<Voucher?> GetVoucherByCodeAsync(string voucherCode);
        
    }
}
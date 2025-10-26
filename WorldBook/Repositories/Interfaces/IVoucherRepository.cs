using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface IVoucherRepository
    {
        Task<IEnumerable<Voucher>> GetAllAsync();
        Task<Voucher?> GetByIdAsync(int id);
        Task<Voucher> CreateAsync(Voucher voucher);
        Task<Voucher> UpdateAsync(Voucher voucher);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<Voucher?> GetByVoucherCodeAsync(string voucherCode);
        Task IncrementUsageCountAsync(int voucherId);
        Task<IEnumerable<Voucher>> GetActiveVouchersAsync();
    }
}
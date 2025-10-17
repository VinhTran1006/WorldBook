using Microsoft.EntityFrameworkCore;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;

namespace WorldBook.Repositories
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly WorldBookDbContext _context;

        public VoucherRepository(WorldBookDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Voucher>> GetAllAsync()
        {
            return await _context.Vouchers
                .OrderByDescending(v => v.VoucherId)
                .ToListAsync();
        }

        public async Task<Voucher?> GetByIdAsync(int id)
        {
            return await _context.Vouchers
                .FirstOrDefaultAsync(v => v.VoucherId == id);
        }

        public async Task<Voucher> CreateAsync(Voucher voucher)
        {
            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();
            return voucher;
        }

        public async Task<Voucher> UpdateAsync(Voucher voucher)
        {
            _context.Entry(voucher).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return voucher;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
                return false;

            _context.Vouchers.Remove(voucher);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Vouchers.AnyAsync(v => v.VoucherId == id);
        }

        public async Task<Voucher?> GetByVoucherCodeAsync(string voucherCode)
        {
            return await _context.Vouchers
                .FirstOrDefaultAsync(v => v.VoucherCode == voucherCode);
        }
    }
}
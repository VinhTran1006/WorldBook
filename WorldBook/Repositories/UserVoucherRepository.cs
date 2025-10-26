// WorldBook/Repositories/UserVoucherRepository.cs
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;

namespace WorldBook.Repositories
{
    public class UserVoucherRepository : IUserVoucherRepository
    {
        private readonly WorldBookDbContext _context;

        public UserVoucherRepository(WorldBookDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Kiểm tra user đã dùng voucher này chưa
        /// </summary>
        public async Task<bool> HasUserUsedVoucherAsync(int userId, int voucherId)
        {
            return await _context.UserVouchers
                .AnyAsync(uv => uv.UserId == userId && uv.VoucherId == voucherId);
        }

        /// <summary>
        /// Thêm record tracking việc user sử dụng voucher
        /// </summary>
        public async Task AddUserVoucherAsync(UserVoucher userVoucher)
        {
            await _context.UserVouchers.AddAsync(userVoucher);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Lấy danh sách voucher đã dùng của user
        /// </summary>
        public async Task<IEnumerable<UserVoucher>> GetUserVouchersAsync(int userId)
        {
            return await _context.UserVouchers
                .Include(uv => uv.Voucher)
                .Include(uv => uv.Order)
                .Where(uv => uv.UserId == userId)
                .OrderByDescending(uv => uv.UsedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy UserVoucher theo OrderId
        /// </summary>
        public async Task<UserVoucher?> GetByOrderIdAsync(int orderId)
        {
            return await _context.UserVouchers
                .Include(uv => uv.Voucher)
                .FirstOrDefaultAsync(uv => uv.OrderId == orderId);
        }
    }
}
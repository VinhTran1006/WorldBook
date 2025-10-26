// WorldBook/Repositories/Interfaces/IUserVoucherRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface IUserVoucherRepository
    {
        /// <summary>
        /// Kiểm tra xem user đã sử dụng voucher này chưa
        /// </summary>
        Task<bool> HasUserUsedVoucherAsync(int userId, int voucherId);

        /// <summary>
        /// Thêm record UserVoucher khi user áp dụng voucher vào order
        /// </summary>
        Task AddUserVoucherAsync(UserVoucher userVoucher);

        /// <summary>
        /// Lấy danh sách voucher mà user đã sử dụng
        /// </summary>
        Task<IEnumerable<UserVoucher>> GetUserVouchersAsync(int userId);

        /// <summary>
        /// Lấy UserVoucher theo OrderId
        /// </summary>
        Task<UserVoucher?> GetByOrderIdAsync(int orderId);
    }
}
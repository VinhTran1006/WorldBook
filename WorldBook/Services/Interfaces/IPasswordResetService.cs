namespace WorldBook.Services.Interfaces
{
    public interface IPasswordResetService
    {
        /// <summary>
        /// Gửi email reset password đến email người dùng
        /// </summary>
        Task<bool> SendPasswordResetEmailAsync(string email);

        /// <summary>
        /// Kiểm tra token có hợp lệ không
        /// </summary>
        Task<bool> ValidateResetTokenAsync(string token);

        /// <summary>
        /// Đặt lại mật khẩu với token
        /// </summary>
        Task<bool> ResetPasswordAsync(string token, string newPassword);

        /// <summary>
        /// Tạo token ngẫu nhiên
        /// </summary>
        string GenerateResetToken();
    }
}

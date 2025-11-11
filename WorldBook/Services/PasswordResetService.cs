using System.Collections.Concurrent;
using System.Security.Cryptography;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;

namespace WorldBook.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        // Lưu token trong memory: Key = token, Value = (email, hết hạn)
        private static readonly ConcurrentDictionary<string, (string Email, DateTime ExpiryTime)> _resetTokens
            = new ConcurrentDictionary<string, (string, DateTime)>();

        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IAuthService _authService;

        public PasswordResetService(
            IUserRepository userRepository,
            IEmailService emailService,
            IAuthService authService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _authService = authService;

            // Chạy background task cleanup token mỗi 5 phút
            _ = CleanupExpiredTokensAsync();
        }

        public string GenerateResetToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "");
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email)
        {
            try
            {
                // Kiểm tra email có tồn tại không
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                    return false;

                // Tạo token mới
                var token = GenerateResetToken();
                var expiryTime = DateTime.Now.AddMinutes(15);

                // Lưu token vào memory
                _resetTokens.AddOrUpdate(token, (email, expiryTime), (k, v) => (email, expiryTime));

                // Tạo URL reset password
                var resetUrl = $"https://localhost:7044/Logins/ResetPassword?token={Uri.EscapeDataString(token)}";

                var subject = "Đặt lại mật khẩu WorldBook";
                var body = $@"
                    <html>
                        <head>
                            <meta charset='utf-8'/>
                            <style>
                                body {{ font-family: Arial, sans-serif; background-color: #f5f5f5; }}
                                .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                                .header {{ color: #2c3e50; border-bottom: 3px solid #e74c3c; padding-bottom: 20px; }}
                                .message {{ color: #34495e; font-size: 16px; line-height: 1.6; margin: 20px 0; }}
                                .warning-box {{ background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; }}
                                .warning-title {{ color: #856404; font-weight: bold; margin-bottom: 10px; }}
                                .warning-text {{ color: #856404; }}
                                .reset-button {{ display: inline-block; background-color: #e74c3c; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                                .reset-button:hover {{ background-color: #c0392b; }}
                                .code-box {{ background-color: #ecf0f1; padding: 10px; border-radius: 5px; font-family: monospace; word-break: break-all; font-size: 12px; }}
                                .footer {{ color: #7f8c8d; font-size: 14px; margin-top: 30px; padding-top: 20px; border-top: 1px solid #bdc3c7; }}
                                .expiry-notice {{ color: #e74c3c; font-weight: bold; margin-top: 15px; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <h1>Yêu cầu đặt lại mật khẩu</h1>
                                </div>
                                
                                <div class='message'>
                                    Xin chào <strong>{user.Name ?? user.Username}</strong>,
                                </div>

                                <div class='message'>
                                    Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. 
                                    Nếu đây là yêu cầu của bạn, vui lòng nhấp vào nút dưới đây để tiếp tục.
                                </div>

                                <center>
                                    <a href='{resetUrl}' class='reset-button'>Đặt lại mật khẩu</a>
                                </center>

                                <div class='warning-box'>
                                    <div class='warning-title'>Lưu ý bảo mật:</div>
                                    <ul class='warning-text' style='margin: 0; padding-left: 20px;'>
                                        <li>Link này sẽ hết hạn trong <strong>15 phút</strong></li>
                                        <li>Đừng chia sẻ link này với bất kỳ ai</li>
                                        <li>Nếu bạn không yêu cầu đặt lại mật khẩu, hãy bỏ qua email này</li>
                                    </ul>
                                </div>

                                <div class='message'>
                                    <strong>Hoặc sao chép link sau vào trình duyệt:</strong>
                                </div>
                                <div class='code-box'>{resetUrl}</div>

                                <div class='expiry-notice'>
                                    ⏰ Link này sẽ hết hạn vào lúc {DateTime.Now.AddMinutes(15):dd/MM/yyyy HH:mm:ss}
                                </div>

                                <div class='footer'>
                                    <p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.</p>
                                    <p>© 2024 WorldBook. Tất cả quyền được bảo lưu.</p>
                                </div>
                            </div>
                        </body>
                    </html>
                ";

                // Gửi email
                return await _emailService.SendEmailForgetPasswordAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi email reset: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ValidateResetTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return false;

                // Kiểm tra token có tồn tại và chưa hết hạn
                if (_resetTokens.TryGetValue(token, out var tokenData))
                {
                    return tokenData.ExpiryTime > DateTime.Now;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            try
            {
                // Validate token
                if (!await ValidateResetTokenAsync(token))
                    return false;

                // Lấy email từ token
                if (!_resetTokens.TryGetValue(token, out var tokenData))
                    return false;

                // Lấy user theo email
                var user = await _userRepository.GetByEmailAsync(tokenData.Email);
                if (user == null)
                    return false;

                // Hash mật khẩu mới
                var hashedPassword = _authService.HashPassword(newPassword);
                user.Password = hashedPassword;

                // Update user trong database
                await _userRepository.UpdateAsync(user);

                // Xóa token (đã sử dụng)
                _resetTokens.TryRemove(token, out _);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đặt lại mật khẩu: {ex.Message}");
                return false;
            }
        }

        // Background task: cleanup token hết hạn mỗi 5 phút
        private async Task CleanupExpiredTokensAsync()
        {
            while (true)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(5));

                    var expiredTokens = _resetTokens
                        .Where(x => x.Value.ExpiryTime <= DateTime.Now)
                        .Select(x => x.Key)
                        .ToList();

                    foreach (var token in expiredTokens)
                    {
                        _resetTokens.TryRemove(token, out _);
                    }

                    if (expiredTokens.Count > 0)
                        Console.WriteLine($"[PasswordReset] Cleanup: Removed {expiredTokens.Count} expired tokens");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PasswordReset] Cleanup error: {ex.Message}");
                }
            }
        }
    }
}

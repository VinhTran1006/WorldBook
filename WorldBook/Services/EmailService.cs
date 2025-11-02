using System.Net.Mail;
using System.Net;
using WorldBook.Services.Interfaces;

namespace WorldBook.Services
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _emailUsername;
        private readonly string _emailPassword;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _emailUsername = configuration["EmailSettings:Username"] ?? "hotahihi123@gmail.com";
            _emailPassword = configuration["EmailSettings:Password"] ?? "giyr mzbv suve haxl";
        }

        public async Task<bool> SendWelcomeEmailAsync(string email, string username, string password, string fullName)
        {
            try
            {
                var subject = "Chào mừng bạn đến với WorldBook! 📚";

                var body = $@"
                    <html>
                        <head>
                            <meta charset='utf-8'/>
                            <style>
                                body {{ font-family: Arial, sans-serif; background-color: #f5f5f5; }}
                                .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                                .header {{ color: #2c3e50; border-bottom: 3px solid #3498db; padding-bottom: 20px; }}
                                .welcome-text {{ color: #34495e; font-size: 16px; line-height: 1.6; margin: 20px 0; }}
                                .credentials {{ background-color: #ecf0f1; padding: 15px; border-left: 4px solid #3498db; margin: 20px 0; }}
                                .credentials-label {{ color: #2c3e50; font-weight: bold; }}
                                .credentials-value {{ color: #e74c3c; font-family: monospace; font-size: 14px; word-break: break-all; }}
                                .info-box {{ background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; }}
                                .info-box-title {{ color: #856404; font-weight: bold; margin-bottom: 10px; }}
                                .info-list {{ margin-left: 20px; color: #856404; }}
                                .info-list li {{ margin: 8px 0; }}
                                .footer {{ color: #7f8c8d; font-size: 14px; margin-top: 30px; padding-top: 20px; border-top: 1px solid #bdc3c7; }}
                                .button {{ display: inline-block; background-color: #3498db; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <h1>🎉 Chào mừng bạn, {fullName}!</h1>
                                </div>
                                
                                <div class='welcome-text'>
                                    Cảm ơn bạn đã đăng ký tài khoản trên <strong>WorldBook</strong>. 
                                    Chúng tôi rất vui được phục vụ bạn trong hành trình khám phá thế giới sách!
                                </div>

                                <div class='credentials'>
                                    <div class='credentials-label'>🔐 Thông tin tài khoản của bạn:</div>
                                    <div style='margin-top: 10px;'>
                                        <div><strong>Email:</strong> <span class='credentials-value'>{email}</span></div>
                                        <div><strong>Mật khẩu tạm thời:</strong> <span class='credentials-value'>{password}</span></div>
                                    </div>
                                </div>

                                <div class='info-box'>
                                    <div class='info-box-title'>⚠️ Bảo mật tài khoản:</div>
                                    <ul class='info-list'>
                                        <li>Vui lòng <strong>lưu mật khẩu này</strong> ở nơi an toàn</li>
                                        <li>Bạn có thể sử dụng email và mật khẩu trên để đăng nhập</li>
                                        <li>Lưu ý! mật khẩu trên chỉ là mật khẩu tạm thời, để an toàn hơn vui lòng đổi mật khẩu mới</li>
                                        <li>Bạn cũng có thể tiếp tục đăng nhập bằng Google</li>
                                    </ul>
                                </div>

                                <div class='welcome-text'>
                                    <strong>✨ Cách đăng nhập:</strong>
                                    <ul style='margin-left: 20px;'>
                                        <li>Sử dụng tên đăng nhập và mật khẩu ở trên</li>
                                        <li>Hoặc tiếp tục sử dụng Google Sign-In</li>
                                    </ul>
                                </div>

                                <a href='https://localhost:7044/Logins/Login' class='button'>Đăng Nhập Ngay</a>

                                <div class='footer'>
                                    <p>Nếu bạn không tạo tài khoản này, vui lòng bỏ qua email này.</p>
                                    <p>© 2024 WorldBook. Tất cả quyền được bảo lưu.</p>
                                </div>
                            </div>
                        </body>
                    </html>
                ";

                return await SendEmailAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi email welcome: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendProfileCompletionReminderAsync(string email, string username, string fullName)
        {
            try
            {
                var subject = "Hoàn tất hồ sơ để mua sắm dễ dàng hơn 🛍️";

                var body = $@"
                    <html>
                        <head>
                            <meta charset='utf-8'/>
                            <style>
                                body {{ font-family: Arial, sans-serif; background-color: #f5f5f5; }}
                                .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                                .header {{ color: #2c3e50; border-bottom: 3px solid #27ae60; padding-bottom: 20px; }}
                                .message {{ color: #34495e; font-size: 16px; line-height: 1.6; margin: 20px 0; }}
                                .missing-info {{ background-color: #e8f5e9; border-left: 4px solid #27ae60; padding: 15px; margin: 20px 0; }}
                                .missing-info-title {{ color: #1b5e20; font-weight: bold; margin-bottom: 10px; }}
                                .missing-list {{ margin-left: 20px; color: #1b5e20; }}
                                .missing-list li {{ margin: 8px 0; }}
                                .button {{ display: inline-block; background-color: #27ae60; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                                .footer {{ color: #7f8c8d; font-size: 14px; margin-top: 30px; padding-top: 20px; border-top: 1px solid #bdc3c7; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <h1>Hoàn tất hồ sơ của bạn! 📋</h1>
                                </div>
                                
                                <div class='message'>
                                    Xin chào <strong>{fullName}</strong>,
                                </div>

                                <div class='message'>
                                    Chúng tôi nhận thấy hồ sơ của bạn còn thiếu một số thông tin quan trọng. 
                                    Vui lòng cập nhật để trải nghiệm mua sắm tốt hơn trên WorldBook!
                                </div>

                                <div class='missing-info'>
                                    <div class='missing-info-title'>📝 Thông tin cần bổ sung:</div>
                                    <ul class='missing-list'>
                                        <li>📞 Số điện thoại liên lạc</li>
                                        <li>👤 Giới tính</li>
                                        <li>📍 Địa chỉ giao hàng</li>
                                    </ul>
                                </div>

                                <div class='message'>
                                    <strong>✨ Lợi ích khi hoàn tất hồ sơ:</strong>
                                    <ul style='margin-left: 20px;'>
                                        <li>Thanh toán nhanh chóng hơn</li>
                                        <li>Giao hàng chính xác đến địa chỉ của bạn</li>
                                        <li>Nhận thông báo đơn hàng kịp thời</li>
                                        <li>Sử dụng các ưu đãi độc quyền</li>
                                    </ul>
                                </div>

                                <div class='footer'>
                                    <p>Bạn có thể cập nhật thông tin này bất cứ lúc nào trong phần hồ sơ của mình.</p>
                                    <p>© 2024 WorldBook. Tất cả quyền được bảo lưu.</p>
                                </div>
                            </div>
                        </body>
                    </html>
                ";

                return await SendEmailAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi email nhắc nhở: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_emailUsername, _emailPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_emailUsername, "WorldBook"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi SMTP: {ex.Message}");
                return false;
            }
        }
    }
}

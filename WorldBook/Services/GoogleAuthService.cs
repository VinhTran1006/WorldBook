using System.Security.Cryptography;
using System.Text;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;

namespace WorldBook.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public GoogleAuthService(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<User> GetOrCreateUserFromGoogleAsync(string email, string fullName)
        {
            // Bước 1: Tìm user theo email
            var existingUser = await _userRepository.GetByEmailAsync(email);

            if (existingUser != null)
            {
                // User đã tồn tại - trả về luôn (đã có roles từ GetByEmailAsync)
                return existingUser;
            }

            // Bước 2: User chưa tồn tại - tạo mới
            var randomPassword = GenerateRandomPassword();
            var hashedPassword = HashPassword(randomPassword);

            // Tạo username từ email
            var username = email.Split('@')[0];

            // Đảm bảo username unique
            var baseUsername = username;
            int counter = 1;
            while (await _userRepository.GetByUsernameAsync(username) != null)
            {
                username = $"{baseUsername}{counter}";
                counter++;
            }

            // Bước 3: Tạo user entity
            var newUser = new User
            {
                Username = username,
                Email = email,
                Name = fullName ?? username,
                Password = hashedPassword,
                IsActive = true,
                AddedAt = DateTime.Now
            };

            // Bước 4: Lưu user vào DB
            await _userRepository.AddAsync(newUser);

            // Bước 5: Gán role Customer
            var customerRole = await _userRepository.GetRoleByNameAsync("Customer");
            if (customerRole != null)
            {
                await _userRepository.AssignRoleAsync(newUser.UserId, customerRole.RoleId);
            }

            // Bước 6: Load lại user với roles đầy đủ
            var createdUser = await _userRepository.GetByIdAsync(newUser.UserId);

            // Bước 7: Gửi email welcome
            await _emailService.SendWelcomeEmailAsync(email, username, randomPassword, fullName ?? username);

            // Bước 8: Kiểm tra xem user có thiếu thông tin không
            bool missingPhone = string.IsNullOrEmpty(createdUser.Phone);
            bool missingGender = string.IsNullOrEmpty(createdUser.Gender);
            bool missingAddress = string.IsNullOrEmpty(createdUser.Address);

            if (missingPhone || missingGender || missingAddress)
            {
                await _emailService.SendProfileCompletionReminderAsync(email, username, fullName ?? username);
            }

            return createdUser;
        }

        public string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public string GenerateRandomPassword(int length = 16)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
            using var rng = new RNGCryptoServiceProvider();
            var randomBytes = new byte[length];
            rng.GetBytes(randomBytes);

            var result = new StringBuilder(length);
            foreach (byte b in randomBytes)
            {
                result.Append(validChars[b % validChars.Length]);
            }
            return result.ToString();
        }
    }
}


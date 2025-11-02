using Microsoft.AspNetCore.Authentication;
using System.Text;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace WorldBook.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        public AuthService(IUserRepository userRepository) => _userRepository = userRepository;

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var hash = HashPassword(password);
            return await _userRepository.GetUserAsync(username, hash);
        }

        public string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

    }
}

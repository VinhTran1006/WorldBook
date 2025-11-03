using WorldBook.Models;

namespace WorldBook.Services.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<User> GetOrCreateUserFromGoogleAsync(string email, string fullName);
        string HashPassword(string password);
        string GenerateRandomPassword(int length = 16);
    }
}

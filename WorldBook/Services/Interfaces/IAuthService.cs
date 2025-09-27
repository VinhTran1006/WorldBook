using WorldBook.Models;

namespace WorldBook.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> ValidateUserAsync(string username, string password);
        string HashPassword(string password);
    }
}

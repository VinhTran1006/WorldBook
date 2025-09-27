using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserAsync(string username, string hashedPassword);
    }
}

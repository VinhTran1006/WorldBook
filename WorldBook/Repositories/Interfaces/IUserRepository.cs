using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserAsync(string username, string hashedPassword);

        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);

        Task<IEnumerable<User>> GetStaffsAsync(); // custom
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);

        Task AssignRoleAsync(int userId, int roleId);

        Task<User> GetByPhoneAsync(string phone);
    }
}

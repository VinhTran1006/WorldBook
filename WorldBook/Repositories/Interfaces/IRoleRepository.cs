using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string roleName);
        Task<Role> AddAsync(Role role);
    }
}

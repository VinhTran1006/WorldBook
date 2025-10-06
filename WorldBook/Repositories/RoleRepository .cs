using Microsoft.EntityFrameworkCore;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;

namespace WorldBook.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly WorldBookDbContext _db;
        public RoleRepository(WorldBookDbContext db) => _db = db;

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            return await _db.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task<Role> AddAsync(Role role)
        {
            _db.Roles.Add(role);
            await _db.SaveChangesAsync();
            return role;
        }
    }
}

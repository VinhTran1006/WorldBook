using Microsoft.EntityFrameworkCore;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;

namespace WorldBook.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly WorldBookDbContext _db;
        public UserRepository(WorldBookDbContext db) => _db = db;

        public async Task<User?> GetUserAsync(string username, string hashedPassword)
        {
            return await _db.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u =>
                    u.Username == username &&
                    u.Password == hashedPassword);
        }
    }
}

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

        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u != null)
            {
                _db.Users.Remove(u);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _db.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _db.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task UpdateAsync(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetStaffsAsync()
        {
            return await _db.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Staff") && u.IsActive)
                .ToListAsync();
        }


        public async Task<User> GetByUsernameAsync(string username)
            => await _db.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<User> GetByEmailAsync(string email)
            => await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task AssignRoleAsync(int userId, int roleId)
        {
            _db.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                CreateAt = DateTime.Now
            });
            await _db.SaveChangesAsync();
        }

        public async Task<User> GetByPhoneAsync(string phone)
    => await _db.Users.FirstOrDefaultAsync(u => u.Phone == phone);


    }
}


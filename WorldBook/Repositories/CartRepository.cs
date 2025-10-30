using Microsoft.EntityFrameworkCore;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;

namespace WorldBook.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly WorldBookDbContext _context;

        public CartRepository(WorldBookDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cart>> GetCartByUserAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Book)
                    .ThenInclude(b => b.Publisher)
                .Include(c => c.Book)
                    .ThenInclude(b => b.Supplier)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Cart?> GetCartItemAsync(int userId, int bookId)
        {
            return await _context.Carts
                .Include(c => c.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);
        }

        public async Task AddItemAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
        }

        public Task UpdateItemAsync(Cart cart)
        {
            _context.Carts.Update(cart);
            return Task.CompletedTask;
        }

        public async Task RemoveItemAsync(int userId, int bookId)
        {
            var cartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            var cartItems = await _context.Carts
                .Where(c => c.UserId == userId)
                .ToListAsync();

            _context.Carts.RemoveRange(cartItems);
        }

        public async Task<bool> ItemExistsAsync(int userId, int bookId)
        {
            return await _context.Carts
                .AnyAsync(c => c.UserId == userId && c.BookId == bookId);
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            return await _context.Carts
                .Where(c => c.UserId == userId)
                .CountAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Cart>> GetCartItemsByUserIdAsync(int userId)
        {
            return await GetCartByUserAsync(userId);
        }
    }
}
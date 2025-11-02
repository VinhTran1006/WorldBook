using Microsoft.EntityFrameworkCore;
using System;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;

namespace WorldBook.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly WorldBookDbContext _db;

        public AuthorRepository(WorldBookDbContext db)
        {
            _db = db;
        }

        public async Task<Author?> GetByNameAsync(string name)
        {
            
            return await _db.Authors
                .FirstOrDefaultAsync(a => a.AuthorName.ToLower() == name.ToLower());
        }

        public async Task<Author> AddAsync(Author author)
        {
            _db.Authors.Add(author);
            await _db.SaveChangesAsync();
            return author;
        }

        public async Task<Author?> GetByIdAsync(int authorId)
        {
           return await _db.Authors.FindAsync(authorId);
        }

        public async Task<IEnumerable<Author>> GetAuthorsAsync()
        {
           return await _db.Authors.Where(a => a.IsActive == true).ToListAsync();
        }

        public async Task UpdateAsync(Author author)
        {
            _db.Authors.Update(author);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int authorId)
        {
            Author author = await _db.Authors.FindAsync(authorId);
              author!.IsActive = false;
             _db.SaveChanges();
        }
    }
}

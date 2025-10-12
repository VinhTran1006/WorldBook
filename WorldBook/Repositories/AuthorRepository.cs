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
            // So sánh không phân biệt hoa thường
            return await _db.Authors
                .FirstOrDefaultAsync(a => a.AuthorName.ToLower() == name.ToLower());
        }

        public async Task<Author> AddAsync(Author author)
        {
            _db.Authors.Add(author);
            await _db.SaveChangesAsync();
            return author;
        }
    }
}

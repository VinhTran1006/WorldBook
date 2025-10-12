using Microsoft.EntityFrameworkCore;
using System;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;

public class PublisherRepository : IPublisherRepository
{
    private readonly WorldBookDbContext _db;

    public PublisherRepository(WorldBookDbContext db)
    {
        _db = db;
    }

    public async Task<Publisher?> GetByNameAsync(string name)
    {
        return await _db.Publishers
            .FirstOrDefaultAsync(p => p.PublisherName.ToLower() == name.ToLower());
    }

    public async Task<Publisher> AddAsync(Publisher publisher)
    {
        _db.Publishers.Add(publisher);
        await _db.SaveChangesAsync();
        return publisher;
    }
}

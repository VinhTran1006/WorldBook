using Microsoft.EntityFrameworkCore;
using System;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;

public class SupplierRepository : ISupplierRepository
{
    private readonly WorldBookDbContext _db;

    public SupplierRepository(WorldBookDbContext db)
    {
        _db = db;
    }

    public async Task<Supplier?> GetByNameAsync(string name)
    {
        return await _db.Suppliers
            .FirstOrDefaultAsync(s => s.SupplierName.ToLower() == name.ToLower());
    }

    public async Task<Supplier> AddAsync(Supplier supplier)
    {
        _db.Suppliers.Add(supplier);
        await _db.SaveChangesAsync();
        return supplier;
    }
}

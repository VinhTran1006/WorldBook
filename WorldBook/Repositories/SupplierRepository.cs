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

    public async Task<IEnumerable<Supplier>> GetSuppliers()
    {
        return await _db.Suppliers.Where(s => s.IsActive == true).ToListAsync();
    }

    public async Task<Supplier> GetSupplierByID(int id)
    {
        return await _db.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id);
    }

    public async Task UpdateSupplier(Supplier sup)
    {
        _db.Suppliers.Update(sup);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteSupplierAsync(int id)
    {
      var sup = _db.Suppliers.FirstOrDefault(s => s.SupplierId == id);
        sup.IsActive = false;
        await _db.SaveChangesAsync();
    }
}

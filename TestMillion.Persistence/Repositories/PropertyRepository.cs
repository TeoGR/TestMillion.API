using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TestMillion.Domain.Contracts;
using TestMillion.Domain.Entities;
using TestMillion.Persistence.Context;

namespace TestMillion.Persistence.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly AppDbContext _context;

    public PropertyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Property> items, long total)> GetAsync(int page, int limit, string? name, string? address, decimal? minPrice, decimal? maxPrice)
    {
        var query = _context.Properties.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p => p.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(address))
        {
            query = query.Where(p => p.Address.Contains(address));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        var total = await query.LongCountAsync();

        var items = await query
            .OrderBy(p => p.Id)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Property?> GetByIdAsync(int id)
        => await _context.Properties.FindAsync(id);

    public async Task<Property> CreateAsync(Property p)
    {
        _context.Properties.Add(p);
        await _context.SaveChangesAsync();
        return p;
    }

    public async Task UpdateAsync(Property p)
    {
        _context.Properties.Update(p);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePriceAsync(int id, decimal newPrice)
    {
        var entity = await _context.Properties.FindAsync(id) ?? throw new KeyNotFoundException("Property no encontrada");
        entity.Price = newPrice;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Properties.FindAsync(id) ?? throw new KeyNotFoundException("Property no encontrada");
        _context.Properties.Remove(entity);
        await _context.SaveChangesAsync();
    }
}

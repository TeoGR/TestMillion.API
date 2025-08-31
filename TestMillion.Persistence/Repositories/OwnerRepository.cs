using Microsoft.EntityFrameworkCore;
using TestMillion.Domain.Contracts;
using TestMillion.Domain.Entities;
using TestMillion.Persistence.Context;

namespace TestMillion.Persistence.Repositories;

public class OwnerRepository : IOwnerRepository
{
    private readonly AppDbContext _context;

    public OwnerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Owner> owners, long total)> GetAllAsync(int page, int limit, string? name)
    {
        var query = _context.Owners.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(o => o.Name.Contains(name));
        }

        var total = await query.LongCountAsync();

        var owners = await query
            .OrderBy(o => o.Id)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (owners, total);
    }

    public async Task<Owner?> GetByIdAsync(int id)
        => await _context.Owners.FindAsync(id);

    public async Task<Owner> CreateAsync(Owner owner)
    {
        _context.Owners.Add(owner);
        await _context.SaveChangesAsync();
        return owner;
    }

    public async Task UpdateAsync(Owner owner)
    {
        _context.Owners.Update(owner);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Owners.FindAsync(id);
        if (entity is null) return;
        _context.Owners.Remove(entity);
        await _context.SaveChangesAsync();
    }
}

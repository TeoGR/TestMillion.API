using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TestMillion.Domain.Contracts;
using TestMillion.Domain.Entities;
using TestMillion.Persistence.Context;

namespace TestMillion.Persistence.Repositories;

public class PropertyTraceRepository : IPropertyTraceRepository
{
    private readonly AppDbContext _context;

    public PropertyTraceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PropertyTrace> CreateAsync(PropertyTrace trace)
    {
        _context.PropertyTraces.Add(trace);
        await _context.SaveChangesAsync();
        return trace;
    }

    public async Task<IEnumerable<PropertyTrace>> GetByPropertyIdAsync(int propertyId)
        => await _context.PropertyTraces.AsNoTracking().Where(t => t.PropertyId == propertyId).ToListAsync();
}

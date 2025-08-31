using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TestMillion.Domain.Contracts;
using TestMillion.Domain.Entities;
using TestMillion.Persistence.Context;

namespace TestMillion.Persistence.Repositories;

public class PropertyImageRepository : IPropertyImageRepository
{
    private readonly AppDbContext _context;

    public PropertyImageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PropertyImage> CreateAsync(PropertyImage image)
    {
        _context.PropertyImages.Add(image);
        await _context.SaveChangesAsync();
        return image;
    }

    public async Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(int propertyId)
        => await _context.PropertyImages.AsNoTracking().Where(p => p.PropertyId == propertyId).ToListAsync();
}

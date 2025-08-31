using TestMillion.Domain.Entities;

namespace TestMillion.Domain.Contracts;

public interface IPropertyRepository
{
    Task<(IEnumerable<Property> items, long total)> GetAsync(int page, int limit, string? name, string? address, decimal? minPrice, decimal? maxPrice);
    Task<Property?> GetByIdAsync(int id);
    Task<Property> CreateAsync(Property p);
    Task UpdateAsync(Property p);
    Task UpdatePriceAsync(int id, decimal newPrice);
    Task DeleteAsync(int id);
}

using TestMillion.Domain.Entities;

namespace TestMillion.Domain.Contracts;

public interface IOwnerRepository
{
    Task<(IEnumerable<Owner> owners, long total)> GetAllAsync(int page, int limit, string? name);
    Task<Owner?> GetByIdAsync(int id);
    Task<Owner> CreateAsync(Owner owner);
    Task UpdateAsync(Owner owner);
    Task DeleteAsync(int id);
}

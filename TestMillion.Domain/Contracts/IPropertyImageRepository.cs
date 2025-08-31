namespace TestMillion.Domain.Contracts;

using System.Collections.Generic;
using TestMillion.Domain.Entities;

public interface IPropertyImageRepository
{
    Task<PropertyImage> CreateAsync(PropertyImage image);
    Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(int propertyId);
}

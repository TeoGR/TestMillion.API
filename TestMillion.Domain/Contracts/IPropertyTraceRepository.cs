using System.Collections.Generic;
using TestMillion.Domain.Entities;

namespace TestMillion.Domain.Contracts;

public interface IPropertyTraceRepository
{
    Task<PropertyTrace> CreateAsync(PropertyTrace trace);
    Task<IEnumerable<PropertyTrace>> GetByPropertyIdAsync(int propertyId);
}

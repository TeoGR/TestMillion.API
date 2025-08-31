using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TestMillion.Services.Models.Request;
using TestMillion.Services.Models.Response;

namespace TestMillion.Services.Contracts;

public interface IPropertyService
{
    Task<PagedResponse<PropertyResponse>> GetAsync(PropertyFiltersRequest request);
    Task<PropertyResponse?> GetByIdAsync(int id);
    Task<PropertyResponse> CreateAsync(PropertyRequest req, IFormFile? file);
    Task<PropertyResponse> UpdateAsync(int id, PropertyRequest req, IFormFile? file);
    Task<PropertyResponse> ChangePriceAsync(int id, decimal price);
    Task<PropertyImageResponse> AddImageAsync(int propertyId, IFormFile file, bool enabled);
    /// <summary>Retrieves images associated with a property.</summary>
    Task<IEnumerable<PropertyImageResponse>> GetImagesAsync(int propertyId);
    /// <summary>Retrieves price change traces for a property.</summary>
    Task<IEnumerable<PropertyTraceResponse>> GetTracesAsync(int propertyId);
    Task DeleteAsync(int id);
}

using Microsoft.AspNetCore.Http;
using TestMillion.Services.Models.Request;
using TestMillion.Services.Models.Response;

namespace TestMillion.Services.Contracts;

public interface IOwnerService
{
    Task<PagedResponse<OwnerResponse>> GetAllAsync(int page, int limit, string? name);
    Task<OwnerResponse?> GetByIdAsync(int id);
    Task<OwnerResponse> CreateAsync(OwnerRequest request, IFormFile? file);
    Task<OwnerResponse> UpdateAsync(int id, OwnerRequest request, IFormFile? file);
    Task DeleteAsync(int id);
}

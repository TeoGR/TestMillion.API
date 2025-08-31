using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Logging;
using TestMillion.Domain.Contracts;
using TestMillion.Domain.Entities;
using TestMillion.Services.Contracts;
using TestMillion.Services.Models.Request;
using TestMillion.Services.Models.Response;

namespace TestMillion.Services.Implementations;

/// <summary>
/// Service for owner CRUD operations.
/// </summary>
public class OwnerService : IOwnerService
{
    private readonly IOwnerRepository _repository;
    private readonly IFileStorageService _storage;
    private readonly ILogger<OwnerService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="OwnerService"/>.
    /// </summary>
    public OwnerService(IOwnerRepository repository, IFileStorageService storage, ILogger<OwnerService> logger)
    {
        _repository = repository;
        _storage = storage;
        _logger = logger;
    }

    private OwnerResponse MapToResponse(Owner owner) => new()
    {
        Id = owner.Id,
        Name = owner.Name,
        Address = owner.Address,
        Photo = owner.Photo,
        Birthday = owner.Birthday
    };

    /// <inheritdoc />
    public async Task<PagedResponse<OwnerResponse>> GetAllAsync(int page, int limit, string? name)
    {
        var (owners, total) = await _repository.GetAllAsync(page, limit, name);

        var ownerResponses = owners.Select(MapToResponse).ToList();

        return new PagedResponse<OwnerResponse>()
        {
            Data = ownerResponses,
            Total = total
        };
    }

    /// <inheritdoc />
    public async Task<OwnerResponse?> GetByIdAsync(int id)
    {
        var owner = await _repository.GetByIdAsync(id);
        if (owner is null) return null;

        return new OwnerResponse
        {
            Id = owner.Id,
            Name = owner.Name,
            Address = owner.Address,
            Photo = owner.Photo,
            Birthday = owner.Birthday
        };
    }

    /// <inheritdoc />
    public async Task<OwnerResponse> CreateAsync(OwnerRequest request, IFormFile? file)
    {
        string? photoUrl = null;
        if (file is not null)
        {
            var fileName = $"owners/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            try
            {
                await using var stream = file.OpenReadStream();
                photoUrl = await _storage.UploadAsync(stream, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading owner photo");
                throw new InvalidOperationException("No se pudo subir la imagen");
            }
        }
        else if (!string.IsNullOrWhiteSpace(request.Photo))
        {
            photoUrl = request.Photo.Trim();
        }

        var entity = new Owner
        {
            Name = request.Name.Trim(),
            Address = request.Address.Trim(),
            Photo = photoUrl,
            Birthday = request.Birthday.HasValue ?
                        new DateTime(request.Birthday.Value.Year, request.Birthday.Value.Month, request.Birthday.Value.Day, 0, 0, 0, DateTimeKind.Unspecified) :
                        (DateTime?)null
        };

        var created = await _repository.CreateAsync(entity);

        return MapToResponse(created);
    }

    /// <inheritdoc />
    public async Task<OwnerResponse> UpdateAsync(int id, OwnerRequest request, IFormFile? file)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) throw new KeyNotFoundException("Owner no encontrado.");

        existing.Name = request.Name.Trim();
        existing.Address = request.Address.Trim();

        if (file is not null)
        {
            var fileName = $"owners/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            try
            {
                await using var stream = file.OpenReadStream();
                existing.Photo = await _storage.UploadAsync(stream, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading owner photo");
                throw new InvalidOperationException("No se pudo subir la imagen");
            }
        }
        else
        {
            existing.Photo = string.IsNullOrWhiteSpace(request.Photo) ? null : request.Photo.Trim();
        }

        existing.Birthday = request.Birthday.HasValue ?
                            new DateTime(request.Birthday.Value.Year, request.Birthday.Value.Month, request.Birthday.Value.Day, 0, 0, 0, DateTimeKind.Unspecified) :
                            (DateTime?)null;

        await _repository.UpdateAsync(existing);
        return MapToResponse(existing);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id) =>
        await _repository.DeleteAsync(id);
}

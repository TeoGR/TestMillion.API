using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using TestMillion.Domain.Contracts;
using TestMillion.Domain.Entities;
using TestMillion.Services.Contracts;
using TestMillion.Services.Models.Request;
using TestMillion.Services.Models.Response;

namespace TestMillion.Services.Implementations
{
    /// <summary>
    /// Service for managing property operations.
    /// </summary>
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _repository;
        private readonly IOwnerRepository _owners;
        private readonly IFileStorageService _storage;
        private readonly IPropertyImageRepository _images;
        private readonly IPropertyTraceRepository _traces;
        private readonly ILogger<PropertyService> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="PropertyService"/>.
        /// </summary>
        public PropertyService(IPropertyRepository repository, IOwnerRepository owners, IFileStorageService storage, IPropertyImageRepository images, IPropertyTraceRepository traces, ILogger<PropertyService> logger)
        {
            _repository = repository;
            _owners = owners;
            _storage = storage;
            _images = images;
            _traces = traces;
            _logger = logger;
        }

        private static PropertyImageResponse MapImage(PropertyImage img) => new()
        {
            Id = img.Id,
            PropertyId = img.PropertyId,
            ImageUrl = img.ImageUrl,
            Enabled = img.Enabled
        };

        private static PropertyTraceResponse MapTrace(PropertyTrace trace) => new()
        {
            Id = trace.Id,
            PropertyId = trace.PropertyId,
            DateSale = trace.DateSale,
            Price = trace.Price
        };

        private static PropertyResponse MapToResponse(Property p, IEnumerable<PropertyImage>? images = null, IEnumerable<PropertyTrace>? traces = null) => new()
        {
            Id = p.Id,
            IdOwner = p.IdOwner,
            Name = p.Name,
            Address = p.Address,
            Price = p.Price,
            CodeInternal = p.CodeInternal,
            Year = p.Year,
            Image = p.Image,
            Images = images?.Select(MapImage).ToList() ?? new List<PropertyImageResponse>(),
            Traces = traces?.Select(MapTrace).ToList() ?? new List<PropertyTraceResponse>()
        };

        /// <inheritdoc />
        public async Task<PagedResponse<PropertyResponse>> GetAsync(PropertyFiltersRequest request)
        {
            var (properties, total) = await _repository.GetAsync(
                request.Page,
                request.Limit,
                request.Name?.Trim(),
                request.Address?.Trim(),
                request.MinPrice,
                request.MaxPrice);

            var propertiesResponses = new List<PropertyResponse>();
            foreach (var prop in properties)
            {
                var imgs = await _images.GetByPropertyIdAsync(prop.Id);
                var trs = await _traces.GetByPropertyIdAsync(prop.Id);
                propertiesResponses.Add(MapToResponse(prop, imgs, trs));
            }

            return new PagedResponse<PropertyResponse>()
            {
                Data = propertiesResponses,
                Total = total
            };
        }

        /// <inheritdoc />
        public async Task<PropertyResponse?> GetByIdAsync(int id)
        {
            var p = await _repository.GetByIdAsync(id);
            if (p is null) return null;
            var imgs = await _images.GetByPropertyIdAsync(p.Id);
            var trs = await _traces.GetByPropertyIdAsync(p.Id);
            return MapToResponse(p, imgs, trs);
        }

        /// <inheritdoc />
        public async Task<PropertyResponse> CreateAsync(PropertyRequest request, IFormFile? file)
        {
            var owner = await _owners.GetByIdAsync(request.IdOwner);
            if (owner is null)
                throw new KeyNotFoundException("El propietario especificado no existe.");

            string imageUrl = string.Empty;
            if (file is not null)
            {
                var fileName = $"properties/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                try
                {
                    await using var stream = file.OpenReadStream();
                    imageUrl = await _storage.UploadAsync(stream, fileName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading property image");
                    throw new InvalidOperationException("No se pudo subir la imagen");
                }
            }
            else if (!string.IsNullOrWhiteSpace(request.Image))
            {
                imageUrl = request.Image.Trim();
            }

            var entity = new Property
            {
                IdOwner = request.IdOwner,
                Name = request.Name.Trim(),
                Address = request.Address.Trim(),
                Price = request.Price,
                CodeInternal = request.CodeInternal.Trim().ToUpperInvariant(),
                Year = request.Year,
                Image = imageUrl
            };
            var created = await _repository.CreateAsync(entity);

            await _traces.CreateAsync(new PropertyTrace
            {
                PropertyId = created.Id,
                DateSale = DateTime.UtcNow,
                Price = created.Price
            });

            if (!string.IsNullOrWhiteSpace(created.Image))
            {
                await _images.CreateAsync(new PropertyImage
                {
                    PropertyId = created.Id,
                    ImageUrl = created.Image!,
                    Enabled = true
                });
            }

            var imgs = await _images.GetByPropertyIdAsync(created.Id);
            var trs = await _traces.GetByPropertyIdAsync(created.Id);
            return MapToResponse(created, imgs, trs);
        }

        /// <inheritdoc />
        public async Task<PropertyResponse> UpdateAsync(int id, PropertyRequest request, IFormFile? file)
        {
            var owner = await _owners.GetByIdAsync(request.IdOwner);
            if (owner is null)
                throw new KeyNotFoundException("El propietario especificado no existe.");

            var current = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Property no encontrada");
            current.IdOwner = request.IdOwner;
            current.Name = request.Name.Trim();
            current.Address = request.Address.Trim();
            current.Price = request.Price;
            current.CodeInternal = request.CodeInternal.Trim().ToUpperInvariant();
            current.Year = request.Year;

            var imageProvided = false;
            if (file is not null)
            {
                var fileName = $"properties/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                try
                {
                    await using var stream = file.OpenReadStream();
                    current.Image = await _storage.UploadAsync(stream, fileName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading property image");
                    throw new InvalidOperationException("No se pudo subir la imagen");
                }
                imageProvided = true;
            }
            else if (!string.IsNullOrWhiteSpace(request.Image))
            {
                current.Image = request.Image.Trim();
                imageProvided = true;
            }

            await _repository.UpdateAsync(current);
            await _traces.CreateAsync(new PropertyTrace
            {
                PropertyId = current.Id,
                DateSale = DateTime.UtcNow,
                Price = current.Price
            });

            if (imageProvided)
            {
                await _images.CreateAsync(new PropertyImage
                {
                    PropertyId = current.Id,
                    ImageUrl = current.Image!,
                    Enabled = true
                });
            }

            var imgs = await _images.GetByPropertyIdAsync(current.Id);
            var trs = await _traces.GetByPropertyIdAsync(current.Id);
            return MapToResponse(current, imgs, trs);
        }

        /// <inheritdoc />
        public async Task<PropertyResponse> ChangePriceAsync(int id, decimal price)
        {
            await _repository.UpdatePriceAsync(id, price);
            var updated = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Property no encontrada");

            await _traces.CreateAsync(new PropertyTrace
            {
                PropertyId = updated.Id,
                DateSale = DateTime.UtcNow,
                Price = updated.Price
            });

            var imgs = await _images.GetByPropertyIdAsync(updated.Id);
            var trs = await _traces.GetByPropertyIdAsync(updated.Id);
            return MapToResponse(updated, imgs, trs);
        }

        /// <inheritdoc />
        public async Task<PropertyImageResponse> AddImageAsync(int propertyId, IFormFile file, bool enabled)
        {
            var property = await _repository.GetByIdAsync(propertyId) ?? throw new KeyNotFoundException("Property no encontrada");
            var fileName = $"properties/{propertyId}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string imageUrl;
            try
            {
                await using var stream = file.OpenReadStream();
                imageUrl = await _storage.UploadAsync(stream, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading property image");
                throw new InvalidOperationException("No se pudo subir la imagen");
            }

            var image = new PropertyImage
            {
                PropertyId = property.Id,
                ImageUrl = imageUrl,
                Enabled = enabled
            };

            var created = await _images.CreateAsync(image);

            return MapImage(created);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PropertyImageResponse>> GetImagesAsync(int propertyId)
        {
            var imgs = await _images.GetByPropertyIdAsync(propertyId);
            return imgs.Select(MapImage);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PropertyTraceResponse>> GetTracesAsync(int propertyId)
        {
            var trs = await _traces.GetByPropertyIdAsync(propertyId);
            return trs.Select(MapTrace);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using TestMillion.Services.Contracts;

namespace TestMillion.API.Controllers;

/// <summary>
/// Provides read-only access to property images.
/// </summary>
[ApiController]
[Route("api/properties/{propertyId:int}/images")]
public class PropertyImagesController : ControllerBase
{
    private readonly IPropertyService _service;

    public PropertyImagesController(IPropertyService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves all images for a property.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get(int propertyId)
    {
        var images = await _service.GetImagesAsync(propertyId);
        return Ok(images);
    }
}

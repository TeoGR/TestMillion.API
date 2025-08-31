using Microsoft.AspNetCore.Mvc;
using TestMillion.Services.Contracts;

namespace TestMillion.API.Controllers;

/// <summary>
/// Provides read-only access to property traces.
/// </summary>
[ApiController]
[Route("api/properties/{propertyId:int}/traces")]
public class PropertyTracesController : ControllerBase
{
    private readonly IPropertyService _service;

    public PropertyTracesController(IPropertyService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves all traces for a property.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get(int propertyId)
    {
        var traces = await _service.GetTracesAsync(propertyId);
        return Ok(traces);
    }
}

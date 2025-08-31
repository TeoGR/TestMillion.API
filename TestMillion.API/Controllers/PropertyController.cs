using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TestMillion.Services.Contracts;
using TestMillion.Services.Models.Request;

namespace TestMillion.API.Controllers;


/// <summary>
/// API endpoints for managing properties.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertiesController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    /// <summary>
    /// Retrieves paginated list of properties.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PropertyFiltersRequest request)
    {
        request.Page = request.Page <= 0 ? 1 : request.Page;
        request.Limit = request.Limit <= 0 ? 10 : request.Limit;
        var properties = await _propertyService.GetAsync(request);
        return Ok(properties);
    }

    /// <summary>
    /// Retrieves a property by identifier.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var property = await _propertyService.GetByIdAsync(id);
        return property is null ? NotFound() : Ok(property);
    }

    /// <summary>
    /// Creates a new property.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] PropertyRequest request, IFormFile? file)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var created = await _propertyService.CreateAsync(request, file);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing property.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] PropertyRequest request, IFormFile? file)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var updated = await _propertyService.UpdateAsync(id, request, file);

        return Ok(updated);
    }

    /// <summary>
    /// Deletes a property.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _propertyService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Changes the price of a property.
    /// </summary>
    [HttpPatch("{id}/price")]
    public async Task<IActionResult> ChangePrice(int id, [FromBody, Required] decimal price)
    {
        var updated = await _propertyService.ChangePriceAsync(id, price);
        return Ok(updated);
    }

    /// <summary>
    /// Adds an additional image to a property.
    /// </summary>
    [HttpPost("{id}/images")]
    public async Task<IActionResult> AddImage(int id, [FromForm] PropertyImageRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var created = await _propertyService.AddImageAsync(id, request.File, request.Enabled);
        return Ok(created);
    }
}


using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TestMillion.Services.Contracts;
using TestMillion.Services.Models.Request;

namespace TestMillion.API.Controllers;

/// <summary>
/// API endpoints for managing owners.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OwnersController : ControllerBase
{
    private readonly IOwnerService _ownerService;

    public OwnersController(IOwnerService ownerService)
    {
        _ownerService = ownerService;
    }

    /// <summary>
    /// Retrieves paginated list of owners.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] OwnerFiltersRequest request)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var limit = request.Limit <= 0 ? 10 : request.Limit;
        var owners = await _ownerService.GetAllAsync(page, limit, request.Name);
        return Ok(owners);
    }

    /// <summary>
    /// Retrieves a single owner by identifier.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var owner = await _ownerService.GetByIdAsync(id);
        return owner is null ? NotFound() : Ok(owner);
    }

    /// <summary>
    /// Creates a new owner.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] OwnerRequest request, IFormFile? file)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var created = await _ownerService.CreateAsync(request, file);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing owner.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm, Required] OwnerRequest request, IFormFile? file)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var updated = await _ownerService.UpdateAsync(id, request, file);
        return Ok(updated);
    }

    /// <summary>
    /// Deletes an owner.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _ownerService.DeleteAsync(id);
        return NoContent();
    }
}

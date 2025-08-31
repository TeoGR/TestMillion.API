using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TestMillion.Services.Models.Request;

public class PropertyImageRequest
{
    [Required]
    public IFormFile File { get; set; } = default!;
    public bool Enabled { get; set; } = true;
}

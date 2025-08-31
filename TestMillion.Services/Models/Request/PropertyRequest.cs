using System.ComponentModel.DataAnnotations;
using TestMillion.Services.Validation;

namespace TestMillion.Services.Models.Request;

public class PropertyRequest
{
    [Required, MinLength(2), MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MinLength(5), MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    [Range(0, 1_000_000_000)]
    public decimal Price { get; set; }

    [Required, RegularExpression(@"^[A-Z]{3}[0-9]{3}$", ErrorMessage = "El código debe ser 3 letras mayúsculas seguidas de 3 números, ej: ABC123")]
    public string CodeInternal { get; set; } = string.Empty;

    [ValidYear(ErrorMessage = "El año debe estar entre 1900 y el próximo año")]
    public int Year { get; set; }

    [Url, MaxLength(2048)]
    public string? Image { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int IdOwner { get; set; }
}

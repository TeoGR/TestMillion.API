using System.ComponentModel.DataAnnotations;

namespace TestMillion.Services.Models.Request;

public class OwnerRequest
{
    [Required, MinLength(2), MaxLength(50)]
    [RegularExpression(@"^[\p{L}\p{M}\s.'-]+$", ErrorMessage = "El nombre solo permite letras y (' . -)")]
    public string Name { get; set; } = string.Empty;

    [Required, MinLength(5), MaxLength(100)]
    public string Address { get; set; } = string.Empty;

    [Url, MaxLength(2048)]
    public string? Photo { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? Birthday { get; set; }
}

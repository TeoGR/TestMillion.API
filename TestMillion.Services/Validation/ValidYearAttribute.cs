using System.ComponentModel.DataAnnotations;

namespace TestMillion.Services.Validation;

public sealed class ValidYearAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is not int year)
            return new ValidationResult("Año inválido.");
        var max = DateTime.UtcNow.Year + 1;
        return (year >= 1900 && year <= max)
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage ?? $"El año debe estar entre 1900 y {max}");
    }
}

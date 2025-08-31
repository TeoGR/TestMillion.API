using System.Collections.Generic;

namespace TestMillion.Services.Models.Response;

public class PropertyResponse
{
    public int Id { get; set; }
    public int IdOwner { get; set; }
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public decimal Price { get; set; }
    public string CodeInternal { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Image { get; set; } = default!;
    public List<PropertyImageResponse> Images { get; set; } = new();
    public List<PropertyTraceResponse> Traces { get; set; } = new();
}

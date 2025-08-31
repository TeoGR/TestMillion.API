namespace TestMillion.Services.Models.Response;

public class PropertyImageResponse
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool Enabled { get; set; }
}

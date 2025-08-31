namespace TestMillion.Services.Models.Response;

public class OwnerResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
    public DateTime? Birthday { get; set; }
}

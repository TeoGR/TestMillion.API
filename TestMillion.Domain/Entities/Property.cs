namespace TestMillion.Domain.Entities;

public class Property
{
    public int Id { get; set; }

    public int IdOwner { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string CodeInternal { get; set; } = string.Empty;

    public int Year { get; set; }

    public string Image { get; set; } = string.Empty;
}

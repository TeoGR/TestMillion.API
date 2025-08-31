using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TestMillion.Services.Models.Request;

public class PropertyFiltersRequest
{
    [FromQuery(Name = "page"), Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "limit"), Range(1, int.MaxValue)]
    public int Limit { get; set; } = 10;

    [FromQuery(Name = "name")]
    public string? Name { get; set; }

    [FromQuery(Name = "address")]
    public string? Address { get; set; }

    [FromQuery(Name = "minPrice")]
    public decimal? MinPrice { get; set; }

    [FromQuery(Name = "maxPrice")]
    public decimal? MaxPrice { get; set; }
}

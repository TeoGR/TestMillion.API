using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TestMillion.Services.Models.Request;

public class OwnerFiltersRequest
{
    [FromQuery(Name = "page"), Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "limit"), Range(1, int.MaxValue)]
    public int Limit { get; set; } = 10;

    [FromQuery(Name = "name")]
    public string? Name { get; set; }
}

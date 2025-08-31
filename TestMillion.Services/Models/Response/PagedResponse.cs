namespace TestMillion.Services.Models.Response;

public class PagedResponse<T>
{
    public List<T> Data { get; init; } = new List<T>();
    public long Total { get; init; }
}

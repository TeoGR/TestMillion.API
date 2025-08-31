using System.IO;
using System.Threading.Tasks;
using TestMillion.Services.Contracts;

namespace TestMillion.Tests;

public class FakeFileStorageService : IFileStorageService
{
    public Task<string> UploadAsync(Stream fileStream, string fileName)
        => Task.FromResult($"https://storage/{fileName}");
}

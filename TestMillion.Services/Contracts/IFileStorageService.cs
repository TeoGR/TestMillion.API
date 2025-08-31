using System.IO;
using System.Threading.Tasks;

namespace TestMillion.Services.Contracts;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName);
}


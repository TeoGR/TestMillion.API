using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using TestMillion.Services.Contracts;

namespace TestMillion.Services.Implementations;

public class FirebaseStorageService : IFileStorageService
{
    private readonly string _bucket;
    private readonly StorageClient _storageClient;

    public FirebaseStorageService(IConfiguration configuration)
    {
        var section = configuration.GetSection("Firebase");
        _bucket = section["Bucket"] ?? throw new InvalidOperationException("Firebase bucket not configured");

        var credentialJson = section["CredentialJson"]
            ?? Environment.GetEnvironmentVariable("FIREBASE_CREDENTIAL_JSON");

        GoogleCredential? credential = null;

        if (!string.IsNullOrWhiteSpace(credentialJson))
        {
            credential = GoogleCredential.FromJson(credentialJson);
        }
        else
        {
            var credentialsPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")
                ?? section["CredentialsPath"]
                ?? Path.Combine(AppContext.BaseDirectory, "firebase_credentials.json");

            if (Directory.Exists(credentialsPath))
            {
                credentialsPath = Directory.EnumerateFiles(credentialsPath, "*.json").FirstOrDefault()
                    ?? throw new InvalidOperationException($"No credentials file found in {credentialsPath}");
            }

            if (File.Exists(credentialsPath))
            {
                credential = GoogleCredential.FromFile(credentialsPath);
            }
        }

        if (credential is null)
            throw new InvalidOperationException("Google credentials not configured.");

        _storageClient = StorageClient.Create(credential);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName)
    {
        await _storageClient.UploadObjectAsync(_bucket, fileName, null, fileStream);
        return $"https://storage.googleapis.com/{_bucket}/{fileName}";
    }
}

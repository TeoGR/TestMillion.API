using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestMillion.Persistence.Context;
using TestMillion.Services.Contracts;
using TestMillion.Services.Implementations;

namespace TestMillion.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IOwnerService, OwnerService>();
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<IFileStorageService, FirebaseStorageService>();

        return services;
    }

    public static IServiceCollection AddDataBase(this IServiceCollection services, string connString)
    {
        services.AddDbContext<AppDbContext>(op =>
        {
            op.UseSqlServer(connString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(60),
                    errorNumbersToAdd: null);
            });
        });
        return services;
    }
}

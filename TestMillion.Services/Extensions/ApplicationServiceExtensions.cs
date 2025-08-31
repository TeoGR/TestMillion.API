using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestMillion.Domain.Contracts;
using TestMillion.Persistence.Repositories;

namespace TestMillion.Services.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddServices();
        services.AddDataBase(connString);

        services.AddScoped<IOwnerRepository, OwnerRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
        services.AddScoped<IPropertyTraceRepository, PropertyTraceRepository>();

        return services;
    }
}

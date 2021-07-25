using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Az101.Gallery.Infrastructure.HealthCheck;
using Az101.Gallery.Infrastructure.Options;
using Az101.Gallery.Infrastructure.Persistence;
using Az101.Gallery.Infrastructure.Storage;

using Azure.Identity;
using Azure.Storage.Blobs;

namespace Az101.Gallery.Infrastructure.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddHealthCheckServices(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<StorageHealthCheck>("storage")
                .AddDbContextCheck<ApplicationDbContext>("db");

            return services;
        }

        public static IServiceCollection AddDbServices(this IServiceCollection services, IConfiguration configuration)
        {
            var defaultConnectionString = configuration.GetConnectionString("Default");

            services.AddDbContextPool<ApplicationDbContext>(option =>
                {
                    option.UseSqlServer(defaultConnectionString, o =>
                    {
                        o.EnableRetryOnFailure();
                    })
                    .EnableDetailedErrors(true)
                    .EnableSensitiveDataLogging(false);
                });

            return services;
        }

        public static IServiceCollection AddStorageServices(this IServiceCollection services, IConfiguration configuration)
        {
            var azureStorageOption = configuration.GetSection("Storage").Get<AzureStorageOption>();

            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(azureStorageOption.ConnectionString);
                builder.UseCredential(new EnvironmentCredential());
            });

            services.AddSingleton(azureStorageOption);

            services.AddScoped<IStorage, AzureStorage>();

            return services;
        }

        public static void EnsureDependencyServices(this IApplicationBuilder app, IServiceProvider service)
        {
            using var scope = service.CreateScope();

            var option = service.GetRequiredService<AzureStorageOption>();
            var serviceClient = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();
            var containerClient = serviceClient.GetBlobContainerClient(option.ContainerName);
            containerClient.CreateIfNotExists();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();

        }

    }
}

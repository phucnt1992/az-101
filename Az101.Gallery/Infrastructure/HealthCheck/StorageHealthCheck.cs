using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

using Az101.Gallery.Infrastructure.Options;

using Azure;
using Azure.Storage.Blobs;

namespace Az101.Gallery.Infrastructure.HealthCheck
{
    public class StorageHealthCheck : IHealthCheck
    {
        private readonly ILogger<StorageHealthCheck> logger;
        private readonly BlobServiceClient serviceClient;
        private readonly AzureStorageOption option;

        public StorageHealthCheck(
            [NotNull] ILogger<StorageHealthCheck> logger,
            [NotNull] BlobServiceClient serviceClient,
            [NotNull] AzureStorageOption option)
        {
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.serviceClient = serviceClient ?? throw new System.ArgumentNullException(nameof(serviceClient));
            this.option = option ?? throw new System.ArgumentNullException(nameof(option));
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var containerClient = serviceClient.GetBlobContainerClient(option.ContainerName);

                var containerExist = await containerClient.ExistsAsync(cancellationToken: cancellationToken);

                if (!containerExist.Value)
                {
                    return HealthCheckResult.Unhealthy($"{option.ContainerName} container does not exist.");
                }
            }
            catch (RequestFailedException exception)
            {
                var message = "Cannot connect to Storage Service!";
                logger.LogError(exception, message);
                return HealthCheckResult.Unhealthy(message);
            }

            return HealthCheckResult.Healthy("Storage Service is OK.");
        }
    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Az101.Gallery.Infrastructure.Options;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Az101.Gallery.Infrastructure.Storage
{
    public class AzureStorage : IStorage
    {
        private readonly BlobContainerClient containerClient;

        public AzureStorage([NotNull] BlobServiceClient serviceClient, [NotNull] AzureStorageOption option)
        {
            if (serviceClient is null)
            {
                throw new ArgumentNullException(nameof(serviceClient));
            }

            if (option is null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            this.containerClient = serviceClient.GetBlobContainerClient(option.ContainerName);
        }

        public async Task DeleteAsync(string fileName, CancellationToken cancellationToken)
        {
            await containerClient.DeleteBlobIfExistsAsync(fileName, cancellationToken: cancellationToken);
        }

        public async Task EnsureContainerCreatedAsync(CancellationToken cancellationToken = default)
        {
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);
        }

        public async Task<Stream> GetFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var blobClient = containerClient.GetBlobClient(fileName);
            var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);

            return response.Value.Content;
        }

        public string GetContainerUrl()
        {
            return containerClient.Uri.AbsoluteUri;
        }

        public async Task SaveAsync(string fileName, Stream fileStream, CancellationToken cancellationToken = default)
        {
            await containerClient.UploadBlobAsync(fileName, fileStream, cancellationToken);
        }
    }
}

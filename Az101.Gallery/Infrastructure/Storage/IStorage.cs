using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Az101.Gallery.Infrastructure.Storage;

public interface IStorage
{
    string GetContainerUrl();
    Task<Stream> GetFileAsync(string fileName, CancellationToken cancellationToken = default);
    Task SaveAsync(string fileName, Stream fileStream, CancellationToken cancellationToken = default);
    Task DeleteAsync(string fileName, CancellationToken cancellationToken = default);
    Task EnsureContainerCreatedAsync(CancellationToken cancellationToken = default);
}


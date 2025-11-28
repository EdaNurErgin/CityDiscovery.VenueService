using System.IO;

namespace CityDiscovery.Venues.Application.Interfaces.Services;

public interface IFileStorageService
{
    /// <summary>
    /// Dosyayı bir storage (local, Azure Blob vs.) alanına kaydeder
    /// ve onu erişmek için kullanılacak tam URL'yi döner.
    /// </summary>
    Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);
}

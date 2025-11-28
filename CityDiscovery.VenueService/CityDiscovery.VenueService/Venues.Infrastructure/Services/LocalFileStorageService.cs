using CityDiscovery.Venues.Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using System.IO;

namespace CityDiscovery.Venues.Infrastructure.Services;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly LocalFileStorageOptions _options;

    public LocalFileStorageService(IOptions<LocalFileStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (fileStream is null || fileStream.Length == 0)
            throw new ArgumentException("File stream is empty.", nameof(fileStream));

        if (string.IsNullOrWhiteSpace(_options.RootPath))
            throw new InvalidOperationException("LocalFileStorageOptions.RootPath is not configured.");

        if (string.IsNullOrWhiteSpace(_options.BaseUrl))
            throw new InvalidOperationException("LocalFileStorageOptions.BaseUrl is not configured.");

        // 1) Klasör yoksa oluştur
        Directory.CreateDirectory(_options.RootPath);

        // 2) Benzersiz isim üret (Guid + orijinal extension)
        var extension = Path.GetExtension(fileName);
        var newFileName = $"{Guid.NewGuid():N}{extension}";
        var physicalPath = Path.Combine(_options.RootPath, newFileName);

        // 3) Dosyayı diske yaz
        await using (var file = new FileStream(physicalPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await fileStream.CopyToAsync(file, cancellationToken);
        }

        // 4) Dışarıya verilecek URL'yi oluştur
        // Örn: BaseUrl = "https://localhost:7071/uploads"
        var fileUrl = $"{_options.BaseUrl.TrimEnd('/')}/{newFileName}";

        return fileUrl;
    }
}

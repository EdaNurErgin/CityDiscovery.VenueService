namespace CityDiscovery.Venues.Infrastructure.Services;

public sealed class LocalFileStorageOptions
{
    /// <summary>
    /// Fiziksel dosyaların kaydedileceği root klasör.
    /// Örn: "C:\\CityDiscoveryUploads"
    /// </summary>
    public string RootPath { get; set; } = default!;

    /// <summary>
    /// Bu klasörü dışarıdan erişmek için kullanılacak base URL.
    /// Örn: "https://localhost:7071/uploads"
    /// </summary>
    public string BaseUrl { get; set; } = default!;
}

using CityDiscovery.Venues.Shared.Common.Events.Identity;
using CityDiscovery.Venues.Shared.Common.DTOs.Identity;


namespace CityDiscovery.Venues.Application.Interfaces.ExternalServices;

/// <summary>
/// VenueService → Identity Service senkron iletişim.
/// Venue oluştururken owner kontrolü için kullanılır.
/// </summary>
public interface IIdentityServiceClient
{
    /// <summary>
    /// Kullanıcı bilgilerini getirir.
    /// </summary>
    Task<UserDto?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kullanıcının belirtilen rolde olup olmadığını kontrol eder.
    /// </summary>
    Task<bool> CheckUserRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kullanıcı var mı kontrol eder (hızlı).
    /// </summary>
    Task<bool> CheckUserExistsAsync(Guid userId, CancellationToken cancellationToken = default);
}
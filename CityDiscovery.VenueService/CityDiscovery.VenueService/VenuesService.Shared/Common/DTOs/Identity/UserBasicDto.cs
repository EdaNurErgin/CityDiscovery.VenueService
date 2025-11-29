namespace CityDiscovery.Venues.Shared.Common.DTOs.Identity;

/// <summary>
/// Sadece temel kullanıcı bilgisi (performans için).
/// </summary>
public sealed class UserBasicDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = default!;
    public string? AvatarUrl { get; init; }
}
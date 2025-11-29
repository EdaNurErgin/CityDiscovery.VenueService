namespace CityDiscovery.Venues.Shared.Common.DTOs.Identity;

public sealed class UserDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Role { get; init; } = default!;
    public string? AvatarUrl { get; init; }
    public DateTime CreatedAt { get; init; }
}
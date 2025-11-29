namespace CityDiscovery.Venues.Application.Interfaces.Services;

/// <summary>
/// JWT token'dan gelen kullanıcı bilgilerini sağlar
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? UserName { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
    bool IsOwner { get; }
    bool IsAdmin { get; }
}
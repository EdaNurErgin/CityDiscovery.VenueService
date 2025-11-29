using CityDiscovery.Venues.Application.Interfaces.ExternalServices;
using CityDiscovery.Venues.Shared.Common.DTOs.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;


namespace CityDiscovery.Venues.Infrastructure.ExternalServices;

public sealed class IdentityServiceClient : IIdentityServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly string _identityServiceUrl;
    private readonly ILogger<IdentityServiceClient> _logger;

    public IdentityServiceClient(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<IdentityServiceClient> logger)
    {
        _httpClient = httpClient;
        _identityServiceUrl = configuration["Services:IdentityService:Url"]
            ?? "http://identity-service";
        _logger = logger;
        
        // Timeout ayarla (varsayılan 100 saniye çok uzun)
        _httpClient.Timeout = TimeSpan.FromSeconds(5);
    }

    public async Task<UserDto?> GetUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(3)); // 3 saniye timeout

            var response = await _httpClient.GetAsync(
                $"{_identityServiceUrl}/api/users/{userId}",
                timeoutCts.Token);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Identity Service returned {StatusCode} for user {UserId}", 
                    response.StatusCode, userId);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<UserDto>(timeoutCts.Token);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Identity Service request timeout for user {UserId}", userId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Identity Service for user {UserId}", userId);
            return null;
        }
    }

    public async Task<bool> CheckUserRoleAsync(
        Guid userId,
        string role,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await GetUserAsync(userId, cancellationToken);
            return user?.Role == role;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user role for user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> CheckUserExistsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(3)); // 3 saniye timeout

            var response = await _httpClient.GetAsync(
                $"{_identityServiceUrl}/api/users/{userId}/exists",
                timeoutCts.Token);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Identity Service returned {StatusCode} for user exists check {UserId}", 
                    response.StatusCode, userId);
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<bool>(timeoutCts.Token);
            return result;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Identity Service request timeout for user exists check {UserId}", userId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user exists for user {UserId}", userId);
            return false;
        }
    }
}
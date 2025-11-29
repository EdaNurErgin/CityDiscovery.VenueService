using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace CityDiscovery.Venues.Infrastructure.Security;

/// <summary>
/// Kullanıcının venue'nin sahibi olup olmadığını kontrol eder
/// </summary>
public class VenueOwnerAuthorizationHandler : AuthorizationHandler<VenueOwnerRequirement, Guid>
{
    private readonly IVenueRepository _venueRepository;
    private readonly ICurrentUserService _currentUserService;

    public VenueOwnerAuthorizationHandler(
        IVenueRepository venueRepository,
        ICurrentUserService currentUserService)
    {
        _venueRepository = venueRepository;
        _currentUserService = currentUserService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        VenueOwnerRequirement requirement,
        Guid venueId)
    {
        // Admin her zaman izinli
        if (_currentUserService.IsAdmin)
        {
            context.Succeed(requirement);
            return;
        }

        // Owner kontrolü
        if (!_currentUserService.UserId.HasValue)
        {
            context.Fail();
            return;
        }

        var venue = await _venueRepository.GetByIdAsync(venueId);

        if (venue != null && venue.OwnerUserId == _currentUserService.UserId.Value)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}

public class VenueOwnerRequirement : IAuthorizationRequirement { }
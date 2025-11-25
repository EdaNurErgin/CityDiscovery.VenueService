using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.RemoveCategoryFromVenue;

public sealed class RemoveCategoryFromVenueCommandHandler
    : IRequestHandler<RemoveCategoryFromVenueCommand, Unit>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IVenueCategoryRepository _venueCategoryRepository;

    public RemoveCategoryFromVenueCommandHandler(
        IVenueRepository venueRepository,
        IVenueCategoryRepository venueCategoryRepository)
    {
        _venueRepository = venueRepository;
        _venueCategoryRepository = venueCategoryRepository;
    }

    public async Task<Unit> Handle(
        RemoveCategoryFromVenueCommand request,
        CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);
        if (venue is null)
        {
            throw new KeyNotFoundException($"Venue with id '{request.VenueId}' was not found.");
        }

        var hasCategory = await _venueCategoryRepository
            .VenueHasCategoryAsync(request.VenueId, request.CategoryId, cancellationToken);

        if (!hasCategory)
        {
            return Unit.Value;
        }

        await _venueCategoryRepository.RemoveCategoryFromVenueAsync(
            request.VenueId,
            request.CategoryId,
            cancellationToken);

        return Unit.Value;
    }
}

using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.VenueService.VenuesService.Application.Interfaces.Services;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.RemoveCategoryFromVenue;

public sealed class RemoveCategoryFromVenueCommandHandler
    : IRequestHandler<RemoveCategoryFromVenueCommand, Unit>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IVenueCategoryRepository _venueCategoryRepository;
    private readonly ICityDiscoveryService _cityDiscoveryService; // EKLENDİ
    public RemoveCategoryFromVenueCommandHandler(
        IVenueRepository venueRepository,
        IVenueCategoryRepository venueCategoryRepository, ICityDiscoveryService cityDiscoveryService)
    {
        _venueRepository = venueRepository;
        _venueCategoryRepository = venueCategoryRepository;
        _cityDiscoveryService = cityDiscoveryService;
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

        var venueForElastic = await _venueRepository.GetVenueWithDetailsAsync(request.VenueId, cancellationToken);

        // Onaylı bir mekan ise, elasticsearch üzerindeki belgesini zenginleştirerek güncelliyoruz
        // Böylece silinen kategori JSON belgesinden de (venueCategory alanından) kalkmış oluyor
        if (venueForElastic != null && venueForElastic.IsApproved)
        {
            await _cityDiscoveryService.IndexVenueWithDetailsAsync(venueForElastic);
        }

        return Unit.Value;
    }
}

using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.VenueService.VenuesService.Application.Interfaces.Services;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.AddCategoryToVenue;

public sealed class AddCategoryToVenueCommandHandler
    : IRequestHandler<AddCategoryToVenueCommand, Unit>
{
    private readonly IVenueRepository _venueRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IVenueCategoryRepository _venueCategoryRepository;
    private readonly ICityDiscoveryService _cityDiscoveryService; 

    public AddCategoryToVenueCommandHandler(
        IVenueRepository venueRepository,
        ICategoryRepository categoryRepository,
        IVenueCategoryRepository venueCategoryRepository, ICityDiscoveryService cityDiscoveryService)
    {
        _venueRepository = venueRepository;
        _categoryRepository = categoryRepository;
        _venueCategoryRepository = venueCategoryRepository;
        _cityDiscoveryService = cityDiscoveryService;
    }

    public async Task<Unit> Handle(
        AddCategoryToVenueCommand request,
        CancellationToken cancellationToken)
    {
        // 1) Venue var mı?
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);
        if (venue is null)
        {
            throw new KeyNotFoundException($"Venue with id '{request.VenueId}' was not found.");
        }

        // 2) Category var mı?
        var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            throw new KeyNotFoundException($"Category with id '{request.CategoryId}' was not found.");
        }

        // 3) Zaten o kategoriye sahip mi?
        var alreadyHasCategory = await _venueCategoryRepository
            .VenueHasCategoryAsync(request.VenueId, request.CategoryId, cancellationToken);

        if (alreadyHasCategory)
        {
            // Sessizce çıkıyoruz; istersen exception da atabilirsin
            return Unit.Value;
        }

        // 4) Ekle
        await _venueCategoryRepository.AddCategoryToVenueAsync(
            request.VenueId,
            request.CategoryId,
            cancellationToken);


        var venueForElastic = await _venueRepository.GetVenueWithDetailsAsync(request.VenueId, cancellationToken);

        // Onaylı bir mekan ise, elasticsearch üzerindeki belgesini zenginleştirerek güncelliyoruz
        if (venueForElastic != null && venueForElastic.IsApproved)
        {
            await _cityDiscoveryService.IndexVenueWithDetailsAsync(venueForElastic);
        }
        return Unit.Value;
    }
}

using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.SetVenueAddress;

public sealed class SetVenueAddressCommandHandler : IRequestHandler<SetVenueAddressCommand>
{
    private readonly IVenueRepository _venueRepository;

    public SetVenueAddressCommandHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task Handle(SetVenueAddressCommand request, CancellationToken cancellationToken)
    {
        // 1. Mekanı adres tablosuyla (Include(v => v.Address)) birlikte getirir
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);

        if (venue == null)
            throw new KeyNotFoundException("Mekan bulunamadı.");

        // 2. Upsert (Insert or Update) Mantığı
        if (venue.Address == null)
        {
            // Veritabanında adres YOK -> INSERT işlemi yapılacak
            var newAddress = VenueAddress.Create(
                venueId: venue.Id,
                countryId: request.CountryId,
                cityId: request.CityId,
                districtId: request.DistrictId,
                neighborhood: request.Neighborhood, // GÜNCELLENDİ
                street: request.Street,             // GÜNCELLENDİ
                buildingNo: request.BuildingNo,     // GÜNCELLENDİ
                fullAddress: request.FullAddress
            );

            venue.SetAddress(newAddress);
        }
        else
        {
            // Veritabanında adres VAR -> UPDATE işlemi yapılacak
            venue.Address.Update(
                countryId: request.CountryId,
                cityId: request.CityId,
                districtId: request.DistrictId,
                neighborhood: request.Neighborhood, 
                street: request.Street,             
                buildingNo: request.BuildingNo,     
                fullAddress: request.FullAddress
            );

            // Mekanın UpdatedAt alanının güncellenmesi için SetAddress'i tekrar çağırıyoruz
            venue.SetAddress(venue.Address);

        }

        // 3. Değişiklikleri kaydet
        await _venueRepository.UpdateAsync(venue, cancellationToken);
    }
}
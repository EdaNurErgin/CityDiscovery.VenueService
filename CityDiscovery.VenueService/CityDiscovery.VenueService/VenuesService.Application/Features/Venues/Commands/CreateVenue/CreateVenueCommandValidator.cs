using FluentValidation;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.CreateVenue;

public sealed class CreateVenueCommandValidator
    : AbstractValidator<CreateVenueCommand>
{
    public CreateVenueCommandValidator()
    {
        RuleFor(x => x.OwnerUserId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.PriceLevel)
            .InclusiveBetween((byte)1, (byte)5)
            .When(x => x.PriceLevel.HasValue);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180);
    }
}

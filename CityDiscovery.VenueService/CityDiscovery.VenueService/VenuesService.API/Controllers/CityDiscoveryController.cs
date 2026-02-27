using CityDiscovery.Venues.Application.Features.Venues.Queries.GetNearbyVenues;
using CityDiscovery.VenueService.VenuesService.Application.Features.Venues.Queries.DiscoverVenuesElastic;
using CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CityDiscovery.VenueService.VenuesService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityDiscoveryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CityDiscoveryController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Kullanıcının girdigi filtre secicmlerine gore mekanlari listeler
        /// </summary>
        [HttpGet("citydiscovery")]
        public async Task<ActionResult<List<VenueBasicDto>>> Discover([FromQuery] DiscoverVenuesElasticQuery query)
        {
            var venues = await _mediator.Send(query);
            return Ok(venues);
        }

        /// <summary>
        /// Yakındaki mekanları getirir (Public endpoint)
        /// </summary>
        /// <param name="lat">Enlem (Latitude)</param>
        /// <param name="lon">Boylam (Longitude)</param>
        /// <param name="radius">Arama yarıçapı (metre, varsayılan: 2000)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Yakındaki mekanlar listesi</returns>
        /// <response code="200">Başarılı - Mekan listesi döner</response>
        [HttpGet("nearby")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNearby(
            [FromQuery] double lat,
            [FromQuery] double lon,
            [FromQuery] double radius = 2000,
            CancellationToken cancellationToken = default)
        {
            var query = new GetNearbyVenuesQuery(lat, lon, radius);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

    }
}
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Threading.Tasks;
using System.Collections.Generic;
using CityDiscovery.VenueService.VenuesService.Application.Features.Venues.Queries.DiscoverVenuesElastic;
using CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;

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

        [HttpGet("citydiscovery")]
        public async Task<ActionResult<List<VenueBasicDto>>> Discover([FromQuery] DiscoverVenuesElasticQuery query)
        {
            var venues = await _mediator.Send(query);
            return Ok(venues);
        }
    }
}
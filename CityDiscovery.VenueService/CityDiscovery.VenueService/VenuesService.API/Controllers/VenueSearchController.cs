using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Threading.Tasks;
using System.Collections.Generic;
using CityDiscovery.VenueService.VenuesService.Application.Features.Venues.Queries.SearchVenuesElastic;
using CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;

namespace CityDiscovery.VenueService.VenuesService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenueSearchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VenueSearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<VenueBasicDto>>> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Arama kelimesi boş olamaz.");
            }

            var query = new SearchVenuesElasticQuery { Keyword = keyword };
            var venues = await _mediator.Send(query);

            return Ok(venues);
        }
    }
}
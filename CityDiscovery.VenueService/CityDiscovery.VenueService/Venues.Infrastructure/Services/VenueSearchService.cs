using Elastic.Clients.Elasticsearch;
using CityDiscovery.VenueService.VenuesService.Application.Interfaces.Services;
using CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;

namespace CityDiscovery.VenueService.VenuesService.Infrastructure.Services
{
    public class VenueSearchService : IVenueSearchService
    {
        private readonly ElasticsearchClient _client;
        private readonly string _defaultIndex;

        public VenueSearchService(IConfiguration configuration)
        {
            _defaultIndex = configuration["ElasticsearchSettings:DefaultIndex"] ?? "venues";
            var uri = configuration["ElasticsearchSettings:Uri"];

            var settings = new ElasticsearchClientSettings(new Uri(uri))
                .DefaultIndex(_defaultIndex);

            _client = new ElasticsearchClient(settings);
        }

        public async Task<bool> IndexVenueAsync(VenueBasicDto venue)
        {
            var response = await _client.IndexAsync(venue, idx => idx.Index(_defaultIndex));
            return response.IsValidResponse;
        }

        public async Task<IEnumerable<VenueBasicDto>> SearchVenuesAsync(string keyword)
        {
            var response = await _client.SearchAsync<VenueBasicDto>(s => s
                .Index(_defaultIndex)
                .Query(q => q
                    .MultiMatch(m => m
                        .Query(keyword)
                        .Fields(new[] { "name", "description", "address" }) // Arama yapılacak alanlar
                        .Fuzziness(new Fuzziness("AUTO")) // Yazım hatalarını tolere etmek için
                    )
                )
            );

            if (!response.IsValidResponse)
            {
                // Loglama yapılabilir
                return new List<VenueBasicDto>();
            }

            return response.Documents;
        }
    }
}
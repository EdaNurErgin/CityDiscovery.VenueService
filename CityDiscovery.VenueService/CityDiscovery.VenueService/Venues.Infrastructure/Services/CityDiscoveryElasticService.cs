using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using CityDiscovery.VenueService.VenuesService.Application.Interfaces.Services;
using CityDiscovery.VenueService.VenuesService.Application.Features.Venues.Queries.DiscoverVenuesElastic;
using CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;
using CityDiscovery.Venues.Domain.Entities; // Venuex ve Address için

namespace CityDiscovery.VenueService.Venues.Infrastructure.Services
{
    public class CityDiscoveryElasticService : ICityDiscoveryService
    {
        private readonly ElasticsearchClient _client;
        private readonly string _defaultIndex;

        public CityDiscoveryElasticService(IConfiguration configuration)
        {
            _defaultIndex = configuration["ElasticsearchSettings:DefaultIndex"] ?? "venues";
            var uri = configuration["ElasticsearchSettings:Uri"];

            var settings = new ElasticsearchClientSettings(new Uri(uri))
                .DefaultIndex(_defaultIndex);

            _client = new ElasticsearchClient(settings);
        }

        // =====================================================================
        // 1. YAZMA (INDEXING) İŞLEMİ: Havuzu zenginleştiren yeni metodumuz
        // =====================================================================
        public async Task<bool> IndexVenueWithDetailsAsync(Venuex venue)
        {
            // Metodun en başına ekleyin
            Console.WriteLine($"--- DEBUG ELASTIC --- Name: {venue.Name}, HasAddress: {venue.Address != null}, CityName: {venue.Address?.City?.Name}");
            var elasticData = new
            {
                id = venue.Id,
                name = venue.Name,
                description = venue.Description,
                // .Name özelliklerine erişirken null kontrolü yapıyoruz
                country = venue.Address?.Country?.Name ?? "Bilinmiyor",
                city = venue.Address?.City?.Name ?? "Bilinmiyor",
                district = venue.Address?.District?.Name ?? "Bilinmiyor",
                venueCategory = venue.VenueCategories?.FirstOrDefault()?.Category?.Name ?? "Genel",
                priceLevel = venue.PriceLevel?.Value,
                isActive = venue.IsActive,
                averageRating = venue.AverageRating,
                location = new
                {
                    latitude = venue.Location.Y,
                    longitude = venue.Location.X
                }
            };

            var response = await _client.IndexAsync(elasticData, idx => idx.Index(_defaultIndex));
            return response.IsValidResponse;
        }

        // 2. OKUMA (SEARCH) İŞLEMİ: Zenginleşmiş havuzda filtreleme yapan metodumuz
  
        public async Task<IEnumerable<VenueBasicDto>> DiscoverVenuesAsync(DiscoverVenuesElasticQuery request)
        {
            var filters = new List<Query>();

            if (!string.IsNullOrWhiteSpace(request.Country))
                filters.Add(new MatchQuery("country") { Query = request.Country });

            if (!string.IsNullOrWhiteSpace(request.City))
                filters.Add(new MatchQuery("city") { Query = request.City });

            if (!string.IsNullOrWhiteSpace(request.District))
                filters.Add(new MatchQuery("district") { Query = request.District });

            if (!string.IsNullOrWhiteSpace(request.VenueCategory))
                filters.Add(new MatchQuery("venueCategory") { Query = request.VenueCategory });

            if (request.PriceLevel.HasValue)
                filters.Add(new TermQuery("priceLevel") { Value = request.PriceLevel.Value });

            if (request.IsActive.HasValue)
                filters.Add(new TermQuery("isActive") { Value = request.IsActive.Value });

            if (request.MinAverageRating.HasValue)
                filters.Add(new NumberRangeQuery("averageRating") { Gte = request.MinAverageRating.Value });

            var response = await _client.SearchAsync<VenueBasicDto>(s => s
                .Index(_defaultIndex)
                .Query(q => q
                    .Bool(b => b
                        .Filter(filters)
                    )
                )
            );

            if (!response.IsValidResponse) return new List<VenueBasicDto>();

            return response.Documents;
        }
    }
}
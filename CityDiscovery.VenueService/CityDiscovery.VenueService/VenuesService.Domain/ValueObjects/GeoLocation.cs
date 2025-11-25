using Microsoft.SqlServer.Types;

namespace CityDiscovery.Venues.Domain.ValueObjects;

public sealed class GeoLocation
{
    public double Latitude { get; }
    public double Longitude { get; }

    public SqlGeography Point =>
        SqlGeography.Point(Latitude, Longitude, 4326);

    private GeoLocation(double lat, double lon)
    {
        Latitude = lat;
        Longitude = lon;
    }

    public static GeoLocation Create(double lat, double lon)
    {
        return new GeoLocation(lat, lon);
    }

    public static GeoLocation FromDb(SqlGeography geography)
    {
        return new GeoLocation(
            geography.Lat.Value,
            geography.Long.Value
        );
    }
}

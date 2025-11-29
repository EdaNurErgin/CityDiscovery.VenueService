namespace CityDiscovery.Venues.Shared.Common.Exceptions;

public sealed class UnauthorizedAccessException : Exception
{
    public UnauthorizedAccessException()
        : base("Bu işlem için yetkiniz yok.")
    {
    }

    public UnauthorizedAccessException(string message)
        : base(message)
    {
    }
}
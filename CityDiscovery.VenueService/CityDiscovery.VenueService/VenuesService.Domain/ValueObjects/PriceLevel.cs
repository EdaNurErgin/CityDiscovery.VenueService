namespace CityDiscovery.Venues.Domain.ValueObjects;

/// <summary>
/// 1-5 arası fiyat seviyesi (örn: $ - $$$$$).
/// DB'de TINYINT olarak tutulacak.
/// </summary>
public sealed class PriceLevel : IEquatable<PriceLevel>
{
    public byte Value { get; }

    private PriceLevel(byte value)
    {
        Value = value;
    }

    public static PriceLevel Create(byte value)
    {
        if (value is < 1 or > 5)
            throw new ArgumentOutOfRangeException(nameof(value), "PriceLevel must be between 1 and 5.");

        return new PriceLevel(value);
    }

    public static PriceLevel? FromNullable(byte? value)
    {
        return value.HasValue ? Create(value.Value) : null;
    }

    #region Equality

    public bool Equals(PriceLevel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => Equals(obj as PriceLevel);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(PriceLevel? left, PriceLevel? right) =>
        Equals(left, right);

    public static bool operator !=(PriceLevel? left, PriceLevel? right) =>
        !Equals(left, right);

    #endregion

    public override string ToString() => Value.ToString();
}

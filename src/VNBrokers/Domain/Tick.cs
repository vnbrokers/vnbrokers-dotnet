namespace VNBrokers.Domain;

/// <summary>
/// Normalized trade tick.
/// </summary>
/// <param name="Symbol">Instrument symbol.</param>
/// <param name="Price">Trade price.</param>
/// <param name="Quantity">Optional trade quantity.</param>
/// <param name="ReceivedAt">Broker timestamp.</param>
/// <param name="Raw">Optional raw broker payload.</param>
public sealed record Tick(
    string Symbol,
    decimal Price,
    decimal? Quantity = null,
    string? ReceivedAt = null,
    RawPayload? Raw = null);

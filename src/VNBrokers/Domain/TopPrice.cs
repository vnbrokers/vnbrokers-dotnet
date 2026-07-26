namespace VNBrokers.Domain;

/// <summary>
/// Normalized best bid and ask price.
/// </summary>
/// <param name="Symbol">Instrument symbol.</param>
/// <param name="BidPrice">Best bid price.</param>
/// <param name="BidQuantity">Best bid quantity.</param>
/// <param name="AskPrice">Best ask price.</param>
/// <param name="AskQuantity">Best ask quantity.</param>
/// <param name="ReceivedAt">Broker timestamp.</param>
/// <param name="Raw">Optional raw broker payload.</param>
public sealed record TopPrice(
    string Symbol,
    decimal? BidPrice = null,
    decimal? BidQuantity = null,
    decimal? AskPrice = null,
    decimal? AskQuantity = null,
    string? ReceivedAt = null,
    RawPayload? Raw = null);

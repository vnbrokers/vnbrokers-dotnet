namespace VNBrokers.Domain;

/// <summary>
/// Normalized quote snapshot.
/// </summary>
/// <param name="Symbol">Instrument symbol.</param>
/// <param name="LastPrice">Last traded price.</param>
/// <param name="BidPrice">Best bid price.</param>
/// <param name="AskPrice">Best ask price.</param>
/// <param name="ReceivedAt">Broker timestamp.</param>
/// <param name="Raw">Optional raw broker payload.</param>
public sealed record Quote(
    string Symbol,
    decimal? LastPrice = null,
    decimal? BidPrice = null,
    decimal? AskPrice = null,
    string? ReceivedAt = null,
    RawPayload? Raw = null);

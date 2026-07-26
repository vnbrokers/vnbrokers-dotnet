namespace VNBrokers.Domain;

/// <summary>
/// Normalized OHLCV candle.
/// </summary>
/// <param name="Symbol">Instrument symbol.</param>
/// <param name="Interval">Candle interval or resolution.</param>
/// <param name="OpenedAt">Broker timestamp for candle open time.</param>
/// <param name="Open">Open price.</param>
/// <param name="High">High price.</param>
/// <param name="Low">Low price.</param>
/// <param name="Close">Close price.</param>
/// <param name="Volume">Traded volume.</param>
/// <param name="Raw">Optional raw broker payload.</param>
public sealed record Candle(
    string Symbol,
    string Interval,
    string OpenedAt,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume,
    RawPayload? Raw = null);

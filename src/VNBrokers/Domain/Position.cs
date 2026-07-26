namespace VNBrokers.Domain;

/// <summary>
/// Normalized account position.
/// </summary>
/// <param name="AccountId">Broker account identifier.</param>
/// <param name="Symbol">Instrument symbol.</param>
/// <param name="Quantity">Open quantity.</param>
/// <param name="AvailableQuantity">Available quantity.</param>
/// <param name="AveragePrice">Average cost or fill price.</param>
/// <param name="MarketValue">Current market value.</param>
/// <param name="Raw">Optional raw broker payload.</param>
public sealed record Position(
    string AccountId,
    string Symbol,
    decimal Quantity,
    decimal? AvailableQuantity = null,
    decimal? AveragePrice = null,
    decimal? MarketValue = null,
    RawPayload? Raw = null);

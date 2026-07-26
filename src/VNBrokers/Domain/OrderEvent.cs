namespace VNBrokers.Domain;

/// <summary>
/// Realtime order event.
/// </summary>
/// <param name="Broker">Broker name.</param>
/// <param name="AccountId">Optional account identifier.</param>
/// <param name="OrderId">Broker order identifier.</param>
/// <param name="Symbol">Optional instrument symbol.</param>
/// <param name="Status">Normalized order status.</param>
/// <param name="RawStatus">Raw broker status value.</param>
/// <param name="FilledQuantity">Raw filled quantity value.</param>
/// <param name="ReceivedAt">Broker receive or event timestamp.</param>
/// <param name="Raw">Raw broker payload.</param>
public sealed record OrderEvent(
    string Broker,
    string? AccountId,
    string OrderId,
    string? Symbol,
    OrderStatus Status,
    string? RawStatus,
    string? FilledQuantity,
    string? ReceivedAt,
    RawPayload Raw);

namespace VNBrokers.Domain;

/// <summary>
/// Normalized broker order.
/// </summary>
/// <param name="Broker">Broker name.</param>
/// <param name="AccountId">Broker account identifier.</param>
/// <param name="OrderId">Broker order identifier.</param>
/// <param name="Symbol">Instrument symbol.</param>
/// <param name="Side">Order side.</param>
/// <param name="OrderType">Order type.</param>
/// <param name="Quantity">Order quantity.</param>
/// <param name="Status">Order status.</param>
/// <param name="Price">Optional order price.</param>
/// <param name="Raw">Optional raw broker payload.</param>
public sealed record Order(
    string Broker,
    string AccountId,
    string OrderId,
    string Symbol,
    OrderSide Side,
    OrderType OrderType,
    decimal Quantity,
    OrderStatus Status,
    decimal? Price = null,
    RawPayload? Raw = null);

/// <summary>
/// Request to place a broker order.
/// </summary>
/// <param name="AccountId">Broker account identifier.</param>
/// <param name="Symbol">Instrument symbol.</param>
/// <param name="Side">Order side.</param>
/// <param name="OrderType">Order type.</param>
/// <param name="Quantity">Order quantity.</param>
/// <param name="Price">Optional order price.</param>
/// <param name="TimeInForce">Order time-in-force instruction.</param>
public sealed record PlaceOrderRequest(
    string AccountId,
    string Symbol,
    OrderSide Side,
    OrderType OrderType,
    decimal Quantity,
    decimal? Price = null,
    TimeInForce TimeInForce = TimeInForce.Day);

/// <summary>
/// Broker response after placing an order.
/// </summary>
/// <param name="OrderId">Broker order identifier.</param>
/// <param name="Status">Initial order status.</param>
/// <param name="Raw">Optional raw broker payload.</param>
public sealed record PlaceOrderResponse(
    string OrderId,
    OrderStatus Status,
    RawPayload? Raw = null);

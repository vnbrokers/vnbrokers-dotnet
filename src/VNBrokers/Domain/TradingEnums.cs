namespace VNBrokers.Domain;

/// <summary>
/// Side of an order.
/// </summary>
public enum OrderSide
{
    /// <summary>Buy order.</summary>
    Buy,

    /// <summary>Sell order.</summary>
    Sell,
}

/// <summary>
/// Normalized order type.
/// </summary>
public enum OrderType
{
    /// <summary>Limit order.</summary>
    Limit,

    /// <summary>Market order.</summary>
    Market,

    /// <summary>Stop order.</summary>
    Stop,

    /// <summary>Unknown or broker-specific order type.</summary>
    Unknown,
}

/// <summary>
/// Order time-in-force instruction.
/// </summary>
public enum TimeInForce
{
    /// <summary>Valid for the current trading day.</summary>
    Day,

    /// <summary>Immediate-or-cancel.</summary>
    Ioc,

    /// <summary>Fill-or-kill.</summary>
    Fok,

    /// <summary>Unknown or broker-specific instruction.</summary>
    Unknown,
}

/// <summary>
/// Normalized lifecycle status of an order.
/// </summary>
public enum OrderStatus
{
    /// <summary>Unknown or unmapped status.</summary>
    Unknown,

    /// <summary>Order is pending broker processing.</summary>
    Pending,

    /// <summary>Order was accepted by the broker.</summary>
    Accepted,

    /// <summary>Order has been partially filled.</summary>
    PartiallyFilled,

    /// <summary>Order has been fully filled.</summary>
    Filled,

    /// <summary>Order has been cancelled.</summary>
    Cancelled,

    /// <summary>Order was rejected.</summary>
    Rejected,
}

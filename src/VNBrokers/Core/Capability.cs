using System.Reflection;
using System.Runtime.Serialization;

namespace VNBrokers.Core;

/// <summary>
/// Broker capability identifiers used for feature discovery and enforcement.
/// </summary>
public enum Capability
{
    /// <summary>List trading accounts.</summary>
    [EnumMember(Value = "trading.accounts.list")]
    TradingAccountsList,

    /// <summary>Place trading orders.</summary>
    [EnumMember(Value = "trading.orders.place")]
    TradingOrdersPlace,

    /// <summary>Cancel trading orders.</summary>
    [EnumMember(Value = "trading.orders.cancel")]
    TradingOrdersCancel,

    /// <summary>List trading positions.</summary>
    [EnumMember(Value = "trading.positions.list")]
    TradingPositionsList,

    /// <summary>Subscribe to realtime order updates.</summary>
    [EnumMember(Value = "trading.realtime.orders")]
    TradingRealtimeOrders,

    /// <summary>Subscribe to realtime position updates.</summary>
    [EnumMember(Value = "trading.realtime.positions")]
    TradingRealtimePositions,

    /// <summary>Read brokerage care-by accounts.</summary>
    [EnumMember(Value = "brokerage.accounts.care_by")]
    BrokerageCareBy,

    /// <summary>List market data symbols.</summary>
    [EnumMember(Value = "marketdata.symbols.list")]
    MarketDataSymbolsList,

    /// <summary>Read market data quote snapshots.</summary>
    [EnumMember(Value = "marketdata.quotes.snapshot")]
    MarketDataQuotes,

    /// <summary>Read historical candles.</summary>
    [EnumMember(Value = "marketdata.candles.history")]
    MarketDataCandles,

    /// <summary>Subscribe to realtime ticks.</summary>
    [EnumMember(Value = "marketdata.realtime.ticks")]
    MarketDataRealtimeTicks,

    /// <summary>Subscribe to realtime top price updates.</summary>
    [EnumMember(Value = "marketdata.realtime.top_price")]
    MarketDataRealtimeTopPrice,

    /// <summary>Subscribe to realtime candles.</summary>
    [EnumMember(Value = "marketdata.realtime.candles")]
    MarketDataRealtimeCandles,

    /// <summary>Subscribe to raw market data streams.</summary>
    [EnumMember(Value = "marketdata.realtime.raw")]
    MarketDataRealtimeRaw,
}

/// <summary>
/// Helper methods for broker capability identifiers.
/// </summary>
public static class CapabilityExtensions
{
    /// <summary>
    /// Returns the stable wire value used by the Python SDK.
    /// </summary>
    /// <param name="capability">Capability to convert.</param>
    /// <returns>Wire value for the capability.</returns>
    public static string ToWireValue(this Capability capability)
    {
        var member = typeof(Capability).GetMember(capability.ToString()).Single();
        return member.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? capability.ToString();
    }
}

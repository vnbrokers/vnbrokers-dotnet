using VNBrokers.Core;

namespace VNBrokers.Brokers.Dnse;

/// <summary>
/// DNSE capability declarations.
/// </summary>
public static class DnseCapabilities
{
    /// <summary>
    /// Gets capabilities implemented by the DNSE adapter.
    /// </summary>
    public static IReadOnlyCollection<Capability> All { get; } =
    [
        Capability.TradingAccountsList,
        Capability.TradingOrdersPlace,
        Capability.TradingOrdersCancel,
        Capability.TradingPositionsList,
        Capability.TradingRealtimeOrders,
        Capability.TradingRealtimePositions,
        Capability.BrokerageCareBy,
        Capability.MarketDataSymbolsList,
        Capability.MarketDataQuotes,
        Capability.MarketDataCandles,
        Capability.MarketDataRealtimeTicks,
        Capability.MarketDataRealtimeTopPrice,
        Capability.MarketDataRealtimeCandles,
        Capability.MarketDataRealtimeRaw,
    ];
}

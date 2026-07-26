using VNBrokers.Transport;

namespace VNBrokers.Brokers.Dnse;

/// <summary>
/// DNSE trading service group.
/// </summary>
public sealed class DnseTradingService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DnseTradingService"/> class.
    /// </summary>
    /// <param name="broker">DNSE broker.</param>
    /// <param name="config">DNSE configuration.</param>
    /// <param name="transport">HTTP transport.</param>
    public DnseTradingService(DnseBroker broker, DnseConfig config, IHttpTransport transport)
    {
        Accounts = new DnseTradingAccountsService(broker, config, transport);
        Orders = new DnseTradingOrdersService(broker, config, transport);
        Positions = new DnseTradingPositionsService(broker, config, transport);
    }

    /// <summary>
    /// Gets account query operations.
    /// </summary>
    public DnseTradingAccountsService Accounts { get; }

    /// <summary>
    /// Gets order operations.
    /// </summary>
    public DnseTradingOrdersService Orders { get; }

    /// <summary>
    /// Gets position query operations.
    /// </summary>
    public DnseTradingPositionsService Positions { get; }
}

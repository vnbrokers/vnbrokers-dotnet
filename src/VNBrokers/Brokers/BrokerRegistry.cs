using VNBrokers.Brokers.Dnse;
using VNBrokers.Brokers.Ssi;
using VNBrokers.Brokers.Tcbs;
using VNBrokers.Core;

namespace VNBrokers.Brokers;

/// <summary>
/// Built-in broker registry matching the Python SDK factory helpers.
/// </summary>
public static class BrokerRegistry
{
    /// <summary>
    /// Gets the default broker factory.
    /// </summary>
    public static BrokerFactory DefaultFactory { get; } = new(
    [
        new BrokerRegistration<DnseConfig, DnseBroker>("dnse", config => new DnseBroker(config)),
        new BrokerRegistration<SsiConfig, SsiBroker>("ssi", config => new SsiBroker(config)),
        new BrokerRegistration<TcbsConfig, TcbsBroker>("tcbs", config => new TcbsBroker(config)),
    ]);

    /// <summary>
    /// Lists built-in broker names.
    /// </summary>
    /// <returns>Sorted broker names.</returns>
    public static IReadOnlyList<string> ListBrokers() => DefaultFactory.ListBrokers();

    /// <summary>
    /// Creates a broker from an explicit config.
    /// </summary>
    /// <param name="name">Broker name.</param>
    /// <param name="config">Broker config.</param>
    /// <returns>Created broker adapter.</returns>
    public static IBroker CreateBroker(string name, BrokerConfig config)
        => DefaultFactory.CreateBroker(name, config);

    /// <summary>
    /// Creates a broker from a config factory.
    /// </summary>
    /// <typeparam name="TConfig">Broker config type.</typeparam>
    /// <param name="name">Broker name.</param>
    /// <param name="configFactory">Configuration factory.</param>
    /// <returns>Created broker adapter.</returns>
    public static IBroker CreateBroker<TConfig>(string name, Func<TConfig> configFactory)
        where TConfig : BrokerConfig
        => DefaultFactory.CreateBroker(name, configFactory);

    /// <summary>
    /// Gets a built-in broker registration by name.
    /// </summary>
    /// <param name="name">Broker name.</param>
    /// <returns>Broker registration.</returns>
    public static IBrokerRegistration GetBrokerRegistration(string name)
        => DefaultFactory.GetRegistration(name);
}

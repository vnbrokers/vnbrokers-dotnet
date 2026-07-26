using VNBrokers.Core;

namespace VNBrokers.Brokers.Tcbs;

/// <summary>
/// TCBS broker adapter skeleton.
/// </summary>
public sealed class TcbsBroker : BrokerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TcbsBroker"/> class.
    /// </summary>
    /// <param name="config">TCBS configuration.</param>
    public TcbsBroker(TcbsConfig config)
        : base("tcbs", [])
    {
        Config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Gets the TCBS configuration.
    /// </summary>
    public TcbsConfig Config { get; }
}

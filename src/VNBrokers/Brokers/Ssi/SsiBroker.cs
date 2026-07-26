using VNBrokers.Core;

namespace VNBrokers.Brokers.Ssi;

/// <summary>
/// SSI broker adapter skeleton.
/// </summary>
public sealed class SsiBroker : BrokerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SsiBroker"/> class.
    /// </summary>
    /// <param name="config">SSI configuration.</param>
    public SsiBroker(SsiConfig config)
        : base("ssi", [])
    {
        Config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Gets the SSI configuration.
    /// </summary>
    public SsiConfig Config { get; }
}

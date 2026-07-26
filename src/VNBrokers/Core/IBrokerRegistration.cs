namespace VNBrokers.Core;

/// <summary>
/// Non-generic broker registration contract used by the broker factory.
/// </summary>
public interface IBrokerRegistration
{
    /// <summary>
    /// Gets the normalized broker name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the broker adapter type.
    /// </summary>
    Type BrokerType { get; }

    /// <summary>
    /// Gets the broker configuration type.
    /// </summary>
    Type ConfigType { get; }

    /// <summary>
    /// Creates a broker from an explicit config object.
    /// </summary>
    /// <param name="config">Broker configuration.</param>
    /// <returns>Created broker adapter.</returns>
    IBroker Create(BrokerConfig config);

    /// <summary>
    /// Creates a broker from a default configuration.
    /// </summary>
    /// <returns>Created broker adapter.</returns>
    IBroker CreateDefault();
}

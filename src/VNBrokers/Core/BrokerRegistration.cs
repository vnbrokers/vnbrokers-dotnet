namespace VNBrokers.Core;

/// <summary>
/// Typed broker registration used by <see cref="BrokerFactory"/>.
/// </summary>
/// <typeparam name="TConfig">Broker configuration type.</typeparam>
/// <typeparam name="TBroker">Broker adapter type.</typeparam>
public sealed class BrokerRegistration<TConfig, TBroker> : IBrokerRegistration
    where TConfig : BrokerConfig, new()
    where TBroker : IBroker
{
    private readonly Func<TConfig, TBroker> factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="BrokerRegistration{TConfig, TBroker}"/> class.
    /// </summary>
    /// <param name="name">Broker name.</param>
    /// <param name="factory">Broker factory delegate.</param>
    public BrokerRegistration(string name, Func<TConfig, TBroker> factory)
    {
        Name = Normalize(name);
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public Type BrokerType => typeof(TBroker);

    /// <inheritdoc />
    public Type ConfigType => typeof(TConfig);

    /// <inheritdoc />
    public IBroker Create(BrokerConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);
        if (config is not TConfig typedConfig)
        {
            throw new ArgumentException(
                $"Broker '{Name}' requires config type {typeof(TConfig).Name}.",
                nameof(config));
        }

        return factory(typedConfig);
    }

    /// <inheritdoc />
    public IBroker CreateDefault() => factory(new TConfig());

    private static string Normalize(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Broker name cannot be blank.", nameof(name));
        }

        return name.Trim().ToLowerInvariant();
    }
}

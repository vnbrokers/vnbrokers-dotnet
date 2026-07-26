namespace VNBrokers.Core;

/// <summary>
/// Factory for creating registered broker adapters.
/// </summary>
public sealed class BrokerFactory
{
    private readonly Dictionary<string, IBrokerRegistration> registrations = new(StringComparer.Ordinal);

    /// <summary>
    /// Initializes a new instance of the <see cref="BrokerFactory"/> class.
    /// </summary>
    /// <param name="registrations">Initial broker registrations.</param>
    public BrokerFactory(IEnumerable<IBrokerRegistration>? registrations = null)
    {
        foreach (var registration in registrations ?? [])
        {
            Register(registration);
        }
    }

    /// <summary>
    /// Registers or replaces a broker registration.
    /// </summary>
    /// <param name="registration">Broker registration.</param>
    public void Register(IBrokerRegistration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        registrations[Normalize(registration.Name)] = registration;
    }

    /// <summary>
    /// Lists registered broker names.
    /// </summary>
    /// <returns>Sorted broker names.</returns>
    public IReadOnlyList<string> ListBrokers() => registrations.Keys.Order().ToArray();

    /// <summary>
    /// Gets a broker registration by name.
    /// </summary>
    /// <param name="name">Broker name.</param>
    /// <returns>Matching broker registration.</returns>
    public IBrokerRegistration GetRegistration(string name)
    {
        var normalized = Normalize(name);
        if (!registrations.TryGetValue(normalized, out var registration))
        {
            throw new ArgumentException($"Unsupported broker: {normalized}", nameof(name));
        }

        return registration;
    }

    /// <summary>
    /// Creates a broker from an explicit config object.
    /// </summary>
    /// <param name="name">Broker name.</param>
    /// <param name="config">Broker configuration.</param>
    /// <returns>Created broker adapter.</returns>
    public IBroker CreateBroker(string name, BrokerConfig config)
        => GetRegistration(name).Create(config);

    /// <summary>
    /// Creates a broker from a config factory.
    /// </summary>
    /// <typeparam name="TConfig">Broker configuration type.</typeparam>
    /// <param name="name">Broker name.</param>
    /// <param name="configFactory">Configuration factory.</param>
    /// <returns>Created broker adapter.</returns>
    public IBroker CreateBroker<TConfig>(string name, Func<TConfig> configFactory)
        where TConfig : BrokerConfig
    {
        ArgumentNullException.ThrowIfNull(configFactory);
        return GetRegistration(name).Create(configFactory());
    }

    /// <summary>
    /// Creates a broker with the registration's default configuration.
    /// </summary>
    /// <param name="name">Broker name.</param>
    /// <returns>Created broker adapter.</returns>
    public IBroker CreateBroker(string name) => GetRegistration(name).CreateDefault();

    private static string Normalize(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Broker name cannot be blank.", nameof(name));
        }

        return name.Trim().ToLowerInvariant();
    }
}

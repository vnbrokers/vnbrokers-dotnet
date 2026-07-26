using VNBrokers.Errors;

namespace VNBrokers.Core;

/// <summary>
/// Base class for broker adapters with capability enforcement.
/// </summary>
public abstract class BrokerBase : IBroker
{
    private readonly HashSet<Capability> capabilities;

    /// <summary>
    /// Initializes a new instance of the <see cref="BrokerBase"/> class.
    /// </summary>
    /// <param name="name">Normalized broker name.</param>
    /// <param name="capabilities">Supported capabilities.</param>
    protected BrokerBase(string name, IEnumerable<Capability> capabilities)
    {
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Broker name cannot be blank.", nameof(name))
            : name;
        this.capabilities = new HashSet<Capability>(capabilities);
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public IReadOnlyCollection<Capability> Capabilities => capabilities;

    /// <inheritdoc />
    public bool Supports(Capability capability) => capabilities.Contains(capability);

    /// <inheritdoc />
    public void RequireCapability(Capability capability)
    {
        if (!Supports(capability))
        {
            throw new UnsupportedCapabilityException(Name, capability);
        }
    }
}

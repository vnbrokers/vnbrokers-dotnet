using VNBrokers.Core;

namespace VNBrokers.Errors;

/// <summary>
/// Represents a request for a capability the broker does not support.
/// </summary>
public sealed class UnsupportedCapabilityException : CapabilityException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedCapabilityException"/> class.
    /// </summary>
    /// <param name="broker">Broker name.</param>
    /// <param name="capability">Unsupported capability.</param>
    public UnsupportedCapabilityException(string broker, Capability capability)
        : base($"Broker '{broker}' does not support capability '{capability.ToWireValue()}'", broker)
    {
        Capability = capability;
    }

    /// <summary>
    /// Gets the unsupported capability.
    /// </summary>
    public Capability Capability { get; }
}

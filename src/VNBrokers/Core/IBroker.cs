namespace VNBrokers.Core;

/// <summary>
/// Common contract implemented by all broker adapters.
/// </summary>
public interface IBroker
{
    /// <summary>
    /// Gets the normalized broker name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the capabilities supported by the broker.
    /// </summary>
    IReadOnlyCollection<Capability> Capabilities { get; }

    /// <summary>
    /// Returns whether the broker supports a capability.
    /// </summary>
    /// <param name="capability">Capability to check.</param>
    /// <returns><see langword="true"/> when supported.</returns>
    bool Supports(Capability capability);

    /// <summary>
    /// Throws if the broker does not support a capability.
    /// </summary>
    /// <param name="capability">Required capability.</param>
    void RequireCapability(Capability capability);
}

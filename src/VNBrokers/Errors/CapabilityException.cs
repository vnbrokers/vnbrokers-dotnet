using System.Text.Json;

namespace VNBrokers.Errors;

/// <summary>
/// Represents a broker capability failure.
/// </summary>
public class CapabilityException : VnBrokerException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CapabilityException"/> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="broker">Optional broker name.</param>
    /// <param name="raw">Optional raw payload.</param>
    /// <param name="innerException">Optional inner exception.</param>
    public CapabilityException(
        string message,
        string? broker = null,
        JsonElement? raw = null,
        Exception? innerException = null)
        : base(message, broker, raw, innerException)
    {
    }
}

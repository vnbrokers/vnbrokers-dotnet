using System.Text.Json;

namespace VNBrokers.Errors;

/// <summary>
/// Base exception for VNBrokers SDK errors.
/// </summary>
public class VnBrokerException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VnBrokerException"/> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="broker">Optional broker name.</param>
    /// <param name="raw">Optional raw payload.</param>
    /// <param name="innerException">Optional inner exception.</param>
    public VnBrokerException(
        string message,
        string? broker = null,
        JsonElement? raw = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Broker = broker;
        Raw = raw;
    }

    /// <summary>
    /// Gets the broker associated with the error, when known.
    /// </summary>
    public string? Broker { get; }

    /// <summary>
    /// Gets the raw broker payload associated with the error, when available.
    /// </summary>
    public JsonElement? Raw { get; }
}

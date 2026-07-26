using System.Text.Json;

namespace VNBrokers.Errors;

/// <summary>
/// Represents a broker-side rejection response.
/// </summary>
public sealed class BrokerRejectedException : VnBrokerException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BrokerRejectedException"/> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="broker">Optional broker name.</param>
    /// <param name="code">Optional broker error code.</param>
    /// <param name="raw">Optional raw payload.</param>
    /// <param name="innerException">Optional inner exception.</param>
    public BrokerRejectedException(
        string message,
        string? broker = null,
        string? code = null,
        JsonElement? raw = null,
        Exception? innerException = null)
        : base(message, broker, raw, innerException)
    {
        Code = code;
    }

    /// <summary>
    /// Gets the broker-specific rejection code, when available.
    /// </summary>
    public string? Code { get; }
}

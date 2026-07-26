using System.Text.Json;

namespace VNBrokers.Errors;

/// <summary>
/// Represents a broker rate-limit response.
/// </summary>
public sealed class RateLimitException : VnBrokerException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitException"/> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="broker">Optional broker name.</param>
    /// <param name="raw">Optional raw payload.</param>
    /// <param name="innerException">Optional inner exception.</param>
    public RateLimitException(
        string message,
        string? broker = null,
        JsonElement? raw = null,
        Exception? innerException = null)
        : base(message, broker, raw, innerException)
    {
    }
}

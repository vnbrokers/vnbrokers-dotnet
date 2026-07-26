using System.Text.Json;

namespace VNBrokers.Errors;

/// <summary>
/// Represents a failure to decode a broker payload.
/// </summary>
public sealed class DecodeException : VnBrokerException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DecodeException"/> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="broker">Optional broker name.</param>
    /// <param name="raw">Optional raw payload.</param>
    /// <param name="innerException">Optional inner exception.</param>
    public DecodeException(
        string message,
        string? broker = null,
        JsonElement? raw = null,
        Exception? innerException = null)
        : base(message, broker, raw, innerException)
    {
    }
}

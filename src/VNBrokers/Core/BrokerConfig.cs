namespace VNBrokers.Core;

/// <summary>
/// Base configuration shared by broker adapters.
/// </summary>
public class BrokerConfig
{
    /// <summary>
    /// Gets or sets the broker HTTP base URL.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the broker realtime stream URL.
    /// </summary>
    public string? StreamUrl { get; set; }

    /// <summary>
    /// Gets or sets request timeout in seconds.
    /// </summary>
    public double TimeoutSeconds { get; set; } = 30.0;
}

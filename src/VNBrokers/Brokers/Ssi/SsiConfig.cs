using VNBrokers.Core;

namespace VNBrokers.Brokers.Ssi;

/// <summary>
/// SSI broker configuration.
/// </summary>
public class SsiConfig : BrokerConfig
{
    /// <summary>Gets or sets the SSI API key.</summary>
    public string? ApiKey { get; set; }

    /// <summary>Gets or sets the SSI API secret.</summary>
    public string? ApiSecret { get; set; }
}

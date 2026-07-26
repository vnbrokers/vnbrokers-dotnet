using VNBrokers.Core;

namespace VNBrokers.Brokers.Tcbs;

/// <summary>
/// TCBS broker configuration.
/// </summary>
public class TcbsConfig : BrokerConfig
{
    /// <summary>Gets or sets the TCBS token.</summary>
    public string? Token { get; set; }
}

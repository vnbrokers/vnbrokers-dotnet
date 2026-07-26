using VNBrokers.Core;
using VNBrokers.Transport;

namespace VNBrokers.Brokers.Dnse;

/// <summary>
/// DNSE broker adapter.
/// </summary>
public sealed class DnseBroker : BrokerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DnseBroker"/> class.
    /// </summary>
    /// <param name="config">DNSE configuration.</param>
    /// <param name="httpTransport">Optional HTTP transport for tests or custom clients.</param>
    public DnseBroker(DnseConfig config, IHttpTransport? httpTransport = null)
        : base("dnse", DnseCapabilities.All)
    {
        Config = config ?? throw new ArgumentNullException(nameof(config));
        HttpTransport = httpTransport ?? CreateDefaultHttpTransport(config);
        Auth = new DnseAuthService(Config, HttpTransport);
        Trading = new DnseTradingService(this, Config, HttpTransport);
    }

    /// <summary>
    /// Gets the DNSE configuration.
    /// </summary>
    public DnseConfig Config { get; }

    /// <summary>
    /// Gets the HTTP transport used by DNSE services.
    /// </summary>
    public IHttpTransport HttpTransport { get; }

    /// <summary>
    /// Gets the DNSE authentication service.
    /// </summary>
    public DnseAuthService Auth { get; }

    /// <summary>
    /// Gets DNSE trading services.
    /// </summary>
    public DnseTradingService Trading { get; }

    private static HttpClientTransport CreateDefaultHttpTransport(DnseConfig config)
    {
        var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds),
        };
        return new HttpClientTransport(httpClient);
    }
}

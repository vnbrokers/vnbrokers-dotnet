namespace VNBrokers.Brokers.Dnse;

/// <summary>
/// Creates DNSE authentication helpers from configuration.
/// </summary>
public sealed class DnseAuth
{
    private readonly DnseConfig config;

    /// <summary>
    /// Initializes a new instance of the <see cref="DnseAuth"/> class.
    /// </summary>
    /// <param name="config">DNSE configuration.</param>
    public DnseAuth(DnseConfig config)
    {
        this.config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Creates a DNSE signer when both API key and secret are configured.
    /// </summary>
    /// <returns>Signer or <see langword="null"/>.</returns>
    public DnseSigner? CreateSigner()
    {
        if (string.IsNullOrEmpty(config.ApiKey) || string.IsNullOrEmpty(config.ApiSecret))
        {
            return null;
        }

        return new DnseSigner(config.ApiKey, config.ApiSecret);
    }
}

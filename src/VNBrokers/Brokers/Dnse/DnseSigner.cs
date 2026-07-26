using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using VNBrokers.Transport;

namespace VNBrokers.Brokers.Dnse;

/// <summary>
/// Signs DNSE HTTP requests with HMAC-SHA256 headers.
/// </summary>
public sealed class DnseSigner
{
    private readonly Func<DateTimeOffset> now;
    private readonly Func<string> nonce;

    /// <summary>
    /// Initializes a new instance of the <see cref="DnseSigner"/> class.
    /// </summary>
    /// <param name="apiKey">DNSE API key.</param>
    /// <param name="apiSecret">DNSE API secret.</param>
    /// <param name="now">Clock used for deterministic tests.</param>
    /// <param name="nonce">Nonce generator used for deterministic tests.</param>
    public DnseSigner(
        string apiKey,
        string apiSecret,
        Func<DateTimeOffset>? now = null,
        Func<string>? nonce = null)
    {
        ApiKey = string.IsNullOrWhiteSpace(apiKey)
            ? throw new ArgumentException("API key cannot be blank.", nameof(apiKey))
            : apiKey;
        ApiSecret = string.IsNullOrWhiteSpace(apiSecret)
            ? throw new ArgumentException("API secret cannot be blank.", nameof(apiSecret))
            : apiSecret;
        this.now = now ?? (() => DateTimeOffset.UtcNow);
        this.nonce = nonce ?? CreateNonce;
    }

    /// <summary>
    /// Gets the DNSE API key.
    /// </summary>
    public string ApiKey { get; }

    /// <summary>
    /// Gets the DNSE API secret.
    /// </summary>
    public string ApiSecret { get; }

    /// <summary>
    /// Signs a transport request.
    /// </summary>
    /// <param name="request">Request to sign.</param>
    /// <returns>Signed request.</returns>
    public HttpTransportRequest Sign(HttpTransportRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var date = now()
            .ToUniversalTime()
            .ToString("ddd, dd MMM yyyy HH:mm:ss +0000", CultureInfo.InvariantCulture);
        var nonceValue = nonce();
        var path = string.IsNullOrEmpty(request.Url.AbsolutePath) ? "/" : request.Url.AbsolutePath;
        var signingString =
            $"(request-target): {request.Method.Method.ToLowerInvariant()} {path}\n" +
            $"x-aux-date: {date}\n" +
            $"nonce: {nonceValue}";
        var digest = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(ApiSecret),
            Encoding.UTF8.GetBytes(signingString));
        var signature = Uri.EscapeDataString(Convert.ToBase64String(digest));

        var headers = new Dictionary<string, string>(
            request.Headers,
            StringComparer.OrdinalIgnoreCase)
        {
            ["X-API-Key"] = ApiKey,
            ["X-Aux-Date"] = date,
            ["X-Signature"] =
                $"Signature keyId=\"{ApiKey}\",algorithm=\"hmac-sha256\"," +
                $"headers=\"(request-target) x-aux-date\",signature=\"{signature}\"," +
                $"nonce=\"{nonceValue}\"",
        };

        return request with { Headers = headers };
    }

    private static string CreateNonce()
    {
        Span<byte> bytes = stackalloc byte[16];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}

using System.Text.Json;
using System.Text.Json.Nodes;
using VNBrokers.Domain;
using VNBrokers.Transport;

namespace VNBrokers.Brokers.Dnse;

/// <summary>
/// DNSE authentication endpoint service.
/// </summary>
public sealed class DnseAuthService
{
    private readonly DnseConfig config;
    private readonly DnseHttpSender sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="DnseAuthService"/> class.
    /// </summary>
    /// <param name="config">DNSE configuration.</param>
    /// <param name="transport">HTTP transport.</param>
    public DnseAuthService(DnseConfig config, IHttpTransport transport)
    {
        this.config = config ?? throw new ArgumentNullException(nameof(config));
        sender = new DnseHttpSender(config, transport);
    }

    /// <summary>
    /// Requests DNSE to send an email OTP.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Raw DNSE response payload.</returns>
    public async Task<RawPayload> SendEmailOtpAsync(CancellationToken cancellationToken = default)
    {
        var response = await sender.SendAsync(
            new HttpTransportRequest(
                HttpMethod.Post,
                new Uri($"{BaseUrl()}/registration/send-email-otp"),
                ApiHeaders()),
            cancellationToken).ConfigureAwait(false);

        return new RawPayload("dnse", ExpectJsonObject(response));
    }

    /// <summary>
    /// Requests a DNSE trading token.
    /// </summary>
    /// <param name="otpType">OTP type.</param>
    /// <param name="passcode">OTP passcode.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Raw DNSE response payload.</returns>
    public async Task<RawPayload> GetTradingTokenAsync(
        string otpType,
        string passcode,
        CancellationToken cancellationToken = default)
    {
        var headers = ApiHeaders();
        headers["Content-Type"] = "application/json";

        var response = await sender.SendAsync(
            new HttpTransportRequest(
                HttpMethod.Post,
                new Uri($"{BaseUrl()}/registration/trading-token"),
                headers,
                JsonSerializer.SerializeToNode(new { otpType, passcode })),
            cancellationToken).ConfigureAwait(false);

        return new RawPayload("dnse", ExpectJsonObject(response));
    }

    private string BaseUrl() => (config.BaseUrl ?? "https://openapi.dnse.com.vn").TrimEnd('/');

    private Dictionary<string, string> ApiHeaders()
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrEmpty(config.ApiKey))
        {
            headers["X-API-Key"] = config.ApiKey;
        }

        return headers;
    }

    private static JsonElement ExpectJsonObject(HttpTransportResponse response)
    {
        if (response.JsonBody is not { } body || body.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException("Expected DNSE response body to be a JSON object.");
        }

        return body;
    }
}

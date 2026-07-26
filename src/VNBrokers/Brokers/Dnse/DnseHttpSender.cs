using System.Text.Json;
using VNBrokers.Errors;
using VNBrokers.Transport;

namespace VNBrokers.Brokers.Dnse;

/// <summary>
/// Shared DNSE HTTP sender for signing and rejection handling.
/// </summary>
public sealed class DnseHttpSender
{
    private readonly DnseConfig config;
    private readonly IHttpTransport transport;

    /// <summary>
    /// Initializes a new instance of the <see cref="DnseHttpSender"/> class.
    /// </summary>
    /// <param name="config">DNSE configuration.</param>
    /// <param name="transport">HTTP transport.</param>
    public DnseHttpSender(DnseConfig config, IHttpTransport transport)
    {
        this.config = config ?? throw new ArgumentNullException(nameof(config));
        this.transport = transport ?? throw new ArgumentNullException(nameof(transport));
    }

    /// <summary>
    /// Sends a DNSE request.
    /// </summary>
    /// <param name="request">Request to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP response.</returns>
    public async Task<HttpTransportResponse> SendAsync(
        HttpTransportRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var signer = new DnseAuth(config).CreateSigner();
        if (signer is not null)
        {
            request = signer.Sign(request);
        }

        var response = await transport.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode >= 400)
        {
            ThrowRejected(response);
        }

        return response;
    }

    private static void ThrowRejected(HttpTransportResponse response)
    {
        var message = $"DNSE request failed with status {response.StatusCode}";
        string? code = null;
        JsonElement? raw = null;

        if (response.JsonBody is { } body)
        {
            raw = body;
            if (body.ValueKind == JsonValueKind.Object)
            {
                if (body.TryGetProperty("message", out var messageElement)
                    && messageElement.ValueKind == JsonValueKind.String)
                {
                    message = messageElement.GetString() ?? message;
                }

                if (body.TryGetProperty("code", out var codeElement)
                    && codeElement.ValueKind is JsonValueKind.String or JsonValueKind.Number)
                {
                    code = codeElement.ToString();
                }
            }
        }

        throw new BrokerRejectedException(message, broker: "dnse", code: code, raw: raw);
    }
}

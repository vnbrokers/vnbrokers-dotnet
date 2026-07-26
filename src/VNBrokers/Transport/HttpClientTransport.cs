using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace VNBrokers.Transport;

/// <summary>
/// <see cref="HttpClient"/> implementation of <see cref="IHttpTransport"/>.
/// </summary>
public sealed class HttpClientTransport : IHttpTransport
{
    private readonly HttpClient httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientTransport"/> class.
    /// </summary>
    /// <param name="httpClient">HTTP client to use.</param>
    public HttpClientTransport(HttpClient httpClient)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<HttpTransportResponse> SendAsync(
        HttpTransportRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var message = new HttpRequestMessage(request.Method, request.Url);
        foreach (var (key, value) in request.Headers)
        {
            message.Headers.TryAddWithoutValidation(key, value);
        }

        if (request.JsonBody is not null)
        {
            message.Content = new StringContent(
                request.JsonBody.ToJsonString(),
                Encoding.UTF8,
                "application/json");
        }

        using var response = await httpClient.SendAsync(message, cancellationToken).ConfigureAwait(false);
        var content = response.Content is null
            ? string.Empty
            : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var headers = ReadHeaders(response);
        var mediaType = response.Content?.Headers.ContentType?.MediaType;

        if (IsJson(mediaType) && !string.IsNullOrWhiteSpace(content))
        {
            using var document = JsonDocument.Parse(content);
            return new HttpTransportResponse(
                (int)response.StatusCode,
                headers,
                document.RootElement.Clone(),
                null);
        }

        return new HttpTransportResponse((int)response.StatusCode, headers, null, content);
    }

    private static bool IsJson(string? mediaType)
        => string.Equals(mediaType, "application/json", StringComparison.OrdinalIgnoreCase)
        || string.Equals(mediaType, "text/json", StringComparison.OrdinalIgnoreCase)
        || (mediaType?.EndsWith("+json", StringComparison.OrdinalIgnoreCase) ?? false);

    private static Dictionary<string, string> ReadHeaders(HttpResponseMessage response)
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var header in response.Headers)
        {
            headers[header.Key] = string.Join(",", header.Value);
        }

        foreach (var header in response.Content.Headers)
        {
            headers[header.Key] = string.Join(",", header.Value);
        }

        return headers;
    }
}

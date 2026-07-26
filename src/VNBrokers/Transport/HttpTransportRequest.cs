using System.Text.Json.Nodes;

namespace VNBrokers.Transport;

/// <summary>
/// Broker HTTP transport request.
/// </summary>
public sealed record HttpTransportRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpTransportRequest"/> class.
    /// </summary>
    /// <param name="method">HTTP method.</param>
    /// <param name="url">Request URL.</param>
    /// <param name="headers">Request headers.</param>
    /// <param name="jsonBody">Optional JSON request body.</param>
    public HttpTransportRequest(
        HttpMethod method,
        Uri url,
        IReadOnlyDictionary<string, string>? headers = null,
        JsonNode? jsonBody = null)
    {
        Method = method ?? throw new ArgumentNullException(nameof(method));
        Url = url ?? throw new ArgumentNullException(nameof(url));
        Headers = headers ?? new Dictionary<string, string>();
        JsonBody = jsonBody;
    }

    /// <summary>
    /// Gets the HTTP method.
    /// </summary>
    public HttpMethod Method { get; init; }

    /// <summary>
    /// Gets the request URL.
    /// </summary>
    public Uri Url { get; init; }

    /// <summary>
    /// Gets request headers.
    /// </summary>
    public IReadOnlyDictionary<string, string> Headers { get; init; }

    /// <summary>
    /// Gets the optional JSON request body.
    /// </summary>
    public JsonNode? JsonBody { get; init; }
}

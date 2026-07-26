using System.Text.Json;

namespace VNBrokers.Transport;

/// <summary>
/// Broker HTTP transport response.
/// </summary>
/// <param name="StatusCode">HTTP status code.</param>
/// <param name="Headers">Response headers.</param>
/// <param name="JsonBody">JSON response body, when the response is JSON.</param>
/// <param name="TextBody">Text response body, when the response is not JSON.</param>
public sealed record HttpTransportResponse(
    int StatusCode,
    IReadOnlyDictionary<string, string> Headers,
    JsonElement? JsonBody,
    string? TextBody);

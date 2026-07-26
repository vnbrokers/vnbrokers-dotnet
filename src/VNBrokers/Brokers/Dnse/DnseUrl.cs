using System.Text.Json;
using System.Text.Json.Nodes;
using VNBrokers.Transport;

namespace VNBrokers.Brokers.Dnse;

/// <summary>
/// DNSE URL and payload helpers.
/// </summary>
internal static class DnseUrl
{
    public static string BaseUrl(DnseConfig config)
        => (config.BaseUrl ?? "https://openapi.dnse.com.vn").TrimEnd('/');

    public static Dictionary<string, string> ApiHeaders(DnseConfig config)
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrEmpty(config.ApiKey))
        {
            headers["X-API-Key"] = config.ApiKey;
        }

        return headers;
    }

    public static Dictionary<string, string> TradingHeaders(
        DnseConfig config,
        bool includeTradingToken,
        bool includeContentType = false)
    {
        var headers = ApiHeaders(config);
        if (includeTradingToken && !string.IsNullOrEmpty(config.TradingToken))
        {
            headers["trading-token"] = config.TradingToken;
        }

        if (includeContentType)
        {
            headers["Content-Type"] = "application/json";
        }

        return headers;
    }

    public static string MarketOrderQuery(DnseConfig config)
        => Query(("marketType", config.MarketType), ("orderCategory", config.OrderCategory));

    public static string Query(params (string Key, string Value)[] parameters)
        => string.Join(
            "&",
            parameters.Select(parameter =>
                $"{Uri.EscapeDataString(parameter.Key)}={Uri.EscapeDataString(parameter.Value)}"));

    public static JsonElement ExpectJsonObject(HttpTransportResponse response)
    {
        if (response.JsonBody is not { } body || body.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException("Expected DNSE response body to be a JSON object.");
        }

        return body;
    }

    public static JsonNode? NumberNode(decimal? value)
    {
        if (value is null)
        {
            return null;
        }

        return decimal.Truncate(value.Value) == value.Value
            ? JsonValue.Create(decimal.ToInt64(value.Value))
            : JsonValue.Create(value.Value);
    }

    public static string NumberString(decimal value)
        => decimal.Truncate(value) == value
            ? decimal.ToInt64(value).ToString()
            : value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}

using System.Globalization;
using System.Text.Json;
using VNBrokers.Domain;

namespace VNBrokers.Brokers.Dnse;

/// <summary>
/// Maps DNSE payloads into normalized SDK domain models.
/// </summary>
public static class DnseMapper
{
    private static readonly Dictionary<string, OrderStatus> OrderStatusMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["PENDING"] = OrderStatus.Pending,
            ["NEW"] = OrderStatus.Accepted,
            ["ACCEPTED"] = OrderStatus.Accepted,
            ["PARTIALLY_FILLED"] = OrderStatus.PartiallyFilled,
            ["FILLED"] = OrderStatus.Filled,
            ["CANCELLED"] = OrderStatus.Cancelled,
            ["CANCELED"] = OrderStatus.Cancelled,
            ["REJECTED"] = OrderStatus.Rejected,
        };

    /// <summary>
    /// Maps a DNSE order side.
    /// </summary>
    /// <param name="rawSide">Raw side value.</param>
    /// <returns>Normalized order side.</returns>
    public static OrderSide MapSide(string? rawSide)
        => rawSide switch
        {
            "NB" => OrderSide.Buy,
            "NS" => OrderSide.Sell,
            _ => throw new ArgumentException($"Unsupported DNSE order side: {rawSide}", nameof(rawSide)),
        };

    /// <summary>
    /// Maps a DNSE order type.
    /// </summary>
    /// <param name="rawType">Raw order type value.</param>
    /// <returns>Normalized order type.</returns>
    public static OrderType MapOrderType(string? rawType)
        => rawType switch
        {
            "LO" => OrderType.Limit,
            "MP" or "MTL" or "MOK" or "MAK" => OrderType.Market,
            _ => OrderType.Unknown,
        };

    /// <summary>
    /// Maps a DNSE order status.
    /// </summary>
    /// <param name="rawStatus">Raw status value.</param>
    /// <returns>Normalized order status.</returns>
    public static OrderStatus MapOrderStatus(string? rawStatus)
    {
        if (rawStatus is null)
        {
            return OrderStatus.Unknown;
        }

        return OrderStatusMap.GetValueOrDefault(rawStatus, OrderStatus.Unknown);
    }

    /// <summary>
    /// Maps account list payload.
    /// </summary>
    /// <param name="payload">DNSE payload.</param>
    /// <returns>Accounts.</returns>
    public static IReadOnlyList<Account> MapAccounts(JsonElement payload)
    {
        if (!payload.TryGetProperty("accounts", out var accountsElement)
            || accountsElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var displayName = OptionalString(payload, "name");
        var accounts = new List<Account>();
        foreach (var accountElement in accountsElement.EnumerateArray())
        {
            if (accountElement.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            var accountId = OptionalString(accountElement, "id");
            if (accountId is null)
            {
                continue;
            }

            accounts.Add(new Account(
                accountId,
                "dnse",
                displayName,
                new RawPayload("dnse", payload.Clone())));
        }

        return accounts;
    }

    /// <summary>
    /// Maps account balance payload.
    /// </summary>
    /// <param name="accountId">Account identifier.</param>
    /// <param name="payload">DNSE payload.</param>
    /// <returns>Balance.</returns>
    public static Balance MapBalance(string accountId, JsonElement payload)
    {
        var stock = payload.TryGetProperty("stock", out var stockElement)
            && stockElement.ValueKind == JsonValueKind.Object
            ? stockElement
            : default;
        var availableCash = OptionalDecimal(stock, "availableCash");

        return new Balance(
            accountId,
            availableCash,
            OptionalDecimal(stock, "totalCash"),
            availableCash,
            "VND",
            new RawPayload("dnse", payload.Clone()));
    }

    /// <summary>
    /// Maps a single order payload.
    /// </summary>
    /// <param name="payload">DNSE payload.</param>
    /// <returns>Order.</returns>
    public static Order MapOrder(JsonElement payload)
        => new(
            "dnse",
            OptionalString(payload, "accountNo") ?? string.Empty,
            OptionalString(payload, "id") ?? string.Empty,
            OptionalString(payload, "symbol") ?? string.Empty,
            MapSide(OptionalString(payload, "side")),
            MapOrderType(OptionalString(payload, "orderType")),
            DecimalOrZero(payload, "quantity"),
            MapOrderStatus(OptionalString(payload, "orderStatus")),
            OptionalDecimal(payload, "price"),
            new RawPayload("dnse", payload.Clone()));

    /// <summary>
    /// Maps order list payload.
    /// </summary>
    /// <param name="payload">DNSE payload.</param>
    /// <returns>Orders.</returns>
    public static IReadOnlyList<Order> MapOrders(JsonElement payload)
        => MapOrderArray(payload, "orders");

    /// <summary>
    /// Maps order history payload.
    /// </summary>
    /// <param name="payload">DNSE payload.</param>
    /// <returns>Orders.</returns>
    public static IReadOnlyList<Order> MapOrderHistory(JsonElement payload)
        => MapOrderArray(payload, "data");

    /// <summary>
    /// Maps a DNSE place order response.
    /// </summary>
    /// <param name="payload">DNSE payload.</param>
    /// <returns>Place order response.</returns>
    public static PlaceOrderResponse MapPlaceOrderResponse(JsonElement payload)
        => new(
            OptionalString(payload, "id") ?? string.Empty,
            MapOrderStatus(OptionalString(payload, "orderStatus")),
            new RawPayload("dnse", payload.Clone()));

    /// <summary>
    /// Maps position list payload.
    /// </summary>
    /// <param name="payload">DNSE payload.</param>
    /// <returns>Positions.</returns>
    public static IReadOnlyList<Position> MapPositions(JsonElement payload)
    {
        if (!payload.TryGetProperty("positions", out var positionsElement)
            || positionsElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var positions = new List<Position>();
        foreach (var positionElement in positionsElement.EnumerateArray())
        {
            if (positionElement.ValueKind == JsonValueKind.Object)
            {
                positions.Add(MapPosition(positionElement));
            }
        }

        return positions;
    }

    /// <summary>
    /// Maps a single position payload.
    /// </summary>
    /// <param name="payload">DNSE payload.</param>
    /// <returns>Position.</returns>
    public static Position MapPosition(JsonElement payload)
    {
        if (payload.TryGetProperty("data", out var dataElement)
            && dataElement.ValueKind == JsonValueKind.Object)
        {
            payload = dataElement;
        }

        var accountId = OptionalString(payload, "accountNo")
            ?? throw new ArgumentException("DNSE position payload must include accountNo.", nameof(payload));
        var symbol = OptionalString(payload, "symbol")
            ?? throw new ArgumentException("DNSE position payload must include symbol.", nameof(payload));
        var quantity = OptionalDecimal(payload, "openQuantity")
            ?? OptionalDecimal(payload, "tradeQuantity")
            ?? 0m;
        var marketPrice = OptionalDecimal(payload, "marketPrice");

        return new Position(
            accountId,
            symbol,
            quantity,
            quantity,
            OptionalDecimal(payload, "averageCostPrice") ?? OptionalDecimal(payload, "costPrice"),
            marketPrice is null ? null : quantity * marketPrice,
            new RawPayload("dnse", payload.Clone()));
    }

    private static List<Order> MapOrderArray(JsonElement payload, string propertyName)
    {
        if (!payload.TryGetProperty(propertyName, out var ordersElement)
            || ordersElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var orders = new List<Order>();
        foreach (var orderElement in ordersElement.EnumerateArray())
        {
            if (orderElement.ValueKind == JsonValueKind.Object)
            {
                orders.Add(MapOrder(orderElement));
            }
        }

        return orders;
    }

    private static string? OptionalString(JsonElement element, string propertyName)
    {
        if (element.ValueKind != JsonValueKind.Object
            || !element.TryGetProperty(propertyName, out var property)
            || property.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            return null;
        }

        return property.ValueKind == JsonValueKind.String ? property.GetString() : property.ToString();
    }

    private static decimal DecimalOrZero(JsonElement element, string propertyName)
        => OptionalDecimal(element, propertyName) ?? 0m;

    private static decimal? OptionalDecimal(JsonElement element, string propertyName)
    {
        if (element.ValueKind != JsonValueKind.Object
            || !element.TryGetProperty(propertyName, out var property)
            || property.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            return null;
        }

        if (property.ValueKind == JsonValueKind.Number && property.TryGetDecimal(out var decimalValue))
        {
            return decimalValue;
        }

        var value = property.ValueKind == JsonValueKind.String ? property.GetString() : property.ToString();
        return decimal.TryParse(
            value,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out var parsed)
            ? parsed
            : null;
    }
}

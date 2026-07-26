using System.Text.Json.Nodes;
using VNBrokers.Core;
using VNBrokers.Domain;
using VNBrokers.Transport;

namespace VNBrokers.Brokers.Dnse;

/// <summary>
/// DNSE order query and mutation service.
/// </summary>
public sealed class DnseTradingOrdersService
{
    private readonly DnseBroker broker;
    private readonly DnseConfig config;
    private readonly DnseHttpSender sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="DnseTradingOrdersService"/> class.
    /// </summary>
    /// <param name="broker">DNSE broker.</param>
    /// <param name="config">DNSE configuration.</param>
    /// <param name="transport">HTTP transport.</param>
    public DnseTradingOrdersService(DnseBroker broker, DnseConfig config, IHttpTransport transport)
    {
        this.broker = broker ?? throw new ArgumentNullException(nameof(broker));
        this.config = config ?? throw new ArgumentNullException(nameof(config));
        sender = new DnseHttpSender(config, transport);
    }

    /// <summary>
    /// Places an order.
    /// </summary>
    /// <param name="request">Place order request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Place order response.</returns>
    public async Task<PlaceOrderResponse> PlaceOrderAsync(
        PlaceOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        broker.RequireCapability(Capability.TradingOrdersPlace);
        var response = await sender.SendAsync(
            new HttpTransportRequest(
                HttpMethod.Post,
                Url("/accounts/orders", includeOrderCategory: true),
                DnseUrl.TradingHeaders(config, includeTradingToken: true, includeContentType: true),
                PlaceOrderBody(request)),
            cancellationToken).ConfigureAwait(false);

        return DnseMapper.MapPlaceOrderResponse(DnseUrl.ExpectJsonObject(response));
    }

    /// <summary>
    /// Cancels an order.
    /// </summary>
    /// <param name="accountId">Account identifier.</param>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Completion task.</returns>
    public async Task CancelOrderAsync(
        string accountId,
        string orderId,
        CancellationToken cancellationToken = default)
    {
        broker.RequireCapability(Capability.TradingOrdersCancel);
        await sender.SendAsync(
            new HttpTransportRequest(
                HttpMethod.Delete,
                OrderUrl(accountId, orderId),
                DnseUrl.TradingHeaders(config, includeTradingToken: true)),
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a single order.
    /// </summary>
    /// <param name="accountId">Account identifier.</param>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Order.</returns>
    public async Task<Order> GetOrderAsync(
        string accountId,
        string orderId,
        CancellationToken cancellationToken = default)
    {
        broker.RequireCapability(Capability.TradingOrdersPlace);
        var response = await sender.SendAsync(
            new HttpTransportRequest(
                HttpMethod.Get,
                OrderUrl(accountId, orderId),
                DnseUrl.TradingHeaders(config, includeTradingToken: false)),
            cancellationToken).ConfigureAwait(false);

        return DnseMapper.MapOrder(DnseUrl.ExpectJsonObject(response));
    }

    /// <summary>
    /// Updates an order.
    /// </summary>
    /// <param name="accountId">Account identifier.</param>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="price">New price.</param>
    /// <param name="quantity">New quantity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Raw DNSE payload.</returns>
    public async Task<RawPayload> UpdateOrderAsync(
        string accountId,
        string orderId,
        decimal price,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        broker.RequireCapability(Capability.TradingOrdersPlace);
        var body = new JsonObject
        {
            ["price"] = DnseUrl.NumberNode(price),
            ["quantity"] = quantity,
        };
        var response = await sender.SendAsync(
            new HttpTransportRequest(
                HttpMethod.Put,
                OrderUrl(accountId, orderId),
                DnseUrl.TradingHeaders(config, includeTradingToken: true, includeContentType: true),
                body),
            cancellationToken).ConfigureAwait(false);

        return new RawPayload("dnse", DnseUrl.ExpectJsonObject(response));
    }

    private Uri Url(string path, bool includeOrderCategory)
    {
        var query = includeOrderCategory
            ? DnseUrl.MarketOrderQuery(config)
            : DnseUrl.Query(("marketType", config.MarketType));
        return new Uri($"{DnseUrl.BaseUrl(config)}{path}?{query}");
    }

    private Uri OrderUrl(string accountId, string orderId)
    {
        var encodedAccountId = Uri.EscapeDataString(accountId);
        var encodedOrderId = Uri.EscapeDataString(orderId);
        return Url($"/accounts/{encodedAccountId}/orders/{encodedOrderId}", includeOrderCategory: true);
    }

    private JsonObject PlaceOrderBody(PlaceOrderRequest request)
    {
        var body = new JsonObject
        {
            ["accountNo"] = request.AccountId,
            ["orderType"] = DnseOrderType(request.OrderType),
            ["price"] = DnseUrl.NumberNode(request.Price),
            ["quantity"] = decimal.ToInt32(request.Quantity),
            ["side"] = DnseSide(request.Side),
            ["symbol"] = request.Symbol,
        };

        if (config.LoanPackageId is not null)
        {
            body["loanPackageId"] = config.LoanPackageId.Value;
        }

        return body;
    }

    private static string DnseSide(OrderSide side)
        => side switch
        {
            OrderSide.Buy => "NB",
            OrderSide.Sell => "NS",
            _ => throw new ArgumentException($"Unsupported order side: {side}", nameof(side)),
        };

    private static string DnseOrderType(OrderType orderType)
        => orderType switch
        {
            OrderType.Limit => "LO",
            OrderType.Market => "MTL",
            _ => throw new ArgumentException($"Unsupported order type: {orderType}", nameof(orderType)),
        };
}

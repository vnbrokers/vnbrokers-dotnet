using VNBrokers.Core;
using VNBrokers.Domain;
using VNBrokers.Transport;

namespace VNBrokers.Brokers.Dnse;

/// <summary>
/// DNSE account query service.
/// </summary>
public sealed class DnseTradingAccountsService
{
    private readonly DnseBroker broker;
    private readonly DnseConfig config;
    private readonly DnseHttpSender sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="DnseTradingAccountsService"/> class.
    /// </summary>
    /// <param name="broker">DNSE broker.</param>
    /// <param name="config">DNSE configuration.</param>
    /// <param name="transport">HTTP transport.</param>
    public DnseTradingAccountsService(DnseBroker broker, DnseConfig config, IHttpTransport transport)
    {
        this.broker = broker ?? throw new ArgumentNullException(nameof(broker));
        this.config = config ?? throw new ArgumentNullException(nameof(config));
        sender = new DnseHttpSender(config, transport);
    }

    /// <summary>
    /// Lists DNSE trading accounts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Accounts.</returns>
    public async Task<IReadOnlyList<Account>> ListAccountsAsync(
        CancellationToken cancellationToken = default)
    {
        broker.RequireCapability(Capability.TradingAccountsList);
        var response = await sender.SendAsync(
            new HttpTransportRequest(
                HttpMethod.Get,
                new Uri($"{DnseUrl.BaseUrl(config)}/accounts"),
                DnseUrl.ApiHeaders(config)),
            cancellationToken).ConfigureAwait(false);

        return DnseMapper.MapAccounts(DnseUrl.ExpectJsonObject(response));
    }

    /// <summary>
    /// Gets an account balance.
    /// </summary>
    /// <param name="accountId">Account identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Balance.</returns>
    public async Task<Balance> GetBalanceAsync(
        string accountId,
        CancellationToken cancellationToken = default)
    {
        broker.RequireCapability(Capability.TradingAccountsList);
        var encodedAccountId = Uri.EscapeDataString(accountId);
        var response = await sender.SendAsync(
            new HttpTransportRequest(
                HttpMethod.Get,
                new Uri($"{DnseUrl.BaseUrl(config)}/accounts/{encodedAccountId}/balances"),
                DnseUrl.ApiHeaders(config)),
            cancellationToken).ConfigureAwait(false);

        return DnseMapper.MapBalance(accountId, DnseUrl.ExpectJsonObject(response));
    }

    /// <summary>
    /// Lists active orders for an account.
    /// </summary>
    /// <param name="accountId">Account identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Orders.</returns>
    public async Task<IReadOnlyList<Order>> ListOrdersAsync(
        string accountId,
        CancellationToken cancellationToken = default)
    {
        broker.RequireCapability(Capability.TradingOrdersPlace);
        var encodedAccountId = Uri.EscapeDataString(accountId);
        var response = await sender.SendAsync(
            new HttpTransportRequest(
                HttpMethod.Get,
                new Uri(
                    $"{DnseUrl.BaseUrl(config)}/accounts/{encodedAccountId}/orders?{DnseUrl.MarketOrderQuery(config)}"),
                DnseUrl.ApiHeaders(config)),
            cancellationToken).ConfigureAwait(false);

        return DnseMapper.MapOrders(DnseUrl.ExpectJsonObject(response));
    }

    /// <summary>
    /// Lists historical orders for an account.
    /// </summary>
    /// <param name="accountId">Account identifier.</param>
    /// <param name="fromDate">Start date.</param>
    /// <param name="toDate">End date.</param>
    /// <param name="pageIndex">Page index.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Historical orders.</returns>
    public async Task<IReadOnlyList<Order>> ListOrderHistoryAsync(
        string accountId,
        string fromDate,
        string toDate,
        int pageIndex = 0,
        CancellationToken cancellationToken = default)
    {
        broker.RequireCapability(Capability.TradingOrdersPlace);
        var encodedAccountId = Uri.EscapeDataString(accountId);
        var query = DnseUrl.Query(
            ("marketType", config.MarketType),
            ("from", fromDate),
            ("to", toDate),
            ("pageIndex", pageIndex.ToString()));
        var response = await sender.SendAsync(
            new HttpTransportRequest(
                HttpMethod.Get,
                new Uri($"{DnseUrl.BaseUrl(config)}/accounts/{encodedAccountId}/orders/history?{query}"),
                DnseUrl.ApiHeaders(config)),
            cancellationToken).ConfigureAwait(false);

        return DnseMapper.MapOrderHistory(DnseUrl.ExpectJsonObject(response));
    }

    /// <summary>
    /// Gets raw execution details for an order.
    /// </summary>
    /// <param name="accountId">Account identifier.</param>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Raw payload.</returns>
    public async Task<RawPayload> GetExecutionsAsync(
        string accountId,
        string orderId,
        CancellationToken cancellationToken = default)
    {
        broker.RequireCapability(Capability.TradingOrdersPlace);
        var encodedAccountId = Uri.EscapeDataString(accountId);
        var encodedOrderId = Uri.EscapeDataString(orderId);
        var response = await sender.SendAsync(
            new HttpTransportRequest(
                HttpMethod.Get,
                new Uri(
                    $"{DnseUrl.BaseUrl(config)}/accounts/{encodedAccountId}/executions/{encodedOrderId}?{DnseUrl.MarketOrderQuery(config)}"),
                DnseUrl.ApiHeaders(config)),
            cancellationToken).ConfigureAwait(false);

        return new RawPayload("dnse", DnseUrl.ExpectJsonObject(response));
    }

    /// <summary>
    /// Gets DNSE PPSE information.
    /// </summary>
    /// <param name="accountId">Account identifier.</param>
    /// <param name="symbol">Symbol.</param>
    /// <param name="price">Price.</param>
    /// <param name="loanPackageId">Optional loan package identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Raw payload.</returns>
    public async Task<RawPayload> GetPpseAsync(
        string accountId,
        string symbol,
        decimal price,
        int? loanPackageId = null,
        CancellationToken cancellationToken = default)
    {
        broker.RequireCapability(Capability.TradingAccountsList);
        var encodedAccountId = Uri.EscapeDataString(accountId);
        var query = DnseUrl.Query(
            ("marketType", config.MarketType),
            ("symbol", symbol),
            ("loanPackageId", (loanPackageId ?? config.LoanPackageId ?? 0).ToString()),
            ("price", DnseUrl.NumberString(price)));
        var response = await sender.SendAsync(
            new HttpTransportRequest(
                HttpMethod.Get,
                new Uri($"{DnseUrl.BaseUrl(config)}/accounts/{encodedAccountId}/ppse?{query}"),
                DnseUrl.ApiHeaders(config)),
            cancellationToken).ConfigureAwait(false);

        return new RawPayload("dnse", DnseUrl.ExpectJsonObject(response));
    }

    /// <summary>
    /// Gets loan packages for a symbol.
    /// </summary>
    /// <param name="accountId">Account identifier.</param>
    /// <param name="symbol">Symbol.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Raw payload.</returns>
    public async Task<RawPayload> GetLoanPackagesAsync(
        string accountId,
        string symbol,
        CancellationToken cancellationToken = default)
    {
        broker.RequireCapability(Capability.TradingAccountsList);
        var encodedAccountId = Uri.EscapeDataString(accountId);
        var query = DnseUrl.Query(("marketType", config.MarketType), ("symbol", symbol));
        var response = await sender.SendAsync(
            new HttpTransportRequest(
                HttpMethod.Get,
                new Uri($"{DnseUrl.BaseUrl(config)}/accounts/{encodedAccountId}/loan-packages?{query}"),
                DnseUrl.ApiHeaders(config)),
            cancellationToken).ConfigureAwait(false);

        return new RawPayload("dnse", DnseUrl.ExpectJsonObject(response));
    }
}

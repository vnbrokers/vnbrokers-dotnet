using VNBrokers.Core;
using VNBrokers.Domain;
using VNBrokers.Transport;

namespace VNBrokers.Brokers.Dnse;

/// <summary>
/// DNSE position query service.
/// </summary>
public sealed class DnseTradingPositionsService
{
    private readonly DnseBroker broker;
    private readonly DnseConfig config;
    private readonly DnseHttpSender sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="DnseTradingPositionsService"/> class.
    /// </summary>
    /// <param name="broker">DNSE broker.</param>
    /// <param name="config">DNSE configuration.</param>
    /// <param name="transport">HTTP transport.</param>
    public DnseTradingPositionsService(DnseBroker broker, DnseConfig config, IHttpTransport transport)
    {
        this.broker = broker ?? throw new ArgumentNullException(nameof(broker));
        this.config = config ?? throw new ArgumentNullException(nameof(config));
        sender = new DnseHttpSender(config, transport);
    }

    /// <summary>
    /// Lists account positions.
    /// </summary>
    /// <param name="accountId">Account identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Positions.</returns>
    public async Task<IReadOnlyList<Position>> ListPositionsAsync(
        string accountId,
        CancellationToken cancellationToken = default)
    {
        broker.RequireCapability(Capability.TradingPositionsList);
        var encodedAccountId = Uri.EscapeDataString(accountId);
        var query = DnseUrl.Query(
            ("marketType", config.MarketType),
            ("pageSize", config.PositionsPageSize.ToString()));
        var response = await sender.SendAsync(
            new HttpTransportRequest(
                HttpMethod.Get,
                new Uri($"{DnseUrl.BaseUrl(config)}/accounts/{encodedAccountId}/positions?{query}"),
                DnseUrl.ApiHeaders(config)),
            cancellationToken).ConfigureAwait(false);

        return DnseMapper.MapPositions(DnseUrl.ExpectJsonObject(response));
    }

    /// <summary>
    /// Gets a single position.
    /// </summary>
    /// <param name="positionId">Position identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Position.</returns>
    public async Task<Position> GetPositionAsync(
        string positionId,
        CancellationToken cancellationToken = default)
    {
        broker.RequireCapability(Capability.TradingPositionsList);
        var encodedPositionId = Uri.EscapeDataString(positionId);
        var query = DnseUrl.Query(("marketType", config.MarketType));
        var response = await sender.SendAsync(
            new HttpTransportRequest(
                HttpMethod.Get,
                new Uri($"{DnseUrl.BaseUrl(config)}/accounts/positions/{encodedPositionId}?{query}"),
                DnseUrl.ApiHeaders(config)),
            cancellationToken).ConfigureAwait(false);

        return DnseMapper.MapPosition(DnseUrl.ExpectJsonObject(response));
    }

    /// <summary>
    /// Closes an open position.
    /// </summary>
    /// <param name="positionId">Position identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Raw DNSE payload.</returns>
    public async Task<RawPayload> ClosePositionAsync(
        string positionId,
        CancellationToken cancellationToken = default)
    {
        broker.RequireCapability(Capability.TradingOrdersPlace);
        var encodedPositionId = Uri.EscapeDataString(positionId);
        var query = DnseUrl.Query(("marketType", config.MarketType));
        var response = await sender.SendAsync(
            new HttpTransportRequest(
                HttpMethod.Post,
                new Uri($"{DnseUrl.BaseUrl(config)}/accounts/positions/{encodedPositionId}/close?{query}"),
                DnseUrl.TradingHeaders(config, includeTradingToken: true)),
            cancellationToken).ConfigureAwait(false);

        return new RawPayload("dnse", DnseUrl.ExpectJsonObject(response));
    }
}

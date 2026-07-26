using VNBrokers.Core;

namespace VNBrokers.Brokers.Dnse;

/// <summary>
/// DNSE broker configuration.
/// </summary>
public class DnseConfig : BrokerConfig
{

#pragma warning disable S1075
    public const string DefaultBaseUrl =
        "https://openapi.dnse.com.vn";

    public const string DefaultStreamUrl =
        "wss://ws-openapi.dnse.com.vn/v1/stream?encoding=msgpack";
#pragma warning restore S1075

    /// <summary>
    /// Initializes a new instance of the <see cref="DnseConfig"/> class.
    /// </summary>
    public DnseConfig()
    {
        BaseUrl = DefaultBaseUrl;
        StreamUrl = DefaultStreamUrl;
    }

    /// <summary>Gets or sets the DNSE API key.</summary>
    public string? ApiKey { get; set; }

    /// <summary>Gets or sets the DNSE API secret.</summary>
    public string? ApiSecret { get; set; }

    /// <summary>Gets or sets an access token.</summary>
    public string? AccessToken { get; set; }

    /// <summary>Gets or sets a trading token.</summary>
    public string? TradingToken { get; set; }

    /// <summary>Gets or sets stream encoding.</summary>
    public string StreamEncoding { get; set; } = "json";

    /// <summary>Gets or sets DNSE market type.</summary>
    public string MarketType { get; set; } = "DERIVATIVE";

    /// <summary>Gets or sets DNSE order category.</summary>
    public string OrderCategory { get; set; } = "NORMAL";

    /// <summary>Gets or sets optional loan package identifier.</summary>
    public int? LoanPackageId { get; set; }

    /// <summary>Gets or sets positions page size.</summary>
    public int PositionsPageSize { get; set; } = 20;

    /// <summary>Gets or sets instrument query limit.</summary>
    public int MarketDataSymbolLimit { get; set; } = 1000;

    /// <summary>Gets or sets market data board ID.</summary>
    public string MarketDataBoardId { get; set; } = "G1";

    /// <summary>Gets or sets candle market type.</summary>
    public string CandleMarketType { get; set; } = "STOCK";

    /// <summary>Gets or sets default candle lookback in seconds.</summary>
    public int CandleLookbackSeconds { get; set; } = 86400;
}

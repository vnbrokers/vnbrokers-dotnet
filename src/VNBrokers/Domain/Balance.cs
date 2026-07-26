namespace VNBrokers.Domain;

/// <summary>
/// Normalized account balance.
/// </summary>
/// <param name="AccountId">Broker account identifier.</param>
/// <param name="CashAvailable">Cash available for trading.</param>
/// <param name="CashTotal">Total cash balance.</param>
/// <param name="BuyingPower">Buying power reported by the broker.</param>
/// <param name="Currency">Balance currency.</param>
/// <param name="Raw">Optional raw broker payload.</param>
public sealed record Balance(
    string AccountId,
    decimal? CashAvailable = null,
    decimal? CashTotal = null,
    decimal? BuyingPower = null,
    string Currency = "VND",
    RawPayload? Raw = null);

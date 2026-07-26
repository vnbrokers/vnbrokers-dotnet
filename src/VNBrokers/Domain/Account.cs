namespace VNBrokers.Domain;

/// <summary>
/// Normalized brokerage account.
/// </summary>
/// <param name="AccountId">Broker account identifier.</param>
/// <param name="Broker">Broker name.</param>
/// <param name="DisplayName">Optional account display name.</param>
/// <param name="Raw">Optional raw broker payload.</param>
public sealed record Account(
    string AccountId,
    string Broker,
    string? DisplayName = null,
    RawPayload? Raw = null);

namespace VNBrokers.Domain;

/// <summary>
/// Normalized tradable symbol.
/// </summary>
/// <param name="Code">Instrument code.</param>
/// <param name="Exchange">Optional exchange or market identifier.</param>
/// <param name="DisplayName">Optional display name.</param>
/// <param name="Raw">Optional raw broker payload.</param>
public sealed record Symbol(
    string Code,
    string? Exchange = null,
    string? DisplayName = null,
    RawPayload? Raw = null);

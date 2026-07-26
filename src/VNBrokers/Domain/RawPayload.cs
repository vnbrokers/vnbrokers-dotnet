using System.Text.Json;

namespace VNBrokers.Domain;

/// <summary>
/// Preserves the broker-specific payload that produced a normalized SDK model.
/// </summary>
/// <param name="Source">Broker or transport source for the payload.</param>
/// <param name="Data">Raw JSON payload data.</param>
public sealed record RawPayload(string Source, JsonElement Data);

namespace VNBrokers.Realtime;

/// <summary>
/// Reconnect settings for realtime subscriptions.
/// </summary>
/// <param name="Enabled">Whether reconnect is enabled.</param>
/// <param name="MaxAttempts">Maximum reconnect attempts.</param>
/// <param name="InitialDelaySeconds">Initial reconnect delay in seconds.</param>
/// <param name="MaxDelaySeconds">Maximum reconnect delay in seconds.</param>
public sealed record ReconnectPolicy(
    bool Enabled = false,
    int MaxAttempts = 3,
    double InitialDelaySeconds = 1.0,
    double MaxDelaySeconds = 30.0);

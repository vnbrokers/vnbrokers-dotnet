namespace VNBrokers.Realtime;

/// <summary>
/// Queue overflow behavior for realtime subscriptions.
/// </summary>
public enum BackpressureMode
{
    /// <summary>Throw when the queue overflows.</summary>
    ErrorOnOverflow,

    /// <summary>Drop the oldest item when the queue overflows.</summary>
    DropOldest,
}

/// <summary>
/// Backpressure settings for realtime subscriptions.
/// </summary>
/// <param name="Mode">Overflow behavior.</param>
/// <param name="MaxQueueSize">Maximum queued item count.</param>
public sealed record BackpressurePolicy(
    BackpressureMode Mode = BackpressureMode.ErrorOnOverflow,
    int MaxQueueSize = 1024);

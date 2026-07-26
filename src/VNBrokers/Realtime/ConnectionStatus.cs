namespace VNBrokers.Realtime;

/// <summary>
/// Realtime subscription connection status.
/// </summary>
public enum ConnectionStatus
{
    /// <summary>Subscription is idle.</summary>
    Idle,

    /// <summary>Transport is connecting.</summary>
    Connecting,

    /// <summary>Transport is connected.</summary>
    Connected,

    /// <summary>Subscription is authenticating.</summary>
    Authenticating,

    /// <summary>Subscription is subscribed.</summary>
    Subscribed,

    /// <summary>Subscription is reconnecting.</summary>
    Reconnecting,

    /// <summary>Subscription is closing.</summary>
    Closing,

    /// <summary>Subscription is closed.</summary>
    Closed,

    /// <summary>Subscription failed.</summary>
    Failed,
}

namespace VNBrokers.Realtime;

/// <summary>
/// Realtime subscription abstraction.
/// </summary>
/// <typeparam name="T">Event type.</typeparam>
public interface ISubscription<T> : IAsyncDisposable
{
    /// <summary>
    /// Gets a value indicating whether the subscription is closed.
    /// </summary>
    bool Closed { get; }

    /// <summary>
    /// Reads published events.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async event stream.</returns>
    IAsyncEnumerable<T> EventsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads published errors.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async error stream.</returns>
    IAsyncEnumerable<Exception> ErrorsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads connection status changes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async status stream.</returns>
    IAsyncEnumerable<ConnectionStatus> StatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes the subscription.
    /// </summary>
    /// <returns>Completion task.</returns>
    Task CloseAsync();
}

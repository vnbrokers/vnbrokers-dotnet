namespace VNBrokers.Transport;

/// <summary>
/// Broker WebSocket transport abstraction.
/// </summary>
public interface IWebSocketTransport : IAsyncDisposable
{
    /// <summary>
    /// Sends a text message.
    /// </summary>
    /// <param name="message">Text message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Completion task.</returns>
    Task SendTextAsync(string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a binary message.
    /// </summary>
    /// <param name="message">Binary message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Completion task.</returns>
    Task SendBytesAsync(byte[] message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Receives messages until the socket closes or cancellation is requested.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async message sequence.</returns>
    IAsyncEnumerable<WebSocketMessage> ReceiveAsync(CancellationToken cancellationToken = default);
}

namespace VNBrokers.Transport;

/// <summary>
/// Message received from or sent to a WebSocket stream.
/// </summary>
/// <param name="Text">Text payload, when present.</param>
/// <param name="Bytes">Binary payload, when present.</param>
public sealed record WebSocketMessage(string? Text = null, byte[]? Bytes = null)
{
    /// <summary>
    /// Creates a text WebSocket message.
    /// </summary>
    /// <param name="text">Text payload.</param>
    /// <returns>WebSocket message.</returns>
    public static WebSocketMessage FromText(string text) => new(text, null);

    /// <summary>
    /// Creates a binary WebSocket message.
    /// </summary>
    /// <param name="bytes">Binary payload.</param>
    /// <returns>WebSocket message.</returns>
    public static WebSocketMessage FromBytes(byte[] bytes) => new(null, bytes);
}

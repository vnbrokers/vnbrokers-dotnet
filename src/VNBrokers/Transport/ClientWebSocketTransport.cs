using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace VNBrokers.Transport;

/// <summary>
/// <see cref="ClientWebSocket"/> implementation of <see cref="IWebSocketTransport"/>.
/// </summary>
public sealed class ClientWebSocketTransport : IWebSocketTransport
{
    private readonly ClientWebSocket socket;

    private ClientWebSocketTransport(ClientWebSocket socket)
    {
        this.socket = socket;
    }

    /// <summary>
    /// Opens a WebSocket connection.
    /// </summary>
    /// <param name="url">WebSocket URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Connected transport.</returns>
    public static async Task<ClientWebSocketTransport> ConnectAsync(
        Uri url,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(url);
        var socket = new ClientWebSocket();
        await socket.ConnectAsync(url, cancellationToken).ConfigureAwait(false);
        return new ClientWebSocketTransport(socket);
    }

    /// <inheritdoc />
    public Task SendTextAsync(string message, CancellationToken cancellationToken = default)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        return socket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task SendBytesAsync(byte[] message, CancellationToken cancellationToken = default)
        => socket.SendAsync(
            new ArraySegment<byte>(message),
            WebSocketMessageType.Binary,
            endOfMessage: true,
            cancellationToken);

    /// <inheritdoc />
    public async IAsyncEnumerable<WebSocketMessage> ReceiveAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var buffer = new byte[8192];
        while (socket.State == WebSocketState.Open)
        {
            using var stream = new MemoryStream();
            WebSocketReceiveResult result;
            do
            {
                result = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cancellationToken).ConfigureAwait(false);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    yield break;
                }

                await stream.WriteAsync(buffer.AsMemory(0, result.Count), cancellationToken);
            }
            while (!result.EndOfMessage);

            var payload = stream.ToArray();
            if (result.MessageType == WebSocketMessageType.Text)
            {
                yield return WebSocketMessage.FromText(Encoding.UTF8.GetString(payload));
            }
            else
            {
                yield return WebSocketMessage.FromBytes(payload);
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (socket.State is WebSocketState.Open or WebSocketState.CloseReceived)
        {
            await socket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Closing",
                CancellationToken.None).ConfigureAwait(false);
        }

        socket.Dispose();
    }
}

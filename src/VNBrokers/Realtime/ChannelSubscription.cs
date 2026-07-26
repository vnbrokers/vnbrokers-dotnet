using System.Threading.Channels;

namespace VNBrokers.Realtime;

/// <summary>
/// Channel-backed implementation of <see cref="ISubscription{T}"/>.
/// </summary>
/// <typeparam name="T">Event type.</typeparam>
public class ChannelSubscription<T> : ISubscription<T>
{
    private readonly Channel<T> events;
    private readonly Channel<Exception> errors;
    private readonly Channel<ConnectionStatus> statuses;
    private int closed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChannelSubscription{T}"/> class.
    /// </summary>
    /// <param name="maxQueueSize">Maximum queue size.</param>
    public ChannelSubscription(int maxQueueSize = 1024)
    {
        var options = new BoundedChannelOptions(maxQueueSize)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false,
        };
        events = Channel.CreateBounded<T>(options);
        errors = Channel.CreateBounded<Exception>(options);
        statuses = Channel.CreateBounded<ConnectionStatus>(options);
    }

    /// <inheritdoc />
    public bool Closed => Volatile.Read(ref closed) == 1;

    /// <summary>
    /// Publishes an event.
    /// </summary>
    /// <param name="eventValue">Event value.</param>
    public void PublishEvent(T eventValue)
    {
        events.Writer.TryWrite(eventValue);
    }

    /// <summary>
    /// Publishes an error.
    /// </summary>
    /// <param name="error">Error value.</param>
    public void PublishError(Exception error)
    {
        errors.Writer.TryWrite(error);
    }

    /// <summary>
    /// Publishes a status value.
    /// </summary>
    /// <param name="status">Connection status.</param>
    public void PublishStatus(ConnectionStatus status)
    {
        statuses.Writer.TryWrite(status);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<T> EventsAsync(CancellationToken cancellationToken = default)
        => events.Reader.ReadAllAsync(cancellationToken);

    /// <inheritdoc />
    public IAsyncEnumerable<Exception> ErrorsAsync(CancellationToken cancellationToken = default)
        => errors.Reader.ReadAllAsync(cancellationToken);

    /// <inheritdoc />
    public IAsyncEnumerable<ConnectionStatus> StatusAsync(CancellationToken cancellationToken = default)
        => statuses.Reader.ReadAllAsync(cancellationToken);

    /// <inheritdoc />
    public Task CloseAsync()
    {
        if (Interlocked.Exchange(ref closed, 1) == 1)
        {
            return Task.CompletedTask;
        }

        statuses.Writer.TryWrite(ConnectionStatus.Closed);
        events.Writer.TryComplete();
        errors.Writer.TryComplete();
        statuses.Writer.TryComplete();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await CloseAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}

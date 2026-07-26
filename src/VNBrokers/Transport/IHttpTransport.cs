namespace VNBrokers.Transport;

/// <summary>
/// Sends broker HTTP requests.
/// </summary>
public interface IHttpTransport
{
    /// <summary>
    /// Sends an HTTP request.
    /// </summary>
    /// <param name="request">Request to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Transport response.</returns>
    Task<HttpTransportResponse> SendAsync(
        HttpTransportRequest request,
        CancellationToken cancellationToken = default);
}

using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VNBrokers.Transport;

namespace VNBrokers.Tests.Transport;

[TestClass]
public sealed class HttpTransportTests
{
    [TestMethod]
    public async Task HttpClientTransport_sends_method_url_headers_and_json_body()
    {
        var handler = new RecordingHandler("""{"ok":true}""", "application/json");
        var transport = new HttpClientTransport(new HttpClient(handler));
        var body = JsonSerializer.SerializeToNode(new { symbol = "HPG" });
        var request = new HttpTransportRequest(
            HttpMethod.Post,
            new Uri("https://broker.example/orders"),
            new Dictionary<string, string> { ["X-API-Key"] = "key" },
            body);

        var response = await transport.SendAsync(request);

        handler.Request.Should().NotBeNull();
        handler.Request!.Method.Should().Be(HttpMethod.Post);
        handler.Request.RequestUri.Should().Be(new Uri("https://broker.example/orders"));
        handler.Request.Headers.GetValues("X-API-Key").Should().Equal("key");
        handler.Body.Should().Be("""{"symbol":"HPG"}""");
        response.StatusCode.Should().Be(200);
        response.JsonBody!.Value.GetProperty("ok").GetBoolean().Should().BeTrue();
    }

    [TestMethod]
    public async Task HttpClientTransport_preserves_non_json_response_as_text()
    {
        var handler = new RecordingHandler("plain response", "text/plain");
        var transport = new HttpClientTransport(new HttpClient(handler));
        var request = new HttpTransportRequest(HttpMethod.Get, new Uri("https://broker.example"));

        var response = await transport.SendAsync(request);

        response.JsonBody.Should().BeNull();
        response.TextBody.Should().Be("plain response");
    }

    private sealed class RecordingHandler(string responseBody, string contentType) : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }

        public string? Body { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Request = request;
            Body = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, contentType),
            };
        }
    }
}

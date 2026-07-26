using System.Text.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VNBrokers.Brokers.Dnse;
using VNBrokers.Errors;
using VNBrokers.Transport;

namespace VNBrokers.Tests.Brokers.Dnse;

[TestClass]
public sealed class DnseAuthServiceTests
{
    [TestMethod]
    public async Task SendEmailOtp_posts_signed_request_and_returns_raw_payload()
    {
        using var document = JsonDocument.Parse("""{"requestId":"REQ-1"}""");
        var transport = new FakeHttpTransport(new HttpTransportResponse(
            200,
            new Dictionary<string, string>(),
            document.RootElement.Clone(),
            null));
        var service = new DnseAuthService(
            new DnseConfig
            {
                BaseUrl = "https://api.dnse.example",
                ApiKey = "key",
                ApiSecret = "secret",
            },
            transport);

        var payload = await service.SendEmailOtpAsync();

        transport.Requests.Should().HaveCount(1);
        var request = transport.Requests[0];
        request.Method.Should().Be(HttpMethod.Post);
        request.Url.Should().Be(new Uri("https://api.dnse.example/registration/send-email-otp"));
        request.Headers.Should().ContainKey("X-API-Key");
        request.Headers.Should().ContainKey("X-Aux-Date");
        request.Headers.Should().ContainKey("X-Signature");
        payload.Source.Should().Be("dnse");
        payload.Data.GetProperty("requestId").GetString().Should().Be("REQ-1");
    }

    [TestMethod]
    public async Task GetTradingToken_posts_json_body_and_content_type()
    {
        using var document = JsonDocument.Parse("""{"tradingToken":"token"}""");
        var transport = new FakeHttpTransport(new HttpTransportResponse(
            200,
            new Dictionary<string, string>(),
            document.RootElement.Clone(),
            null));
        var service = new DnseAuthService(
            new DnseConfig { BaseUrl = "https://api.dnse.example", ApiKey = "key" },
            transport);

        await service.GetTradingTokenAsync("EMAIL", "123456");

        var request = transport.Requests[0];
        request.Method.Should().Be(HttpMethod.Post);
        request.Url.Should().Be(new Uri("https://api.dnse.example/registration/trading-token"));
        request.Headers.Should().Contain("Content-Type", "application/json");
        request.JsonBody!.ToJsonString().Should().Be(
            """{"otpType":"EMAIL","passcode":"123456"}""");
    }

    [TestMethod]
    public async Task Auth_request_raises_broker_rejected_exception_for_error_response()
    {
        using var document = JsonDocument.Parse("""{"code":"AUTH-001","message":"Forbidden"}""");
        var transport = new FakeHttpTransport(new HttpTransportResponse(
            403,
            new Dictionary<string, string>(),
            document.RootElement.Clone(),
            null));
        var service = new DnseAuthService(
            new DnseConfig { BaseUrl = "https://api.dnse.example", ApiKey = "key" },
            transport);

        var act = async () => await service.SendEmailOtpAsync();

        var assertion = await act.Should().ThrowAsync<BrokerRejectedException>();
        assertion.Which.Broker.Should().Be("dnse");
        assertion.Which.Code.Should().Be("AUTH-001");
        assertion.Which.Message.Should().Be("Forbidden");
        assertion.Which.Raw.Should().NotBeNull();
    }

    private sealed class FakeHttpTransport : IHttpTransport
    {
        private readonly Queue<HttpTransportResponse> responses;

        public FakeHttpTransport(params HttpTransportResponse[] responses)
        {
            this.responses = new Queue<HttpTransportResponse>(responses);
        }

        public List<HttpTransportRequest> Requests { get; } = [];

        public Task<HttpTransportResponse> SendAsync(
            HttpTransportRequest request,
            CancellationToken cancellationToken = default)
        {
            Requests.Add(request);
            return Task.FromResult(responses.Dequeue());
        }
    }
}

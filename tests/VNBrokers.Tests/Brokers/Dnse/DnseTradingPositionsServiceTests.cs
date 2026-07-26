using System.Text.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VNBrokers.Brokers.Dnse;
using VNBrokers.Transport;

namespace VNBrokers.Tests.Brokers.Dnse;

[TestClass]
public sealed class DnseTradingPositionsServiceTests
{
    [TestMethod]
    public async Task ListPositions_uses_market_type_and_page_size()
    {
        using var document = JsonDocument.Parse(
            """{"positions":[{"accountNo":"000123D","symbol":"VN30F2506","openQuantity":2,"costPrice":1200,"marketPrice":1210}],"total":1}""");
        var transport = new FakeHttpTransport(new HttpTransportResponse(
            200,
            new Dictionary<string, string>(),
            document.RootElement.Clone(),
            null));
        var broker = new DnseBroker(
            new DnseConfig
            {
                BaseUrl = "https://api.dnse.example",
                ApiKey = "key",
                MarketType = "STOCK",
                PositionsPageSize = 50,
            },
            transport);

        var positions = await broker.Trading.Positions.ListPositionsAsync("000123D");

        transport.Requests[0].Url.Should().Be(
            new Uri("https://api.dnse.example/accounts/000123D/positions?marketType=STOCK&pageSize=50"));
        positions[0].Symbol.Should().Be("VN30F2506");
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

using System.Text.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VNBrokers.Brokers.Dnse;
using VNBrokers.Domain;
using VNBrokers.Transport;

namespace VNBrokers.Tests.Brokers.Dnse;

[TestClass]
public sealed class DnseTradingMutationsTests
{
    [TestMethod]
    public async Task PlaceOrder_builds_signed_dnse_request()
    {
        using var document = JsonDocument.Parse(
            """{"id":116,"symbol":"VN30F2506","side":"NB","orderType":"LO","orderStatus":"FILLED","price":1200.5,"quantity":2,"accountNo":"000123"}""");
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
                ApiSecret = "secret",
                TradingToken = "trade-token",
                LoanPackageId = 1775,
            },
            transport);

        var response = await broker.Trading.Orders.PlaceOrderAsync(new PlaceOrderRequest(
            AccountId: "000123",
            Symbol: "VN30F2506",
            Side: OrderSide.Buy,
            OrderType: OrderType.Limit,
            Quantity: 2m,
            Price: 1200.5m));

        var request = transport.Requests[0];
        request.Method.Should().Be(HttpMethod.Post);
        request.Url.Should().Be(
            new Uri("https://api.dnse.example/accounts/orders?marketType=DERIVATIVE&orderCategory=NORMAL"));
        request.Headers.Should().Contain("X-API-Key", "key");
        request.Headers.Should().Contain("trading-token", "trade-token");
        request.JsonBody!.ToJsonString().Should().Be(
            """{"accountNo":"000123","orderType":"LO","price":1200.5,"quantity":2,"side":"NB","symbol":"VN30F2506","loanPackageId":1775}""");
        response.OrderId.Should().Be("116");
        response.Status.Should().Be(OrderStatus.Filled);
    }

    [TestMethod]
    public async Task CancelOrder_uses_delete_and_trading_token()
    {
        using var document = JsonDocument.Parse("""{}""");
        var transport = new FakeHttpTransport(new HttpTransportResponse(
            200,
            new Dictionary<string, string>(),
            document.RootElement.Clone(),
            null));
        var broker = new DnseBroker(
            new DnseConfig
            {
                BaseUrl = "https://api.dnse.example",
                TradingToken = "trade-token",
                MarketType = "STOCK",
            },
            transport);

        await broker.Trading.Orders.CancelOrderAsync("000123", "116");

        transport.Requests[0].Method.Should().Be(HttpMethod.Delete);
        transport.Requests[0].Url.Should().Be(
            new Uri("https://api.dnse.example/accounts/000123/orders/116?marketType=STOCK&orderCategory=NORMAL"));
        transport.Requests[0].Headers.Should().Contain("trading-token", "trade-token");
    }

    [TestMethod]
    public async Task UpdateOrder_builds_put_request_with_trading_token()
    {
        using var document = JsonDocument.Parse("""{"id":42,"quantity":200}""");
        var transport = new FakeHttpTransport(new HttpTransportResponse(
            200,
            new Dictionary<string, string>(),
            document.RootElement.Clone(),
            null));
        var broker = new DnseBroker(
            new DnseConfig
            {
                BaseUrl = "https://api.dnse.example",
                TradingToken = "trade-token",
                MarketType = "STOCK",
            },
            transport);

        var payload = await broker.Trading.Orders.UpdateOrderAsync(
            "0001179019",
            "42",
            price: 23100m,
            quantity: 200);

        transport.Requests[0].Method.Should().Be(HttpMethod.Put);
        transport.Requests[0].Url.Should().Be(
            new Uri("https://api.dnse.example/accounts/0001179019/orders/42?marketType=STOCK&orderCategory=NORMAL"));
        transport.Requests[0].Headers.Should().Contain("trading-token", "trade-token");
        transport.Requests[0].Headers.Should().Contain("Content-Type", "application/json");
        transport.Requests[0].JsonBody!.ToJsonString().Should().Be("""{"price":23100,"quantity":200}""");
        payload.Data.GetProperty("quantity").GetInt32().Should().Be(200);
    }

    [TestMethod]
    public async Task ClosePosition_builds_post_request_with_trading_token()
    {
        using var document = JsonDocument.Parse("""{"id":42,"symbol":"VN30F2505","quantity":10}""");
        var transport = new FakeHttpTransport(new HttpTransportResponse(
            200,
            new Dictionary<string, string>(),
            document.RootElement.Clone(),
            null));
        var broker = new DnseBroker(
            new DnseConfig
            {
                BaseUrl = "https://api.dnse.example",
                TradingToken = "trade-token",
            },
            transport);

        var payload = await broker.Trading.Positions.ClosePositionAsync("2183078");

        transport.Requests[0].Method.Should().Be(HttpMethod.Post);
        transport.Requests[0].Url.Should().Be(
            new Uri("https://api.dnse.example/accounts/positions/2183078/close?marketType=DERIVATIVE"));
        transport.Requests[0].Headers.Should().Contain("trading-token", "trade-token");
        payload.Data.GetProperty("symbol").GetString().Should().Be("VN30F2505");
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

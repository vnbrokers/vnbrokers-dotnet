using System.Text.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VNBrokers.Brokers.Dnse;
using VNBrokers.Domain;
using VNBrokers.Transport;

namespace VNBrokers.Tests.Brokers.Dnse;

[TestClass]
public sealed class DnseTradingOrdersServiceTests
{
    [TestMethod]
    public async Task ListOrders_uses_market_type_and_order_category()
    {
        using var document = JsonDocument.Parse(
            """{"orders":[{"id":116,"side":"NS","accountNo":"000123","symbol":"VN30F2506","price":1201,"quantity":1,"orderType":"LO","orderStatus":"PENDING"}]}""");
        var transport = new FakeHttpTransport(new HttpTransportResponse(
            200,
            new Dictionary<string, string>(),
            document.RootElement.Clone(),
            null));
        var broker = new DnseBroker(
            new DnseConfig { BaseUrl = "https://api.dnse.example", ApiKey = "key" },
            transport);

        var orders = await broker.Trading.Accounts.ListOrdersAsync("000123");

        transport.Requests[0].Method.Should().Be(HttpMethod.Get);
        transport.Requests[0].Url.Should().Be(
            new Uri("https://api.dnse.example/accounts/000123/orders?marketType=DERIVATIVE&orderCategory=NORMAL"));
        orders[0].OrderId.Should().Be("116");
        orders[0].Side.Should().Be(OrderSide.Sell);
    }

    [TestMethod]
    public async Task ListOrderHistory_uses_date_range_and_page_index()
    {
        using var document = JsonDocument.Parse(
            """{"data":[{"id":"H1","side":"NB","accountNo":"000123","symbol":"HPG","quantity":100,"orderType":"LO","orderStatus":"FILLED"}]}""");
        var transport = new FakeHttpTransport(new HttpTransportResponse(
            200,
            new Dictionary<string, string>(),
            document.RootElement.Clone(),
            null));
        var broker = new DnseBroker(
            new DnseConfig { BaseUrl = "https://api.dnse.example", MarketType = "STOCK" },
            transport);

        var orders = await broker.Trading.Accounts.ListOrderHistoryAsync(
            "000123",
            "2026-05-01",
            "2026-05-26",
            pageIndex: 2);

        transport.Requests[0].Url.Should().Be(
            new Uri("https://api.dnse.example/accounts/000123/orders/history?marketType=STOCK&from=2026-05-01&to=2026-05-26&pageIndex=2"));
        orders[0].Status.Should().Be(OrderStatus.Filled);
    }

    [TestMethod]
    public async Task GetOrder_maps_dnse_response()
    {
        using var document = JsonDocument.Parse(
            """{"id":116,"side":"NS","accountNo":"000123","symbol":"VN30F2506","price":1201,"quantity":1,"orderType":"LO","orderStatus":"PENDING"}""");
        var transport = new FakeHttpTransport(new HttpTransportResponse(
            200,
            new Dictionary<string, string>(),
            document.RootElement.Clone(),
            null));
        var broker = new DnseBroker(
            new DnseConfig { BaseUrl = "https://api.dnse.example", ApiKey = "key" },
            transport);

        var order = await broker.Trading.Orders.GetOrderAsync("000123", "116");

        transport.Requests[0].Url.Should().Be(
            new Uri("https://api.dnse.example/accounts/000123/orders/116?marketType=DERIVATIVE&orderCategory=NORMAL"));
        order.OrderId.Should().Be("116");
        order.Side.Should().Be(OrderSide.Sell);
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

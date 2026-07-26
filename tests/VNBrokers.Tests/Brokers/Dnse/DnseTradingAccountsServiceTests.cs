using System.Text.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VNBrokers.Brokers.Dnse;
using VNBrokers.Errors;
using VNBrokers.Transport;

namespace VNBrokers.Tests.Brokers.Dnse;

[TestClass]
public sealed class DnseTradingAccountsServiceTests
{
    [TestMethod]
    public async Task ListAccounts_builds_signed_dnse_request()
    {
        using var document = JsonDocument.Parse(
            """{"name":"Nguyen Van A","accounts":[{"id":"000123"}]}""");
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
            },
            transport);

        var accounts = await broker.Trading.Accounts.ListAccountsAsync();

        var request = transport.Requests[0];
        request.Method.Should().Be(HttpMethod.Get);
        request.Url.Should().Be(new Uri("https://api.dnse.example/accounts"));
        request.Headers.Should().ContainKey("X-API-Key");
        request.Headers.Should().ContainKey("X-Aux-Date");
        request.Headers.Should().ContainKey("X-Signature");
        accounts[0].AccountId.Should().Be("000123");
    }

    [TestMethod]
    public async Task GetBalance_url_encodes_account_id()
    {
        using var document = JsonDocument.Parse("""{"stock":{"totalCash":1000,"availableCash":700}}""");
        var transport = new FakeHttpTransport(new HttpTransportResponse(
            200,
            new Dictionary<string, string>(),
            document.RootElement.Clone(),
            null));
        var broker = new DnseBroker(
            new DnseConfig { BaseUrl = "https://api.dnse.example", ApiKey = "key" },
            transport);

        var balance = await broker.Trading.Accounts.GetBalanceAsync("000 123");

        transport.Requests[0].Url.Should()
            .Be(new Uri("https://api.dnse.example/accounts/000%20123/balances"));
        balance.CashAvailable.Should().Be(700m);
    }

    [TestMethod]
    public async Task Account_query_raises_broker_rejected_exception()
    {
        using var document = JsonDocument.Parse("""{"code":"AUTH-001","message":"Forbidden"}""");
        var transport = new FakeHttpTransport(new HttpTransportResponse(
            403,
            new Dictionary<string, string>(),
            document.RootElement.Clone(),
            null));
        var broker = new DnseBroker(
            new DnseConfig { BaseUrl = "https://api.dnse.example", ApiKey = "key" },
            transport);

        var act = async () => await broker.Trading.Accounts.ListAccountsAsync();

        var assertion = await act.Should().ThrowAsync<BrokerRejectedException>();
        assertion.Which.Broker.Should().Be("dnse");
        assertion.Which.Code.Should().Be("AUTH-001");
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

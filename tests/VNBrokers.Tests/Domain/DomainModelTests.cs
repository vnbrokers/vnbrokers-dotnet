using System.Text.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VNBrokers.Domain;

namespace VNBrokers.Tests.Domain;

[TestClass]
public sealed class DomainModelTests
{
    [TestMethod]
    public void RawPayload_preserves_source_and_json_data()
    {
        using var document = JsonDocument.Parse("""{"id":"ORD-1","quantity":10}""");
        var payload = new RawPayload("dnse", document.RootElement.Clone());

        payload.Source.Should().Be("dnse");
        payload.Data.GetProperty("id").GetString().Should().Be("ORD-1");
        payload.Data.GetProperty("quantity").GetInt32().Should().Be(10);
    }

    [TestMethod]
    public void PlaceOrderRequest_defaults_time_in_force_to_day()
    {
        var request = new PlaceOrderRequest(
            AccountId: "0001",
            Symbol: "HPG",
            Side: OrderSide.Buy,
            OrderType: OrderType.Limit,
            Quantity: 100,
            Price: 27.3m);

        request.TimeInForce.Should().Be(TimeInForce.Day);
    }

    [TestMethod]
    public void Balance_defaults_currency_to_vnd()
    {
        var balance = new Balance(AccountId: "0001");

        balance.Currency.Should().Be("VND");
    }

    [TestMethod]
    public void Unknown_enum_values_are_available_for_lossy_broker_mapping()
    {
        OrderStatus.Unknown.Should().BeDefined();
        OrderType.Unknown.Should().BeDefined();
        TimeInForce.Unknown.Should().BeDefined();
    }
}

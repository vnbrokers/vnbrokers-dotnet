using System.Text.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VNBrokers.Core;
using VNBrokers.Errors;

namespace VNBrokers.Tests.Errors;

[TestClass]
public sealed class ErrorTests
{
    [TestMethod]
    public void BrokerRejectedException_preserves_broker_code_and_raw_payload()
    {
        using var document = JsonDocument.Parse("""{"code":"E401","message":"Rejected"}""");

        var exception = new BrokerRejectedException(
            "Rejected",
            broker: "dnse",
            code: "E401",
            raw: document.RootElement.Clone());

        exception.Message.Should().Be("Rejected");
        exception.Broker.Should().Be("dnse");
        exception.Code.Should().Be("E401");
        exception.Raw.Should().NotBeNull();
    }

    [TestMethod]
    public void UnsupportedCapabilityException_mentions_broker_and_wire_capability_value()
    {
        var exception = new UnsupportedCapabilityException(
            "ssi",
            Capability.TradingOrdersPlace);

        exception.Message.Should().Be(
            "Broker 'ssi' does not support capability 'trading.orders.place'");
        exception.Broker.Should().Be("ssi");
        exception.Capability.Should().Be(Capability.TradingOrdersPlace);
    }
}

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VNBrokers.Core;
using VNBrokers.Errors;

namespace VNBrokers.Tests.Core;

[TestClass]
public sealed class CapabilityTests
{
    [TestMethod]
    public void ToWireValue_returns_python_capability_values()
    {
        Capability.TradingAccountsList.ToWireValue().Should().Be("trading.accounts.list");
        Capability.TradingOrdersPlace.ToWireValue().Should().Be("trading.orders.place");
        Capability.BrokerageCareBy.ToWireValue().Should().Be("brokerage.accounts.care_by");
        Capability.MarketDataRealtimeTopPrice.ToWireValue().Should()
            .Be("marketdata.realtime.top_price");
    }

    [TestMethod]
    public void BrokerBase_enforces_supported_capabilities()
    {
        var broker = new TestBroker("test", Capability.TradingAccountsList);

        broker.Supports(Capability.TradingAccountsList).Should().BeTrue();
        broker.Supports(Capability.TradingOrdersPlace).Should().BeFalse();
        broker.Invoking(item => item.RequireCapability(Capability.TradingOrdersPlace))
            .Should()
            .Throw<UnsupportedCapabilityException>()
            .WithMessage("Broker 'test' does not support capability 'trading.orders.place'");
    }

    private sealed class TestBroker : BrokerBase
    {
        public TestBroker(string name, params Capability[] capabilities)
            : base(name, capabilities)
        {
        }
    }
}

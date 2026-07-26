using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VNBrokers.Brokers;
using VNBrokers.Brokers.Dnse;
using VNBrokers.Brokers.Ssi;
using VNBrokers.Brokers.Tcbs;
using VNBrokers.Core;

namespace VNBrokers.Tests.Core;

[TestClass]
public sealed class BrokerFactoryTests
{
    [TestMethod]
    public void ListBrokers_returns_builtin_names()
    {
        BrokerRegistry.ListBrokers().Should().Equal("dnse", "ssi", "tcbs");
    }

    [TestMethod]
    public void CreateBroker_accepts_explicit_config()
    {
        var broker = BrokerRegistry.CreateBroker("dnse", new DnseConfig { ApiKey = "key" });

        broker.Should().BeOfType<DnseBroker>();
        ((DnseBroker)broker).Config.ApiKey.Should().Be("key");
    }

    [TestMethod]
    public void CreateBroker_normalizes_broker_name()
    {
        var broker = BrokerRegistry.CreateBroker(" TCBS ", new TcbsConfig());

        broker.Should().BeOfType<TcbsBroker>();
    }

    [TestMethod]
    public void CreateBroker_builds_config_from_factory()
    {
        var broker = BrokerRegistry.CreateBroker("ssi", () => new SsiConfig
        {
            ApiKey = "key",
            ApiSecret = "secret",
        });

        broker.Should().BeOfType<SsiBroker>();
        ((SsiBroker)broker).Config.ApiKey.Should().Be("key");
        ((SsiBroker)broker).Config.ApiSecret.Should().Be("secret");
    }

    [TestMethod]
    public void GetBrokerRegistration_returns_registered_types()
    {
        var registration = BrokerRegistry.GetBrokerRegistration("dnse");

        registration.Name.Should().Be("dnse");
        registration.BrokerType.Should().Be(typeof(DnseBroker));
        registration.ConfigType.Should().Be(typeof(DnseConfig));
    }

    [TestMethod]
    public void CreateBroker_rejects_unknown_broker()
    {
        Action act = () => BrokerRegistry.CreateBroker("unknown", new DnseConfig());

        act.Should().Throw<ArgumentException>().WithMessage("Unsupported broker: unknown*");
    }
}

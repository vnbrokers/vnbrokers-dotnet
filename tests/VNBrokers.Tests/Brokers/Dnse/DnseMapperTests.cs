using System.Text.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VNBrokers.Brokers.Dnse;
using VNBrokers.Domain;

namespace VNBrokers.Tests.Brokers.Dnse;

[TestClass]
public sealed class DnseMapperTests
{
    [TestMethod]
    public void MapAccounts_returns_accounts_with_display_name_and_raw_payload()
    {
        using var document = JsonDocument.Parse(
            """{"name":"Nguyen Van A","accounts":[{"id":"000123"},{"id":"000456"}]}""");

        var accounts = DnseMapper.MapAccounts(document.RootElement);

        accounts.Should().HaveCount(2);
        accounts[0].AccountId.Should().Be("000123");
        accounts[0].Broker.Should().Be("dnse");
        accounts[0].DisplayName.Should().Be("Nguyen Van A");
        accounts[0].Raw.Should().NotBeNull();
    }

    [TestMethod]
    public void MapBalance_maps_stock_cash_fields()
    {
        using var document = JsonDocument.Parse("""{"stock":{"totalCash":1000,"availableCash":700}}""");

        var balance = DnseMapper.MapBalance("000123", document.RootElement);

        balance.AccountId.Should().Be("000123");
        balance.CashAvailable.Should().Be(700m);
        balance.CashTotal.Should().Be(1000m);
        balance.BuyingPower.Should().Be(700m);
        balance.Currency.Should().Be("VND");
    }

    [TestMethod]
    public void MapOrder_maps_dnse_wire_values_to_domain_values()
    {
        using var document = JsonDocument.Parse(
            """{"id":"OID1","accountNo":"000123","symbol":"HPG","side":"NB","orderType":"LO","orderStatus":"NEW","quantity":100,"price":27.3}""");

        var order = DnseMapper.MapOrder(document.RootElement);

        order.OrderId.Should().Be("OID1");
        order.Side.Should().Be(OrderSide.Buy);
        order.OrderType.Should().Be(OrderType.Limit);
        order.Status.Should().Be(OrderStatus.Accepted);
        order.Quantity.Should().Be(100m);
        order.Price.Should().Be(27.3m);
    }

    [TestMethod]
    public void MapPositions_uses_open_quantity_and_market_value()
    {
        using var document = JsonDocument.Parse(
            """{"positions":[{"accountNo":"000123D","symbol":"VN30F2506","openQuantity":2,"costPrice":1200,"marketPrice":1210}]}""");

        var positions = DnseMapper.MapPositions(document.RootElement);

        positions.Should().HaveCount(1);
        positions[0].AccountId.Should().Be("000123D");
        positions[0].Symbol.Should().Be("VN30F2506");
        positions[0].Quantity.Should().Be(2m);
        positions[0].AveragePrice.Should().Be(1200m);
        positions[0].MarketValue.Should().Be(2420m);
    }

    [TestMethod]
    public void MapOrderStatus_returns_unknown_for_unmapped_status()
    {
        DnseMapper.MapOrderStatus("SOMETHING_NEW").Should().Be(OrderStatus.Unknown);
        DnseMapper.MapOrderStatus(null).Should().Be(OrderStatus.Unknown);
    }
}

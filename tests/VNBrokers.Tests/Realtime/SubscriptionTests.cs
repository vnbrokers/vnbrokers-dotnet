using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VNBrokers.Realtime;

namespace VNBrokers.Tests.Realtime;

[TestClass]
public sealed class SubscriptionTests
{
    [TestMethod]
    public async Task ChannelSubscription_publishes_events_errors_status_and_closed()
    {
        var subscription = new ChannelSubscription<string>();
        var error = new InvalidOperationException("decode failed");

        subscription.PublishStatus(ConnectionStatus.Connected);
        subscription.PublishEvent("evt-1");
        subscription.PublishError(error);
        await subscription.CloseAsync();

        var events = await ReadAllAsync(subscription.EventsAsync());
        var errors = await ReadAllAsync(subscription.ErrorsAsync());
        var statuses = await ReadAllAsync(subscription.StatusAsync());

        events.Should().Equal("evt-1");
        errors.Should().Equal(error);
        statuses.Should().ContainInOrder(ConnectionStatus.Connected, ConnectionStatus.Closed);
        subscription.Closed.Should().BeTrue();
    }

    [TestMethod]
    public async Task ChannelSubscription_close_is_idempotent()
    {
        var subscription = new ChannelSubscription<string>();

        await subscription.CloseAsync();
        await subscription.CloseAsync();

        subscription.Closed.Should().BeTrue();
    }

    private static async Task<IReadOnlyList<T>> ReadAllAsync<T>(IAsyncEnumerable<T> source)
    {
        var values = new List<T>();
        await foreach (var value in source)
        {
            values.Add(value);
        }

        return values;
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using AlterNats;
using Squadron;
using Xunit;
using Xunit.Abstractions;

namespace Nats.Tests;

public class NatsResourceTests : IClassFixture<NatsResource>
{
    private readonly NatsResource _nats;
    private readonly ITestOutputHelper _helper;

    public NatsResourceTests(NatsResource nats, ITestOutputHelper helper)
    {
        _nats = nats;
        _helper = helper;
    }
    
    [Fact]
    public async Task Client_Will_Connect()
    {
        var options = NatsOptions.Default with { Url = _nats.NatsConnectionString };
        var client = new NatsConnection(options);

        await client.ConnectAsync();
        _helper.WriteLine("NATS Client is connected to: " + _nats.NatsConnectionString);

        await client.DisposeAsync();
        _helper.WriteLine("NATS Client disconnected.");

        Assert.True(true);
    }

    [Fact]
    public async Task Publish_Subscribe_Topic()
    {
        var options = NatsOptions.Default with { Url = _nats.NatsConnectionString };
        var client = new NatsConnection(options);

        await client.ConnectAsync();
        _helper.WriteLine("NATS Client is connected to: " + _nats.NatsConnectionString);

        var semaphore = new SemaphoreSlim(0);
        await client.SubscribeAsync("test", () =>
        {
             _helper.WriteLine("Received message on test topic");
             semaphore.Release();
        });

        var timeout = Task.Delay(TimeSpan.FromSeconds(2));

        await client.PublishAsync("test", "Hello World!");
        _helper.WriteLine("Published message on test topic");

        Task winner = await Task.WhenAny(new[]
        {
            semaphore.WaitAsync(),
            timeout
        });

        if (winner == timeout)
        {
            Assert.True(false, "Timeout waiting for message");
        }

        await client.DisposeAsync();
        semaphore.Dispose();

        _helper.WriteLine("NATS Client disconnected.");

        Assert.True(true);

    }
}

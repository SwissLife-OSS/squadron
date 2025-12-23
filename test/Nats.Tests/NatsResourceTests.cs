using System;
using System.Threading;
using System.Threading.Tasks;
using AlterNats;
using Squadron;
using Xunit;
using Xunit.Abstractions;

namespace Nats.Tests;

public class NatsResourceTests(NatsResource nats, ITestOutputHelper helper) : IClassFixture<NatsResource>
{
    [Fact]
    public async Task Client_Will_Connect()
    {
        var options = NatsOptions.Default with { Url = nats.NatsConnectionString };
        var client = new NatsConnection(options);

        await client.ConnectAsync();
        helper.WriteLine("NATS Client is connected to: " + nats.NatsConnectionString);

        await client.DisposeAsync();
        helper.WriteLine("NATS Client disconnected.");

        Assert.True(true);
    }

    [Fact]
    public async Task Publish_Subscribe_Topic()
    {
        var options = NatsOptions.Default with { Url = nats.NatsConnectionString };
        var client = new NatsConnection(options);

        await client.ConnectAsync();
        helper.WriteLine("NATS Client is connected to: " + nats.NatsConnectionString);

        var semaphore = new SemaphoreSlim(0);
        await client.SubscribeAsync("test", () =>
        {
             helper.WriteLine("Received message on test topic");
             semaphore.Release();
        });

        var timeout = Task.Delay(TimeSpan.FromSeconds(2));

        await client.PublishAsync("test", "Hello World!");
        helper.WriteLine("Published message on test topic");

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

        helper.WriteLine("NATS Client disconnected.");

        Assert.True(true);
    }
}

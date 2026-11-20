using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MassTransit.ActiveMQ;
using SharpAbp.Abp.MassTransit.PostgreSql;
using SharpAbp.Abp.MassTransit.RabbitMQ;
using SharpAbp.Abp.MassTransit.SqlServer;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.MassTransit;

public class ProduceServiceDeliveryTest
{
    [Theory]
    [InlineData("RabbitMQ")]
    [InlineData("ActiveMQ")]
    [InlineData("PostgreSql")]
    [InlineData("SqlServer")]
    public async Task Real_Produce_Services_Should_Deliver_All_Publish_And_Send_Overloads_Through_InMemory_Transport(string providerName)
    {
        var received = Channel.CreateUnbounded<DeliveryMessage>();
        var queueName = "unit-" + Guid.NewGuid().ToString("N");
        var bus = Bus.Factory.CreateUsingInMemory(configuration =>
            configuration.ReceiveEndpoint(queueName, endpoint => endpoint.Handler<DeliveryMessage>(context =>
                received.Writer.WriteAsync(context.Message).AsTask())));
        using var services = new ServiceCollection()
            .AddSingleton<IPublishEndpoint>(bus)
            .AddSingleton<ISendEndpointProvider>(bus)
            .BuildServiceProvider();
        var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();
        IPublishProvider provider = providerName switch
        {
            "RabbitMQ" => new RabbitMqPublishProvider(new RabbitMqProduceService(scopeFactory)),
            "ActiveMQ" => new ActiveMqPublishProvider(new ActiveMqProduceService(scopeFactory)),
            "PostgreSql" => new PostgreSqlPublishProvider(new PostgreSqlProduceService(scopeFactory)),
            "SqlServer" => new SqlServerPublishProvider(new SqlServerProduceService(scopeFactory)),
            _ => throw new ArgumentOutOfRangeException(nameof(providerName))
        };
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        await bus.StartAsync(timeout.Token);
        try
        {
            await provider.PublishAsync(new DeliveryMessage { Value = "generic-publish" }, timeout.Token);
            Assert.Equal("generic-publish", (await received.Reader.ReadAsync(timeout.Token)).Value);
            await provider.PublishAsync(new DeliveryMessage { Value = "dynamic-publish" }, messageType: null, cancellationToken: timeout.Token);
            Assert.Equal("dynamic-publish", (await received.Reader.ReadAsync(timeout.Token)).Value);
            await provider.PublishAsync(new DeliveryMessage { Value = "explicit-publish" }, typeof(DeliveryMessage), timeout.Token);
            Assert.Equal("explicit-publish", (await received.Reader.ReadAsync(timeout.Token)).Value);
            await provider.SendAsync("queue:" + queueName, new DeliveryMessage { Value = "generic-send" }, timeout.Token);
            Assert.Equal("generic-send", (await received.Reader.ReadAsync(timeout.Token)).Value);
            await provider.SendAsync("queue:" + queueName, (object)new DeliveryMessage { Value = "dynamic-send" }, timeout.Token);
            Assert.Equal("dynamic-send", (await received.Reader.ReadAsync(timeout.Token)).Value);
        }
        finally
        {
            await bus.StopAsync(CancellationToken.None);
        }
    }
}

public class DeliveryMessage
{
    public string Value { get; set; }
}

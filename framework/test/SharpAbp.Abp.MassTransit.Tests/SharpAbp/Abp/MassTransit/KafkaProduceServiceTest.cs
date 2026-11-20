using MassTransit;
using MassTransit.KafkaIntegration;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SharpAbp.Abp.MassTransit.Kafka;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.MassTransit;

public class KafkaProduceServiceTest
{
    [Fact]
    public async Task Publish_Overloads_Should_Resolve_Real_Service_And_Produce_Message_With_Unique_Keys()
    {
        var produced = new List<(string Key, DeliveryMessage Message, CancellationToken Token)>();
        var producer = new Mock<ITopicProducer<string, DeliveryMessage>>(MockBehavior.Strict);
        producer.Setup(x => x.Produce(It.IsAny<string>(), It.IsAny<DeliveryMessage>(), It.IsAny<CancellationToken>()))
            .Callback<string, DeliveryMessage, CancellationToken>((key, message, token) => produced.Add((key, message, token)))
            .Returns(Task.CompletedTask);
        using var services = new ServiceCollection().AddScoped(_ => producer.Object).BuildServiceProvider();
        var service = new KafkaProduceService(services.GetRequiredService<IServiceScopeFactory>());
        var message = new DeliveryMessage { Value = "payload" };
        using var cancellation = new CancellationTokenSource();

        await service.ProduceStringKeyAsync(message, cancellation.Token);
        await service.PublishAsync(message, cancellation.Token);
        await service.PublishAsync(message, messageType: null, cancellationToken: cancellation.Token);
        await service.PublishAsync(message, typeof(DeliveryMessage), cancellation.Token);

        Assert.Equal(4, produced.Count);
        var keys = new HashSet<string>();
        foreach (var item in produced)
        {
            Assert.Same(message, item.Message);
            Assert.Equal(cancellation.Token, item.Token);
            Assert.True(Guid.TryParse(item.Key, out _));
            Assert.Equal(item.Key.ToUpperInvariant(), item.Key);
            Assert.True(keys.Add(item.Key));
        }
    }

    [Fact]
    public async Task Produce_Should_Preserve_Explicit_Key_And_Cancellation()
    {
        (int Key, DeliveryMessage Message, CancellationToken Token)? produced = null;
        var producer = new Mock<ITopicProducer<int, DeliveryMessage>>(MockBehavior.Strict);
        producer.Setup(x => x.Produce(It.IsAny<int>(), It.IsAny<DeliveryMessage>(), It.IsAny<CancellationToken>()))
            .Callback<int, DeliveryMessage, CancellationToken>((key, message, token) => produced = (key, message, token))
            .Returns(Task.CompletedTask);
        using var services = new ServiceCollection().AddScoped(_ => producer.Object).BuildServiceProvider();
        var service = new KafkaProduceService(services.GetRequiredService<IServiceScopeFactory>());
        var message = new DeliveryMessage { Value = "payload" };
        using var cancellation = new CancellationTokenSource();

        await service.ProduceAsync(42, message, cancellation.Token);

        Assert.True(produced.HasValue);
        Assert.Equal(42, produced.Value.Key);
        Assert.Same(message, produced.Value.Message);
        Assert.Equal(cancellation.Token, produced.Value.Token);
    }

    [Theory]
    [InlineData("topic:orders", false)]
    [InlineData("queue:orders", false)]
    [InlineData("kafka://broker/orders", false)]
    [InlineData("topic:orders", true)]
    [InlineData("queue:orders", true)]
    [InlineData("kafka://broker/orders", true)]
    public async Task Send_Should_Resolve_Target_Topic_For_Typed_And_Dynamic_Messages(string address, bool dynamicMessage)
    {
        var sent = new List<DeliveryMessage>();
        var producer = new Mock<ITopicProducer<Null, DeliveryMessage>>(MockBehavior.Strict);
        using var cancellation = new CancellationTokenSource();
        producer.Setup(x => x.Produce(It.IsAny<Null>(), It.IsAny<DeliveryMessage>(), cancellation.Token))
            .Callback<Null, DeliveryMessage, CancellationToken>((_, message, _) => sent.Add(message))
            .Returns(Task.CompletedTask);
        producer.Setup(x => x.Produce(It.IsAny<Null>(), It.IsAny<DeliveryMessage>(), It.IsAny<IPipe<KafkaSendContext<Null, DeliveryMessage>>>(), cancellation.Token))
            .Callback<Null, DeliveryMessage, IPipe<KafkaSendContext<Null, DeliveryMessage>>, CancellationToken>((_, message, _, _) => sent.Add(message))
            .Returns(Task.CompletedTask);
        Uri target = null;
        var provider = new Mock<ITopicProducerProvider>(MockBehavior.Strict);
        provider.Setup(x => x.GetProducer<Null, DeliveryMessage>(It.IsAny<Uri>()))
            .Callback<Uri>(uri => target = uri)
            .Returns(producer.Object);
        using var services = new ServiceCollection().AddScoped(_ => provider.Object).BuildServiceProvider();
        var service = new KafkaProduceService(services.GetRequiredService<IServiceScopeFactory>());
        var message = new DeliveryMessage { Value = "payload" };

        if (dynamicMessage)
        {
            await service.SendAsync(address, (object)message, cancellation.Token);
        }
        else
        {
            await service.SendAsync(address, message, cancellation.Token);
        }

        Assert.Equal("topic:orders", target.OriginalString);
        Assert.Same(message, Assert.Single(sent));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Send_Should_Reject_Address_Without_Topic(bool dynamicMessage)
    {
        var provider = new Mock<ITopicProducerProvider>(MockBehavior.Strict);
        using var services = new ServiceCollection().AddScoped(_ => provider.Object).BuildServiceProvider();
        var service = new KafkaProduceService(services.GetRequiredService<IServiceScopeFactory>());
        var message = new DeliveryMessage();

        await Assert.ThrowsAsync<ArgumentException>(() => dynamicMessage
            ? service.SendAsync("topic:/", (object)message)
            : service.SendAsync("topic:/", message));
    }

    [Fact]
    public async Task Dynamic_Publish_Should_Preserve_Asynchronous_Producer_Failure()
    {
        var failure = new IOException("Broker unavailable");
        var producer = new Mock<ITopicProducer<string, DeliveryMessage>>(MockBehavior.Strict);
        producer.Setup(x => x.Produce(It.IsAny<string>(), It.IsAny<DeliveryMessage>(), It.IsAny<CancellationToken>())).ThrowsAsync(failure);
        using var services = new ServiceCollection().AddScoped(_ => producer.Object).BuildServiceProvider();
        var service = new KafkaProduceService(services.GetRequiredService<IServiceScopeFactory>());

        var actual = await Assert.ThrowsAsync<IOException>(() => service.PublishAsync(new DeliveryMessage(), messageType: typeof(DeliveryMessage)));

        Assert.Same(failure, actual);
    }
}

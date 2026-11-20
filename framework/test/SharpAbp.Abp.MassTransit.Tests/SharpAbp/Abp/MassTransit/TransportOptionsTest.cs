using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using SharpAbp.Abp.MassTransit.ActiveMQ;
using SharpAbp.Abp.MassTransit.Kafka;
using SharpAbp.Abp.MassTransit.PostgreSql;
using SharpAbp.Abp.MassTransit.RabbitMQ;
using SharpAbp.Abp.MassTransit.SqlServer;
using Xunit;

namespace SharpAbp.Abp.MassTransit;

public class TransportOptionsTest
{
    [Fact]
    public void RabbitMq_Should_Bind_Settings_And_Copy_Independent_Collections()
    {
        var configuration = Configuration(new()
        {
            ["MassTransitOptions:RabbitMqOptions:Host"] = "broker.example",
            ["MassTransitOptions:RabbitMqOptions:Port"] = "5671",
            ["MassTransitOptions:RabbitMqOptions:VirtualHost"] = "application",
            ["MassTransitOptions:RabbitMqOptions:Username"] = "app",
            ["MassTransitOptions:RabbitMqOptions:Password"] = "test-password",
            ["MassTransitOptions:RabbitMqOptions:ConnectionName"] = "client",
            ["MassTransitOptions:RabbitMqOptions:UseSSL"] = "true",
            ["MassTransitOptions:RabbitMqOptions:UseCluster"] = "true",
            ["MassTransitOptions:RabbitMqOptions:ClusterNodes:0"] = "second.example",
            ["MassTransitOptions:RabbitMqOptions:DefaultQueuePrefix"] = "worker",
            ["MassTransitOptions:RabbitMqOptions:DefaultConcurrentMessageLimit"] = "3",
            ["MassTransitOptions:RabbitMqOptions:DefaultPrefetchCount"] = "12",
            ["MassTransitOptions:RabbitMqOptions:DefaultDurable"] = "false",
            ["MassTransitOptions:RabbitMqOptions:DefaultAutoDelete"] = "true",
            ["MassTransitOptions:RabbitMqOptions:DefaultExchangeType"] = "direct"
        });
        var source = new AbpMassTransitRabbitMqOptions().PreConfigure(configuration);
        source.Producers.Add(new RabbitMqProducerConfiguration { ExchangeName = "orders" });
        source.Consumers.Add(new RabbitMqConsumerConfiguration { ExchangeName = "orders", QueueName = "orders-worker" });
        source.DefaultQueueNameFormatFunc = RabbitMqUtil.QueueNameFormat;
        source.RabbitMqPreConfigures.Add((_, _) => { });
        source.RabbitMqConfigures.Add((_, _) => { });
        source.RabbitMqPostConfigures.Add((_, _) => { });

        var copy = new AbpMassTransitRabbitMqOptions().CopyFrom(source);
        source.ClusterNodes.Clear();
        source.Producers.Clear();
        source.Consumers.Clear();
        source.RabbitMqConfigures.Clear();

        Assert.Equal("broker.example", copy.Host);
        Assert.Equal(5671, copy.Port);
        Assert.Equal("application", copy.VirtualHost);
        Assert.Equal("app", copy.Username);
        Assert.Equal("test-password", copy.Password);
        Assert.Equal("client", copy.ConnectionName);
        Assert.True(copy.UseSSL);
        Assert.True(copy.UseCluster);
        Assert.Equal(new[] { "second.example" }, copy.ClusterNodes);
        Assert.Equal("prefix.worker.orders", copy.DefaultQueueNameFormatFunc("prefix", copy.DefaultQueuePrefix, "orders"));
        Assert.Equal(3, copy.DefaultConcurrentMessageLimit);
        Assert.Equal(12, copy.DefaultPrefetchCount);
        Assert.False(copy.DefaultDurable);
        Assert.True(copy.DefaultAutoDelete);
        Assert.Equal("direct", copy.DefaultExchangeType);
        Assert.Single(copy.Producers);
        Assert.Single(copy.Consumers);
        Assert.Single(copy.RabbitMqPreConfigures);
        Assert.Single(copy.RabbitMqConfigures);
        Assert.Single(copy.RabbitMqPostConfigures);
        Assert.Throws<ArgumentNullException>(() => copy.CopyFrom(null));
    }

    [Fact]
    public void ActiveMq_Should_Bind_Settings_And_Preserve_Callbacks_When_Copying()
    {
        var source = new AbpMassTransitActiveMqOptions().PreConfigure(Configuration(new()
        {
            ["MassTransitOptions:ActiveMqOptions:Host"] = "broker.example",
            ["MassTransitOptions:ActiveMqOptions:Port"] = "61617",
            ["MassTransitOptions:ActiveMqOptions:Username"] = "app",
            ["MassTransitOptions:ActiveMqOptions:Password"] = "test-password",
            ["MassTransitOptions:ActiveMqOptions:UseSSL"] = "true",
            ["MassTransitOptions:ActiveMqOptions:DefaultConcurrentMessageLimit"] = "3",
            ["MassTransitOptions:ActiveMqOptions:DefaultPrefetchCount"] = "12",
            ["MassTransitOptions:ActiveMqOptions:DefaultDurable"] = "false",
            ["MassTransitOptions:ActiveMqOptions:DefaultAutoDelete"] = "true",
            ["MassTransitOptions:ActiveMqOptions:DefaultExclude"] = "true"
        }));
        source.Producers.Add(new ActiveMqProducerConfiguration());
        source.Consumers.Add(new ActiveMqConsumerConfiguration());
        source.ActiveMqPreConfigures.Add((_, _) => { });
        source.ActiveMqConfigures.Add((_, _) => { });
        source.ActiveMqPostConfigures.Add((_, _) => { });
        source.DefaultQueueNameFormatFunc = ActiveMqUtil.QueueNameFormat;

        var copy = new AbpMassTransitActiveMqOptions().CopyFrom(source);
        source.Producers.Clear();
        source.Consumers.Clear();
        source.ActiveMqConfigures.Clear();

        Assert.Equal("broker.example", copy.Host);
        Assert.Equal(61617, copy.Port);
        Assert.Equal("app", copy.Username);
        Assert.Equal("test-password", copy.Password);
        Assert.True(copy.UseSSL);
        Assert.Equal("prefix.orders", copy.DefaultQueueNameFormatFunc("prefix", "orders"));
        Assert.Equal(3, copy.DefaultConcurrentMessageLimit);
        Assert.Equal(12, copy.DefaultPrefetchCount);
        Assert.False(copy.DefaultDurable);
        Assert.True(copy.DefaultAutoDelete);
        Assert.True(copy.DefaultExclude);
        Assert.Single(copy.Producers);
        Assert.Single(copy.Consumers);
        Assert.Single(copy.ActiveMqPreConfigures);
        Assert.Single(copy.ActiveMqConfigures);
        Assert.Single(copy.ActiveMqPostConfigures);
        Assert.Throws<ArgumentNullException>(() => copy.CopyFrom(null));
    }

    [Fact]
    public void Kafka_Should_Bind_Consumer_Policy_And_Copy_Independent_Pipelines()
    {
        var source = new AbpMassTransitKafkaOptions().PreConfigure(Configuration(new()
        {
            ["MassTransitOptions:KafkaOptions:Server"] = "broker.example:9093",
            ["MassTransitOptions:KafkaOptions:UseSSL"] = "true",
            ["MassTransitOptions:KafkaOptions:DefaultClientId"] = "client",
            ["MassTransitOptions:KafkaOptions:DefaultGroupId"] = "group",
            ["MassTransitOptions:KafkaOptions:DefaultConcurrentMessageLimit"] = "3",
            ["MassTransitOptions:KafkaOptions:AutoCreateTopic"] = "false",
            ["MassTransitOptions:KafkaOptions:DefaultNumPartitions"] = "5",
            ["MassTransitOptions:KafkaOptions:DefaultReplicationFactor"] = "2",
            ["MassTransitOptions:KafkaOptions:DefaultConcurrentConsumerLimit"] = "4",
            ["MassTransitOptions:KafkaOptions:DefaultMessageLimit"] = "100",
            ["MassTransitOptions:KafkaOptions:DefaultPrefetchCount"] = "50",
            ["MassTransitOptions:KafkaOptions:DefaultMaxPollInterval"] = "00:02:00",
            ["MassTransitOptions:KafkaOptions:DefaultSessionTimeout"] = "00:00:30",
            ["MassTransitOptions:KafkaOptions:DefaultCheckpointInterval"] = "00:00:10",
            ["MassTransitOptions:KafkaOptions:DefaultCheckpointMessageCount"] = "75",
            ["MassTransitOptions:KafkaOptions:DefaultEnableAutoOffsetStore"] = "true",
            ["MassTransitOptions:KafkaOptions:DefaultAutoOffsetReset"] = "Latest",
            ["MassTransitOptions:KafkaOptions:DefaultHeartbeatInterval"] = "00:00:03",
            ["MassTransitOptions:KafkaOptions:DefaultReconnectBackoff"] = "00:00:01",
            ["MassTransitOptions:KafkaOptions:DefaultReconnectBackoffMax"] = "00:00:05"
        }));
        source.DefaultTopicFormatFunc = KafkaUtil.TopicFormat;
        source.Producers.Add(new KafkaProducerConfiguration { Topic = "orders" });
        source.Consumers.Add(new KafkaConsumerConfiguration { Topic = "orders", GroupId = "group" });
        source.RiderPreConfigures.Add(_ => { });
        source.RiderConfigures.Add(_ => { });
        source.RiderPostConfigures.Add(_ => { });
        source.KafkaPreConfigures.Add((_, _) => { });
        source.KafkaConfigures.Add((_, _) => { });
        source.KafkaPostConfigures.Add((_, _) => { });

        var copy = new AbpMassTransitKafkaOptions().CopyFrom(source);
        source.Producers.Clear();
        source.Consumers.Clear();
        source.RiderConfigures.Clear();
        source.KafkaConfigures.Clear();

        Assert.Equal("broker.example:9093", copy.Server);
        Assert.True(copy.UseSSL);
        Assert.Equal("client", copy.DefaultClientId);
        Assert.Equal("group", copy.DefaultGroupId);
        Assert.False(copy.AutoCreateTopic);
        Assert.Equal(3, copy.DefaultConcurrentMessageLimit);
        Assert.Equal(5, copy.DefaultNumPartitions);
        Assert.Equal(2, copy.DefaultReplicationFactor);
        Assert.Equal(4, copy.DefaultConcurrentConsumerLimit);
        Assert.Equal(100, copy.DefaultMessageLimit);
        Assert.Equal(50, copy.DefaultPrefetchCount);
        Assert.Equal(TimeSpan.FromMinutes(2), copy.DefaultMaxPollInterval);
        Assert.Equal(TimeSpan.FromSeconds(30), copy.DefaultSessionTimeout);
        Assert.Equal(TimeSpan.FromSeconds(10), copy.DefaultCheckpointInterval);
        Assert.Equal(75, copy.DefaultCheckpointMessageCount);
        Assert.True(copy.DefaultEnableAutoOffsetStore);
        Assert.Equal(Confluent.Kafka.AutoOffsetReset.Latest, copy.DefaultAutoOffsetReset);
        Assert.Equal(TimeSpan.FromSeconds(3), copy.DefaultHeartbeatInterval);
        Assert.Equal(TimeSpan.FromSeconds(1), copy.DefaultReconnectBackoff);
        Assert.Equal(TimeSpan.FromSeconds(5), copy.DefaultReconnectBackoffMax);
        Assert.Equal("prefix.orders", copy.DefaultTopicFormatFunc("prefix", "orders"));
        Assert.Single(copy.Producers);
        Assert.Single(copy.Consumers);
        Assert.Single(copy.RiderPreConfigures);
        Assert.Single(copy.RiderConfigures);
        Assert.Single(copy.RiderPostConfigures);
        Assert.Single(copy.KafkaPreConfigures);
        Assert.Single(copy.KafkaConfigures);
        Assert.Single(copy.KafkaPostConfigures);
        Assert.Throws<ArgumentNullException>(() => copy.CopyFrom(null));
    }

    [Theory]
    [InlineData("orders", null, null, "orders")]
    [InlineData("orders", "app", null, "app.orders")]
    [InlineData("orders", null, "worker", "worker.orders")]
    [InlineData("orders", "app", "worker", "app.worker.orders")]
    public void Queue_Names_Should_Keep_Optional_Prefixes_In_Order(string queue, string prefix, string worker, string expected)
    {
        Assert.Equal(expected, RabbitMqUtil.QueueNameFormat(prefix, worker, queue));
        Assert.Equal(string.IsNullOrWhiteSpace(prefix) ? "orders" : "app.orders", ActiveMqUtil.QueueNameFormat(prefix, queue));
    }

    private static IConfiguration Configuration(Dictionary<string, string> values) => new ConfigurationBuilder().AddInMemoryCollection(values).Build();
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using global::MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using SharpAbp.Abp.MassTransit.ActiveMQ;
using SharpAbp.Abp.MassTransit.Kafka;
using SharpAbp.Abp.MassTransit.PostgreSql;
using SharpAbp.Abp.MassTransit.RabbitMQ;
using SharpAbp.Abp.MassTransit.SqlServer;
using Volo.Abp;
using Volo.Abp.Modularity;
using Xunit;

namespace SharpAbp.Abp.MassTransit;

public class TransportModuleConfigurationTest
{
    [Theory]
    [InlineData("rAbBiTmQ")]
    [InlineData("aCtIvEmQ")]
    [InlineData("pOsTgReSqL")]
    [InlineData("sQlSeRvEr")]
    [InlineData("kAfKa")]
    public void Should_Build_Selected_Transport_And_Run_Configuration_Pipeline_Without_Starting_Broker(string provider)
    {
        using var application = AbpApplicationFactory.Create<TransportConfigurationTestModule>(creation =>
            creation.Services.PreConfigure<AbpMassTransitOptions>(options => options.Provider = provider));
        application.Initialize();
        var state = application.ServiceProvider.GetRequiredService<TransportConfigurationState>();
        var bus = application.ServiceProvider.GetRequiredService<IBus>();

        Assert.NotNull(bus.Address);
        Assert.Equal(provider, application.ServiceProvider.GetRequiredService<IOptions<AbpMassTransitOptions>>().Value.Provider);
        Assert.Contains("pre", state.Calls);
        Assert.Contains("configure", state.Calls);
        Assert.Contains("post", state.Calls);
        Assert.Equal(new[] { "pre", "configure", "post" }, state.Calls.Where(call => call is "pre" or "configure" or "post"));
        if (provider.ToLowerInvariant() == "rabbitmq")
        {
            Assert.Equal("test.orders", state.MessageName);
            Assert.Equal("test.worker.orders", state.QueueName);
            var options = application.ServiceProvider.GetRequiredService<IOptions<AbpMassTransitRabbitMqOptions>>().Value;
            var endpoint = new Mock<IRabbitMqReceiveEndpointConfigurator>();
            endpoint.SetupAllProperties();
            options.DefaultReceiveEndpointConfigure("test.orders", state.QueueName, endpoint.Object);
            Assert.Equal(3, endpoint.Object.ConcurrentMessageLimit);
            Assert.Equal(12, endpoint.Object.PrefetchCount);
            endpoint.VerifySet(configurator => configurator.Durable = true);
        }
        else if (provider.ToLowerInvariant() == "activemq")
        {
            Assert.Equal("test.orders", state.QueueName);
            var options = application.ServiceProvider.GetRequiredService<IOptions<AbpMassTransitActiveMqOptions>>().Value;
            var endpoint = new Mock<IActiveMqReceiveEndpointConfigurator>();
            endpoint.SetupAllProperties();
            options.DefaultReceiveEndpointConfigure(state.QueueName, endpoint.Object);
            Assert.Equal(3, endpoint.Object.ConcurrentMessageLimit);
            Assert.Equal(12, endpoint.Object.PrefetchCount);
            endpoint.VerifySet(configurator => configurator.Durable = true);
        }
        else if (provider.ToLowerInvariant() == "kafka")
        {
            Assert.Equal("test.orders", state.MessageName);
            Assert.Contains("rider-pre", state.Calls);
            Assert.Contains("rider-configure", state.Calls);
            Assert.Contains("rider-post", state.Calls);
            var options = application.ServiceProvider.GetRequiredService<IOptions<AbpMassTransitKafkaOptions>>().Value;
            var endpoint = new Mock<IKafkaTopicReceiveEndpointConfigurator>();
            endpoint.SetupAllProperties();
            options.DefaultReceiveEndpointConfigure(endpoint.Object);
            Assert.Equal(3, endpoint.Object.ConcurrentMessageLimit);
            Assert.Equal(50, endpoint.Object.PrefetchCount);
        }
        else
        {
            var options = application.ServiceProvider.GetRequiredService<IOptions<SqlTransportOptions>>().Value;
            Assert.Equal("localhost", options.Host);
            Assert.Equal("messages", options.Database);
            Assert.Equal("app", options.Username);
            Assert.Equal("test-password", options.Password);
        }
    }
}

[DependsOn(typeof(AbpMassTransitRabbitMqModule), typeof(AbpMassTransitActiveMqModule),
    typeof(AbpMassTransitKafkaModule), typeof(AbpMassTransitPostgreSqlModule), typeof(AbpMassTransitSqlServerModule),
    typeof(AbpTestBaseModule))]
public class TransportConfigurationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var state = new TransportConfigurationState();
        context.Services.AddSingleton(state);
        PreConfigure<AbpMassTransitOptions>(options => options.Prefix = "test");
        PreConfigure<AbpMassTransitRabbitMqOptions>(options =>
        {
            options.Host = "localhost";
            options.Port = 5672;
            options.VirtualHost = "/";
            options.Username = "app";
            options.Password = "test-password";
            options.ConnectionName = "unit-tests";
            options.DefaultQueuePrefix = "worker";
            options.DefaultConcurrentMessageLimit = 3;
            options.DefaultPrefetchCount = 12;
            options.RabbitMqPreConfigures.Add((_, _) => state.Calls.Add("pre"));
            options.RabbitMqConfigures.Add((_, _) => state.Calls.Add("configure"));
            options.RabbitMqPostConfigures.Add((_, _) => state.Calls.Add("post"));
            options.Producers.Add(new RabbitMqProducerConfiguration
            {
                ExchangeName = "orders",
                MessageConfigure = (name, factory) => { state.MessageName = name; factory.Message<TransportConfigurationMessage>(message => message.SetEntityName(name)); },
                PublishConfigure = (configure, _, factory) => factory.Publish<TransportConfigurationMessage>(topology => configure(topology))
            });
            options.Consumers.Add(new RabbitMqConsumerConfiguration
            {
                ExchangeName = "orders", QueueName = "orders",
                MessageConfigure = (name, factory) => { state.MessageName = name; factory.Message<TransportConfigurationMessage>(message => message.SetEntityName(name)); },
                ReceiveEndpoint = (exchange, queue, configure, _, factory) =>
                {
                    state.QueueName = queue;
                    factory.ReceiveEndpoint(queue, endpoint => { configure(exchange, queue, endpoint); endpoint.Handler<TransportConfigurationMessage>(_ => Task.CompletedTask); });
                }
            });
        });
        PreConfigure<AbpMassTransitActiveMqOptions>(options =>
        {
            options.Host = "localhost";
            options.Port = 61616;
            options.Username = "app";
            options.Password = "test-password";
            options.DefaultConcurrentMessageLimit = 3;
            options.DefaultPrefetchCount = 12;
            options.ActiveMqPreConfigures.Add((_, _) => state.Calls.Add("pre"));
            options.ActiveMqConfigures.Add((_, _) => state.Calls.Add("configure"));
            options.ActiveMqPostConfigures.Add((_, _) => state.Calls.Add("post"));
            options.Producers.Add(new ActiveMqProducerConfiguration
            {
                PublishConfigure = (configure, _, factory) => factory.Publish<TransportConfigurationMessage>(topology => configure(topology))
            });
            options.Consumers.Add(new ActiveMqConsumerConfiguration
            {
                QueueName = "orders",
                ReceiveEndpoint = (queue, configure, _, factory) =>
                {
                    state.QueueName = queue;
                    factory.ReceiveEndpoint(queue, endpoint => { configure(queue, endpoint); endpoint.Handler<TransportConfigurationMessage>(_ => Task.CompletedTask); });
                }
            });
        });
        PreConfigure<AbpMassTransitPostgreSqlOptions>(options =>
        {
            options.ConnectionString = "Host=localhost;Database=messages;Username=app;Password=test-password";
            options.Create = false;
            options.SqlTransportOptions.Host = "localhost";
            options.SqlTransportOptions.Database = "messages";
            options.SqlTransportOptions.Username = "app";
            options.SqlTransportOptions.Password = "test-password";
            options.PostgreSqlPreConfigures.Add((_, _) => state.Calls.Add("pre"));
            options.PostgreSqlConfigures.Add((_, _) => state.Calls.Add("configure"));
            options.PostgreSqlPostConfigures.Add((_, _) => state.Calls.Add("post"));
            options.Producers.Add(new PostgreSqlProducerConfiguration { MessageTypes = [typeof(TransportConfigurationMessage)] });
        });
        PreConfigure<AbpMassTransitSqlServerOptions>(options =>
        {
            options.ConnectionString = "Server=localhost;Database=messages;User ID=app;Password=test-password";
            options.Create = false;
            options.SqlTransportOptions.Host = "localhost";
            options.SqlTransportOptions.Database = "messages";
            options.SqlTransportOptions.Username = "app";
            options.SqlTransportOptions.Password = "test-password";
            options.SqlServerPreConfigures.Add((_, _) => state.Calls.Add("pre"));
            options.SqlServerConfigures.Add((_, _) => state.Calls.Add("configure"));
            options.SqlServerPostConfigures.Add((_, _) => state.Calls.Add("post"));
            options.Producers.Add(new SqlServerProducerConfiguration { MessageTypes = [typeof(TransportConfigurationMessage)] });
        });
        PreConfigure<AbpMassTransitKafkaOptions>(options =>
        {
            options.Server = "localhost:9092";
            options.AutoCreateTopic = false;
            options.DefaultConcurrentMessageLimit = 3;
            options.DefaultPrefetchCount = 50;
            options.KafkaPreConfigures.Add((_, _) => state.Calls.Add("pre"));
            options.KafkaConfigures.Add((_, _) => state.Calls.Add("configure"));
            options.KafkaPostConfigures.Add((_, _) => state.Calls.Add("post"));
            options.RiderPreConfigures.Add(_ => state.Calls.Add("rider-pre"));
            options.RiderConfigures.Add(_ => state.Calls.Add("rider-configure"));
            options.RiderPostConfigures.Add(_ => state.Calls.Add("rider-post"));
            options.Producers.Add(new KafkaProducerConfiguration
            {
                Topic = "orders", Configure = (topic, rider) => { state.MessageName = topic; rider.AddProducer<TransportConfigurationMessage>(topic); }
            });
        });
    }
}

public class TransportConfigurationState
{
    public List<string> Calls { get; } = [];
    public string MessageName { get; set; }
    public string QueueName { get; set; }
}

public class TransportConfigurationMessage
{
    public string Value { get; set; }
}

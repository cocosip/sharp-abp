# MassTransit

> MassTransit module

## Modules

- `SharpAbp.Abp.MassTransit`
- `SharpAbp.Abp.MassTransit.Kafka`
- `SharpAbp.Abp.MassTransit.RabbitMQ`
- `SharpAbp.Abp.MassTransit.ActiveMQ`
  
## Use

``` c#
[DependsOn(
    typeof(AbpMassTransitRabbitMqModule),
    typeof(AbpMassTransitKafkaModule),
    typeof(AbpMassTransitActiveMqModule),
    typeof(AbpAutofacModule)
    )]
public class MassTransitSampleProducerModule : AbpModule
{

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var abpMassTransitOptions = configuration
            .GetSection("MassTransitOptions")
            .Get<AbpMassTransitOptions>();

        if (abpMassTransitOptions.Provider.Equals(MassTransitRabbitMqConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
        {
            PreConfigure<AbpMassTransitRabbitMqOptions>(options =>
            {
                options.Producers.Add(new RabbitMqProducerConfiguration()
                {
                    ExchangeName = RabbitMqQueues.Exchange1,
                    MessageConfigure = new Action<string, IRabbitMqBusFactoryConfigurator>((exchangeName, c) =>
                    {
                        c.Message<MassTransitSampleMessage>(e =>
                        {
                            e.SetEntityName(exchangeName);
                        });
                    }),
                    PublishConfigure = new Action<Action<IRabbitMqMessagePublishTopologyConfigurator>, IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>((preConfigure, ctx, cfg) =>
                    {
                        cfg.Publish<MassTransitSampleMessage>(c =>
                        {
                            preConfigure?.Invoke(c);
                        });
                    })
                });
            });
        }
        else if (abpMassTransitOptions.Provider.Equals(MassTransitKafkaConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
        {
            PreConfigure<AbpMassTransitKafkaOptions>(options =>
            {
                options.Producers.Add(new KafkaProducerConfiguration()
                {
                    Topic = KafkaTopics.Topic1,
                    Configure = new Action<string, IRiderRegistrationConfigurator>((topic, c) =>
                    {
                        c.AddProducer<string, MassTransitSampleMessage>(topic);
                    })
                });
            });
        }
        else if (abpMassTransitOptions.Provider.Equals(MassTransitActiveMqConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
        {
            PreConfigure<AbpMassTransitActiveMqOptions>(options =>
            {
                options.Producers.Add(new ActiveMqProducerConfiguration()
                {
                    QueueName = ActiveMqQueues.Queue1,
                    PublishConfigure = new Action<Action<IActiveMqMessagePublishTopologyConfigurator>, IBusRegistrationContext, IActiveMqBusFactoryConfigurator>((p, ctx, cfg) =>
                    {
                        cfg.Publish<MassTransitSampleMessage>(c =>
                        {
                            p?.Invoke(c);
                        });
                    }),

                    MapConfigure = new Action<Uri>(u =>
                    {
                        EndpointConvention.Map<MassTransitSampleMessage>(u);
                    }),
                });
            });
        }
    }
}
```
> `appsettings.json`
``` json
{
  "MassTransitOptions": {
    "Prefix": "SharpAbp",
    "Provider": "RabbitMQ",
    "WaitUntilStarted": true,
    "StartTimeoutMilliSeconds": 30000,
    "StopTimeoutMilliSeconds": 10000,
    "KafkaOptions": {
      "Server": "192.168.0.100:9092,192.168.0.101:9092,192.168.0.102:9092",
      "UseSSL": false,
      "DefaultGroupId": "SharpAbp.Consumer1",
      "DefaultClientId": "rdkafka",
      "DefaultConcurrentMessageLimit": 1,
      "AutoCreateTopic": true,
      "DefaultNumPartitions": 3,
      "DefaultReplicationFactor": 3,
      "DefaultConcurrentConsumerLimit": 1,
      "DefaultMessageLimit": 1000,
      "DefaultPrefetchCount": 500,
      "DefaultMaxPollInterval": "00:00:30",
      "DefaultSessionTimeout": "00:05:00",
      "DefaultCheckpointInterval": "00:00:20",
      "DefaultCheckpointMessageCount": 1000,
      "DefaultEnableAutoOffsetStore": false,
      "DefaultAutoOffsetReset": "Earliest",
      "DefaultHeartbeatInterval": "00:00:20",
      "DefaultReconnectBackoff": "00:00:10",
      "DefaultReconnectBackoffMax": "00:00:00.100"
    },
    "RabbitMqOptions": {
      "Host": "127.0.0.1",
      "Port": 5672,
      "VirtualHost": "/",
      "Username": "guest",
      "Password": "guest",
      "UseSsl": false,
      "UseCluster": false,
      "ClusterNodes": [],
      "ConnectionName": "",
      "DefaultQueuePrefix": "queue",
      "DefaultConcurrentMessageLimit": 3,
      "DefaultPrefetchCount": 5,
      "DefaultDurable": true,
      "DefaultAutoDelete": false,
      "DefaultExchangeType": "fanout"
    },
    "ActiveMqOptions": {
      "Host": "127.0.0.1",
      "Port": 1331,
      "Username": "admin",
      "Password": "admin",
      "UseSsl": false,
      "DefaultConcurrentMessageLimit": 1,
      "DefaultPrefetchCount": 4,
      "DefaultDurable": true,
      "DefaultAutoDelete": false,
      "DefaultExclude": true,
      "DefaultEnableArtemisCompatibility": true
    }
  }
}
```
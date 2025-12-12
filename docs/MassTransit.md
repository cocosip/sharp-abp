# MassTransit Message Queue

MassTransit message bus integration providing support for RabbitMQ, Kafka, and ActiveMQ.

## Installation

```bash
# Core MassTransit package
dotnet add package SharpAbp.Abp.MassTransit

# Choose your transport:
dotnet add package SharpAbp.Abp.MassTransit.RabbitMQ
dotnet add package SharpAbp.Abp.MassTransit.Kafka
dotnet add package SharpAbp.Abp.MassTransit.ActiveMQ

# Event Bus integration (optional):
dotnet add package SharpAbp.Abp.EventBus.MassTransit
dotnet add package SharpAbp.Abp.EventBus.MassTransit.RabbitMQ
dotnet add package SharpAbp.Abp.EventBus.MassTransit.Kafka
dotnet add package SharpAbp.Abp.EventBus.MassTransit.ActiveMQ
```

## Configuration

### RabbitMQ

Configure in `appsettings.json`:

```json
{
  "MassTransit": {
    "RabbitMQ": {
      "Host": "localhost",
      "Port": 5672,
      "VirtualHost": "/",
      "Username": "guest",
      "Password": "guest"
    }
  }
}
```

Add the module dependency:

```csharp
[DependsOn(
    typeof(AbpMassTransitModule),
    typeof(AbpMassTransitRabbitMQModule)
)]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpMassTransitOptions>(options =>
        {
            options.ConfigureMassTransit = (ctx, cfg) =>
            {
                cfg.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(
                        configuration["MassTransit:RabbitMQ:Host"],
                        configuration.GetValue<ushort>("MassTransit:RabbitMQ:Port"),
                        configuration["MassTransit:RabbitMQ:VirtualHost"],
                        h =>
                        {
                            h.Username(configuration["MassTransit:RabbitMQ:Username"]);
                            h.Password(configuration["MassTransit:RabbitMQ:Password"]);
                        });

                    configurator.ConfigureEndpoints(context);
                });
            };
        });
    }
}
```

### Kafka

Configure in `appsettings.json`:

```json
{
  "MassTransit": {
    "Kafka": {
      "Servers": "localhost:9092",
      "GroupId": "my-consumer-group",
      "TopicPrefix": "my-app"
    }
  }
}
```

Add the module dependency:

```csharp
[DependsOn(
    typeof(AbpMassTransitModule),
    typeof(AbpMassTransitKafkaModule)
)]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpMassTransitOptions>(options =>
        {
            options.ConfigureMassTransit = (ctx, cfg) =>
            {
                cfg.UsingInMemory((context, configurator) =>
                {
                    configurator.ConfigureEndpoints(context);
                });

                cfg.AddRider(rider =>
                {
                    rider.UsingKafka((context, configurator) =>
                    {
                        configurator.Host(configuration["MassTransit:Kafka:Servers"]);
                    });
                });
            };
        });
    }
}
```

### ActiveMQ

Configure in `appsettings.json`:

```json
{
  "MassTransit": {
    "ActiveMQ": {
      "Host": "localhost",
      "Port": 61616,
      "Username": "admin",
      "Password": "admin"
    }
  }
}
```

Add the module dependency:

```csharp
[DependsOn(
    typeof(AbpMassTransitModule),
    typeof(AbpMassTransitActiveMQModule)
)]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpMassTransitOptions>(options =>
        {
            options.ConfigureMassTransit = (ctx, cfg) =>
            {
                cfg.UsingActiveMq((context, configurator) =>
                {
                    configurator.Host(
                        configuration["MassTransit:ActiveMQ:Host"],
                        configuration.GetValue<ushort>("MassTransit:ActiveMQ:Port"),
                        h =>
                        {
                            h.Username(configuration["MassTransit:ActiveMQ:Username"]);
                            h.Password(configuration["MassTransit:ActiveMQ:Password"]);
                        });

                    configurator.ConfigureEndpoints(context);
                });
            };
        });
    }
}
```

## Usage Examples

### Publishing Messages

```csharp
public class OrderService : ApplicationService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task CreateOrderAsync(CreateOrderDto input)
    {
        // Create order logic
        var order = new Order { ... };

        // Publish order created event
        await _publishEndpoint.Publish(new OrderCreatedEvent
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            CreatedTime = DateTime.UtcNow
        });
    }
}
```

### Consuming Messages

```csharp
public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly IEmailSender _emailSender;

    public OrderCreatedConsumer(
        ILogger<OrderCreatedConsumer> logger,
        IEmailSender emailSender)
    {
        _logger = logger;
        _emailSender = emailSender;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("Processing order: {OrderId}", message.OrderId);

        // Send confirmation email
        await _emailSender.SendOrderConfirmationAsync(
            message.CustomerId,
            message.OrderId
        );

        _logger.LogInformation("Order processed: {OrderId}", message.OrderId);
    }
}
```

Register the consumer in your module:

```csharp
Configure<AbpMassTransitOptions>(options =>
{
    options.ConfigureMassTransit = (ctx, cfg) =>
    {
        // Add consumer
        cfg.AddConsumer<OrderCreatedConsumer>();

        cfg.UsingRabbitMq((context, configurator) =>
        {
            configurator.Host(...);
            configurator.ConfigureEndpoints(context);
        });
    };
});
```

### Request/Response Pattern

#### Define Request and Response

```csharp
public class GetCustomerRequest
{
    public Guid CustomerId { get; set; }
}

public class GetCustomerResponse
{
    public Guid CustomerId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
```

#### Request Consumer

```csharp
public class GetCustomerConsumer : IConsumer<GetCustomerRequest>
{
    private readonly IRepository<Customer> _customerRepository;

    public GetCustomerConsumer(IRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task Consume(ConsumeContext<GetCustomerRequest> context)
    {
        var customer = await _customerRepository.GetAsync(context.Message.CustomerId);

        await context.RespondAsync(new GetCustomerResponse
        {
            CustomerId = customer.Id,
            Name = customer.Name,
            Email = customer.Email
        });
    }
}
```

#### Request Client

```csharp
public class OrderService : ApplicationService
{
    private readonly IRequestClient<GetCustomerRequest> _customerClient;

    public OrderService(IRequestClient<GetCustomerRequest> customerClient)
    {
        _customerClient = customerClient;
    }

    public async Task<CustomerDto> GetCustomerInfoAsync(Guid customerId)
    {
        var response = await _customerClient.GetResponse<GetCustomerResponse>(
            new GetCustomerRequest { CustomerId = customerId }
        );

        return new CustomerDto
        {
            Id = response.Message.CustomerId,
            Name = response.Message.Name,
            Email = response.Message.Email
        };
    }
}
```

### Sagas (State Machines)

```csharp
public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public Guid OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedTime { get; set; }
}

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public State Submitted { get; private set; }
    public State PaymentProcessing { get; private set; }
    public State Completed { get; private set; }
    public State Cancelled { get; private set; }

    public Event<OrderSubmittedEvent> OrderSubmitted { get; private set; }
    public Event<PaymentReceivedEvent> PaymentReceived { get; private set; }
    public Event<OrderCancelledEvent> OrderCancelled { get; private set; }

    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => PaymentReceived, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => OrderCancelled, x => x.CorrelateById(m => m.Message.OrderId));

        Initially(
            When(OrderSubmitted)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.TotalAmount = context.Message.TotalAmount;
                    context.Saga.CreatedTime = DateTime.UtcNow;
                })
                .TransitionTo(Submitted)
                .PublishAsync(context => context.Init<OrderAcceptedEvent>(new
                {
                    context.Message.OrderId
                }))
        );

        During(Submitted,
            When(PaymentReceived)
                .TransitionTo(PaymentProcessing)
                .PublishAsync(context => context.Init<ProcessPaymentEvent>(new
                {
                    context.Saga.OrderId,
                    context.Saga.TotalAmount
                })),
            When(OrderCancelled)
                .TransitionTo(Cancelled)
        );

        During(PaymentProcessing,
            When(PaymentReceived)
                .TransitionTo(Completed)
                .Finalize()
        );
    }
}
```

### Event Bus Integration

When using the Event Bus integration, ABP events can be published via MassTransit:

```csharp
[DependsOn(typeof(AbpEventBusMassTransitRabbitMQModule))]
public class YourModule : AbpModule
{
}

// Define an event
public class ProductCreatedEto : EtoBase
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Publish event
public class ProductAppService : ApplicationService
{
    private readonly IDistributedEventBus _distributedEventBus;

    public ProductAppService(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task CreateProductAsync(CreateProductDto input)
    {
        // Create product
        var product = new Product { ... };

        // Publish event (will use MassTransit)
        await _distributedEventBus.PublishAsync(new ProductCreatedEto
        {
            ProductId = product.Id,
            Name = product.Name,
            Price = product.Price
        });
    }
}

// Handle event
public class ProductCreatedEventHandler : IDistributedEventHandler<ProductCreatedEto>
{
    public async Task HandleEventAsync(ProductCreatedEto eventData)
    {
        // Handle the event
        Console.WriteLine($"Product created: {eventData.Name}");
    }
}
```

## Best Practices

### 1. Message Design

Keep messages simple and immutable:

```csharp
public class OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime CreatedTime { get; init; }
}
```

### 2. Error Handling

Implement retry policies:

```csharp
Configure<AbpMassTransitOptions>(options =>
{
    options.ConfigureMassTransit = (ctx, cfg) =>
    {
        cfg.AddConsumer<OrderCreatedConsumer>(consumerConfigurator =>
        {
            consumerConfigurator.UseMessageRetry(r => r.Intervals(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            ));
        });
    };
});
```

### 3. Idempotency

Ensure consumers are idempotent:

```csharp
public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IRepository<ProcessedMessage> _processedMessageRepository;

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var messageId = context.MessageId.ToString();

        // Check if already processed
        if (await _processedMessageRepository.AnyAsync(x => x.MessageId == messageId))
        {
            return; // Already processed
        }

        // Process message
        // ...

        // Mark as processed
        await _processedMessageRepository.InsertAsync(new ProcessedMessage
        {
            MessageId = messageId,
            ProcessedAt = DateTime.UtcNow
        });
    }
}
```

### 4. Monitoring

Use MassTransit's built-in metrics and health checks:

```csharp
services.AddHealthChecks()
    .AddRabbitMQ(configuration["MassTransit:RabbitMQ:ConnectionString"]);
```

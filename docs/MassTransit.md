# MassTransit Message Queue

MassTransit message bus integration providing support for RabbitMQ, Kafka, and ActiveMQ.

Local tests cover ABP registration, transport configuration, in-memory message delivery,
unit-of-work publication and inbox/outbox payload handling. Broker acceptance remains a
separate check. Dynamic inbox replay restores tenant metadata stored in `IncomingEventInfo`.
The current `AbpMassTransitEventData` broker envelope carries no tenant metadata; tenant
routing for dynamic broker delivery requires host configuration.

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

All transports read `MassTransitOptions`. Set `Provider` to the transport name (`RabbitMQ`, `Kafka` or `ActiveMQ`)
and put transport-specific values in the matching `*Options` section.

### RabbitMQ

```json
{
  "MassTransitOptions": {
    "Provider": "RabbitMQ",
    "Prefix": "SharpAbp",
    "RabbitMqOptions": {
      "Host": "localhost",
      "Port": 5672,
      "VirtualHost": "/",
      "Username": "guest",
      "Password": "guest"
    }
  }
}
```

Add the module dependency; the transport module registers MassTransit and configures the host from the options:

```csharp
[DependsOn(
    typeof(AbpMassTransitModule),
    typeof(AbpMassTransitRabbitMqModule)
)]
public class YourModule : AbpModule
{
}
```

For advanced scenarios, use `AbpMassTransitRabbitMqOptions.RabbitMqPreConfigures`,
`RabbitMqConfigures`, `RabbitMqPostConfigures`, `Producers` and `Consumers` instead of configuring the bus directly.

### Kafka

```json
{
  "MassTransitOptions": {
    "Provider": "Kafka",
    "Prefix": "SharpAbp",
    "KafkaOptions": {
      "Server": "localhost:9092",
      "DefaultGroupId": "my-consumer-group",
      "AutoCreateTopic": true
    }
  }
}
```

```csharp
[DependsOn(
    typeof(AbpMassTransitModule),
    typeof(AbpMassTransitKafkaModule)
)]
public class YourModule : AbpModule
{
}
```

### ActiveMQ

```json
{
  "MassTransitOptions": {
    "Provider": "ActiveMQ",
    "Prefix": "SharpAbp",
    "ActiveMqOptions": {
      "Host": "localhost",
      "Port": 61616,
      "Username": "admin",
      "Password": "admin"
    }
  }
}
```

```csharp
[DependsOn(
    typeof(AbpMassTransitModule),
    typeof(AbpMassTransitActiveMqModule)
)]
public class YourModule : AbpModule
{
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
Configure<AbpMassTransitRabbitMqOptions>(options =>
{
    options.Consumers.Add(new RabbitMqConsumerConfiguration
    {
        Configure = cfg => cfg.AddConsumer<OrderCreatedConsumer>(),
        // Optional explicit exchange/queue names; defaults are derived from the message type.
        ExchangeName = "OrderCreatedEvent",
        QueueName = "order-created"
    });
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
Configure<AbpMassTransitRabbitMqOptions>(options =>
{
    options.Consumers.Add(new RabbitMqConsumerConfiguration
    {
        Configure = cfg => cfg.AddConsumer<OrderCreatedConsumer>(consumerConfigurator =>
        {
            consumerConfigurator.UseMessageRetry(r => r.Intervals(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            ));
        })
    });
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
var rabbitMq = configuration
    .GetSection("MassTransitOptions:RabbitMqOptions")
    .Get<AbpMassTransitRabbitMqOptions>()!;

services.AddHealthChecks()
    .AddRabbitMQ($"amqp://{rabbitMq.Username}:{rabbitMq.Password}@{rabbitMq.Host}:{rabbitMq.Port}{rabbitMq.VirtualHost}");
```

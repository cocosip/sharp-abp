# Observability

OpenTelemetry integration for comprehensive application observability including tracing, metrics, and logging.

## OpenTelemetry

Complete OpenTelemetry observability platform with support for Zipkin, Prometheus, and OTLP exporters.

### Installation

```bash
# Core OpenTelemetry
dotnet add package SharpAbp.Abp.OpenTelemetry
dotnet add package SharpAbp.Abp.OpenTelemetry.Abstractions

# Exporters (choose one or more):
dotnet add package SharpAbp.Abp.OpenTelemetry.Exporter.Console
dotnet add package SharpAbp.Abp.OpenTelemetry.Exporter.Zipkin
dotnet add package SharpAbp.Abp.OpenTelemetry.Exporter.Otlp
dotnet add package SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus.AspNetCore
dotnet add package SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus.HttpListener
```

### Configuration

Configure in `appsettings.json`:

```json
{
  "OpenTelemetry": {
    "ServiceName": "MyApplication",
    "ServiceVersion": "1.0.0",
    "Tracing": {
      "Enabled": true,
      "Zipkin": {
        "Endpoint": "http://localhost:9411/api/v2/spans"
      },
      "Otlp": {
        "Endpoint": "http://localhost:4317"
      }
    },
    "Metrics": {
      "Enabled": true,
      "Prometheus": {
        "Port": 9090,
        "Path": "/metrics"
      }
    }
  }
}
```

Add the module dependency:

```csharp
[DependsOn(
    typeof(AbpOpenTelemetryModule),
    typeof(AbpOpenTelemetryExporterZipkinModule),
    typeof(AbpOpenTelemetryExporterPrometheusAspNetCoreModule)
)]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var serviceName = configuration["OpenTelemetry:ServiceName"];

        Configure<AbpOpenTelemetryOptions>(options =>
        {
            options.ServiceName = serviceName;
            options.ServiceVersion = "1.0.0";
        });

        // Configure tracing
        context.Services.AddOpenTelemetryTracing(builder =>
        {
            builder
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(serviceName))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddZipkinExporter(options =>
                {
                    options.Endpoint = new Uri(
                        configuration["OpenTelemetry:Tracing:Zipkin:Endpoint"]
                    );
                });
        });

        // Configure metrics
        context.Services.AddOpenTelemetryMetrics(builder =>
        {
            builder
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(serviceName))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddPrometheusExporter();
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        // Add Prometheus scraping endpoint
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
    }
}
```

### Usage Example

#### Custom Tracing

```csharp
public class OrderService : ApplicationService
{
    private readonly ActivitySource _activitySource;

    public OrderService()
    {
        _activitySource = new ActivitySource("OrderService");
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto input)
    {
        using (var activity = _activitySource.StartActivity("CreateOrder"))
        {
            activity?.SetTag("order.customerId", input.CustomerId);
            activity?.SetTag("order.totalAmount", input.TotalAmount);

            try
            {
                // Validate order
                using (var validateActivity = _activitySource.StartActivity("ValidateOrder"))
                {
                    await ValidateOrderAsync(input);
                    validateActivity?.SetTag("validation.result", "success");
                }

                // Create order
                var order = await CreateOrderInternalAsync(input);

                activity?.SetTag("order.id", order.Id);
                activity?.SetStatus(ActivityStatusCode.Ok);

                return ObjectMapper.Map<Order, OrderDto>(order);
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.RecordException(ex);
                throw;
            }
        }
    }

    private async Task<Order> CreateOrderInternalAsync(CreateOrderDto input)
    {
        using (var activity = _activitySource.StartActivity("CreateOrderInternal"))
        {
            // Implementation
            return new Order();
        }
    }
}
```

#### Custom Metrics

```csharp
public class MetricsService : ITransientDependency
{
    private readonly Meter _meter;
    private readonly Counter<long> _orderCounter;
    private readonly Histogram<double> _orderAmount;
    private readonly ObservableGauge<int> _activeOrders;

    public MetricsService()
    {
        _meter = new Meter("OrderMetrics", "1.0.0");

        // Counter: tracks number of orders
        _orderCounter = _meter.CreateCounter<long>(
            "orders.created",
            description: "Number of orders created"
        );

        // Histogram: tracks distribution of order amounts
        _orderAmount = _meter.CreateHistogram<double>(
            "orders.amount",
            unit: "USD",
            description: "Distribution of order amounts"
        );

        // Gauge: tracks current active orders
        _activeOrders = _meter.CreateObservableGauge<int>(
            "orders.active",
            () => GetActiveOrderCount(),
            description: "Number of active orders"
        );
    }

    public void RecordOrderCreated(decimal amount, string status)
    {
        _orderCounter.Add(1,
            new KeyValuePair<string, object>("status", status)
        );

        _orderAmount.Record((double)amount,
            new KeyValuePair<string, object>("status", status)
        );
    }

    private int GetActiveOrderCount()
    {
        // Implementation
        return 0;
    }
}
```

#### Distributed Tracing

```csharp
public class DistributedService : ApplicationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ActivitySource _activitySource;

    public DistributedService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _activitySource = new ActivitySource("DistributedService");
    }

    public async Task<Result> ProcessDistributedOperationAsync()
    {
        using (var activity = _activitySource.StartActivity("DistributedOperation"))
        {
            activity?.SetTag("operation.type", "distributed");

            // Call external service - trace context is automatically propagated
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://api.example.com/data");

            activity?.SetTag("external.status", (int)response.StatusCode);

            // Process response
            var data = await response.Content.ReadAsStringAsync();

            return new Result { Data = data };
        }
    }
}
```

#### Logging with OpenTelemetry

```csharp
public class LoggingService : ApplicationService
{
    private readonly ILogger<LoggingService> _logger;

    public async Task ProcessWithLoggingAsync()
    {
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["OrderId"] = Guid.NewGuid(),
            ["CustomerId"] = 123
        }))
        {
            _logger.LogInformation("Processing order");

            try
            {
                await ProcessAsync();
                _logger.LogInformation("Order processed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process order");
                throw;
            }
        }
    }
}
```

---

## Exporter Configurations

### Zipkin Exporter

```csharp
builder.AddZipkinExporter(options =>
{
    options.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
    options.MaxPayloadSizeInBytes = 4096;
});
```

### Jaeger Exporter (via OTLP)

```csharp
builder.AddOtlpExporter(options =>
{
    options.Endpoint = new Uri("http://localhost:4317");
    options.Protocol = OtlpExportProtocol.Grpc;
});
```

### Prometheus Exporter

```csharp
// AspNetCore
builder.AddPrometheusExporter();

// Configure endpoint in OnApplicationInitialization
app.UseOpenTelemetryPrometheusScrapingEndpoint(options =>
{
    options.Path = "/metrics";
});

// HttpListener (standalone)
builder.AddPrometheusHttpListener(options =>
{
    options.Uris = new[] { "http://localhost:9090/" };
});
```

### Console Exporter (Development)

```csharp
builder.AddConsoleExporter();
```

---

## Best Practices

### 1. Naming Conventions

Follow OpenTelemetry semantic conventions:

```csharp
public class BestPracticeService
{
    private readonly ActivitySource _activitySource;

    public BestPracticeService()
    {
        // Use descriptive, hierarchical names
        _activitySource = new ActivitySource("MyApp.OrderService");
    }

    public async Task ProcessOrderAsync(Guid orderId)
    {
        using (var activity = _activitySource.StartActivity(
            "ProcessOrder",
            ActivityKind.Internal))
        {
            // Use semantic convention tags
            activity?.SetTag("order.id", orderId);
            activity?.SetTag("order.status", "processing");

            // Add events for significant moments
            activity?.AddEvent(new ActivityEvent("order.validation.started"));

            await ValidateOrderAsync(orderId);

            activity?.AddEvent(new ActivityEvent("order.validation.completed"));
        }
    }
}
```

### 2. Sampling

Configure sampling to control data volume:

```csharp
context.Services.AddOpenTelemetryTracing(builder =>
{
    builder
        .SetSampler(new TraceIdRatioBasedSampler(0.1)) // Sample 10% of traces
        .AddAspNetCoreInstrumentation();
});
```

### 3. Resource Attributes

Add resource attributes for better identification:

```csharp
builder.SetResourceBuilder(
    ResourceBuilder.CreateDefault()
        .AddService(
            serviceName: "MyService",
            serviceVersion: "1.0.0",
            serviceInstanceId: Environment.MachineName)
        .AddAttributes(new[]
        {
            new KeyValuePair<string, object>("environment", "production"),
            new KeyValuePair<string, object>("region", "us-east-1")
        })
);
```

### 4. Error Handling

Always record exceptions:

```csharp
public async Task SafeOperationAsync()
{
    using (var activity = _activitySource.StartActivity("SafeOperation"))
    {
        try
        {
            await RiskyOperationAsync();
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);
            throw;
        }
    }
}
```

### 5. Performance Considerations

Be mindful of performance overhead:

```csharp
// Don't create too many spans for simple operations
public async Task<int> GetCountAsync()
{
    // NO - too granular
    // using (var activity = _activitySource.StartActivity("GetCount"))
    // {
    //     return await _repository.CountAsync();
    // }

    // YES - appropriate granularity
    return await _repository.CountAsync();
}

// DO create spans for significant operations
public async Task<ComplexResult> ComplexOperationAsync()
{
    using (var activity = _activitySource.StartActivity("ComplexOperation"))
    {
        // This is worth tracing
        return await PerformComplexCalculationAsync();
    }
}
```

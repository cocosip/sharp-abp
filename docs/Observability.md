# Observability

本文档用于说明 SharpAbp 当前 OpenTelemetry 相关模块的使用方式，包括：

- 基础安装
- 配置文件结构
- OTLP / Prometheus exporter 说明
- `traces` 与 `metrics` 的业务埋点方式
- `ServiceName`、`SourceNames`、`MeterNames` 的区别

## 1. 总体说明

`SharpAbp.Abp.OpenTelemetry` 提供 OpenTelemetry 的核心 ABP 模块与配置模型。

可选 exporter 由独立包提供，例如：

- `SharpAbp.Abp.OpenTelemetry.Exporter.Otlp`
- `SharpAbp.Abp.OpenTelemetry.Exporter.Console`
- `SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus.AspNetCore`
- `SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus.HttpListener`

当前代码结构已经完成收敛：

- `SharpAbp.Abp.OpenTelemetry.Abstractions` 主要负责配置契约与公共结构
- `SharpAbp.Abp.OpenTelemetry` 负责配置绑定与运行时装配
- 各 exporter 模块负责 exporter 自身行为与 exporter 选项

## 2. 安装

```bash
# Core packages
dotnet add package SharpAbp.Abp.OpenTelemetry
dotnet add package SharpAbp.Abp.OpenTelemetry.Abstractions

# Exporters
dotnet add package SharpAbp.Abp.OpenTelemetry.Exporter.Otlp
dotnet add package SharpAbp.Abp.OpenTelemetry.Exporter.Console
dotnet add package SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus.AspNetCore
dotnet add package SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus.HttpListener
```

## 3. 配置模型

推荐优先使用配置文件方式。

当前核心配置分成四个部分：

- `OpenTelemetry:Resource`
- `OpenTelemetry:Tracing`
- `OpenTelemetry:Metrics`
- `OpenTelemetry:Logging`

exporter 配置放在：

- `OpenTelemetryExporters:Otlp`
- `OpenTelemetryExporters:Console`
- `OpenTelemetryExporters:PrometheusAspNetCore`
- `OpenTelemetryExporters:PrometheusHttpListener`

完整示例可以参考 [OpenTelemetry.appsettings.example.json](/D:/dotnet-code/sharp-abp/docs/OpenTelemetry.appsettings.example.json)。

## 4. 三个容易混淆的名称

在当前实现里，下面三个名字语义不同：

- `Resource.ServiceName`
- `Tracing.SourceNames`
- `Metrics.MeterNames`

### 4.1 ServiceName

`ServiceName` 表示“当前服务是谁”。

例如：

- `OrderService`
- `IdentityService`
- `PaymentService`

### 4.2 SourceNames

`SourceNames` 表示当前应用中会使用哪些 `ActivitySource`。

它应该与业务代码中的 `new ActivitySource("...")` 一致。

例如：

- `SharpAbp.Ordering`
- `SharpAbp.Ordering.Application`
- `SharpAbp.Ordering.Domain`

### 4.3 MeterNames

`MeterNames` 表示当前应用中会使用哪些 `Meter`。

它应该与业务代码中的 `new Meter("...")` 一致。

例如：

- `SharpAbp.Ordering`
- `SharpAbp.Inventory`

### 4.4 默认行为

如果没有显式配置 `Tracing.SourceNames` 和 `Metrics.MeterNames`，当前实现会回退到 `Resource.ServiceName`。

这意味着：

- 简单场景下可以少配一层
- 复杂场景下也可以显式自定义 source 和 meter 名称

## 5. 配置示例

### 5.1 最小 OTLP 示例

```json
{
  "OpenTelemetry": {
    "Resource": {
      "IsEnabled": true,
      "ServiceName": "MyApplication",
      "ServiceNamespace": "SharpAbp.Sample",
      "ServiceVersion": "1.0.0",
      "AutoGenerateServiceInstanceId": true
    },
    "Tracing": {
      "IsEnabled": true,
      "ExporterName": "Otlp"
    },
    "Metrics": {
      "IsEnabled": true,
      "ExporterName": "Otlp"
    },
    "Logging": {
      "IsEnabled": true,
      "ExporterName": "Otlp",
      "IncludeFormattedMessage": true,
      "IncludeScopes": true,
      "ParseStateValues": true
    }
  },
  "OpenTelemetryExporters": {
    "Otlp": {
      "Endpoint": "http://localhost:4318",
      "Protocol": "HttpProtobuf",
      "TimeoutMilliseconds": 10000
    }
  }
}
```

### 5.2 自定义 SourceNames / MeterNames 示例

```json
{
  "OpenTelemetry": {
    "Resource": {
      "IsEnabled": true,
      "ServiceName": "OrderService",
      "ServiceNamespace": "SharpAbp.Samples",
      "ServiceVersion": "1.0.0",
      "AutoGenerateServiceInstanceId": true
    },
    "Tracing": {
      "IsEnabled": true,
      "ExporterName": "Otlp",
      "SourceNames": [ "SharpAbp.Ordering.Application", "SharpAbp.Ordering.Domain" ]
    },
    "Metrics": {
      "IsEnabled": true,
      "ExporterName": "PrometheusAspNetCore",
      "MeterNames": [ "SharpAbp.Ordering", "SharpAbp.Inventory" ]
    },
    "Logging": {
      "IsEnabled": true,
      "ExporterName": "Otlp",
      "IncludeFormattedMessage": true,
      "IncludeScopes": true,
      "ParseStateValues": true
    }
  },
  "OpenTelemetryExporters": {
    "Otlp": {
      "Endpoint": "http://otel-collector:4318",
      "Protocol": "HttpProtobuf",
      "TimeoutMilliseconds": 10000
    },
    "PrometheusAspNetCore": {
      "Name": "prometheus-aspnetcore",
      "ScrapeEndpointPath": "/metrics",
      "ScrapeResponseCacheDurationMilliseconds": 0,
      "UsePrometheusScrapingEndpoint": true
    }
  }
}
```

## 6. OTLP 与 Collector

当前模块设计中只有一个 OTLP exporter。

如果 OTLP 的目标地址指向 OpenTelemetry Collector，那么它本质上仍然是 OTLP 导出，只是目标端是 Collector。

例如：

- 直连后端：`http://vendor-endpoint:4318`
- 通过 Collector：`http://otel-collector:4318`

这表示：

- Collector 是 OTLP 的目标场景
- Collector 不是单独的一类 exporter

### 6.1 Collector 示例

```json
{
  "OpenTelemetry": {
    "Resource": {
      "IsEnabled": true,
      "ServiceName": "MyApplication"
    },
    "Tracing": {
      "IsEnabled": true,
      "ExporterName": "Otlp"
    },
    "Metrics": {
      "IsEnabled": true,
      "ExporterName": "Otlp"
    },
    "Logging": {
      "IsEnabled": true,
      "ExporterName": "Otlp",
      "IncludeFormattedMessage": true,
      "IncludeScopes": true,
      "ParseStateValues": true
    }
  },
  "OpenTelemetryExporters": {
    "Otlp": {
      "Endpoint": "http://otel-collector:4318",
      "Protocol": "HttpProtobuf",
      "TimeoutMilliseconds": 10000,
      "Tracing": {
        "Endpoint": "http://otel-collector:4318/v1/traces"
      },
      "Metrics": {
        "Endpoint": "http://otel-collector:4318/v1/metrics"
      },
      "Logging": {
        "Endpoint": "http://otel-collector:4318/v1/logs"
      }
    }
  }
}
```

## 7. Prometheus 说明

### 7.1 PrometheusAspNetCore

`PrometheusAspNetCore` 适合 ASP.NET Core 应用，通过 middleware 暴露抓取端点。

```json
{
  "OpenTelemetry": {
    "Resource": {
      "IsEnabled": true,
      "ServiceName": "MyApplication"
    },
    "Metrics": {
      "IsEnabled": true,
      "ExporterName": "PrometheusAspNetCore"
    }
  },
  "OpenTelemetryExporters": {
    "PrometheusAspNetCore": {
      "Name": "prometheus-aspnetcore",
      "ScrapeEndpointPath": "/metrics",
      "ScrapeResponseCacheDurationMilliseconds": 0,
      "UsePrometheusScrapingEndpoint": true
    }
  }
}
```

当前行为说明：

- 只有 `Metrics.IsEnabled = true` 且 exporter 选中了 `PrometheusAspNetCore`，才会启用对应 endpoint 行为
- `UsePrometheusScrapingEndpoint = true` 表示使用默认 scraping endpoint middleware
- 如果在代码中提供 `PrometheusScrapingEndpointConfigure`，则由自定义逻辑替代默认 endpoint 注册路径

### 7.2 PrometheusHttpListener

`PrometheusHttpListener` 适合非 ASP.NET Core middleware 的独立监听场景。

```json
{
  "OpenTelemetry": {
    "Resource": {
      "IsEnabled": true,
      "ServiceName": "MyApplication"
    },
    "Metrics": {
      "IsEnabled": true,
      "ExporterName": "PrometheusHttpListener"
    }
  },
  "OpenTelemetryExporters": {
    "PrometheusHttpListener": {
      "Name": "prometheus-httplistener",
      "ScrapeEndpointPath": "/metrics",
      "UriPrefixes": [ "http://localhost:9464/" ]
    }
  }
}
```

当前行为说明：

- `UriPrefixes` 必须至少包含一个非空 URI 前缀
- 如果为空，运行时会抛出明确异常

### 7.3 使用建议

同一个进程里建议只启用你真正需要的 metrics exporter。

通常：

- ASP.NET Core Web 应用优先考虑 `PrometheusAspNetCore`
- 独立监听场景使用 `PrometheusHttpListener`

## 8. 模块中如何配置

如果你希望在模块中通过代码而不是仅通过配置文件来控制 OpenTelemetry，可以使用：

```csharp
public override void PreConfigureServices(ServiceConfigurationContext context)
{
    PreConfigure<AbpOpenTelemetryOptions>(options =>
    {
        options.Resource.ServiceName = "OrderService";
        options.Resource.ServiceVersion = "1.0.0";

        options.EnableTracing("SharpAbp.Ordering.Application", "SharpAbp.Ordering.Domain");
        options.EnableMetrics("SharpAbp.Ordering", "SharpAbp.Inventory");

        options.Tracing.ExporterName = OpenTelemetryExporterNames.Otlp;
        options.Metrics.ExporterName = OpenTelemetryExporterNames.PrometheusAspNetCore;
        options.Logging.ExporterName = OpenTelemetryExporterNames.Otlp;
    });
}
```

也可以逐个追加：

```csharp
public override void PreConfigureServices(ServiceConfigurationContext context)
{
    PreConfigure<AbpOpenTelemetryOptions>(options =>
    {
        options.Resource.ServiceName = "OrderService";

        options.EnableTracing();
        options.EnableMetrics();

        options.AddSource("SharpAbp.Ordering.Application");
        options.AddSource("SharpAbp.Ordering.Domain");

        options.AddMeter("SharpAbp.Ordering");
        options.AddMeter("SharpAbp.Inventory");
    });
}
```

## 9. traces 与 metrics 的埋点说明

自动采集通常只能覆盖基础链路，例如：

- ASP.NET Core 请求
- HttpClient
- 数据库访问
- 某些通用框架行为

如果要采集更细的业务信息，就需要自己写埋点代码。

### 9.1 traces 适合解决什么问题

`traces` 更适合看单次调用过程，例如：

- 某个接口为什么慢
- 某一笔业务在哪一步失败
- 一个请求内部经过了哪些关键步骤

### 9.2 metrics 适合解决什么问题

`metrics` 更适合看整体统计趋势，例如：

- 成功次数
- 失败次数
- 平均耗时
- 金额分布
- 队列长度

## 10. traces 如何埋点

如果要记录业务过程，应使用 `ActivitySource`。

### 10.1 基本示例

```csharp
using System.Diagnostics;

public class OrderAppService : ApplicationService
{
    private static readonly ActivitySource ActivitySource = new("SharpAbp.Ordering.Application");

    public async Task<OrderDto> CreateAsync(CreateOrderInput input)
    {
        using var activity = ActivitySource.StartActivity("Order.Create");

        activity?.SetTag("order.customer_id", input.CustomerId);
        activity?.SetTag("order.item_count", input.Items.Count);
        activity?.SetTag("order.currency", input.Currency);

        try
        {
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
```

### 10.2 关键步骤拆分

如果一个接口内部有几个重要步骤，可以拆成多个 span：

```csharp
using var activity = ActivitySource.StartActivity("Order.Create");

using (var validateActivity = ActivitySource.StartActivity("Order.Validate"))
{
    await ValidateAsync(input);
}

using (var pricingActivity = ActivitySource.StartActivity("Order.CalculatePrice"))
{
    await CalculatePriceAsync(input);
}

using (var persistActivity = ActivitySource.StartActivity("Order.Persist"))
{
    await SaveAsync(input);
}
```

### 10.3 traces 里建议记录什么

建议重点记录：

- 业务主键或关联键
- 租户、渠道、支付方式等业务维度
- 关键业务分支
- 异常与错误原因
- 核心步骤边界

不建议记录：

- 敏感数据
- 超长正文
- 高基数且没有聚合价值的字段组合

## 11. metrics 如何埋点

业务指标建议通过 `Meter` 进行定义与记录。

### 11.1 基本示例

```csharp
using System.Diagnostics.Metrics;

public static class OrderingMetrics
{
    public static readonly Meter Meter = new("SharpAbp.Ordering");

    public static readonly Counter<long> OrderCreatedCounter =
        Meter.CreateCounter<long>("orders.created");

    public static readonly Counter<long> OrderFailedCounter =
        Meter.CreateCounter<long>("orders.failed");

    public static readonly Histogram<double> OrderAmountHistogram =
        Meter.CreateHistogram<double>("orders.amount");

    public static readonly Histogram<double> OrderDurationHistogram =
        Meter.CreateHistogram<double>("orders.duration.ms");
}
```

记录方式示例：

```csharp
OrderingMetrics.OrderCreatedCounter.Add(1,
    new KeyValuePair<string, object?>("payment_method", input.PaymentMethod),
    new KeyValuePair<string, object?>("currency", input.Currency));

OrderingMetrics.OrderAmountHistogram.Record(input.TotalAmount,
    new KeyValuePair<string, object?>("currency", input.Currency));
```

### 11.2 metrics 里建议记录什么

建议重点记录：

- 成功次数
- 失败次数
- 金额分布
- 接口耗时
- 队列长度
- 任务执行次数
- 库存变化等业务统计

### 11.3 metrics 标签建议

推荐使用稳定、可控的标签：

- `status`
- `payment_method`
- `currency`
- `channel`
- `tenant_type`

谨慎使用：

- 用户 ID
- 订单 ID
- 请求 ID

因为高基数标签容易让指标系统压力过大。

## 12. traces 与 metrics 如何分工

可以简单理解为：

- `traces` 看单次调用过程
- `metrics` 看整体趋势和聚合统计

例如订单创建场景：

- trace 适合定位某一笔订单为何失败、卡在哪一步
- metrics 适合看失败率、吞吐量、金额分布、平均耗时

这两者通常应该搭配使用。

## 13. 当前结构下如何方便扩展

当前结构已经支持以下扩展方式。

### 13.1 注册自定义 source / meter

```csharp
PreConfigure<AbpOpenTelemetryOptions>(options =>
{
    options.EnableTracing("SharpAbp.Ordering.Application");
    options.EnableMetrics("SharpAbp.Ordering");
});
```

### 13.2 添加 instrumentation

```csharp
PreConfigure<AbpOpenTelemetryOptions>(options =>
{
    options.AddTracingInstrumentation(builder =>
    {
        builder.AddAspNetCoreInstrumentation();
        builder.AddHttpClientInstrumentation();
    });

    options.AddMetricsInstrumentation(builder =>
    {
        builder.AddAspNetCoreInstrumentation();
        builder.AddHttpClientInstrumentation();
    });
});
```

### 13.3 做更细的 builder 定制

如果需要更高级的行为，也可以继续追加 builder 配置，例如：

- 自定义视图
- 自定义过滤
- 高级 exporter 行为

## 14. 推荐团队实践

建议在团队中统一如下约定：

- `ServiceName` 使用服务级名称
- `ActivitySource` / `Meter` 使用模块级名称
- `ActivitySource` 与 `Meter` 是否分层命名要统一规范
- trace tag 命名风格统一
- metrics 名称统一使用稳定、明确的英文名
- 控制高基数标签

一个较稳妥的约定示例：

- `ServiceName = OrderService`
- `ActivitySource = SharpAbp.Ordering.Application`
- `Meter = SharpAbp.Ordering`

## 15. 推荐落地顺序

如果项目还没有开始做业务埋点，建议按下面顺序推进：

1. 先启用基础 OpenTelemetry 配置
2. 先让自动采集跑起来
3. 给最核心接口补 trace 埋点
4. 给关键业务指标补 metrics
5. 再逐步细化标签和模块划分

## 16. 当前结论

当前 SharpAbp OpenTelemetry 结构已经支持：

- 使用 `ServiceName` 作为默认回退值
- 自定义 `Tracing.SourceNames`
- 自定义 `Metrics.MeterNames`
- 在模块中通过 `PreConfigure<AbpOpenTelemetryOptions>` 做代码配置
- 在业务代码中通过 `ActivitySource` 和 `Meter` 做自定义埋点

因此，如果后续要在接口、应用服务、领域服务中补更细的 traces 或 metrics，现有结构已经可以直接支持，不需要再做额外的框架级改造。

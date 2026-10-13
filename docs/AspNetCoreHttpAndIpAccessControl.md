# ASP.NET Core 请求信息与 IP 访问控制

本文介绍以下两个包的配置方式：

- `SharpAbp.Abp.AspNetCore`：解析 `Forwarded`、`X-Forwarded-*` 和当前请求信息。
- `SharpAbp.Abp.AspNetCore.IpAccessControl`：按 CIDR 网段执行白名单和黑名单控制。

```bash
dotnet add package SharpAbp.Abp.AspNetCore
dotnet add package SharpAbp.Abp.AspNetCore.IpAccessControl
```

## 注册模块

```csharp
[DependsOn(
    typeof(SharpAbpAspNetCoreModule),
    typeof(SharpAbpAspNetCoreIpAccessControlModule)
)]
public class MyHttpApiModule : AbpModule
{
}
```

在应用启动配置阶段主动加载配置。模块不会替宿主应用读取配置文件：

```csharp
public override void ConfigureServices(ServiceConfigurationContext context)
{
    var configuration = context.Services.GetConfiguration();

    Configure<ForwardedHeadersOptions>(options =>
        options.Configure(configuration));

    Configure<AbpIpAccessControlOptions>(options =>
        options.Configure(configuration));
}
```

`ForwardedHeadersOptions` 使用 ASP.NET Core 原生的 `KnownProxies`、`ForwardLimit` 和 `ForwardedHeaders`
配置。可信网络在 .NET 9 使用 `KnownNetworks`，在 .NET 10 使用 `KnownIPNetworks`。
`KnownProxies` 和可信网络中的地址与 CIDR 字符串由 SharpAbp 的
`Configure(IConfiguration)` 扩展解析成强类型集合；模块本身不会读取配置文件。

## appsettings.json

```json
{
  "ForwardedHeaders": {
    "ForwardedHeaders": "XForwardedFor,XForwardedProto,XForwardedHost",
    "ForwardLimit": 1,
    "KnownProxies": [
      "10.0.0.2"
    ],
    "KnownNetworks": [
      "10.0.0.0/8",
      "2001:db8:100::/48"
    ]
  },
  "IpAccessControlOptions": {
    "IsEnabled": true,
    "DenyWhenClientAddressUnavailable": true,
    "AllowedNetworks": [
      "192.0.2.0/24",
      "2001:db8:1::/48"
    ],
    "DeniedNetworks": [
      "192.0.2.128/25",
      "2001:db8:1:ffff::/64"
    ]
  }
}
```

`ForwardedHeadersOptions` 的默认值由 ASP.NET Core 决定。应用有更严格的代理拓扑要求时，可以设置可信代理/可信网络并调整转发层数。

上述 JSON 用于 .NET 9；.NET 10 将 `KnownNetworks` 键改为 `KnownIPNetworks`，CIDR 数组内容不变。

## CIDR 规则

- 白名单和黑名单都使用网段；单个 IPv4 地址写成 `/32`，单个 IPv6 地址写成 `/128`。
- 黑名单优先于白名单。地址同时命中两个集合时，请求被拒绝。
- `IsEnabled` 为 `false` 时，访问控制评估允许请求，中间件继续执行后续管道。
- `IsEnabled` 为 `true` 且没有可用客户端地址时，`DenyWhenClientAddressUnavailable` 决定是否拒绝请求。

## 使用中间件

完整入口先捕获原始连接，再按原生 X-Forwarded 中间件、RFC 7239 中间件的顺序处理两种标准：

```csharp
app.UseSharpAbpForwardedHeaders();
```

同时处理两种标准时，应使用上面的完整入口，并让它先于任何其他 `UseForwardedHeaders()` 执行。
两类头解析出的客户端地址不一致时，解析结果为 `ConflictingHeaders`，客户端地址和端口均不可用。

只处理 RFC 7239 `Forwarded` 时使用独立入口：

```csharp
app.UseSharpAbpRfc7239ForwardedHeaders();
```

两种入口都可以重复调用而不会重复注册 SharpAbp 的 RFC middleware；ASP.NET Core 原生 middleware 不会写入 SharpAbp 的注册标记。

如果宿主已提前执行原生中间件，SharpAbp 无法还原被改写的原始连接，不能据此重新验证 RFC 代理链。
这种部署应统一使用宿主已处理的 `X-Forwarded-*`；需要两种标准共同处理时，应调整顺序，在改写前使用完整入口。

```csharp
app.UseIpAccessControl();
```

该中间件应放在应用希望执行 IP 控制的管道位置。客户端地址由请求信息模块根据 `Forwarded`、`X-Forwarded-For`、可信代理设置和转发层数解析；无法安全解析时不会把未经验证的代理地址当作客户端地址。

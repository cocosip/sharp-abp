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

    Configure<AbpForwardedRequestOptions>(options =>
        options.Configure(configuration));

    Configure<AbpIpAccessControlOptions>(options =>
        options.Configure(configuration));
}
```

`IPAddress` 和 `IPNetwork` 不能由 .NET Configuration Binder 直接从 JSON 字符串转换。因此不要直接使用
`configuration.GetSection("ForwardedRequestOptions").Get<AbpForwardedRequestOptions>()` 期望填充这些集合；应调用上面的
`Configure(IConfiguration)` 方法。该方法仍然只读取 `appsettings.json`，并将地址和 CIDR 字符串解析成强类型集合。

## appsettings.json

```json
{
  "ForwardedRequestOptions": {
    "RequireTrustedProxy": false,
    "ForwardLimit": 2,
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

`RequireTrustedProxy` 默认值为 `false`，`ForwardLimit` 默认值为 `2`。应用有更严格的代理拓扑要求时，可以设置可信代理/可信网络并调整转发层数。

## CIDR 规则

- 白名单和黑名单都使用网段；单个 IPv4 地址写成 `/32`，单个 IPv6 地址写成 `/128`。
- 黑名单优先于白名单。地址同时命中两个集合时，请求被拒绝。
- `IsEnabled` 为 `false` 时，访问控制评估允许请求，中间件继续执行后续管道。
- `IsEnabled` 为 `true` 且没有可用客户端地址时，`DenyWhenClientAddressUnavailable` 决定是否拒绝请求。

## 使用中间件

```csharp
app.UseIpAccessControl();
```

该中间件应放在应用希望执行 IP 控制的管道位置。客户端地址由请求信息模块根据 `Forwarded`、`X-Forwarded-For`、可信代理设置和转发层数解析；无法安全解析时不会把未经验证的代理地址当作客户端地址。

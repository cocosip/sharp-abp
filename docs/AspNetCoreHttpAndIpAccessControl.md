# ASP.NET Core 请求信息与 IP 访问控制

本文介绍以下两个包的配置方式：

- `SharpAbp.Abp.AspNetCore`：解析 `Forwarded`、`X-Forwarded-*` 和当前请求信息。
- `SharpAbp.Abp.AspNetCore.IpAccessControl`：按精确 IP 地址和 CIDR 网段执行白名单和黑名单控制。

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
    "AllowedAddresses": [
      "198.51.100.10",
      "2001:db8::10"
    ],
    "AllowedNetworks": [
      "192.0.2.0/24",
      "2001:db8:1::/48"
    ],
    "DeniedAddresses": [
      "198.51.100.20",
      "2001:db8::20"
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

## IP 地址与 CIDR 规则

- 精确 IP 地址放在 `AllowedAddresses` 或 `DeniedAddresses`，支持 IPv4 和 IPv6。
- 网段放在 `AllowedNetworks` 或 `DeniedNetworks`，使用 CIDR；单个 IPv4 地址也可以写成 `/32`，单个 IPv6 地址写成 `/128`。
- 地址集合和网段集合合并判断，命中任意一项即可。
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

RFC 7239 入口解析 `Forwarded` 中每一跳的 `for`、`proto`、`host`，不直接调用只读取 X 头的原生
`ApplyForwarders`。两种标准使用同一份 `ForwardedHeadersOptions`：`XForwardedFor`、
`XForwardedProto`、`XForwardedHost` 分别启用对应的 RFC 字段；默认不启用任何字段。
`ForwardLimit`、`AllowedHosts` 和可信代理设置同时约束两种标准，无需重复配置。
`RequireHeaderSymmetry` 对原生 X 头检查启用的 X 字段，对 RFC 检查启用的 `for/proto/host` 字段。
RFC 每一跳缺少的字段保持缺失，不会挪用其他跳的值；RFC 没有 `XForwardedPrefix` 的对应字段，
启用 X Prefix 不会要求 RFC 提供 Prefix，也不会阻断 RFC 的其他字段。

| 同一份 options 的字段开关 | 原生 X 头 | RFC 7239 参数 |
| --- | --- | --- |
| `XForwardedFor` | `ForwardedForHeaderName` 指定的头 | `for` |
| `XForwardedProto` | `ForwardedProtoHeaderName` 指定的头 | `proto` |
| `XForwardedHost` | `ForwardedHostHeaderName` 指定的头 | `host` |
| `XForwardedPrefix` | `ForwardedPrefixHeaderName` 指定的头 | 无对应参数 |

`Forwarded*HeaderName` 与 `Original*HeaderName` 是原生 X 头的输入和原始值输出名称配置。
RFC 头名固定为 `Forwarded`，参数名固定为 `for/proto/host`，不受这些名称配置影响；
请求信息中的原始连接仍由 SharpAbp 在转发处理前捕获。

两种标准都从最右侧开始处理代理链。遇到不可信代理或非空但无法解析的客户端 IP
（包括 `unknown`、混淆标识和无效端口）时停止，保留已接受的近端跳；没有接受任何跳时保留当前连接地址和端口。
超过 `ForwardLimit` 的部分不处理，不会因此废弃已处理的结果。畸形 RFC 语法会忽略整个 `Forwarded` 头，
不会覆盖有效的原生 X 头结果；只有两类头都成功解析出的客户端 IP 不一致时才报告冲突。
`KnownProxies` 与可信网络集合都为空时，和原生一样不执行代理信任检查。

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

该中间件应放在应用希望执行 IP 控制的管道位置，通常在转发头处理中间件之后。
IP 访问控制使用请求信息模块提供的当前地址或转发后地址；转发头被忽略时仍可用的连接地址也参与访问控制。
`DenyWhenClientAddressUnavailable` 仅属于 IP 访问控制包，决定地址不可用时是否拒绝请求，不影响转发头解析或代理信任判断。

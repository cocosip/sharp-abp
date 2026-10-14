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
- `DenyWhenClientAddressUnavailable=true` 时返回 403；`false` 时直接允许地址不可用的请求，
  决策原因为 `ClientAddressUnavailable`，不会执行地址或网段匹配。`false` 不保证所有请求都经过黑白名单校验。

## 使用中间件

完整入口先捕获原始连接，再选择一套协议：有 `Forwarded` 头（包括空值）时只处理 RFC；
没有该头时使用原生 `X-Forwarded-*` 中间件；没有转发头时保留普通直接请求行为。
这是 SharpAbp 的协议优先级，不是 RFC 7239 强制规定的优先级。

```csharp
app.UseSharpAbpForwardedHeaders();
```

使用上面的完整入口，并让它先于 IP 访问控制、URL 生成和任何其他转发处理执行。
同一请求中即使两套头的 IP 不同，也不会比较它们或从 X 头补齐 RFC 字段。
RFC 路径保留原始 `PathBase`，不读取 `X-Forwarded-Prefix`。
`ConflictingHeaders` 枚举值为公共契约保留，完整入口的协议选择不再产生该状态。

RFC 7239 入口解析 `Forwarded` 中每一跳的 `for`、`proto`、`host`，不直接调用只读取 X 头的原生
`ApplyForwarders`。两种标准使用同一份 `ForwardedHeadersOptions`：`XForwardedFor`、
`XForwardedProto`、`XForwardedHost` 分别启用对应的 RFC 字段；默认不启用任何字段。
`ForwardLimit`、`AllowedHosts` 和可信代理设置同时约束两种标准，无需重复配置。
`RequireHeaderSymmetry` 对原生 X 头检查启用的 X 字段，对 RFC 检查启用的 `for/proto/host` 字段。
RFC 最终接受的跳缺少 `proto/host` 时保留原始请求对应值（包括原始 Host 的端口），
不会挪用另一跳或 X 头的字段；`host` 存在但没有端口时，URL 不沿用原始 Host 的端口。
RFC 没有 `XForwardedPrefix` 的对应字段，启用 X Prefix 不会要求 RFC 提供 Prefix。

| 同一份 options 的字段开关 | 原生 X 头 | RFC 7239 参数 |
| --- | --- | --- |
| `XForwardedFor` | `ForwardedForHeaderName` 指定的头 | `for` |
| `XForwardedProto` | `ForwardedProtoHeaderName` 指定的头 | `proto` |
| `XForwardedHost` | `ForwardedHostHeaderName` 指定的头 | `host` |
| `XForwardedPrefix` | `ForwardedPrefixHeaderName` 指定的头 | 无对应参数 |

`Forwarded*HeaderName` 与 `Original*HeaderName` 是原生 X 头的输入和原始值输出名称配置。
RFC 头名固定为 `Forwarded`，参数名固定为 `for/proto/host`，不受这些名称配置影响；
请求信息中的原始连接仍由 SharpAbp 在转发处理前捕获。

两种标准都从最右侧开始处理代理链。RFC 首先校验原始 TCP 连接来源，再根据接受的 `for`
地址逐跳验证上一代理。`by` 仅表示代理节点，既不作为客户端地址，也不替代代理信任校验。
IPv4-mapped IPv6 可匹配相应 IPv4 信任规则，访问器向上层返回规范化 IPv4 地址。
遇到更远的不可信代理时停止，保留已接受的近端 RFC 跳；未验证的左侧头不能改变 IP 或 URL。
`ForwardLimit` 限制最大处理跳数，截断时保留已接受的 RFC 结果；`0` 不接受任何跳。
因此 `Resolved` 表示接受边界上的 `for` 可用，并不保证已经到达整条链最左端的原始客户端。

RFC 允许省略 `for`，`for=192.0.2.60` 也是合法语法。缺少 `for`、`for=unknown` 或混淆节点
（例如 `_hidden`）时，可以应用当前可信跳的有效 URL 字段，但不能沿该跳继续验证更远代理。
客户端地址和端口不可用，不会将代理连接 IP、`by` 或 X 头 IP 冒充为 RFC 客户端。
带端口的节点、IPv6 节点和带端口的 Host 应使用引号，例如：

```http
Forwarded: for="[2001:db8::10]:4711";proto=https;host="example.com:8443";by=203.0.113.43
```

语法/节点格式非法（包括重复参数、未闭合引号、非法 IP、非法客户端端口）时拒绝整个 RFC 头的解析。
`RequireHeaderSymmetry=true` 时，启用的 `for/proto/host` 必须数量一致且每个处理跳都具备有效字段，
否则不应用该头。`false` 时，无效或不在 `AllowedHosts` 范围内的 URL 字段保留原始请求对应值，
仍可使用同一 RFC 跳中的有效客户端 IP；不会借用 X 字段。

`KnownProxies` 与可信网络集合都为空时，和原生一样表示显式信任所有代理，生产环境应配置实际代理范围。
RFC 不使用 .NET 9 的 `IgnoreUnknownProxiesWithoutFor` 兼容开关绕过信任校验；
即使没有启用 `for`，应用 URL 字段仍需校验来源。可信代理必须清理或重建客户端可以伪造的转发头，
只配置代理信任列表不能保证代理传来的所有头已经被正确清理。

### 解析状态与访问控制

下表针对已选择的 RFC 路径。`Source` 均为 `Forwarded`，表示协议选择；原始连接信息单独保存在
`ConnectionIpAddress/ConnectionPort` 中。状态描述客户端地址解析结果，URL 字段能否应用另行判断。

| 场景 | ResolutionStatus | ClientIpAddress / ClientPort | URL 字段 | IP 控制开启后的行为 |
| --- | --- | --- | --- | --- |
| 接受的 RFC 跳有有效 `for` 且开关启用 | `Resolved` | RFC 地址/端口，无端口则 null | 应用启用且有效的字段 | 先黑名单，再白名单；拒绝时 403 |
| 缺少/unknown/混淆 `for`，或未启用 `XForwardedFor` | `NoClientAddress` | null / null | 非严格对称时可应用可信跳的有效字段 | 由 `DenyWhenClientAddressUnavailable` 决定 |
| 空头、语法/节点非法、对称约束失败 | `InvalidHeader` | null / null | 保留原始请求字段 | 同上 |
| 原始连接来源不可信 | `UntrustedProxy` | null / null | 保留原始请求字段 | 同上 |
| 原始连接 IP 缺失 | `NoClientAddress` | null / null | 保留原始请求字段 | 同上 |
| `ForwardLimit=0` | `ForwardLimitExceeded` | null / null | 保留原始请求字段 | 同上 |

无 `Forwarded` 时保持现有原生 X 头处理语义，包括原生拒绝转发头后使用连接地址的行为。
不带转发头的直接请求以连接地址参与黑白名单，`Source=Direct`。

只处理 RFC 7239 `Forwarded` 时使用独立入口：

```csharp
app.UseSharpAbpRfc7239ForwardedHeaders();
```

两种入口都可以重复调用而不会重复注册 SharpAbp 的 RFC middleware；ASP.NET Core 原生 middleware 不会写入 SharpAbp 的注册标记。

如果宿主、IIS 集成或其他组件已提前改写连接、Scheme、Host 或 PathBase，SharpAbp 无法还原原始值。
需要 RFC 优先级时，应调整部署和顺序，使完整入口接收原始请求；避免此前无条件运行原生转发处理中间件。

```csharp
app.UseIpAccessControl();
```

该中间件应放在应用希望执行 IP 控制的管道位置，通常在转发头处理中间件之后。
IP 访问控制继续通过 `IHttpRequestInfoAccessor` 获取可用客户端地址；RFC 解析失败时不使用连接地址或 X 地址降级匹配。
`DenyWhenClientAddressUnavailable` 仅属于 IP 访问控制包，决定地址不可用时是否拒绝请求，不影响转发头解析或代理信任判断。

## URL 生成

上层从同一个访问器读取 IP 与 URL 信息。可继续使用现有 `IRouteTranslationUrlService`：

```csharp
var info = requestInfoAccessor.GetRequestInfo();
var host = info.Port.HasValue
    ? new HostString(info.Host!, info.Port.Value)
    : new HostString(info.Host!);
var downloadUrl = routeTranslationUrlService.GetTranslationUrl(
    info.Scheme, host.ToUriComponent(), httpContext.Request.PathBase + "/download", null);
```

可信代理传入 `for=172.16.0.148;proto=https;host="example.com:8443"`，原始 PathBase 为空时，
URL 为 `https://example.com:8443/download`；若原始 PathBase 为 `/original`，则包含 `/original/download`。
缺少 `for` 不必阻止 URL 字段解析；但严格 IP 访问控制可能在业务 URL 生成之前返回 403。

## 空响应体 403 的诊断

之前完整入口先运行原生 X 中间件，然后把其客户端 IP 与 RFC `for` 比较。
不同地址会触发 `ConflictingHeaders` 并在应用 RFC `proto/host` 前返回；访问器随之返回空客户端地址，
严格访问控制以 `ClientAddressUnavailable` 拒绝请求。没有 `for` 时，这个比较可能被跳过，
URL 使用 RFC 信息而 IP 沿用 X 头或连接地址，因此可能出现“加 for 后 URL 失效”的差异。
这个源码路径已通过混合头回归测试复现；未取得线上实际请求的全部头和状态时，不能认定某次线上 403 就是冲突或黑名单命中。

当前 `IpAccessControlService.EvaluateCurrentRequest()` 在拒绝时记录 Warning 日志，分类为
`SharpAbp.Abp.AspNetCore.IpAccessControl.IpAccessControlService`，包含 `Reason`、`ResolutionStatus`、
`Source`、`ClientIpAddress`、`ConnectionIpAddress` 和 `ConnectionPort`，不输出原始转发头。
使用现有宿主日志输出即可看到，例如：

```text
IP access denied. Reason: DeniedAddress; ResolutionStatus: Resolved; Source: Forwarded; ClientIpAddress: 172.16.0.148; ConnectionIpAddress: 172.16.2.0; ConnectionPort: 54321.
IP access denied. Reason: ClientAddressUnavailable; ResolutionStatus: UntrustedProxy; Source: Forwarded; ClientIpAddress: (null); ConnectionIpAddress: 172.16.2.10; ConnectionPort: 54321.
```

`KnownProxies/KnownNetworks` 信任的是把头传给应用的代理连接地址，`Allowed*/Denied*` 匹配的是解析后的客户端 `for`。
例如代理网络配置 `172.16.2.0/32` 只信任 `172.16.2.0`，不包含 `172.16.2.10`；
若实际代理地址为后者，应配置其精确 `/32`，或在实际拓扑允许时配置包含它的网段。
不要为了绕过 403 将客户端 IP 添加为可信代理，也不要取消信任校验。

仅拒绝 `172.16.0.148`、白名单为空时：成功解析的 `.148` 返回 `DeniedAddress`/403，
成功解析的 `.149` 为 `AllowedByDefault`；若原始连接不可信，两者都会是 `ClientAddressUnavailable`/403。
.NET 10 必须用 `KnownIPNetworks` 配置对应网络。

## 回归验证

`ForwardedIpAccessControlPipelineTest` 使用 TestHost 和 HttpClient 执行完整 HTTP 管道，依次运行
转发入口、真实访问器、真实访问控制服务/中间件和现有 URL 生成服务。
覆盖 IPv4/IPv6/端口、混合头、四类地址规则、拒绝优先、字段缺失、非法头、代理信任、多跳/层数、
AllowedHosts、对称约束、原始连接信息、X/直接请求和拒绝日志。
用户报告的 `.148/.149` 与 `172.16.2.0/32` 配置有独立断言，包括 `/32` 和 `/24` 范围对照。

```powershell
dotnet test framework/test/SharpAbp.Abp.AspNetCore.Tests/SharpAbp.Abp.AspNetCore.Tests.csproj -c Release
dotnet test framework/test/SharpAbp.Abp.AspNetCore.IpAccessControl.Tests/SharpAbp.Abp.AspNetCore.IpAccessControl.Tests.csproj -c Release
dotnet build framework/SharpAbp.sln -c Release
```

.NET 9 在 `net9` 分支验证；修复提交合并至 `dev` 后，使用其原始 .NET 10 框架/依赖配置
再次运行两个测试项目，并以 `dotnet build framework/SharpAbp.slnx -c Release` 构建。
分支构建和 TestHost 回归不代替真实部署环境的请求验证。

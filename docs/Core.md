# Core Modules

Core modules provide essential functionality and utilities for ABP vNext applications.

## DotCommon

DotCommon ABP adapter for the DotCommon performance counters (`IPerformanceService`).

### Installation

```bash
dotnet add package SharpAbp.Abp.DotCommon
```

### Configuration

Add the module dependency to your module class:

```csharp
[DependsOn(typeof(AbpDotCommonModule))]
public class YourModule : AbpModule
{
}
```

The module registers a default performance configuration named `default`. Add or customize configurations with
`Configure<AbpPerformanceOptions>`.

### Usage Example

```csharp
public class YourService
{
    private readonly IPerformanceService _performanceService;

    public YourService(IPerformanceServiceFactory performanceServiceFactory)
    {
        _performanceService = performanceServiceFactory.GetOrCreate("YourService");
    }

    public void DoWork()
    {
        var start = DateTime.UtcNow;

        // ... work ...

        _performanceService.IncrementKeyCount("DoWork", (DateTime.UtcNow - start).TotalMilliseconds);
    }
}
```

`GetKeyPerformanceInfo(key)` returns the current `PerformanceInfo` snapshot for a key.

---

## Snowflakes

Distributed unique ID generator based on the Snowflake algorithm.

### Installation

```bash
dotnet add package SharpAbp.Abp.Snowflakes
```

### Configuration

Configure named Snowflake instances in `appsettings.json`:

```json
{
  "SnowflakeOptions": {
    "default": {
      "WorkerId": 1,
      "DatacenterId": 1
    },
    "order": {
      "WorkerId": 2,
      "DatacenterId": 1
    }
  }
}
```

Add the module dependency and bind the section:

```csharp
[DependsOn(typeof(AbpSnowflakesModule))]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpSnowflakesOptions>(options => options.Configure(configuration));
    }
}
```

`WorkerId`, `DatacenterId`, `Twepoch`, `WorkerIdBits`, `DatacenterIdBits` and `SequenceBits` can be set per instance.

### Usage Example

```csharp
public class YourService
{
    private readonly ISnowflakeFactory _snowflakeFactory;

    public YourService(ISnowflakeFactory snowflakeFactory)
    {
        _snowflakeFactory = snowflakeFactory;
    }

    public long CreateUniqueId()
    {
        return _snowflakeFactory.GetDefault().NextId();
    }

    public long CreateOrderId()
    {
        return _snowflakeFactory.Get("order").NextId();
    }
}
```

---

## Crypto

Cryptography primitives built on BouncyCastle: RSA, SM2, SM3 and SM4.

### Installation

```bash
dotnet add package SharpAbp.Abp.Crypto
```

### Configuration

Add the module dependency:

```csharp
[DependsOn(typeof(AbpCryptoModule))]
public class YourModule : AbpModule
{
}
```

The module registers `IRSAEncryptionService`, `ISm2EncryptionService`, `ISm3EncryptionService` and
`ISm4EncryptionService`. SM2 and SM4 defaults are configured with the options classes:

```csharp
Configure<AbpSm2EncryptionOptions>(options =>
{
    options.DefaultCurve = Sm2EncryptionNames.CurveSm2p256v1;
});

Configure<AbpSm4EncryptionOptions>(options =>
{
    options.DefaultIv = Encoding.UTF8.GetBytes("SharpAbpDefault!"); // 16 bytes for CBC
    options.DefaultMode = Sm4EncryptionNames.ModeCBC;
    options.DefaultPadding = Sm4EncryptionNames.PKCS7Padding;
});
```

The SM4 default mode is `ECB` with `NoPadding`; switch to `CBC` and provide a 16-byte IV when you need CBC.

### Usage Example

```csharp
public class YourService
{
    private readonly ISm4EncryptionService _sm4EncryptionService;
    private readonly IRSAEncryptionService _rsaEncryptionService;

    public YourService(
        ISm4EncryptionService sm4EncryptionService,
        IRSAEncryptionService rsaEncryptionService)
    {
        _sm4EncryptionService = sm4EncryptionService;
        _rsaEncryptionService = rsaEncryptionService;
    }

    public string EncryptSm4(string plainText, string keyHex, string ivHex)
    {
        return _sm4EncryptionService.Encrypt(plainText, keyHex, ivHex);
    }

    public string EncryptRsa(string plainText, string publicKeyBase64)
    {
        return _rsaEncryptionService.EncryptFromBase64(
            publicKeyBase64,
            plainText,
            Encoding.UTF8,
            RSAPaddingNames.PKCS1Padding);
    }
}
```

For SM2, use `ISm2EncryptionService.Encrypt(publicKeyHex, plainText, Encoding.UTF8, curve, mode)` and the matching
`Decrypt` overload; for hashing use `ISm3EncryptionService.GetHash(plainText)`.

---

## FreeRedis

FreeRedis client management with named clients and ABP-style configuration.

### Installation

```bash
dotnet add package SharpAbp.Abp.FreeRedis
```

### Configuration

Configure named clients under `FreeRedisOptions` in `appsettings.json`:

```json
{
  "FreeRedisOptions": {
    "default": {
      "Mode": "Standalone",
      "ConnectionString": "127.0.0.1:6379,defaultDatabase=0,poolsize=50"
    }
  }
}
```

Add the module dependency and bind the section:

```csharp
[DependsOn(typeof(AbpFreeRedisModule))]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpFreeRedisOptions>(options => options.Configure(configuration));
    }
}
```

### Usage Example

```csharp
public class YourService
{
    private readonly RedisClient _redisClient;

    public YourService(IRedisClientFactory redisClientFactory)
    {
        _redisClient = redisClientFactory.Get(); // or Get("custom")
    }

    public void SetValue(string key, string value)
    {
        _redisClient.Set(key, value);
    }
}
```

Use `[RedisClientName("custom")]` on a marker class and `redisClientFactory.Get<CustomClient>()` to select a named client.

---

## Validation

Data validation module for input validation and business rule validation.

### Installation

```bash
dotnet add package SharpAbp.Abp.Validation
```

### Configuration

Add the module dependency:

```csharp
[DependsOn(typeof(SharpAbpValidationModule))]
public class YourModule : AbpModule
{
}
```

### Usage Example

```csharp
public class CreateUserDto : IValidatableObject
{
    [Required]
    [StringLength(50)]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Range(18, 120)]
    public int Age { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (UserName == "admin" && Email.EndsWith("@test.com"))
        {
            yield return new ValidationResult(
                "Admin username cannot use test email domain",
                new[] { nameof(UserName), nameof(Email) }
            );
        }
    }
}
```

---

## AspNetCore

AspNetCore integration extensions for ABP framework.

### Installation

```bash
dotnet add package SharpAbp.Abp.AspNetCore
```

### Configuration

Add the module dependency:

```csharp
[DependsOn(
    typeof(SharpAbpAspNetCoreModule),
    typeof(AbpAspNetCoreMvcModule)
)]
public class YourWebModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Configure MVC
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(YourApplicationModule).Assembly);
        });
    }
}
```

### Usage Example

```csharp
// Auto API Controller
public class ProductAppService : ApplicationService, IProductAppService
{
    // Automatically exposed as API: /api/app/product
    public async Task<ProductDto> GetAsync(Guid id)
    {
        // Implementation
    }
}
```

---

## Swashbuckle.Versioning

API versioning management for Swagger documentation.

### Installation

```bash
dotnet add package SharpAbp.Abp.Swashbuckle.Versioning
```

### Configuration

```csharp
[DependsOn(typeof(AbpSwashbuckleVersioningModule))]
public class YourWebModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpSwaggerGenWithOAuth(
            authority: "https://your-auth-server",
            scopes: new Dictionary<string, string>
            {
                {"YourAPI", "Your API"}
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
                options.SwaggerDoc("v2", new OpenApiInfo { Title = "Your API", Version = "v2" });
            }
        );
    }
}
```

### Usage Example

```csharp
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductController : AbpController
{
    [HttpGet]
    [MapToApiVersion("1.0")]
    public async Task<ProductDtoV1> GetV1(Guid id)
    {
        // V1 implementation
    }

    [HttpGet]
    [MapToApiVersion("2.0")]
    public async Task<ProductDtoV2> GetV2(Guid id)
    {
        // V2 implementation with more features
    }
}
```

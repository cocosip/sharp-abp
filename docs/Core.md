# Core Modules

Core modules provide essential functionality and utilities for ABP vNext applications.

## DotCommon

DotCommon ABP adapter that provides Snowflake ID generation and other utilities.

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
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Module is automatically configured
    }
}
```

### Usage Example

```csharp
public class YourService
{
    private readonly ISnowflakeIdGenerator _snowflakeIdGenerator;

    public YourService(ISnowflakeIdGenerator snowflakeIdGenerator)
    {
        _snowflakeIdGenerator = snowflakeIdGenerator;
    }

    public long GenerateId()
    {
        return _snowflakeIdGenerator.NextId();
    }
}
```

---

## Snowflakes

Distributed unique ID generator for generating globally unique IDs.

### Installation

```bash
dotnet add package SharpAbp.Abp.Snowflakes
```

### Configuration

Configure in your `appsettings.json`:

```json
{
  "Snowflakes": {
    "WorkerId": 1,
    "DataCenterId": 1
  }
}
```

Add the module dependency:

```csharp
[DependsOn(typeof(AbpSnowflakesModule))]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<SnowflakesOptions>(options =>
        {
            options.WorkerId = configuration.GetValue<int>("Snowflakes:WorkerId");
            options.DataCenterId = configuration.GetValue<int>("Snowflakes:DataCenterId");
        });
    }
}
```

### Usage Example

```csharp
public class YourService
{
    private readonly ISnowflakeIdGenerator _idGenerator;

    public YourService(ISnowflakeIdGenerator idGenerator)
    {
        _idGenerator = idGenerator;
    }

    public long CreateUniqueId()
    {
        return _idGenerator.NextId();
    }

    public string CreateUniqueStringId()
    {
        return _idGenerator.NextId().ToString();
    }
}
```

---

## Crypto

Encryption and decryption module with SM2/AES support for data security.

### Installation

```bash
dotnet add package SharpAbp.Abp.Crypto
```

### Configuration

Configure encryption settings in `appsettings.json`:

```json
{
  "Crypto": {
    "DefaultAlgorithm": "AES",
    "AES": {
      "Key": "your-32-character-secret-key-here",
      "IV": "your-16-char-iv"
    }
  }
}
```

Add the module dependency:

```csharp
[DependsOn(typeof(AbpCryptoModule))]
public class YourModule : AbpModule
{
}
```

### Usage Example

```csharp
public class YourService
{
    private readonly ICryptoService _cryptoService;

    public YourService(ICryptoService cryptoService)
    {
        _cryptoService = cryptoService;
    }

    public string EncryptData(string plainText)
    {
        return _cryptoService.Encrypt(plainText);
    }

    public string DecryptData(string cipherText)
    {
        return _cryptoService.Decrypt(cipherText);
    }
}
```

---

## FreeRedis

FreeRedis cache library ABP adapter for Redis integration.

### Installation

```bash
dotnet add package SharpAbp.Abp.FreeRedis
```

### Configuration

Configure Redis connection in `appsettings.json`:

```json
{
  "Redis": {
    "Configuration": "127.0.0.1:6379,defaultDatabase=0,poolsize=50"
  }
}
```

Add the module dependency:

```csharp
[DependsOn(typeof(AbpFreeRedisModule))]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var redis = configuration["Redis:Configuration"];

        Configure<FreeRedisOptions>(options =>
        {
            options.Configuration = redis;
        });
    }
}
```

### Usage Example

```csharp
public class YourService
{
    private readonly IDistributedCache<MyCacheItem> _cache;

    public YourService(IDistributedCache<MyCacheItem> cache)
    {
        _cache = cache;
    }

    public async Task<MyCacheItem> GetOrCreateAsync(string key)
    {
        return await _cache.GetOrAddAsync(
            key,
            async () => await CreateItemAsync(),
            () => new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            }
        );
    }

    private async Task<MyCacheItem> CreateItemAsync()
    {
        // Create and return item
        return new MyCacheItem();
    }
}
```

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
[DependsOn(typeof(AbpValidationModule))]
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
    typeof(AbpAspNetCoreModule),
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

## Swashbuckle

Swagger/Swashbuckle API documentation support.

### Installation

```bash
dotnet add package SharpAbp.Abp.Swashbuckle
```

### Configuration

Configure in your Web module:

```csharp
[DependsOn(typeof(AbpSwashbuckleModule))]
public class YourWebModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Your API",
                Version = "v1"
            });

            options.DocInclusionPredicate((docName, description) => true);
            options.CustomSchemaIds(type => type.FullName);
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
        });
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

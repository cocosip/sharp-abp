# Multi-Tenancy

Sharp-ABP provides advanced multi-tenancy features including map-based tenancy isolation and tenant grouping.

## MapTenancy

Map-based tenancy resolves the current tenant from a request value: a tenant `Code` or an external `MapCode`.
`AbpAspNetCoreMapTenancyModule` registers query-string, form, route, header and cookie resolvers, so no middleware call is required.

### Installation

```bash
# Core MapTenancy
dotnet add package SharpAbp.Abp.MapTenancy

# AspNetCore integration (tenant resolvers)
dotnet add package SharpAbp.Abp.AspNetCore.MapTenancy
```

### Configuration

Map codes are configured under `MapTenancyOptions`, keyed by code:

```json
{
  "MapTenancyOptions": {
    "tenant1": {
      "TenantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "TenantName": "Tenant 1",
      "Code": "tenant1",
      "MapCode": "t1"
    },
    "tenant2": {
      "TenantId": "8b3c9d45-6e2a-4f8b-9c5d-1a2b3c4d5e6f",
      "TenantName": "Tenant 2",
      "Code": "tenant2",
      "MapCode": "t2"
    }
  }
}
```

Bind the section (the core module does not bind it automatically):

```csharp
Configure<AbpMapTenancyOptions>(options => options.Configure(configuration));
```

The same mappings can be configured in code:

```csharp
Configure<AbpMapTenancyOptions>(options =>
{
    options.Mappers.Configure("tenant1", mapper =>
    {
        mapper.TenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        mapper.TenantName = "Tenant 1";
        mapper.Code = "tenant1";
        mapper.MapCode = "t1";
    });
});
```

Add the module dependencies:

```csharp
[DependsOn(
    typeof(AbpMapTenancyModule),
    typeof(AbpAspNetCoreMapTenancyModule)
)]
public class YourModule : AbpModule
{
}
```

The resolvers read the request value from the key configured by `AbpAspNetCoreMapTenancyOptions.MapTenantKey`
(default `__tenant`):

```csharp
Configure<AbpAspNetCoreMapTenancyOptions>(options =>
{
    options.MapTenantKey = "__tenant";
});
```

A request such as `?__tenant=t1` resolves the tenant through its `MapCode`; `?__tenant=tenant1` resolves it through its `Code`.

### Usage Example

```csharp
public class TenantAwareService : ITransientDependency
{
    private readonly ICurrentTenant _currentTenant;
    private readonly ITenantStore _tenantStore;

    public TenantAwareService(ICurrentTenant currentTenant, ITenantStore tenantStore)
    {
        _currentTenant = currentTenant;
        _tenantStore = tenantStore;
    }

    public async Task<string> GetCurrentTenantNameAsync()
    {
        if (_currentTenant.Id == null)
        {
            return "Host";
        }

        var tenant = await _tenantStore.FindAsync(_currentTenant.Id.Value);
        return tenant?.Name ?? "Unknown";
    }

    public IDisposable ChangeTenant(Guid tenantId)
    {
        return _currentTenant.Change(tenantId);
    }
}
```

#### Dynamic Tenant Mapping Management

Use `IMapTenantAppService` from `SharpAbp.Abp.MapTenancyManagement.*` to create, update and delete mappings at runtime:

```csharp
public class MapTenantService : ApplicationService
{
    private readonly IMapTenantAppService _mapTenantAppService;

    public MapTenantService(IMapTenantAppService mapTenantAppService)
    {
        _mapTenantAppService = mapTenantAppService;
    }

    public Task<MapTenantDto> CreateAsync(Guid tenantId, string tenantName, string code, string mapCode)
    {
        return _mapTenantAppService.CreateAsync(new CreateMapTenantDto(tenantId, tenantName, code, mapCode));
    }
}
```

For cached lookups without going through the application service, inject `IMapTenantStore`:

```csharp
var mapping = await _mapTenantStore.GetByCodeAsync("tenant1");
using (_currentTenant.Change(mapping.TenantId))
{
    // Work in the mapped tenant context.
}
```
#### Tenant Code Provider

`IMapTenantCodeProvider` resolves a tenant id to the mapping code used by consumers that need a stable tenant code instead of a tenant GUID or tenant name.
For example, `SharpAbp.Abp.FileStoring.MapTenancy` uses this provider to fill `FilePathContext.TenantCode`.

```csharp
public class StorageTenantCodeService : ITransientDependency
{
    private readonly IMapTenantCodeProvider _mapTenantCodeProvider;

    public StorageTenantCodeService(IMapTenantCodeProvider mapTenantCodeProvider)
    {
        _mapTenantCodeProvider = mapTenantCodeProvider;
    }

    public async Task<string?> FindCodeAsync(Guid tenantId)
    {
        var codeInfo = await _mapTenantCodeProvider.FindByTenantIdAsync(tenantId);
        return codeInfo?.Code;
    }
}
```

`MapTenantCodeInfo` contains:

| Property | Description |
|----------|-------------|
| `TenantId` | Tenant id from the mapping. |
| `TenantName` | Optional tenant display/name value from the mapping. |
| `Code` | Tenant mapping code. This is the default tenant path code for FileStoring integration. |
| `MapCode` | Optional alternate mapping code. FileStoring can opt into this with `FilePathTenantCodeSource.MapCode`. |

The default provider reads from `AbpMapTenancyOptions`.
When `SharpAbp.Abp.MapTenancyManagement.Domain` is installed, `DatabaseMapTenantCodeProvider` replaces it and reads tenant code information from `IMapTenantStore`.

---

## TenancyGrouping

Tenant grouping groups tenants so group-level settings (for example connection strings) can be resolved alongside the
current tenant. `SharpAbp.Abp.TenantGroupManagement.*` adds persistent CRUD management on top of the framework module.

### Installation

```bash
# Framework grouping
dotnet add package SharpAbp.Abp.TenancyGrouping

# AspNetCore integration
dotnet add package SharpAbp.Abp.AspNetCore.TenancyGrouping

# Persistent tenant group management (optional)
dotnet add package SharpAbp.Abp.TenantGroupManagement.Application
```

### Configuration

Add the module dependencies:

```csharp
[DependsOn(
    typeof(AbpTenancyGroupingModule),
    typeof(AbpAspNetCoreTenancyGroupingModule)
)]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpTenancyGroupingOptions>(options =>
        {
            options.IsEnabled = true;
        });
    }
}
```

Without the management module, groups can be supplied from configuration through
`AbpDefaultTenantGroupStoreOptions.Groups`. Additional resolvers are added to
`AbpTenantGroupResolveOptions.TenantResolvers`.

### Usage Example

#### Manage Tenant Groups

```csharp
public class TenantGroupService : ApplicationService
{
    private readonly ITenantGroupAppService _tenantGroupAppService;

    public TenantGroupService(ITenantGroupAppService tenantGroupAppService)
    {
        _tenantGroupAppService = tenantGroupAppService;
    }

    public Task<TenantGroupDto> CreateAsync(string name)
    {
        return _tenantGroupAppService.CreateAsync(new CreateTenantGroupDto
        {
            Name = name,
            IsActive = true
        });
    }

    public Task<TenantGroupDto> AddTenantAsync(Guid groupId, Guid tenantId)
    {
        return _tenantGroupAppService.AddTenantAsync(groupId, new AddTenantDto
        {
            TenantId = tenantId
        });
    }

    public Task<PagedResultDto<TenantGroupDto>> GetListAsync(TenantGroupPagedRequestDto input)
    {
        return _tenantGroupAppService.GetPagedListAsync(input);
    }
}
```

`ITenantGroupAppService` also exposes `UpdateAsync`, `DeleteAsync`, `RemoveTenantAsync`,
`GetDefaultConnectionStringAsync`, `UpdateDefaultConnectionStringAsync` and
`DeleteDefaultConnectionStringAsync`.

#### Connection String HTTP Migration

The following updates accept the connection string only as a JSON string in the request body:

| Method | Route |
|--------|-------|
| `PUT` | `/api/tenant-group/tenant-groups/{id}/default-connection-string` |
| `PUT` | `/api/map-tenancy/hybrid-map-tenants/{id}/default-connection-string` |

For example:

```http
PUT /api/tenant-group/tenant-groups/3fa85f64-5717-4562-b3fc-2c963f66afa6/default-connection-string
Content-Type: application/json

"Server=db.example;Database=tenant_database;User Id=tenant_user;Password=replace-me"
```

This is an HTTP contract change: remove the `defaultConnectionString` query parameter and
send the JSON string body, including its surrounding quotes. A JSON object wrapper is not
used. Query-only requests no longer bind the connection string and fail body validation;
when both are present, the body is used. Update HTTP callers and regenerate proxies as needed.
The application-service and controller C# method signatures remain `string` parameters, and
connection strings remain excluded from audit parameters through `[DisableAuditing]`.

#### Current Tenant Group

```csharp
public class GroupAwareService : ITransientDependency
{
    private readonly ICurrentTenantGroup _currentTenantGroup;
    private readonly IRepository<Product, Guid> _productRepository;

    public GroupAwareService(
        ICurrentTenantGroup currentTenantGroup,
        IRepository<Product, Guid> productRepository)
    {
        _currentTenantGroup = currentTenantGroup;
        _productRepository = productRepository;
    }

    public async Task<List<Product>> GetGroupProductsAsync()
    {
        if (!_currentTenantGroup.IsAvailable || _currentTenantGroup.Id == null)
        {
            return [];
        }

        return await _productRepository.GetListAsync();
    }

    public IDisposable ChangeGroup(Guid groupId)
    {
        return _currentTenantGroup.Change(groupId);
    }
}
```

`ICurrentTenantGroup` exposes `IsAvailable`, `Id`, `Name`, `Tenants` and `Change(id, name, tenants)`.

---

## Best Practices

### 1. Tenant Isolation

Always use ABP's tenant resolution mechanisms:

```csharp
public class TenantAwareRepository<TEntity> : ITransientDependency
    where TEntity : class, IEntity, IMultiTenant
{
    private readonly IRepository<TEntity> _repository;
    private readonly ICurrentTenant _currentTenant;

    public async Task<List<TEntity>> GetListAsync()
    {
        // Automatically filtered by current tenant
        return await _repository.GetListAsync();
    }

    public async Task<TEntity> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);

        // Additional tenant check
        if (entity.TenantId != _currentTenant.Id)
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        return entity;
    }
}
```

### 2. Host vs Tenant Operations

Distinguish between host and tenant operations:

```csharp
public class AdminService : ApplicationService
{
    private readonly ICurrentTenant _currentTenant;

    [Authorize(Permissions.SystemAdmin)]
    public async Task<List<TenantDto>> GetAllTenantsAsync()
    {
        // This should only be accessible to host
        if (_currentTenant.Id != null)
        {
            throw new BusinessException("This operation is only available for host");
        }

        var tenants = await _tenantRepository.GetListAsync();
        return ObjectMapper.Map<List<Tenant>, List<TenantDto>>(tenants);
    }

    public async Task<TenantDto> GetCurrentTenantAsync()
    {
        if (_currentTenant.Id == null)
        {
            throw new BusinessException("No tenant context");
        }

        var tenant = await _tenantRepository.GetAsync(_currentTenant.Id.Value);
        return ObjectMapper.Map<Tenant, TenantDto>(tenant);
    }
}
```

### 3. Cross-Tenant Operations

Use tenant switching carefully:

```csharp
public class CrossTenantReportService : ApplicationService
{
    private readonly ICurrentTenant _currentTenant;
    private readonly IRepository<Order> _orderRepository;

    [Authorize(Permissions.CrossTenantReports)]
    public async Task<ConsolidatedReport> GenerateConsolidatedReportAsync(
        List<Guid> tenantIds)
    {
        var report = new ConsolidatedReport();

        foreach (var tenantId in tenantIds)
        {
            using (_currentTenant.Change(tenantId))
            {
                var orders = await _orderRepository.GetListAsync();
                report.AddTenantData(tenantId, orders);
            }
        }

        return report;
    }
}
```

### 4. Tenant Configuration

Store tenant-specific configuration:

```csharp
public class TenantConfigurationService : ApplicationService
{
    private readonly ITenantConfigurationProvider _configProvider;

    public async Task<T> GetTenantConfigAsync<T>(string key)
    {
        var value = await _configProvider.GetAsync(key);
        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetTenantConfigAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        await _configProvider.SetAsync(key, json);
    }
}
```

# Multi-Tenancy

Sharp-ABP provides advanced multi-tenancy features including map-based tenancy isolation and tenant grouping.

## MapTenancy

Map-based tenancy isolation that allows domain-to-tenant binding for seamless tenant resolution.

### Installation

```bash
# Core MapTenancy
dotnet add package SharpAbp.Abp.MapTenancy

# AspNetCore integration
dotnet add package SharpAbp.Abp.AspNetCore.MapTenancy
```

### Configuration

Configure in `appsettings.json`:

```json
{
  "MapTenancy": {
    "TenantMappings": [
      {
        "Domain": "tenant1.myapp.com",
        "TenantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
      },
      {
        "Domain": "tenant2.myapp.com",
        "TenantId": "8b3c9d45-6e2a-4f8b-9c5d-1a2b3c4d5e6f"
      },
      {
        "Domain": "app.customdomain.com",
        "TenantId": "7a4e8f23-9b1c-4d5e-a6f7-3b2c1d0e9f8a"
      }
    ]
  }
}
```

Add the module dependency:

```csharp
[DependsOn(
    typeof(AbpMapTenancyModule),
    typeof(AbpAspNetCoreMapTenancyModule)
)]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpMapTenancyOptions>(options =>
        {
            options.TenantMappings.Add(new TenantMapping
            {
                Domain = "tenant1.myapp.com",
                TenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6")
            });

            options.TenantMappings.Add(new TenantMapping
            {
                Domain = "tenant2.myapp.com",
                TenantId = Guid.Parse("8b3c9d45-6e2a-4f8b-9c5d-1a2b3c4d5e6f")
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        // Add MapTenancy middleware (before MVC)
        app.UseMapTenancy();
        app.UseRouting();
        app.UseConfiguredEndpoints();
    }
}
```

### Usage Example

```csharp
public class TenantAwareService : ITransientDependency
{
    private readonly ICurrentTenant _currentTenant;
    private readonly ITenantStore _tenantStore;

    public TenantAwareService(
        ICurrentTenant _currentTenant,
        ITenantStore tenantStore)
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

    public Guid? GetCurrentTenantId()
    {
        return _currentTenant.Id;
    }

    // Switch tenant context
    public async Task<List<Product>> GetTenantProductsAsync(Guid tenantId)
    {
        using (_currentTenant.Change(tenantId))
        {
            // Operations here will be in the context of the specified tenant
            return await _productRepository.GetListAsync();
        }
    }
}
```

#### Dynamic Tenant Mapping Management

```csharp
public class TenantMappingService : ApplicationService
{
    private readonly ITenantMappingStore _mappingStore;

    public TenantMappingService(ITenantMappingStore mappingStore)
    {
        _mappingStore = mappingStore;
    }

    // Add new domain mapping
    public async Task AddDomainMappingAsync(string domain, Guid tenantId)
    {
        await _mappingStore.AddOrUpdateAsync(new TenantMapping
        {
            Domain = domain,
            TenantId = tenantId
        });
    }

    // Remove domain mapping
    public async Task RemoveDomainMappingAsync(string domain)
    {
        await _mappingStore.RemoveAsync(domain);
    }

    // Get tenant by domain
    public async Task<Guid?> GetTenantIdByDomainAsync(string domain)
    {
        var mapping = await _mappingStore.FindByDomainAsync(domain);
        return mapping?.TenantId;
    }

    // List all mappings for a tenant
    public async Task<List<string>> GetTenantDomainsAsync(Guid tenantId)
    {
        var mappings = await _mappingStore.GetListByTenantIdAsync(tenantId);
        return mappings.Select(m => m.Domain).ToList();
    }
}
```

---

## TenancyGrouping

Tenant grouping isolation allows you to organize tenants into groups and apply group-level configurations and isolation.

### Installation

```bash
# Core TenancyGrouping
dotnet add package SharpAbp.Abp.TenancyGrouping

# AspNetCore integration
dotnet add package SharpAbp.Abp.AspNetCore.TenancyGrouping
```

### Configuration

Add the module dependency:

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
            // Configure grouping options
            options.EnableGroupIsolation = true;
            options.DefaultGroupName = "Default";
        });
    }
}
```

### Usage Example

#### Define Tenant Groups

```csharp
public class TenantGroup : AggregateRoot<Guid>
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public Dictionary<string, object> Properties { get; set; }
    public bool IsActive { get; set; }
}
```

#### Tenant Group Management

```csharp
public class TenantGroupAppService : ApplicationService
{
    private readonly IRepository<TenantGroup, Guid> _groupRepository;
    private readonly ITenantGroupManager _tenantGroupManager;

    public TenantGroupAppService(
        IRepository<TenantGroup, Guid> groupRepository,
        ITenantGroupManager tenantGroupManager)
    {
        _groupRepository = groupRepository;
        _tenantGroupManager = tenantGroupManager;
    }

    // Create tenant group
    public async Task<TenantGroupDto> CreateAsync(CreateTenantGroupDto input)
    {
        var group = new TenantGroup
        {
            Name = input.Name,
            DisplayName = input.DisplayName,
            Description = input.Description,
            IsActive = true
        };

        await _groupRepository.InsertAsync(group);
        return ObjectMapper.Map<TenantGroup, TenantGroupDto>(group);
    }

    // Assign tenant to group
    public async Task AssignTenantToGroupAsync(Guid tenantId, Guid groupId)
    {
        await _tenantGroupManager.AssignTenantToGroupAsync(tenantId, groupId);
    }

    // Get tenants in group
    public async Task<List<TenantDto>> GetTenantsInGroupAsync(Guid groupId)
    {
        var tenants = await _tenantGroupManager.GetTenantsInGroupAsync(groupId);
        return ObjectMapper.Map<List<Tenant>, List<TenantDto>>(tenants);
    }

    // Get tenant's group
    public async Task<TenantGroupDto> GetTenantGroupAsync(Guid tenantId)
    {
        var group = await _tenantGroupManager.GetTenantGroupAsync(tenantId);
        return ObjectMapper.Map<TenantGroup, TenantGroupDto>(group);
    }
}
```

#### Group-Level Data Isolation

```csharp
public class GroupAwareProductService : ApplicationService
{
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly ICurrentTenantGroup _currentTenantGroup;

    public GroupAwareProductService(
        IRepository<Product, Guid> productRepository,
        ICurrentTenantGroup currentTenantGroup)
    {
        _productRepository = productRepository;
        _currentTenantGroup = currentTenantGroup;
    }

    // Get products visible to current tenant's group
    public async Task<List<ProductDto>> GetGroupProductsAsync()
    {
        var groupId = _currentTenantGroup.Id;

        var queryable = await _productRepository.GetQueryableAsync();
        var products = await AsyncExecuter.ToListAsync(
            queryable.Where(p => p.TenantGroupId == groupId)
        );

        return ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
    }

    // Share product across group
    public async Task ShareProductWithGroupAsync(Guid productId, Guid targetGroupId)
    {
        var product = await _productRepository.GetAsync(productId);

        // Create shared product reference
        var sharedProduct = new SharedProduct
        {
            OriginalProductId = productId,
            TargetGroupId = targetGroupId
        };

        // Save shared reference
    }
}
```

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

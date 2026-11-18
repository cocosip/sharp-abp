# Business Modules

Enterprise business modules providing complete functionality for identity management, file storage management, database connection management, and more.

## Account

Account management module providing user registration, login, and verification functionality.

### Installation

```bash
dotnet add package SharpAbp.Abp.Account.Application
dotnet add package SharpAbp.Abp.Account.Application.Contracts
dotnet add package SharpAbp.Abp.Account.HttpApi
dotnet add package SharpAbp.Abp.Account.HttpApi.Client
dotnet add package SharpAbp.Abp.Account.Web
```

### Configuration

```csharp
[DependsOn(
    typeof(AccountApplicationModule),
    typeof(AccountHttpApiModule)
)]
public class YourModule : AbpModule
{
}
```

### Usage

The Account module provides pre-built APIs for common account operations through `IAccountAppService` and `IProfileAppService`.

---

## Identity

Comprehensive identity management module for users, roles, permissions, and organizations.

### Installation

```bash
dotnet add package SharpAbp.Abp.Identity.Domain
dotnet add package SharpAbp.Abp.Identity.Domain.Shared
dotnet add package SharpAbp.Abp.Identity.Application
dotnet add package SharpAbp.Abp.Identity.Application.Contracts
dotnet add package SharpAbp.Abp.Identity.HttpApi
dotnet add package SharpAbp.Abp.Identity.HttpApi.Client
```

### Configuration

```csharp
[DependsOn(
    typeof(IdentityApplicationModule),
    typeof(IdentityHttpApiModule)
)]
public class YourModule : AbpModule
{
}
```

### Usage Example

```csharp
public class UserManagementService : ApplicationService
{
    private readonly IIdentityUserAppService _userAppService;

    public UserManagementService(IIdentityUserAppService userAppService)
    {
        _userAppService = userAppService;
    }

    public async Task<IdentityUserDto> CreateUserAsync(string userName, string email, string password)
    {
        return await _userAppService.CreateAsync(new NewIdentityUserCreateDto
        {
            UserName = userName,
            Email = email,
            Password = password,
            IsActive = true
        });
    }
}
```

---

## OpenIddict

OpenIddict OAuth2/OIDC authentication server module.

### Installation

```bash
dotnet add package SharpAbp.Abp.OpenIddict.Domain
dotnet add package SharpAbp.Abp.OpenIddict.Application
dotnet add package SharpAbp.Abp.OpenIddict.HttpApi
```

The module exposes `IOpenIddictApplicationAppService` and `IOpenIddictScopeAppService` for managing clients and scopes.

---

## AuditLogging

Audit logging module for operation tracking and compliance.

### Installation

```bash
dotnet add package SharpAbp.Abp.AuditLogging.Domain
dotnet add package SharpAbp.Abp.AuditLogging.Application
dotnet add package SharpAbp.Abp.AuditLogging.HttpApi
```

### Usage Example

```csharp
public class AuditLogService : ApplicationService
{
    private readonly IAuditLogAppService _auditLogAppService;

    public AuditLogService(IAuditLogAppService auditLogAppService)
    {
        _auditLogAppService = auditLogAppService;
    }

    public async Task<List<AuditLogDto>> GetAuditLogsAsync(DateTime startDate, DateTime endDate)
    {
        var result = await _auditLogAppService.GetPagedListAsync(new AuditLogPagedRequestDto
        {
            StartTime = startDate,
            EndTime = endDate,
            MaxResultCount = 100
        });

        return result.Items;
    }
}
```

---

## MinId

Distributed ID generation service based on segment IDs.

### Installation

```bash
dotnet add package SharpAbp.MinId.Domain
dotnet add package SharpAbp.MinId.Domain.Shared
dotnet add package SharpAbp.MinId.Application
dotnet add package SharpAbp.MinId.HttpApi
dotnet add package SharpAbp.MinId.EntityFrameworkCore
```

### Configuration

```csharp
[DependsOn(
    typeof(MinIdApplicationModule),
    typeof(MinIdHttpApiModule),
    typeof(MinIdEntityFrameworkCoreModule)
)]
public class YourModule : AbpModule
{
}
```

### Usage Example

```csharp
public class IdGenerationService : ApplicationService
{
    private readonly IMinIdGeneratorAppService _idGeneratorAppService;

    public IdGenerationService(IMinIdGeneratorAppService idGeneratorAppService)
    {
        _idGeneratorAppService = idGeneratorAppService;
    }

    public async Task<long> GenerateIdAsync(string bizType)
    {
        return await _idGeneratorAppService.NextIdAsync(bizType);
    }

    public async Task<List<long>> GenerateBatchIdsAsync(string bizType, int batchSize)
    {
        return await _idGeneratorAppService.NextIdAsync(bizType, batchSize);
    }
}
```

Business types and segment allocations are managed through `IMinIdInfoAppService` and `IMinIdTokenAppService`.

> **Upgrading existing databases:** `MinIdInfo` now has a `ConcurrencyStamp` column used for optimistic concurrency. Add a non-null `ConcurrencyStamp` string column (max length 40) with an empty-string default, then regenerate the consumer application's EF Core migration or apply the equivalent SQL before deploying this version.
>
> **Breaking API change:** the `next-segmentId` endpoint now requires the `token` query parameter and validates it against the configured MinId token. Update existing clients before deploying.

---

## FileStoringManagement

File storage management module for database-backed container configuration. The domain package replaces `IFileContainerConfigurationProvider` with `DatabaseFileContainerConfigurationProvider`, so runtime file containers are loaded from the database and cache instead of `AbpFileStoringOptions.Containers`.

Actual file upload, download, and deletion use the `IFileContainer` service from `SharpAbp.Abp.FileStoring`; this module manages the container definitions and their provider settings.

### Installation

```bash
dotnet add package SharpAbp.Abp.FileStoringManagement.Domain
dotnet add package SharpAbp.Abp.FileStoringManagement.Application
dotnet add package SharpAbp.Abp.FileStoringManagement.HttpApi
dotnet add package SharpAbp.Abp.FileStoringManagement.EntityFrameworkCore
```

### Configuration

```csharp
[DependsOn(
    typeof(FileStoringManagementApplicationModule),
    typeof(FileStoringManagementHttpApiModule),
    typeof(FileStoringManagementEntityFrameworkCoreModule)
)]
public class YourModule : AbpModule
{
}
```

### Usage Example

```csharp
public class ContainerManagementService : ApplicationService
{
    private readonly IContainerAppService _containerAppService;
    private readonly IFileProviderAppService _fileProviderAppService;

    public ContainerManagementService(
        IContainerAppService containerAppService,
        IFileProviderAppService fileProviderAppService)
    {
        _containerAppService = containerAppService;
        _fileProviderAppService = fileProviderAppService;
    }

    public async Task<ContainerDto> CreateFileSystemContainerAsync(
        string name,
        string title,
        string basePath)
    {
        return await _containerAppService.CreateAsync(new CreateOrUpdateContainerDto
        {
            Name = name,
            Title = title,
            Provider = "FileSystem",
            IsMultiTenant = false,
            EnableAutoMultiPartUpload = false,
            MultiPartUploadMinFileSize = 0,
            MultiPartUploadShardingSize = 0,
            HttpAccess = false,
            Items = new List<CreateOrUpdateContainerItemDto>
            {
                new CreateOrUpdateContainerItemDto(null, "FileSystem.BasePath", basePath),
                new CreateOrUpdateContainerItemDto(null, "FileSystem.AppendContainerNameToBasePath", "false")
            }
        });
    }

    public async Task<List<string>> GetProvidersAsync()
    {
        var providers = await _fileProviderAppService.GetProvidersAsync();
        return providers.Select(x => x.Provider).ToList();
    }
}
```

Use `IContainerAppService.GetAllAsync`/`GetPagedListAsync` to list containers, and `IFileProviderAppService.GetOptionsAsync(provider)` to discover the configuration item names a provider accepts.

---

## FileStoringDatabase

Database file storage provider for storing files directly in the database.

### Installation

```bash
dotnet add package SharpAbp.Abp.FileStoring.Database.Domain
dotnet add package SharpAbp.Abp.FileStoring.Database.EntityFrameworkCore  # or MongoDB
```

### Configuration

```csharp
using SharpAbp.Abp.FileStoring;
using SharpAbp.Abp.FileStoring.Database;

Configure<AbpFileStoringOptions>(options =>
{
    options.Containers.ConfigureDefault(container =>
    {
        container.UseDatabase();
    });
});
```

---

## DbConnectionsManagement

Database connection configuration management module. Connection information created here is consumed by the `SharpAbp.Abp.DbConnections` module.

### Installation

```bash
dotnet add package SharpAbp.Abp.DbConnectionsManagement.Domain
dotnet add package SharpAbp.Abp.DbConnectionsManagement.Application
dotnet add package SharpAbp.Abp.DbConnectionsManagement.HttpApi
dotnet add package SharpAbp.Abp.DbConnectionsManagement.EntityFrameworkCore
```

### Usage Example

```csharp
public class ConnectionManagementService : ApplicationService
{
    private readonly IDatabaseConnectionInfoAppService _connectionInfoAppService;
    private readonly IDatabaseProviderAppService _databaseProviderAppService;

    public ConnectionManagementService(
        IDatabaseConnectionInfoAppService connectionInfoAppService,
        IDatabaseProviderAppService databaseProviderAppService)
    {
        _connectionInfoAppService = connectionInfoAppService;
        _databaseProviderAppService = databaseProviderAppService;
    }

    public async Task<DatabaseConnectionInfoDto> CreateConnectionAsync(
        string name,
        string databaseProvider,
        string connectionString)
    {
        return await _connectionInfoAppService.CreateAsync(new CreateDatabaseConnectionInfoDto
        {
            Name = name,
            DatabaseProvider = databaseProvider,
            ConnectionString = connectionString
        });
    }

    public async Task<List<DatabaseConnectionInfoDto>> GetAllConnectionsAsync()
    {
        return await _connectionInfoAppService.GetListAsync();
    }

    public async Task<List<string>> GetSupportedProvidersAsync()
    {
        return await _databaseProviderAppService.GetAllAsync();
    }
}
```

---

## MapTenancyManagement

Map-based tenancy management module for domain-tenant mapping.
The domain package also provides `DatabaseMapTenantCodeProvider`, which replaces the default `IMapTenantCodeProvider` and resolves tenant code data from `IMapTenantStore`.
Consumers such as `SharpAbp.Abp.FileStoring.MapTenancy` can use it to build file paths with the mapped tenant code.

### Installation

```bash
dotnet add package SharpAbp.Abp.MapTenancyManagement.Domain
dotnet add package SharpAbp.Abp.MapTenancyManagement.Application
dotnet add package SharpAbp.Abp.MapTenancyManagement.HttpApi
dotnet add package SharpAbp.Abp.MapTenancyManagement.EntityFrameworkCore
```

### Usage Example

```csharp
public class TenantMappingService : ApplicationService
{
    private readonly IMapTenantAppService _mapTenantAppService;

    public TenantMappingService(IMapTenantAppService mapTenantAppService)
    {
        _mapTenantAppService = mapTenantAppService;
    }

    public async Task<MapTenantDto> CreateMappingAsync(
        Guid tenantId,
        string tenantName,
        string code,
        string mapCode)
    {
        return await _mapTenantAppService.CreateAsync(new CreateMapTenantDto
        {
            TenantId = tenantId,
            TenantName = tenantName,
            Code = code,
            MapCode = mapCode
        });
    }

    public async Task<MapTenantDto> FindByTenantIdAsync(Guid tenantId)
    {
        return await _mapTenantAppService.FindByTenantIdAsync(tenantId);
    }
}
```

Hybrid tenant creation is available through `IHybridMapTenantAppService`.

---

> **Upgrading existing databases:** `MapTenant.TenantId` now has a unique index. Before applying the migration, find duplicate `TenantId` rows and merge or delete them; otherwise index creation fails.

## TenantGroupManagement

Tenant group management module for organizing tenants into groups.

### Installation

```bash
dotnet add package SharpAbp.Abp.TenantGroupManagement.Domain
dotnet add package SharpAbp.Abp.TenantGroupManagement.Application
dotnet add package SharpAbp.Abp.TenantGroupManagement.HttpApi
dotnet add package SharpAbp.Abp.TenantGroupManagement.EntityFrameworkCore
```

### Usage Example

```csharp
public class TenantGroupAdminService : ApplicationService
{
    private readonly ITenantGroupAppService _tenantGroupAppService;

    public TenantGroupAdminService(ITenantGroupAppService tenantGroupAppService)
    {
        _tenantGroupAppService = tenantGroupAppService;
    }

    public async Task<TenantGroupDto> CreateGroupAsync(string name)
    {
        return await _tenantGroupAppService.CreateAsync(new CreateTenantGroupDto
        {
            Name = name,
            IsActive = true
        });
    }

    public async Task<TenantGroupDto> AddTenantAsync(Guid groupId, Guid tenantId)
    {
        return await _tenantGroupAppService.AddTenantAsync(groupId, new AddTenantDto
        {
            TenantId = tenantId
        });
    }
}
```

Use `RemoveTenantAsync`, `GetDefaultConnectionStringAsync`, and `UpdateDefaultConnectionStringAsync` for the remaining group operations.

> **Upgrading existing databases:** the unique index now uses `TenantGroup.NormalizedName` instead of `Name`. Backfill `NormalizedName` and remove duplicate normalized names before applying the migration.

---

## CryptoVault

Crypto vault module for centralized RSA and SM2 key storage. Key material is encrypted at rest, and decrypted keys are only returned through the explicitly authorized `GetDecryptKey` operation.

### Installation

```bash
dotnet add package SharpAbp.Abp.CryptoVault.Domain
dotnet add package SharpAbp.Abp.CryptoVault.Application
dotnet add package SharpAbp.Abp.CryptoVault.HttpApi
dotnet add package SharpAbp.Abp.CryptoVault.EntityFrameworkCore
```

### Usage Example

```csharp
public class RsaKeyService : ApplicationService
{
    private readonly IRSACredsAppService _rsaCredsAppService;

    public RsaKeyService(IRSACredsAppService rsaCredsAppService)
    {
        _rsaCredsAppService = rsaCredsAppService;
    }

    public async Task<RSACredsDto> ImportKeyPairAsync(
        int size,
        string publicKey,
        string privateKey)
    {
        return await _rsaCredsAppService.CreateAsync(new CreateRSACredsDto
        {
            Size = size,
            PublicKey = publicKey,
            PrivateKey = privateKey
        });
    }

    public async Task GenerateKeyPairsAsync(int size, int count)
    {
        await _rsaCredsAppService.GenerateAsync(new GenerateRSACredsDto
        {
            Size = size,
            Count = count
        });
    }

    public async Task<RSACredsDto> FindAsync(string identifier)
    {
        return await _rsaCredsAppService.FindByIdentifierAsync(identifier);
    }

    public async Task<RSACredsKeyDto> GetDecryptKeyAsync(Guid id)
    {
        return await _rsaCredsAppService.GetDecryptKey(id);
    }
}
```

SM2 keys use `ISM2CredsAppService` with `CreateSM2CredsDto`, `GenerateSM2CredsDto`, `SM2CredsDto`, and `SM2CredsKeyDto`.

---

## TransformSecurityManagement

Data transform security management module. It creates and stores the credential metadata used by `SharpAbp.Abp.TransformSecurity`; the corresponding RSA or SM2 key material is stored in CryptoVault.

### Installation

```bash
dotnet add package SharpAbp.Abp.TransformSecurityManagement.Domain
dotnet add package SharpAbp.Abp.TransformSecurityManagement.Application
dotnet add package SharpAbp.Abp.TransformSecurityManagement.HttpApi
dotnet add package SharpAbp.Abp.TransformSecurityManagement.EntityFrameworkCore
```

### Usage Example

```csharp
public class SecurityCredentialAdminService : ApplicationService
{
    private readonly ISecurityCredentialInfoAppService _securityCredentialInfoAppService;

    public SecurityCredentialAdminService(
        ISecurityCredentialInfoAppService securityCredentialInfoAppService)
    {
        _securityCredentialInfoAppService = securityCredentialInfoAppService;
    }

    public async Task<SecurityCredentialInfoDto> CreateAsync(string bizType)
    {
        return await _securityCredentialInfoAppService.CreateAsync(
            new CreateSecurityCredentialInfoDto
            {
                BizType = bizType
            });
    }

    public async Task<List<SecurityCredentialInfoDto>> GetByBizTypeAsync(string bizType)
    {
        return await _securityCredentialInfoAppService.GetListAsync(bizType: bizType);
    }
}
```

See [Security.md](Security.md) for the runtime encryption API that consumes these credentials.

---

## Integration Example

Here's an example of how these modules work together. It assumes `SharpAbp.Abp.FileStoring` is configured with a default file container and the DbConnectionsManagement module is installed:

```csharp
public class EnterpriseApplicationService : ApplicationService
{
    private readonly IMinIdGeneratorAppService _idGeneratorAppService;
    private readonly IFileContainer _fileContainer;
    private readonly IDatabaseConnectionInfoAppService _connectionInfoAppService;

    public EnterpriseApplicationService(
        IMinIdGeneratorAppService idGeneratorAppService,
        IFileContainer fileContainer,
        IDatabaseConnectionInfoAppService connectionInfoAppService)
    {
        _idGeneratorAppService = idGeneratorAppService;
        _fileContainer = fileContainer;
        _connectionInfoAppService = connectionInfoAppService;
    }

    public async Task<OrderDto> CreateOrderWithFilesAsync(CreateOrderDto input)
    {
        // Generate a unique order ID
        var orderId = await _idGeneratorAppService.NextIdAsync("Order");

        // Store files in the default file container
        var fileIds = new List<string>();
        foreach (var file in input.Files)
        {
            var fileId = $"{orderId}/{file.FileName}";
            await _fileContainer.SaveAsync(
                fileId,
                file.Stream,
                Path.GetExtension(file.FileName));
            fileIds.Add(fileId);
        }

        // Resolve the tenant-specific connection information
        var connectionInfo = await _connectionInfoAppService.FindByNameAsync(
            $"tenant-{CurrentTenant.Id}");

        // Open the connection with IDbConnectionFactory from SharpAbp.Abp.DbConnections
        // and persist the order in the tenant database.
        // Audit logging of the application service is handled by ABP.

        return new OrderDto
        {
            Id = orderId,
            FileIds = fileIds
        };
    }
}
```

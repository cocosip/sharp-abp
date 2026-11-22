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

External sign-in identifies existing users by the stored provider and subject. An email claim alone never links an external identity to an existing account: the user must sign in through an existing login method and explicitly link the provider through the host application's authenticated account management flow. Applications that relied on automatic email linking must migrate to that flow; this module does not add a linking endpoint.

Automatic account creation from an external login also respects `SharpAbp.Account.IsSelfRegistrationEnabled`; disabling registration does not prevent an already linked external identity from signing in.

Password-reset links preserve the configured reset URL's query values and fragment. The account service supplies its own `userId`, tenant, reset token, return URL and return fragment fields. OIDC callback return URLs continue to use the decoded `redirect_uri` when present.

Login and registration use the normal Identity sign-in checks, including configured email/account confirmation, lockout and two-factor authentication. A newly registered account and its external login link remain registered when confirmation or another sign-in check denies authentication; no application cookie is issued. Confirmation and recovery flows remain the host application's responsibility. Override `TwoFactorLoginResultAsync` on the login and registration page models to continue an application-specific two-factor flow; the default returns to login without authenticating.

Existing registration hooks keep their signatures. Overrides that complete a checked sign-in continue to redirect successfully when Identity issues a new application principal; an authenticated principal already present before registration does not count as a new sign-in. Overrides can set `RegistrationSignInResult` explicitly to pass confirmation, lockout or two-factor outcomes to the page. Use checked Identity sign-in methods rather than `SignInAsync`, which bypasses those policies.

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

Client and scope updates preserve the JSON `Properties` bag when the request omits it or supplies `null`. An explicit object replaces the bag, so omitted keys are removed and `{}` clears all keys. The DTO collections remain initialized for .NET callers using `Properties.Add`; adding entries supplies a replacement bag. Assign an empty dictionary to clear the bag from a direct .NET call. Serializers that emit an initialized empty collection send an explicit `{}`; set `Properties` to `null` or omit the member to preserve stored values. Protected `BuildModel` overrides can still add server properties before persistence, including when the client omits the bag.

Extra properties use ABP's extension definitions on both the request DTO and persisted entity. To expose an extra property in a response, register it on the entity and response DTO. Updates retain stored extension values outside the request's defined fields.

Scope changes use the OpenIddict manager for validation. On hosts that load `SharpAbp.Abp.OpenIddict.Domain`, scope manager lookups read the configured database directly and follow its transaction isolation. The application service removes previous ABP scope cache entries after a successful unit-of-work commit. Enable the SharpAbp domain module on every host that resolves scopes and use a database with consistent read-after-write behavior. Client and token cache settings remain independent.

Deleting a client and its tokens participates in the caller's transaction. A rollback restores both; a successful commit also evicts application cache entries that another host may have refilled before the deletion committed.

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
Set `Remainder` to a nonnegative value smaller than both `Delta` and `Step`. The generator
uses the nonnegative mathematical residue modulo `Delta`, including for negative IDs.
`Step` controls the width of each reserved segment. A step at least as large as the delta
ensures each segment contains a matching ID. Before deployment, correct persisted rows with
a nonpositive step or delta, a negative remainder, or a remainder at least as large as the delta.

MinId business type names are permanent. Increase `MaxId` only when intentionally advancing
the sequence, and treat its current value as the highest reserved ID. Updating configuration
with an older `MaxId` retains the persisted watermark while applying the other fields, so
an allocation between an administrator's read and edit does not block the edit or reset IDs.
Deleting a business type retires it and keeps its name reserved in the database. Already reserved IDs may still
be issued by running processes. Use a new business type name for a new sequence.

Before deploying, add the non-null boolean `IsDeleted` column with a false default to the
configured MinId information table using your host's EF Core migration. Keep the unique
`BizType` index across active and retired records. Bring all MinId writers onto the upgraded
version before retiring business types. Restore tombstones for names previously hard-deleted
only when you have reliable allocation history. Keep those names out of reuse otherwise.

> **Upgrading existing databases:** `MinIdInfo` now has a `ConcurrencyStamp` column used for optimistic concurrency. Add a non-null `ConcurrencyStamp` string column (max length 40) with an empty-string default, then regenerate the consumer application's EF Core migration or apply the equivalent SQL before deploying this version.
>
### Credential transport and limits

All five ID endpoints under `/api/minid/id` accept a MinId credential in `X-MinId-Token`.
This leaves `Authorization` available for the host's normal JWT or OpenIddict access token.
The existing `Authorization: Bearer <minid-token>` transport remains available when the host
does not require a separate user access token. Query-string `token` credentials are rejected,
including when a valid header is also present. MinId credentials are opaque strings;
the host's normal authentication and authorization policies still apply.

```http
GET /api/minid/id/batch-nextId?bizType=orders&batchSize=100 HTTP/1.1
Authorization: Bearer <host-access-token>
X-MinId-Token: <minid-token>
```

The application service and HTTP endpoints reject batch sizes outside **1–1,000** before
allocating IDs. HTTP validation returns a bad request for an unsupported count. Trusted
in-process callers of `IMinIdGenerator.NextIdAsync(batchSize)` can still request any positive
batch size. Nonpositive counts are rejected rather than silently raised to one.

Administrative token lookup now uses `POST /api/minid/minid-token/find-by-token` with
`{"bizType":"orders","token":"<minid-token>"}` as its JSON body. Use
the `FindAsync(FindMinIdTokenDto)` extension on `IMinIdTokenAppService` for convenient
lookup without adding a new required interface member. Existing .NET HTTP proxy
calls to `FindByTokenAsync(bizType, token)` continue to work through
`POST /api/minid/minid-token/lookup?bizType=orders`, with the token as a JSON string body.
Its old token-bearing GET route was removed. Token listing/filtering now uses
`POST /api/minid/minid-token/search` with a `MinIdTokenPagedRequestDto` JSON body instead of
GET query parameters. Administrative
requests retain their normal user authorization header separately from the body credential.

Business type matching follows the repository database's collation, while token secrets
are compared case-sensitively after lookup. Tokens are checked against persisted state for
every validation; positive cache entries are ignored, including late entries written after
revocation. Route these checks to the
authoritative primary database, and do not reuse a long-lived snapshot transaction for
authentication. Previously authorized in-flight requests may finish. Existing legacy cache
invalidations still run after successful transaction completion for older cache consumers;
all application instances must be upgraded to remove their former cached acceptance window.

The `MinIdResponse` envelope is retained. Invalid credentials and invalid batch requests
receive stable messages; unexpected failures contain a reference ID without backend details.
Server diagnostics retain the reference, exception type and stack without copying arbitrary
exception messages that may contain credentials. Cancellation is propagated rather than
converted into a server-failure envelope. Production hosts must keep ABP detailed exception
responses disabled and redact Authorization and X-MinId-Token headers, request bodies and
any legacy URL credentials in reverse-proxy/APM/access logs.

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

RSA imports accept the formats produced by the Crypto module: base64 DER SubjectPublicKeyInfo public keys, PKCS#1 or PKCS#8 private keys, and the corresponding unencrypted PEM keys. Imported strings retain their original format when decrypted again. Public and private roles, matching key pairs, and the supplied key size are validated. SM2 imports accept a hexadecimal encoded elliptic-curve point and private scalar in the format returned by the SharpAbp export helpers. The point must match the scalar on the supplied curve. Validation applies to new imports. For existing records, identify and rotate unusable keys before removing them.

Database-backed transform credentials convert PEM/PKCS#8 RSA material to their runtime
SPKI/PKCS#1 representation during generation and reload. Vault key strings remain in
their original format. Already compatible SPKI/PKCS#1 strings remain unchanged; only
formats requiring conversion perform native RSA import/export in this normalization step.

When the host loads both CryptoVault and TransformSecurityManagement, the CryptoVault delete operation rejects keys referenced by persisted transform-security credentials, including expired records. Before retiring a key, pause issuance that could select it, wait for in-flight issuance, and remove the relevant credential records after their retention period. The guard runs in the deletion host and checks its configured credential store. It cannot coordinate concurrent issuance across hosts or separate stores.

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

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
    typeof(AbpAccountApplicationModule),
    typeof(AbpAccountHttpApiModule)
)]
public class YourModule : AbpModule
{
}
```

### Usage

The Account module provides pre-built APIs for common account operations.

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
    typeof(AbpIdentityApplicationModule),
    typeof(AbpIdentityHttpApiModule)
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
        return await _userAppService.CreateAsync(new IdentityUserCreateDto
        {
            UserName = userName,
            Email = email,
            Password = password,
            IsActive = true,
            RoleNames = new[] { "User" }
        });
    }
}
```

---

## IdentityServer & OpenIddict

OAuth2/OIDC authentication server modules.

### Installation

```bash
# IdentityServer4
dotnet add package SharpAbp.Abp.IdentityServer.Domain
dotnet add package SharpAbp.Abp.IdentityServer.Application
dotnet add package SharpAbp.Abp.IdentityServer.HttpApi

# OpenIddict (modern alternative)
dotnet add package SharpAbp.Abp.OpenIddict.Domain
dotnet add package SharpAbp.Abp.OpenIddict.Application
dotnet add package SharpAbp.Abp.OpenIddict.HttpApi
```

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
    private readonly IAuditLogRepository _auditLogRepository;

    public async Task<List<AuditLogDto>> GetAuditLogsAsync(DateTime startDate, DateTime endDate)
    {
        var logs = await _auditLogRepository.GetListAsync(
            startTime: startDate,
            endTime: endDate
        );

        return ObjectMapper.Map<List<AuditLog>, List<AuditLogDto>>(logs);
    }
}
```

---

## MinId

Distributed ID generation service for generating globally unique IDs.

### Installation

```bash
dotnet add package SharpAbp.MinId.Domain
dotnet add package SharpAbp.MinId.Domain.Shared
dotnet add package SharpAbp.MinId.Application
dotnet add package SharpAbp.MinId.HttpApi
dotnet add package SharpAbp.MinId.EntityFrameworkCore  # or MongoDB
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
    private readonly IMinIdAppService _minIdService;

    public IdGenerationService(IMinIdAppService minIdService)
    {
        _minIdService = minIdService;
    }

    public async Task<long> GenerateIdAsync(string name)
    {
        return await _minIdService.NextIdAsync(name);
    }

    public async Task<List<long>> GenerateBatchIdsAsync(string name, int count)
    {
        return await _minIdService.NextIdsAsync(name, count);
    }
}
```

---

## FileStoringManagement

File storage management module for centralized file upload, download, and configuration.

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
public class FileManagementService : ApplicationService
{
    private readonly IFileStoringManagementAppService _fileManagementService;

    public async Task<FileInfoDto> UploadFileAsync(IFormFile file, string containerName)
    {
        using (var stream = file.OpenReadStream())
        {
            return await _fileManagementService.CreateAsync(new CreateFileInfoDto
            {
                ContainerName = containerName,
                FileName = file.FileName,
                FileSize = file.Length,
                FileStream = stream
            });
        }
    }

    public async Task<Stream> DownloadFileAsync(Guid fileId)
    {
        return await _fileManagementService.GetFileStreamAsync(fileId);
    }
}
```

---

## FileStoringDatabase

Database file storage provider for storing files directly in the database.

### Installation

```bash
dotnet add package SharpAbp.Abp.FileStoringDatabase.Domain
dotnet add package SharpAbp.Abp.FileStoringDatabase.EntityFrameworkCore  # or MongoDB
```

### Configuration

```csharp
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

Database connection configuration management module.

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
    private readonly IDbConnectionsManagementAppService _connectionService;

    public async Task<DatabaseConnectionDto> CreateConnectionAsync(
        string name,
        DatabaseType databaseType,
        string connectionString)
    {
        return await _connectionService.CreateAsync(new CreateDatabaseConnectionDto
        {
            DatabaseName = name,
            DatabaseType = databaseType,
            ConnectionString = connectionString,
            IsActive = true
        });
    }

    public async Task<List<DatabaseConnectionDto>> GetAllConnectionsAsync()
    {
        return await _connectionService.GetListAsync();
    }

    public async Task<bool> TestConnectionAsync(Guid connectionId)
    {
        return await _connectionService.TestConnectionAsync(connectionId);
    }
}
```

---

## MapTenancyManagement

Map-based tenancy management module for domain-tenant mapping.

### Installation

```bash
dotnet add package SharpAbp.Abp.MapTenancyManagement.Domain
dotnet add package SharpAbp.Abp.MapTenancyManagement.Application
dotnet add package SharpAbp.Abp.MapTenancyManagement.HttpApi
dotnet add package SharpAbp.Abp.MapTenancyManagement.EntityFrameworkCore
```

### Usage Example

```csharp
public class TenantMappingAppService : ApplicationService
{
    private readonly IMapTenancyManagementAppService _mappingService;

    public async Task<TenantMappingDto> CreateMappingAsync(string domain, Guid tenantId)
    {
        return await _mappingService.CreateAsync(new CreateTenantMappingDto
        {
            Domain = domain,
            TenantId = tenantId
        });
    }

    public async Task<List<TenantMappingDto>> GetTenantMappingsAsync(Guid tenantId)
    {
        return await _mappingService.GetByTenantIdAsync(tenantId);
    }
}
```

---

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
public class TenantGroupAppService : ApplicationService
{
    private readonly ITenantGroupManagementAppService _groupService;

    public async Task<TenantGroupDto> CreateGroupAsync(string name, string displayName)
    {
        return await _groupService.CreateAsync(new CreateTenantGroupDto
        {
            Name = name,
            DisplayName = displayName,
            IsActive = true
        });
    }

    public async Task AssignTenantToGroupAsync(Guid tenantId, Guid groupId)
    {
        await _groupService.AssignTenantAsync(new AssignTenantToGroupDto
        {
            TenantId = tenantId,
            GroupId = groupId
        });
    }
}
```

---

## CryptoVault

Crypto vault and secret management module for centralized key and secret storage.

### Installation

```bash
dotnet add package SharpAbp.CryptoVault.Domain
dotnet add package SharpAbp.CryptoVault.Application
dotnet add package SharpAbp.CryptoVault.HttpApi
dotnet add package SharpAbp.CryptoVault.EntityFrameworkCore
```

### Usage Example

```csharp
public class SecretManagementService : ApplicationService
{
    private readonly ICryptoVaultAppService _vaultService;

    public async Task<SecretDto> StoreSecretAsync(string key, string value)
    {
        return await _vaultService.CreateSecretAsync(new CreateSecretDto
        {
            Key = key,
            Value = value,
            IsEncrypted = true
        });
    }

    public async Task<string> GetSecretAsync(string key)
    {
        var secret = await _vaultService.GetSecretByKeyAsync(key);
        return secret.Value;
    }

    public async Task RotateSecretAsync(string key, string newValue)
    {
        await _vaultService.RotateSecretAsync(key, newValue);
    }
}
```

---

## TransformSecurityManagement

Data transform security management module for encryption configuration and management.

### Installation

```bash
dotnet add package SharpAbp.TransformSecurityManagement.Domain
dotnet add package SharpAbp.TransformSecurityManagement.Application
dotnet add package SharpAbp.TransformSecurityManagement.HttpApi
dotnet add package SharpAbp.TransformSecurityManagement.EntityFrameworkCore
```

### Usage Example

```csharp
public class SecurityConfigService : ApplicationService
{
    private readonly ITransformSecurityManagementAppService _securityService;

    public async Task<EncryptionConfigDto> CreateEncryptionConfigAsync(
        string name,
        TransformAlgorithm algorithm)
    {
        return await _securityService.CreateConfigAsync(new CreateEncryptionConfigDto
        {
            Name = name,
            Algorithm = algorithm,
            IsActive = true
        });
    }

    public async Task<List<EncryptionConfigDto>> GetAllConfigsAsync()
    {
        return await _securityService.GetConfigListAsync();
    }
}
```

---

## Integration Example

Here's an example of how these modules work together:

```csharp
public class EnterpriseApplicationService : ApplicationService
{
    private readonly ICurrentTenant _currentTenant;
    private readonly IMinIdAppService _minIdService;
    private readonly IFileStoringManagementAppService _fileService;
    private readonly IDbConnectionsManagementAppService _connectionService;
    private readonly IAuditLogRepository _auditLogRepository;

    public async Task<OrderDto> CreateOrderWithFilesAsync(CreateOrderDto input)
    {
        // Generate unique ID
        var orderId = await _minIdService.NextIdAsync("Order");

        // Upload files to tenant-specific storage
        var fileIds = new List<Guid>();
        foreach (var file in input.Files)
        {
            var fileInfo = await _fileService.CreateAsync(new CreateFileInfoDto
            {
                ContainerName = $"tenant-{_currentTenant.Id}",
                FileName = file.FileName,
                FileStream = file.Stream
            });
            fileIds.Add(fileInfo.Id);
        }

        // Store in tenant-specific database
        using (var connection = await GetTenantConnectionAsync())
        {
            // Create order
            await connection.ExecuteAsync(
                "INSERT INTO Orders (Id, CustomerId, ...) VALUES (...)",
                new { Id = orderId, ... }
            );
        }

        // Audit log is automatically created
        // Multi-tenancy is automatically handled

        return new OrderDto { Id = orderId, FileIds = fileIds };
    }

    private async Task<DbConnection> GetTenantConnectionAsync()
    {
        var connections = await _connectionService.GetListAsync();
        var tenantConnection = connections.FirstOrDefault(
            c => c.TenantId == _currentTenant.Id
        );

        return await CreateConnectionAsync(tenantConnection.ConnectionString);
    }
}
```

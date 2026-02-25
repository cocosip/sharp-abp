# File Storage

Sharp-ABP provides a unified file storage abstraction with support for multiple storage providers including cloud services and distributed file systems.

## FileStoring

Unified file storage abstraction and core implementation that allows you to switch between different storage providers seamlessly.

### Installation

```bash
# Core abstraction
dotnet add package SharpAbp.Abp.FileStoring.Abstractions
dotnet add package SharpAbp.Abp.FileStoring

# Choose one or more storage providers:
dotnet add package SharpAbp.Abp.FileStoring.FileSystem     # Local file system
dotnet add package SharpAbp.Abp.FileStoring.Aliyun         # Aliyun OSS
dotnet add package SharpAbp.Abp.FileStoring.Azure          # Azure Blob Storage
dotnet add package SharpAbp.Abp.FileStoring.Aws            # AWS S3
dotnet add package SharpAbp.Abp.FileStoring.S3             # S3-compatible storage
dotnet add package SharpAbp.Abp.FileStoring.Minio          # MinIO
dotnet add package SharpAbp.Abp.FileStoring.KS3            # Kingsoft Cloud KS3
dotnet add package SharpAbp.Abp.FileStoring.Obs            # Huawei Cloud OBS
dotnet add package SharpAbp.Abp.FileStoring.FastDFS        # FastDFS
```

### Configuration

Configure in `appsettings.json`:

```json
{
  "FileStoring": {
    "Containers": {
      "default": {
        "Provider": "FileSystem",
        "FileSystem": {
          "BasePath": "C:\\Files"
        }
      },
      "profile-pictures": {
        "Provider": "Aliyun",
        "Aliyun": {
          "AccessKeyId": "your-access-key",
          "AccessKeySecret": "your-secret-key",
          "Endpoint": "oss-cn-hangzhou.aliyuncs.com",
          "BucketName": "my-bucket",
          "CreateBucketIfNotExists": true
        }
      },
      "documents": {
        "Provider": "Aws",
        "Aws": {
          "AccessKeyId": "your-access-key",
          "SecretAccessKey": "your-secret-key",
          "Region": "us-east-1",
          "BucketName": "my-documents",
          "CreateBucketIfNotExists": false
        }
      }
    }
  }
}
```

Add the module dependency:

```csharp
[DependsOn(
    typeof(AbpFileStoringModule),
    typeof(AbpFileStoringFileSystemModule),  // or other providers
    typeof(AbpFileStoringAliyunModule)
)]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpFileStoringOptions>(options =>
        {
            options.Containers.ConfigureDefault(container =>
            {
                container.UseFileSystem(fileSystem =>
                {
                    fileSystem.BasePath = "C:\\Files";
                });
            });

            options.Containers.Configure<ProfilePictureContainer>(container =>
            {
                container.UseAliyun(aliyun =>
                {
                    aliyun.AccessKeyId = configuration["FileStoring:Aliyun:AccessKeyId"];
                    aliyun.AccessKeySecret = configuration["FileStoring:Aliyun:AccessKeySecret"];
                    aliyun.Endpoint = configuration["FileStoring:Aliyun:Endpoint"];
                    aliyun.BucketName = configuration["FileStoring:Aliyun:BucketName"];
                    aliyun.CreateBucketIfNotExists = true;
                });
            });
        });
    }
}
```

### Usage Example

#### Basic File Operations

```csharp
public class FileService : ITransientDependency
{
    private readonly IFileContainer _fileContainer;
    private readonly IFileContainer<ProfilePictureContainer> _profilePictureContainer;

    public FileService(
        IFileContainer fileContainer,
        IFileContainer<ProfilePictureContainer> profilePictureContainer)
    {
        _fileContainer = fileContainer;
        _profilePictureContainer = profilePictureContainer;
    }

    // Save file to default container
    public async Task<string> SaveFileAsync(string fileName, byte[] fileBytes)
    {
        await _fileContainer.SaveAsync(fileName, fileBytes);
        return fileName;
    }

    // Save file to specific container
    public async Task<string> SaveProfilePictureAsync(Guid userId, Stream fileStream)
    {
        var fileName = $"{userId}/profile.jpg";
        await _profilePictureContainer.SaveAsync(fileName, fileStream);
        return fileName;
    }

    // Get file
    public async Task<byte[]> GetFileAsync(string fileName)
    {
        return await _fileContainer.GetAllBytesAsync(fileName);
    }

    // Get file as stream
    public async Task<Stream> GetFileStreamAsync(string fileName)
    {
        return await _fileContainer.GetAsync(fileName);
    }

    // Delete file
    public async Task DeleteFileAsync(string fileName)
    {
        await _fileContainer.DeleteAsync(fileName);
    }

    // Check if file exists
    public async Task<bool> FileExistsAsync(string fileName)
    {
        return await _fileContainer.ExistsAsync(fileName);
    }
}
```

#### Advanced File Management

```csharp
public class DocumentService : ApplicationService
{
    private readonly IFileContainer<DocumentContainer> _documentContainer;

    public DocumentService(IFileContainer<DocumentContainer> documentContainer)
    {
        _documentContainer = documentContainer;
    }

    public async Task<Guid> UploadDocumentAsync(IFormFile file)
    {
        var fileId = Guid.NewGuid();
        var fileName = $"{fileId}/{file.FileName}";

        using (var stream = file.OpenReadStream())
        {
            await _documentContainer.SaveAsync(
                fileName,
                stream,
                overrideExisting: false
            );
        }

        return fileId;
    }

    public async Task<FileResult> DownloadDocumentAsync(Guid documentId, string fileName)
    {
        var filePath = $"{documentId}/{fileName}";
        var stream = await _documentContainer.GetAsync(filePath);

        return new FileStreamResult(stream, "application/octet-stream")
        {
            FileDownloadName = fileName
        };
    }

    public async Task<List<string>> ListDocumentsAsync(Guid documentId)
    {
        var prefix = $"{documentId}/";
        // Note: List functionality depends on the provider
        // Not all providers support listing files
        return new List<string>();
    }
}
```

---

## File Path Building

All storage providers delegate path/key construction to a centralised `IFilePathBuilder` service.
This makes it possible to control how files are laid out in your bucket or file system **without touching provider-specific code**.

### Architecture

```
IFileContainer.SaveAsync / GetAsync / DeleteAsync / ...
        │
        ▼
DefaultXxxFileNameCalculator.Calculate(args)   ← one per provider, all delegate to ↓
        │
        ▼
IFilePathBuilder.Build(args)                   ← single point of path logic
        │
        ├─ reads AbpFileStoringAbstractionsOptions.FilePathBuilder  (global config)
        ├─ reads AbpFileStoringAbstractionsOptions.FilePathStrategy (TenantBased / DirectFileId)
        └─ reads IFilePathContextAccessor.Current                   (per-operation context)
```

### Default Path Patterns

| Strategy | Tenant | Result |
|----------|--------|--------|
| `TenantBased` | Host | `host/{fileId}` |
| `TenantBased` | Tenant | `tenants/{tenantId}/{fileId}` |
| `TenantBased` + Prefix | Host | `{prefix}/host/{fileId}` |
| `TenantBased` + TenantName | Tenant | `tenants/{tenantName}/{fileId}` |
| `DirectFileId` | Any | `{fileId}` |

---

### Method 1: Configure via appsettings.json

Add a `FilePathBuilder` block inside `FileStoringOptions`:

```json
{
  "FileStoringOptions": {
    "FilePathBuilder": {
      "FilePathStrategy": "TenantBased",
      "Prefix": "uploads",
      "HostSegment": "host",
      "TenantsSegment": "tenants",
      "TenantIdentifierMode": "TenantName"
    },
    "default": {
      "Provider": "Minio",
      "Properties": { ... }
    }
  }
}
```

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| `FilePathStrategy` | `TenantBased` / `DirectFileId` | `TenantBased` | Overall path building mode |
| `Prefix` | string | _(empty)_ | Static prefix prepended to every path |
| `HostSegment` | string | `host` | Segment name used for host (non-tenant) paths |
| `TenantsSegment` | string | `tenants` | Directory name for tenant paths |
| `TenantIdentifierMode` | `TenantId` / `TenantName` | `TenantId` | Which tenant property is used in the path |

> The `FilePathBuilder` key is reserved and will be skipped when iterating container entries.

Load configuration in your Module:

```csharp
public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
{
    var configuration = context.Services.GetConfiguration();

    Configure<AbpFileStoringOptions>(c =>
    {
        c.Configure(configuration, context);
    });

    return Task.CompletedTask;
}
```

---

### Method 2: Configure via code in Module

Use `Configure<AbpFileStoringAbstractionsOptions>` for full control, including factory delegates:

```csharp
Configure<AbpFileStoringAbstractionsOptions>(opts =>
{
    // Global prefix
    opts.FilePathBuilder.Prefix = "uploads";

    // Use tenant Name as path segment instead of GUID
    opts.FilePathBuilder.TenantIdentifierFactory = (id, name, ctx) =>
        ctx?.TenantCode              // highest priority: per-operation code
        ?? name                      // second: tenant Name
        ?? id.ToString("D");         // fallback: GUID

    // Dynamic prefix driven by per-operation context
    opts.FilePathBuilder.PrefixFactory = ctx =>
        ctx?.Prefix                                          // per-operation override
        ?? ctx?.Extra.GetValueOrDefault("category") as string  // extra param
        ?? "files";                                          // global default
});
```

---

### Method 3: Per-Operation Context (runtime parameters)

Inject `IFilePathContextAccessor` to pass parameters at the call site.
Context is restored automatically when the `using` block exits (async-safe via `AsyncLocal`).

```csharp
public class MyFileService : ITransientDependency
{
    private readonly IFileContainer _fileContainer;
    private readonly IFilePathContextAccessor _filePathContextAccessor;

    public MyFileService(
        IFileContainer fileContainer,
        IFilePathContextAccessor filePathContextAccessor)
    {
        _fileContainer = fileContainer;
        _filePathContextAccessor = filePathContextAccessor;
    }

    // Use a custom tenant code in the path
    public async Task SaveAsync(string fileId, Stream stream, string tenantCode)
    {
        using (_filePathContextAccessor.Change(new FilePathContext { TenantCode = tenantCode }))
        {
            await _fileContainer.SaveAsync(fileId, stream, "jpg");
            // path: tenants/{tenantCode}/{fileId}
        }
    }

    // Add a path prefix for this operation only
    public async Task SaveImageAsync(string fileId, Stream stream)
    {
        using (_filePathContextAccessor.Change(new FilePathContext { Prefix = "images" }))
        {
            await _fileContainer.SaveAsync(fileId, stream, "png");
            // path: images/host/{fileId}
        }
    }

    // Combine tenant code + prefix + arbitrary extra data
    public async Task SaveWithFullContextAsync(string fileId, Stream stream, Tenant tenant)
    {
        using (_filePathContextAccessor.Change(new FilePathContext
        {
            TenantCode = tenant.Code,
            Prefix = "docs",
            Extra = { ["region"] = "cn-north" }   // available to PrefixFactory / TenantIdentifierFactory
        }))
        {
            await _fileContainer.SaveAsync(fileId, stream, "pdf");
            // path: docs/tenants/{tenant.Code}/{fileId}
        }
    }
}
```

> **Important**: Use the same `FilePathContext` for both write and read operations, otherwise the computed paths will differ and the file will not be found.

#### FilePathContext Properties

| Property | Type | Description |
|----------|------|-------------|
| `TenantCode` | `string?` | Custom tenant identifier. Passed as the third argument to `TenantIdentifierFactory`. |
| `Prefix` | `string?` | Per-operation path prefix. Overrides `FilePathBuilderOptions.Prefix` when `PrefixFactory` is not set. |
| `Extra` | `Dictionary<string, object?>` | Arbitrary key-value pairs accessible inside both factory delegates. |

---

### Method 4: Custom Rules by Overriding DefaultFilePathBuilder

For moderately complex requirements (e.g. look up an in-memory cache, combine multiple context fields), **override `DefaultFilePathBuilder`**:

```csharp
public class MyFilePathBuilder : DefaultFilePathBuilder
{
    private readonly ICurrentUser _currentUser;

    public MyFilePathBuilder(
        ICurrentTenant currentTenant,
        IFilePathContextAccessor filePathContextAccessor,
        IOptions<AbpFileStoringAbstractionsOptions> options,
        ICurrentUser currentUser)
        : base(currentTenant, filePathContextAccessor, options)
    {
        _currentUser = currentUser;
    }

    // Override only the tenant segment logic
    protected override string? ResolvePrefix(FilePathBuilderOptions options, FilePathContext? context)
    {
        // Prepend current user's department as a path segment
        var dept = _currentUser.FindClaim("department")?.Value;
        var basePrefix = base.ResolvePrefix(options, context);
        return dept != null ? $"{dept}/{basePrefix}" : basePrefix;
    }
}
```

Register in your Module to replace the default:

```csharp
context.Services.AddTransient<IFilePathBuilder, MyFilePathBuilder>();
```

---

### Method 5: Full Custom IFilePathBuilder (most complex scenarios)

For requirements that `DefaultFilePathBuilder` cannot support even with overrides — e.g. **async tenant code lookup from database** — implement `IFilePathBuilder` from scratch:

```csharp
public class DbTenantCodeFilePathBuilder : IFilePathBuilder, ITransientDependency
{
    private readonly ICurrentTenant _currentTenant;
    private readonly IFilePathContextAccessor _filePathContextAccessor;
    private readonly ITenantCodeCache _tenantCodeCache;   // your own service
    private readonly AbpFileStoringAbstractionsOptions _options;

    public DbTenantCodeFilePathBuilder(
        ICurrentTenant currentTenant,
        IFilePathContextAccessor filePathContextAccessor,
        ITenantCodeCache tenantCodeCache,
        IOptions<AbpFileStoringAbstractionsOptions> options)
    {
        _currentTenant = currentTenant;
        _filePathContextAccessor = filePathContextAccessor;
        _tenantCodeCache = tenantCodeCache;
        _options = options.Value;
    }

    public string Build(FileProviderArgs args)
    {
        var context = _filePathContextAccessor.Current;

        if (_options.FilePathStrategy == FilePathGenerationStrategy.DirectFileId)
        {
            return args.FileId;
        }

        var segments = new List<string>();

        if (_currentTenant.Id == null)
        {
            segments.Add("host");
        }
        else
        {
            // Use explicitly provided code, or look up from cache (sync wrapper)
            var code = context?.TenantCode
                ?? _tenantCodeCache.GetCode(_currentTenant.Id.Value)
                ?? _currentTenant.Id.Value.ToString("D");

            segments.Add($"tenants/{code}");
        }

        segments.Add(args.FileId);
        return string.Join("/", segments);
    }
}
```

> When `ITransientDependency` is present, ABP's DI auto-registration maps the concrete class.
> To register as the implementation of `IFilePathBuilder`, explicitly add:
>
> ```csharp
> context.Services.AddTransient<IFilePathBuilder, DbTenantCodeFilePathBuilder>();
> ```

---

### Extension Points Summary

| Scenario | Recommended approach |
|----------|----------------------|
| Static prefix / tenant-as-Name | `appsettings.json` → `FilePathBuilder` |
| Factory with runtime logic | `Configure<AbpFileStoringAbstractionsOptions>` in Module |
| Per-call parameters (code, prefix, extras) | `IFilePathContextAccessor.Change()` at call site |
| Custom logic reusing base behavior | Override `DefaultFilePathBuilder` |
| Fully custom / async lookup | Implement `IFilePathBuilder` from scratch |

---

## Provider-Specific Configurations

### FileSystem Provider

Local file system storage.

```csharp
Configure<AbpFileStoringOptions>(options =>
{
    options.Containers.ConfigureDefault(container =>
    {
        container.UseFileSystem(fileSystem =>
        {
            fileSystem.BasePath = Path.Combine(Directory.GetCurrentDirectory(), "Files");
        });
    });
});
```

### Aliyun OSS Provider

```csharp
Configure<AbpFileStoringOptions>(options =>
{
    options.Containers.ConfigureDefault(container =>
    {
        container.UseAliyun(aliyun =>
        {
            aliyun.AccessKeyId = "your-access-key";
            aliyun.AccessKeySecret = "your-secret";
            aliyun.Endpoint = "oss-cn-hangzhou.aliyuncs.com";
            aliyun.BucketName = "my-bucket";
            aliyun.CreateBucketIfNotExists = true;
            aliyun.RegionId = "cn-hangzhou";
        });
    });
});
```

### Azure Blob Storage Provider

```csharp
Configure<AbpFileStoringOptions>(options =>
{
    options.Containers.ConfigureDefault(container =>
    {
        container.UseAzure(azure =>
        {
            azure.ConnectionString = "DefaultEndpointsProtocol=https;AccountName=...;AccountKey=...";
            azure.ContainerName = "my-container";
            azure.CreateContainerIfNotExists = true;
        });
    });
});
```

### AWS S3 Provider

```csharp
Configure<AbpFileStoringOptions>(options =>
{
    options.Containers.ConfigureDefault(container =>
    {
        container.UseAws(aws =>
        {
            aws.AccessKeyId = "your-access-key";
            aws.SecretAccessKey = "your-secret-key";
            aws.Region = "us-east-1";
            aws.BucketName = "my-bucket";
            aws.CreateBucketIfNotExists = false;
        });
    });
});
```

### MinIO Provider

```csharp
Configure<AbpFileStoringOptions>(options =>
{
    options.Containers.ConfigureDefault(container =>
    {
        container.UseMinio(minio =>
        {
            minio.EndPoint = "localhost:9000";
            minio.AccessKey = "minioadmin";
            minio.SecretKey = "minioadmin";
            minio.BucketName = "my-bucket";
            minio.WithSSL = false;
            minio.CreateBucketIfNotExists = true;
        });
    });
});
```

### Huawei OBS Provider

```csharp
Configure<AbpFileStoringOptions>(options =>
{
    options.Containers.ConfigureDefault(container =>
    {
        container.UseObs(obs =>
        {
            obs.AccessKeyId = "your-access-key";
            obs.SecretAccessKey = "your-secret-key";
            obs.Endpoint = "obs.cn-north-4.myhuaweicloud.com";
            obs.BucketName = "my-bucket";
            obs.CreateBucketIfNotExists = true;
        });
    });
});
```

### KS3 (Kingsoft Cloud) Provider

```csharp
Configure<AbpFileStoringOptions>(options =>
{
    options.Containers.ConfigureDefault(container =>
    {
        container.UseKS3(ks3 =>
        {
            ks3.AccessKeyId = "your-access-key";
            ks3.AccessKeySecret = "your-secret-key";
            ks3.Endpoint = "ks3-cn-beijing.ksyuncs.com";
            ks3.BucketName = "my-bucket";
            ks3.CreateBucketIfNotExists = true;
        });
    });
});
```

---

## FastDFS

FastDFS distributed file system adapter for high-performance file storage.

### Installation

```bash
dotnet add package SharpAbp.Abp.FastDFS
dotnet add package SharpAbp.Abp.FastDFS.DotNetty        # DotNetty implementation
# OR
dotnet add package SharpAbp.Abp.FastDFS.SuperSocket    # SuperSocket implementation
```

### Configuration

Configure in `appsettings.json`:

```json
{
  "FastDFS": {
    "Trackers": [
      {
        "IPAddress": "192.168.1.100",
        "Port": 22122
      }
    ],
    "ConnectionTimeout": 30,
    "ConnectionLifeTime": 300,
    "Charset": "UTF-8",
    "GroupName": "group1"
  }
}
```

Add the module dependency:

```csharp
[DependsOn(
    typeof(AbpFastDFSModule),
    typeof(AbpFastDFSDotNettyModule)  // or AbpFastDFSSuperSocketModule
)]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<FastDFSOptions>(options =>
        {
            options.Trackers.Add(new TrackerServer
            {
                IPAddress = configuration["FastDFS:Trackers:0:IPAddress"],
                Port = configuration.GetValue<int>("FastDFS:Trackers:0:Port")
            });

            options.ConnectionTimeout = 30;
            options.ConnectionLifeTime = 300;
            options.Charset = "UTF-8";
            options.GroupName = "group1";
        });
    }
}
```

### Usage Example

```csharp
public class FastDFSFileService : ITransientDependency
{
    private readonly IFastDFSClient _fastDFSClient;

    public FastDFSFileService(IFastDFSClient fastDFSClient)
    {
        _fastDFSClient = fastDFSClient;
    }

    // Upload file
    public async Task<string> UploadFileAsync(byte[] fileBytes, string fileExtension)
    {
        var fileId = await _fastDFSClient.UploadFileAsync(fileBytes, fileExtension);
        return fileId; // Returns: group1/M00/00/00/wKgBbF...
    }

    // Upload file to specific group
    public async Task<string> UploadFileToGroupAsync(
        string groupName,
        byte[] fileBytes,
        string fileExtension)
    {
        var fileId = await _fastDFSClient.UploadFileAsync(
            groupName,
            fileBytes,
            fileExtension
        );
        return fileId;
    }

    // Download file
    public async Task<byte[]> DownloadFileAsync(string fileId)
    {
        return await _fastDFSClient.DownloadFileAsync(fileId);
    }

    // Delete file
    public async Task<bool> DeleteFileAsync(string fileId)
    {
        return await _fastDFSClient.RemoveFileAsync(fileId);
    }

    // Get file info
    public async Task<FastDFSFileInfo> GetFileInfoAsync(string fileId)
    {
        return await _fastDFSClient.GetFileInfoAsync(fileId);
    }

    // Upload with metadata
    public async Task<string> UploadWithMetadataAsync(
        byte[] fileBytes,
        string fileExtension,
        Dictionary<string, string> metadata)
    {
        var fileId = await _fastDFSClient.UploadFileAsync(fileBytes, fileExtension);

        await _fastDFSClient.SetMetadataAsync(fileId, metadata);

        return fileId;
    }

    // Get metadata
    public async Task<Dictionary<string, string>> GetMetadataAsync(string fileId)
    {
        return await _fastDFSClient.GetMetadataAsync(fileId);
    }
}
```

---

## AutoS3

AWSSDK.S3 adapter providing compatibility with S3-compatible storage services.

### Installation

```bash
dotnet add package SharpAbp.Abp.AutoS3
dotnet add package SharpAbp.Abp.AutoS3.KS3  # For Kingsoft Cloud KS3
```

### Configuration

Configure in `appsettings.json`:

```json
{
  "AutoS3": {
    "Configs": [
      {
        "Name": "default",
        "ServiceURL": "https://s3.amazonaws.com",
        "AccessKey": "your-access-key",
        "SecretKey": "your-secret-key",
        "UseHttp": false,
        "ForcePathStyle": false
      },
      {
        "Name": "ks3",
        "ServiceURL": "https://ks3-cn-beijing.ksyuncs.com",
        "AccessKey": "your-access-key",
        "SecretKey": "your-secret-key",
        "UseHttp": false,
        "ForcePathStyle": true
      }
    ]
  }
}
```

Add the module dependency:

```csharp
[DependsOn(typeof(AbpAutoS3Module))]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AutoS3Options>(options =>
        {
            options.Configs.Add("default", new AutoS3Config
            {
                ServiceURL = configuration["AutoS3:Configs:0:ServiceURL"],
                AccessKey = configuration["AutoS3:Configs:0:AccessKey"],
                SecretKey = configuration["AutoS3:Configs:0:SecretKey"],
                UseHttp = false
            });
        });
    }
}
```

### Usage Example

```csharp
public class S3FileService : ITransientDependency
{
    private readonly IAmazonS3ClientFactory _s3ClientFactory;

    public S3FileService(IAmazonS3ClientFactory s3ClientFactory)
    {
        _s3ClientFactory = s3ClientFactory;
    }

    public async Task<string> UploadFileAsync(
        string bucketName,
        string key,
        Stream fileStream)
    {
        var client = _s3ClientFactory.Create();

        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = fileStream
        };

        var response = await client.PutObjectAsync(request);

        return response.ETag;
    }

    public async Task<Stream> DownloadFileAsync(string bucketName, string key)
    {
        var client = _s3ClientFactory.Create();

        var response = await client.GetObjectAsync(bucketName, key);

        return response.ResponseStream;
    }

    public async Task DeleteFileAsync(string bucketName, string key)
    {
        var client = _s3ClientFactory.Create();

        await client.DeleteObjectAsync(bucketName, key);
    }

    // Use named client
    public async Task<string> UploadToKS3Async(
        string bucketName,
        string key,
        Stream fileStream)
    {
        var client = _s3ClientFactory.Create("ks3");

        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = fileStream
        };

        var response = await client.PutObjectAsync(request);

        return response.ETag;
    }
}
```

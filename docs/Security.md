# Security & Encryption

`SharpAbp.Abp.TransformSecurity` issues short-lived RSA or SM2 credentials and lets application code encrypt and decrypt values with those credentials. The ASP.NET Core package adds a middleware that processes encrypted request data (the default handler decrypts the `password` field of OAuth2 password-grant requests).

The module is credential-based. It does not provide attribute-based encryption, field-level encryption interceptors, or swappable encryption strategies.

## TransformSecurity

### Installation

```bash
# Core TransformSecurity
dotnet add package SharpAbp.Abp.TransformSecurity

# AspNetCore integration (middleware and security key endpoint)
dotnet add package SharpAbp.Abp.TransformSecurity.AspNetCore
```

### Configuration

Add the module dependency and configure it in code. The module does not bind an `appsettings.json` section; configure the options explicitly:

```csharp
using System;
using Org.BouncyCastle.Crypto.Engines;
using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.Crypto.SM2;
using SharpAbp.Abp.TransformSecurity;
using Volo.Abp.Modularity;

[DependsOn(typeof(AbpTransformSecurityModule))]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpTransformSecurityOptions>(options =>
        {
            // Default: false
            options.IsEnabled = true;

            // Default: RSA. The value is compared case-insensitively against "RSA" and "SM2".
            options.EncryptionAlgo = AbpTransformSecurityNames.RSA;

            // Default: 10 minutes
            options.Expires = TimeSpan.FromMinutes(10);

            // Only business types in this list can request credentials.
            // The module adds "Login" by default.
            options.BizTypes.Add("Payment");
        });

        // Used when EncryptionAlgo is RSA.
        Configure<AbpTransformSecurityRSAOptions>(options =>
        {
            options.KeySize = 2048;
            options.Padding = RSAPaddingNames.PKCS1Padding;
        });

        // Used when EncryptionAlgo is SM2.
        Configure<AbpTransformSecuritySM2Options>(options =>
        {
            options.Curve = Sm2EncryptionNames.CurveSm2p256v1;
            options.Mode = SM2Engine.Mode.C1C2C3;
        });
    }
}
```

`AbpTransformSecurityOptions.IsEnabled` only controls the ASP.NET Core middleware. `ISecurityEncryptionService` and `ISecurityCredentialManager` work regardless of this flag.

### Credential Generation

`ISecurityCredentialManager.GenerateAsync(bizType)` creates a key pair, stores the credential, and returns it. The business type must be listed in `AbpTransformSecurityOptions.BizTypes`; otherwise an `AbpException` is thrown.

```csharp
using System.Threading.Tasks;
using SharpAbp.Abp.TransformSecurity;
using Volo.Abp.DependencyInjection;

public class SecurityCredentialService : ITransientDependency
{
    private readonly ISecurityCredentialManager _credentialManager;
    private readonly ISecurityCredentialStore _credentialStore;

    public SecurityCredentialService(
        ISecurityCredentialManager credentialManager,
        ISecurityCredentialStore credentialStore)
    {
        _credentialManager = credentialManager;
        _credentialStore = credentialStore;
    }

    public async Task<SecurityCredential> IssueAsync(string bizType)
    {
        return await _credentialManager.GenerateAsync(bizType);
    }

    public async Task<SecurityCredential?> FindAsync(string identifier)
    {
        return await _credentialStore.GetAsync(identifier);
    }
}
```

A generated `SecurityCredential` contains `Identifier`, `KeyType` (`RSA` or `SM2`), `BizType`, `PublicKey`, `PrivateKey`, `Expires`, and `CreationTime`.

The default `ISecurityCredentialStore` implementation (`SecurityCredentialStore`) uses ABP's `IDistributedCache<SecurityCredential>` and stores each credential with an absolute expiration equal to `Expires`. It is a cache, not durable storage: in a multi-node deployment all nodes must share the same distributed cache (for example, Redis), otherwise a credential issued by one node is invisible to the others.

### Basic Encryption and Decryption

`ISecurityEncryptionService` selects RSA or SM2 from the stored credential's `KeyType`. Encrypt and decrypt both require the credential identifier; a missing, expired, or unsupported credential throws `AbpException`.

```csharp
using System.Threading.Tasks;
using SharpAbp.Abp.TransformSecurity;
using Volo.Abp.DependencyInjection;

public class DataEncryptionService : ITransientDependency
{
    private readonly ISecurityEncryptionService _encryptionService;

    public DataEncryptionService(ISecurityEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    public async Task<string> EncryptAsync(string plainText, string identifier)
    {
        return await _encryptionService.EncryptAsync(plainText, identifier);
    }

    public async Task<string> DecryptAsync(string cipherText, string identifier)
    {
        return await _encryptionService.DecryptAsync(cipherText, identifier);
    }

    public async Task<SecurityCredentialResultType> ValidateAsync(string identifier)
    {
        var result = await _encryptionService.ValidateAsync(identifier);
        return result.Result;
    }
}
```

`ValidateAsync` returns `SecurityCredentialResultType.Success`, `NotFound`, or `Expired`. Use it to decide whether to issue a new credential before calling `EncryptAsync`/`DecryptAsync`.

### ASP.NET Core Integration

Add `AbpTransformSecurityAspNetCoreModule` to the host module, register any custom middleware handlers, then call `UseAbpTransformSecurity()` in the request pipeline:

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.TransformSecurity;
using SharpAbp.Abp.TransformSecurity.AspNetCore;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.Modularity;

[DependsOn(typeof(AbpTransformSecurityAspNetCoreModule))]
public class YourHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpTransformSecurityOptions>(options =>
        {
            options.IsEnabled = true;
        });

        Configure<AbpTransformSecurityAspNetCoreOptions>(options =>
        {
            // Header that carries the credential identifier. Default: "AbpTransformSecurityIdentifier".
            options.TransformSecurityIdentifierName = "AbpTransformSecurityIdentifier";

            // Fixed-window rate limit for the security key endpoint.
            // Defaults: 60 requests per 60 seconds, partitioned by remote IP.
            options.SecurityKeyRateLimitPermitLimit = 60;
            options.SecurityKeyRateLimitWindowSeconds = 60;

            options.MiddlewareHandlers.Add<MyTransformSecurityRequestHandler>();
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        app.UseRouting();
        app.UseAbpTransformSecurity();
        app.UseConfiguredEndpoints();
    }
}
```

The middleware reads the identifier from the configured request header and runs every registered `IAbpTransformSecurityMiddlewareHandler` before the request reaches the endpoint. When `IsEnabled` is `false`, the middleware passes requests through unchanged. Unhandled `AbpException`s become HTTP 400 responses; `TransformSecurityRequestException` responses use its own status code.

If you prefer not to call the middleware, you can invoke `ISecurityEncryptionService` from your own middleware, filter, or application service.

#### Security Key Endpoint

`AbpTransformSecurityAspNetCoreModule` registers `SecurityKeyController`, which exposes the public key of a newly generated credential:

```text
GET /api/security-credentials/public?bizType=Login
```

Example response (`SecurityCredentialPublicKeyDto`):

```json
{
  "identifier": "3f2a...",
  "bizType": "Login",
  "keyType": "RSA",
  "publicKey": "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8A..."
}
```

The endpoint is rate limited with the `SecurityKey` policy configured above. A client uses `publicKey` to encrypt a value, then sends `identifier` in the `AbpTransformSecurityIdentifier` header together with the encrypted value.

#### Default Password-Grant Handler

The AspNetCore module adds `PasswordGrantTokenHandler` to `MiddlewareHandlers` by default. It applies only to `POST /connect/token` requests with `Content-Type: application/x-www-form-urlencoded` and `grant_type=password`:

- The `password` form field is decrypted using `ISecurityEncryptionService`.
- The identifier header is required for password grants and is rejected when missing.
- Other grant types and content types are passed through unchanged.

For any other encrypted field or endpoint, add a custom `IAbpTransformSecurityMiddlewareHandler`:

```csharp
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SharpAbp.Abp.TransformSecurity;
using SharpAbp.Abp.TransformSecurity.AspNetCore;
using Volo.Abp.DependencyInjection;

public class MyTransformSecurityRequestHandler : IAbpTransformSecurityMiddlewareHandler, ITransientDependency
{
    private readonly ISecurityEncryptionService _encryptionService;

    public MyTransformSecurityRequestHandler(ISecurityEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    public async Task HandleAsync(
        HttpContext context,
        string identifier,
        CancellationToken cancellationToken = default)
    {
        if (!context.Request.Headers.TryGetValue("X-Encrypted-Payload", out var encryptedPayload))
        {
            return;
        }

        var plainText = await _encryptionService.DecryptAsync(
            encryptedPayload.ToString(),
            identifier,
            cancellationToken);

        context.Items["DecryptedPayload"] = plainText;
    }
}
```

### Extension Points

- **Custom credential storage.** Replace `ISecurityCredentialStore` when the distributed cache is not appropriate (for example, to persist credentials in a database):

  ```csharp
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.DependencyInjection.Extensions;
  using SharpAbp.Abp.TransformSecurity;

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
      context.Services.Replace(
          ServiceDescriptor.Transient<ISecurityCredentialStore, DatabaseSecurityCredentialStore>());
  }
  ```

- **Custom request processing.** Implement `IAbpTransformSecurityMiddlewareHandler` and register it in `AbpTransformSecurityAspNetCoreOptions.MiddlewareHandlers`, as shown above. The middleware invokes handlers in registration order.

- **Encryption algorithms.** Only RSA and SM2 are supported. Select one with `AbpTransformSecurityOptions.EncryptionAlgo` and tune it through `AbpTransformSecurityRSAOptions` or `AbpTransformSecuritySM2Options`.

### Best Practices

#### 1. Keep Credentials Short-Lived

Use a small `Expires` window (the default is 600 seconds) and issue a new credential when validation returns `Expired`. `EncryptAsync` and `DecryptAsync` reject expired credentials.

#### 2. Restrict Business Types

Only add business types that actually need credentials to `BizTypes`. `GenerateAsync` throws for any other value, so the allow-list prevents arbitrary key generation.

#### 3. Protect Private Keys

`SecurityCredential.PrivateKey` and decrypted plaintext are sensitive. Do not log them, return them from public endpoints, or store them in client-accessible locations. The security key endpoint returns only `PublicKey`.

#### 4. Use TLS and Rate Limiting

Deploy the security key endpoint behind HTTPS. Keep the built-in fixed-window rate limit enabled, and add authorization to custom handlers when the endpoint is not meant to be anonymous.

#### 5. Share Credential State Across Nodes

Issue and consume credentials against a shared distributed cache (or a custom `ISecurityCredentialStore`). A process-local cache will make credentials issued on one node unusable on another.

#### 6. Handle Decryption Failures

Treat `AbpException` from `EncryptAsync`/`DecryptAsync` as a rejected request (unknown, expired, or unsupported credential) rather than a server fault:

```csharp
using System;
using System.Threading.Tasks;
using SharpAbp.Abp.TransformSecurity;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

public class SafeEncryptionService : ITransientDependency
{
    private readonly ISecurityEncryptionService _encryptionService;

    public SafeEncryptionService(ISecurityEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    public async Task<string> DecryptAsync(string cipherText, string identifier)
    {
        try
        {
            return await _encryptionService.DecryptAsync(cipherText, identifier);
        }
        catch (AbpException ex)
        {
            throw new UserFriendlyException("The security credential is invalid or has expired.", innerException: ex);
        }
    }
}
```

#### 7. Audit at the Application Boundary

The module has no audit logger of its own. Use ABP's auditing (application service calls are audited by default) and add explicit audit entries only where your application needs them. Never include plaintext or private keys in audit data.

### What This Module Does Not Do

- No attribute-based request or response encryption. Use handlers or call `ISecurityEncryptionService` explicitly.
- No entity/field-level encryption interceptor. Encrypt and decrypt values at the service or repository boundary.
- No custom encryption strategy interface. RSA and SM2 are the supported algorithms.

For centralized key storage rather than short-lived request credentials, use the CryptoVault and TransformSecurityManagement business modules.

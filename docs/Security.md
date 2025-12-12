# Security & Encryption

Data security and encryption modules providing SM2/AES encryption and transform security features.

## TransformSecurity

Data transform security module with SM2/AES encryption support for protecting sensitive data.

### Installation

```bash
# Core TransformSecurity
dotnet add package SharpAbp.Abp.TransformSecurity

# AspNetCore integration
dotnet add package SharpAbp.Abp.TransformSecurity.AspNetCore
```

### Configuration

Configure in `appsettings.json`:

```json
{
  "TransformSecurity": {
    "Algorithm": "AES",
    "AES": {
      "Key": "your-32-character-secret-key!!",
      "IV": "your-16-char-iv!"
    },
    "SM2": {
      "PublicKey": "04...",
      "PrivateKey": "..."
    },
    "EnableRequestDecryption": true,
    "EnableResponseEncryption": true
  }
}
```

Add the module dependency:

```csharp
[DependsOn(
    typeof(AbpTransformSecurityModule),
    typeof(AbpTransformSecurityAspNetCoreModule)
)]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<TransformSecurityOptions>(options =>
        {
            options.Algorithm = TransformAlgorithm.AES;

            options.AES = new AESOptions
            {
                Key = configuration["TransformSecurity:AES:Key"],
                IV = configuration["TransformSecurity:AES:IV"]
            };

            options.EnableRequestDecryption = true;
            options.EnableResponseEncryption = true;
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        // Add transform security middleware
        app.UseTransformSecurity();
    }
}
```

### Usage Example

#### Basic Encryption/Decryption

```csharp
public class DataEncryptionService : ITransientDependency
{
    private readonly ITransformSecurityService _securityService;

    public DataEncryptionService(ITransformSecurityService securityService)
    {
        _securityService = securityService;
    }

    // Encrypt data
    public string EncryptData(string plainText)
    {
        return _securityService.Encrypt(plainText);
    }

    // Decrypt data
    public string DecryptData(string cipherText)
    {
        return _securityService.Decrypt(cipherText);
    }

    // Encrypt object
    public string EncryptObject<T>(T obj)
    {
        var json = JsonSerializer.Serialize(obj);
        return _securityService.Encrypt(json);
    }

    // Decrypt object
    public T DecryptObject<T>(string encryptedData)
    {
        var json = _securityService.Decrypt(encryptedData);
        return JsonSerializer.Deserialize<T>(json);
    }
}
```

#### Attribute-Based Encryption

```csharp
// Automatically encrypt request body
[EncryptedRequest]
[HttpPost("api/secure/submit")]
public async Task<ActionResult> SubmitSecureDataAsync([FromBody] SecureDataDto input)
{
    // Request body is automatically decrypted
    // Process the data
    return Ok();
}

// Automatically encrypt response
[EncryptedResponse]
[HttpGet("api/secure/data")]
public async Task<ActionResult<SensitiveDataDto>> GetSensitiveDataAsync()
{
    var data = await _dataService.GetSensitiveDataAsync();

    // Response is automatically encrypted
    return data;
}

// Both request and response encryption
[EncryptedRequest]
[EncryptedResponse]
[HttpPost("api/secure/process")]
public async Task<ActionResult<ResultDto>> ProcessSecureDataAsync([FromBody] InputDto input)
{
    var result = await _processor.ProcessAsync(input);
    return result;
}
```

#### Field-Level Encryption

```csharp
public class User : AggregateRoot<Guid>
{
    public string UserName { get; set; }

    [Encrypted]
    public string Email { get; set; }

    [Encrypted]
    public string PhoneNumber { get; set; }

    [Encrypted]
    public string SensitiveData { get; set; }
}

// Encryption is handled automatically by interceptors
public class UserService : ApplicationService
{
    private readonly IRepository<User> _userRepository;

    public async Task<UserDto> CreateAsync(CreateUserDto input)
    {
        // Email, PhoneNumber, and SensitiveData are automatically encrypted
        var user = new User
        {
            UserName = input.UserName,
            Email = input.Email,
            PhoneNumber = input.PhoneNumber,
            SensitiveData = input.SensitiveData
        };

        await _userRepository.InsertAsync(user);

        return ObjectMapper.Map<User, UserDto>(user);
    }

    public async Task<UserDto> GetAsync(Guid id)
    {
        // Encrypted fields are automatically decrypted
        var user = await _userRepository.GetAsync(id);
        return ObjectMapper.Map<User, UserDto>(user);
    }
}
```

#### SM2 Encryption

```csharp
public class SM2EncryptionService : ITransientDependency
{
    private readonly ISM2CryptoService _sm2Service;

    public SM2EncryptionService(ISM2CryptoService sm2Service)
    {
        _sm2Service = sm2Service;
    }

    // Generate SM2 key pair
    public (string PublicKey, string PrivateKey) GenerateKeyPair()
    {
        return _sm2Service.GenerateKeyPair();
    }

    // Encrypt with public key
    public string Encrypt(string plainText, string publicKey)
    {
        return _sm2Service.Encrypt(plainText, publicKey);
    }

    // Decrypt with private key
    public string Decrypt(string cipherText, string privateKey)
    {
        return _sm2Service.Decrypt(cipherText, privateKey);
    }

    // Sign data
    public string Sign(string data, string privateKey)
    {
        return _sm2Service.Sign(data, privateKey);
    }

    // Verify signature
    public bool Verify(string data, string signature, string publicKey)
    {
        return _sm2Service.Verify(data, signature, publicKey);
    }
}
```

#### Custom Encryption Strategy

```csharp
public class CustomEncryptionStrategy : IEncryptionStrategy
{
    public string Encrypt(string plainText, EncryptionOptions options)
    {
        // Custom encryption logic
        // Could use different algorithms based on data type
        if (options.DataType == "CreditCard")
        {
            return EncryptCreditCard(plainText);
        }
        else if (options.DataType == "SSN")
        {
            return EncryptSSN(plainText);
        }

        return DefaultEncrypt(plainText);
    }

    public string Decrypt(string cipherText, EncryptionOptions options)
    {
        // Custom decryption logic
        if (options.DataType == "CreditCard")
        {
            return DecryptCreditCard(cipherText);
        }
        else if (options.DataType == "SSN")
        {
            return DecryptSSN(cipherText);
        }

        return DefaultDecrypt(cipherText);
    }

    private string EncryptCreditCard(string cardNumber)
    {
        // Implement PCI-DSS compliant encryption
        return cardNumber;
    }

    private string EncryptSSN(string ssn)
    {
        // Implement SSN-specific encryption
        return ssn;
    }
}

// Register custom strategy
public override void ConfigureServices(ServiceConfigurationContext context)
{
    context.Services.AddTransient<IEncryptionStrategy, CustomEncryptionStrategy>();
}
```

---

## Best Practices

### 1. Key Management

Store encryption keys securely:

```csharp
// Use Azure Key Vault, AWS KMS, or similar
public class SecureKeyProvider : ITransientDependency
{
    private readonly IConfiguration _configuration;

    public string GetEncryptionKey()
    {
        // DO NOT hardcode keys
        // Use environment variables or key vault
        return Environment.GetEnvironmentVariable("ENCRYPTION_KEY")
            ?? _configuration["Encryption:Key"];
    }
}
```

### 2. Rotate Keys Periodically

```csharp
public class KeyRotationService : ITransientDependency
{
    private readonly ITransformSecurityService _securityService;
    private readonly IRepository<EncryptedData> _dataRepository;

    public async Task RotateKeysAsync(string newKey)
    {
        var oldKey = _securityService.CurrentKey;

        // Re-encrypt all data with new key
        var allData = await _dataRepository.GetListAsync();

        foreach (var data in allData)
        {
            // Decrypt with old key
            var plainText = _securityService.Decrypt(data.EncryptedValue, oldKey);

            // Encrypt with new key
            data.EncryptedValue = _securityService.Encrypt(plainText, newKey);

            await _dataRepository.UpdateAsync(data);
        }

        // Update current key
        _securityService.UpdateKey(newKey);
    }
}
```

### 3. Encrypt Sensitive Data Only

Don't encrypt everything - balance security with performance:

```csharp
public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; }  // Not encrypted - searchable

    [Encrypted]
    public string Email { get; set; }  // Encrypted - PII

    [Encrypted]
    public string CreditCardNumber { get; set; }  // Encrypted - very sensitive

    [Encrypted]
    public string SSN { get; set; }  // Encrypted - very sensitive

    public DateTime CreatedTime { get; set; }  // Not encrypted
}
```

### 4. Audit Encryption Operations

```csharp
public class AuditedEncryptionService : ITransientDependency
{
    private readonly ITransformSecurityService _securityService;
    private readonly IAuditLogger _auditLogger;

    public async Task<string> EncryptAsync(string plainText, string dataType)
    {
        var encrypted = _securityService.Encrypt(plainText);

        await _auditLogger.LogAsync(new AuditLog
        {
            Action = "Encrypt",
            DataType = dataType,
            Timestamp = DateTime.UtcNow
        });

        return encrypted;
    }

    public async Task<string> DecryptAsync(string cipherText, string dataType)
    {
        var decrypted = _securityService.Decrypt(cipherText);

        await _auditLogger.LogAsync(new AuditLog
        {
            Action = "Decrypt",
            DataType = dataType,
            Timestamp = DateTime.UtcNow
        });

        return decrypted;
    }
}
```

### 5. Handle Encryption Errors Gracefully

```csharp
public class SafeEncryptionService : ITransientDependency
{
    private readonly ITransformSecurityService _securityService;
    private readonly ILogger<SafeEncryptionService> _logger;

    public string SafeEncrypt(string plainText)
    {
        try
        {
            return _securityService.Encrypt(plainText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Encryption failed");
            throw new UserFriendlyException("Failed to encrypt data");
        }
    }

    public string SafeDecrypt(string cipherText)
    {
        try
        {
            return _securityService.Decrypt(cipherText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Decryption failed");
            // Return masked value or throw
            return "***DECRYPTION_FAILED***";
        }
    }
}
```

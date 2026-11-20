using System;
using Volo.Abp.Caching;
using Volo.Abp.Security.Encryption;
using Xunit;

namespace SharpAbp.Abp.FileStoring.Aliyun;

public class AliyunClientFactoryTest : AbpFileStoringAllTestBase
{
    [Fact]
    public void Cached_Sts_Credentials_Should_Be_Decrypted_And_Used_For_Signing()
    {
        var configuration = CreateConfiguration();
        configuration.UseSecurityTokenService = true;
        configuration.RoleArn = "test-role";
        configuration.RoleSessionName = "test-session";
        configuration.TemporaryCredentialsCacheKey = Guid.NewGuid().ToString("N");
        var encryption = GetRequiredService<IStringEncryptionService>();
        GetRequiredService<IDistributedCache<AliyunTemporaryCredentialsCacheItem>>().Set(configuration.TemporaryCredentialsCacheKey,
            new AliyunTemporaryCredentialsCacheItem(encryption.Encrypt("session-access"), encryption.Encrypt("session-secret"), encryption.Encrypt("session-token")));

        var client = GetRequiredService<IOssClientFactory>().Create(configuration);
        var signed = Uri.UnescapeDataString(client.GeneratePresignedUri("bucket", "file", DateTime.UtcNow.AddMinutes(5)).AbsoluteUri);

        Assert.Contains("OSSAccessKeyId=session-access", signed);
        Assert.Contains("security-token=session-token", signed);
        Assert.DoesNotContain("session-secret", signed);
    }

    [Fact]
    public void Static_Credentials_Should_Sign_For_Configured_Endpoint()
    {
        var client = GetRequiredService<IOssClientFactory>().Create(CreateConfiguration());

        var signed = client.GeneratePresignedUri("bucket", "file", DateTime.UtcNow.AddMinutes(5));

        Assert.Equal("bucket.oss.example.invalid", signed.Host);
        Assert.Equal("/file", signed.AbsolutePath);
        Assert.Contains("OSSAccessKeyId=test-access", signed.Query);
    }

    [Fact]
    public void Missing_Sts_Role_Should_Fail_Before_Requesting_Token()
    {
        var configuration = CreateConfiguration();
        configuration.UseSecurityTokenService = true;
        configuration.RoleArn = "";
        Assert.ThrowsAny<ArgumentException>(() => GetRequiredService<IOssClientFactory>().Create(configuration));
    }

    private static AliyunFileProviderConfiguration CreateConfiguration()
    {
        var configuration = new FileContainerConfiguration().GetAliyunConfiguration();
        configuration.Endpoint = "https://oss.example.invalid";
        configuration.AccessKeyId = "test-access";
        configuration.AccessKeySecret = "test-secret";
        return configuration;
    }
}

using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Security.Encryption;
using Xunit;

namespace SharpAbp.Abp.FileStoring.Aws;

public class AwsClientFactoryTest : AbpFileStoringAllTestBase
{
    [Fact]
    public async Task Static_Credentials_Should_Configure_Region_Connections_And_Signing()
    {
        var configuration = CreateConfiguration();
        configuration.MaxConnectionsPerServer = 7;
        using var client = await GetRequiredService<IAmazonS3ClientFactory>().GetAmazonS3Client(configuration);

        var signed = Uri.UnescapeDataString(client.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = "bucket",
            Key = "file",
            Expires = DateTime.UtcNow.AddMinutes(5)
        }));

        Assert.Equal("eu-west-1", client.Config.RegionEndpoint.SystemName);
        Assert.Equal(7, ((Amazon.S3.AmazonS3Config)client.Config).MaxConnectionsPerServer);
        Assert.Contains("test-access", signed);
        Assert.Contains("/eu-west-1/s3/aws4_request", signed);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Temporary_And_Federated_Credentials_Should_Be_Decrypted_From_Cache_For_Signing(bool federated)
    {
        var configuration = CreateConfiguration();
        configuration.UseTemporaryCredentials = !federated;
        configuration.UseTemporaryFederatedCredentials = federated;
        configuration.Name = "session";
        configuration.Policy = "{}";
        configuration.TemporaryCredentialsCacheKey = Guid.NewGuid().ToString("N");
        var encryption = GetRequiredService<IStringEncryptionService>();
        await GetRequiredService<IDistributedCache<AwsTemporaryCredentialsCacheItem>>().SetAsync(configuration.TemporaryCredentialsCacheKey,
            new AwsTemporaryCredentialsCacheItem(encryption.Encrypt("session-access"), encryption.Encrypt("session-secret"), encryption.Encrypt("session-token")));

        using var client = await GetRequiredService<IAmazonS3ClientFactory>().GetAmazonS3Client(configuration);
        var signed = Uri.UnescapeDataString(client.GetPreSignedURL(new GetPreSignedUrlRequest { BucketName = "bucket", Key = "file", Expires = DateTime.UtcNow.AddMinutes(5) }));

        Assert.Contains("session-access", signed);
        Assert.Contains("X-Amz-Security-Token=session-token", signed);
        Assert.DoesNotContain("session-secret", signed);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Static_Client_Should_Reject_Empty_Access_Key(string accessKey)
    {
        var configuration = CreateConfiguration();
        configuration.AccessKeyId = accessKey;
        await Assert.ThrowsAnyAsync<ArgumentException>(() => GetRequiredService<IAmazonS3ClientFactory>().GetAmazonS3Client(configuration));
    }

    private static AwsFileProviderConfiguration CreateConfiguration()
    {
        var configuration = new FileContainerConfiguration().GetAwsConfiguration();
        configuration.Region = "eu-west-1";
        configuration.AccessKeyId = "test-access";
        configuration.SecretAccessKey = "test-secret";
        return configuration;
    }
}

using Aliyun.OSS;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SharpAbp.Abp.FileStoring.Aliyun;
using SharpAbp.Abp.FileStoring.Aws;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;
using Xunit;

namespace SharpAbp.Abp.FileStoring;

public class CloudClientCachingTest : AbpFileStoringAllTestBase
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task Aws_Should_Cache_Static_Credentials_And_Dispose_Temporary_Client_Per_Lease(bool temporary, bool federated)
    {
        var created = 0;
        var disposed = 0;
        var factory = new Mock<IAmazonS3ClientFactory>(MockBehavior.Strict);
        factory.Setup(x => x.GetAmazonS3Client(It.IsAny<AwsFileProviderConfiguration>())).ReturnsAsync(() =>
        {
            created++;
            var client = new Mock<IAmazonS3>(MockBehavior.Strict);
            client.Setup(x => x.Dispose()).Callback(() => disposed++);
            return client.Object;
        });
        using var scopes = new ServiceCollection().AddSingleton(factory.Object).BuildServiceProvider();
        using var cache = new FileProviderClientCache<IAmazonS3>();
        var provider = new AwsProbe(ServiceProvider, scopes.GetRequiredService<IServiceScopeFactory>(), cache);
        var configuration = new FileContainerConfiguration().GetAwsConfiguration();
        configuration.Region = "eu-west-1";
        configuration.AccessKeyId = "test-access";
        configuration.SecretAccessKey = "test-secret";
        configuration.ProfilesLocation = "";
        configuration.UseTemporaryCredentials = temporary;
        configuration.UseTemporaryFederatedCredentials = federated;

        var first = await provider.AcquireAndReturnAsync(configuration);
        var second = await provider.AcquireAndReturnAsync(configuration);

        if (temporary || federated)
        {
            Assert.NotSame(first, second);
            Assert.Equal(2, created);
            Assert.Equal(2, disposed);
        }
        else
        {
            Assert.Same(first, second);
            Assert.Equal(1, created);
            Assert.Equal(0, disposed);
            configuration.SecretAccessKey = "rotated-secret";
            Assert.NotSame(first, await provider.AcquireAndReturnAsync(configuration));
            Assert.Equal(2, created);
            Assert.Equal(0, disposed);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Aliyun_Should_Cache_Static_Credentials_And_Dispose_Sts_Client_Per_Lease(bool temporary)
    {
        var created = 0;
        var disposed = 0;
        var factory = new Mock<IOssClientFactory>(MockBehavior.Strict);
        factory.Setup(x => x.Create(It.IsAny<AliyunFileProviderConfiguration>())).Returns(() =>
        {
            created++;
            var client = new Mock<IOss>(MockBehavior.Strict);
            client.As<IDisposable>().Setup(x => x.Dispose()).Callback(() => disposed++);
            return client.Object;
        });
        using var scopes = new ServiceCollection().AddSingleton(factory.Object).BuildServiceProvider();
        using var cache = new FileProviderClientCache<IOss>();
        var provider = new AliyunProbe(ServiceProvider, scopes.GetRequiredService<IServiceScopeFactory>(), cache);
        var configuration = new FileContainerConfiguration().GetAliyunConfiguration();
        configuration.Endpoint = "https://oss.example.invalid";
        configuration.AccessKeyId = "test-access";
        configuration.AccessKeySecret = "test-secret";
        configuration.UseSecurityTokenService = temporary;

        var first = provider.AcquireAndReturn(configuration);
        var second = provider.AcquireAndReturn(configuration);

        if (temporary)
        {
            Assert.NotSame(first, second);
            Assert.Equal(2, created);
            Assert.Equal(2, disposed);
        }
        else
        {
            Assert.Same(first, second);
            Assert.Equal(1, created);
            Assert.Equal(0, disposed);
            configuration.AccessKeySecret = "rotated-secret";
            Assert.NotSame(first, provider.AcquireAndReturn(configuration));
            Assert.Equal(2, created);
            Assert.Equal(0, disposed);
        }
    }

    [DisableConventionalRegistration]
    private sealed class AwsProbe(IServiceProvider services, IServiceScopeFactory scopes, FileProviderClientCache<IAmazonS3> cache) : AwsFileProvider(
        services.GetRequiredService<ILogger<AwsFileProvider>>(), scopes, services.GetRequiredService<IClock>(), cache,
        services.GetRequiredService<IAwsFileNameCalculator>(), services.GetRequiredService<IFileNormalizeNamingService>())
    {
        public async Task<IAmazonS3> AcquireAndReturnAsync(AwsFileProviderConfiguration configuration)
        {
            var lease = await GetAmazonS3ClientAsync(configuration);
            lease.Return();
            return lease.Client;
        }
    }

    [DisableConventionalRegistration]
    private sealed class AliyunProbe(IServiceProvider services, IServiceScopeFactory scopes, FileProviderClientCache<IOss> cache) : AliyunFileProvider(
        services.GetRequiredService<ILogger<AliyunFileProvider>>(), scopes, services.GetRequiredService<IClock>(), cache,
        services.GetRequiredService<IAliyunFileNameCalculator>(), services.GetRequiredService<IFileNormalizeNamingService>())
    {
        public IOss AcquireAndReturn(AliyunFileProviderConfiguration configuration)
        {
            var lease = GetOssClient(configuration);
            lease.Return();
            return lease.Client;
        }
    }
}

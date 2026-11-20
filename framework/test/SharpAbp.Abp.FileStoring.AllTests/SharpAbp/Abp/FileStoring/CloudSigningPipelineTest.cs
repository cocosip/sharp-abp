using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.FileStoring.KS3;
using SharpAbp.Abp.FileStoring.Obs;
using SharpAbp.Abp.FileStoring.S3;
using System;
using System.Threading.Tasks;
using Volo.Abp.Timing;
using Xunit;

namespace SharpAbp.Abp.FileStoring;

public class CloudSigningPipelineTest : AbpFileStoringAllTestBase
{
    [Theory]
    [InlineData("KS3")]
    [InlineData("Obs")]
    [InlineData("S3")]
    public async Task Production_Provider_Should_Use_Cached_Factory_Client_And_Configured_Endpoint_For_Signing(string providerName)
    {
        GetRequiredService<IOptions<AbpFileStoringAbstractionsOptions>>().Value.FilePathStrategy = FilePathGenerationStrategy.DirectFileId;
        var configuration = new FileContainerConfiguration();
        IFileProvider provider;
        switch (providerName)
        {
            case "KS3":
                configuration.UseKS3(options =>
                {
                    options.BucketName = "bucket";
                    // shortcut: SDK 1.0.0 signs only its default host; custom endpoints need an SDK fix.
                    options.Endpoint = "ks3-cn-beijing.ksyun.com";
                    options.Protocol = "http";
                    options.AccessKey = "test-access";
                    options.SecretKey = "test-secret";
                });
                provider = ActivatorUtilities.CreateInstance<KS3FileProvider>(ServiceProvider);
                break;
            case "Obs":
                configuration.UseObs(options =>
                {
                    options.BucketName = "bucket";
                    options.Endpoint = "https://objects.example.invalid";
                    options.AccessKeyId = "test-access";
                    options.AccessKeySecret = "test-secret";
                });
                provider = ActivatorUtilities.CreateInstance<ObsFileProvider>(ServiceProvider);
                break;
            case "S3":
                configuration.UseS3(options =>
                {
                    options.BucketName = "bucket";
                    options.ServerUrl = "https://objects.example.invalid";
                    options.ForcePathStyle = true;
                    options.AuthenticationRegion = "eu-west-1";
                    options.AccessKeyId = "test-access";
                    options.SecretAccessKey = "test-secret";
                    options.MaxConnectionsPerServer = 4;
                });
                provider = ActivatorUtilities.CreateInstance<S3FileProvider>(ServiceProvider);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(providerName));
        }
        var args = new FileProviderAccessArgs("fallback", configuration, "file.bin", GetRequiredService<IClock>().Now.AddHours(1));

        var first = new Uri(await provider.GetAccessUrlAsync(args));
        var second = new Uri(await provider.GetAccessUrlAsync(args));

        Assert.Contains(providerName == "KS3" ? "ks3-cn-beijing.ksyun.com" : "objects.example.invalid", first.Host);
        Assert.EndsWith("/file.bin", first.AbsolutePath);
        Assert.Contains("test-access", Uri.UnescapeDataString(first.Query));
        Assert.DoesNotContain("test-secret", first.AbsoluteUri);
        Assert.Equal(first.Host, second.Host);
        Assert.Equal(first.AbsolutePath, second.AbsolutePath);
    }
}

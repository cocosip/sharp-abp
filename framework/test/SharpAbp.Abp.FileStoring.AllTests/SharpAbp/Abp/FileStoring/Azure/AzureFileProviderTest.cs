using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;
using Xunit;

namespace SharpAbp.Abp.FileStoring.Azure;

public class AzureFileProviderTest : AbpFileStoringAllTestBase
{
    private readonly FileContainerConfiguration _configuration = new();

    public AzureFileProviderTest()
    {
        _configuration.UseAzure(configuration => configuration.ContainerName = "bucket");
        GetRequiredService<IOptions<AbpFileStoringAbstractionsOptions>>().Value.FilePathStrategy = FilePathGenerationStrategy.DirectFileId;
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Save_Should_Respect_Overwrite_And_Preserve_Caller_Stream(bool overwrite)
    {
        var (provider, blob) = CreateProvider(overwrite);
        Stream uploaded = null;
        using var cancellation = new CancellationTokenSource();
        blob.Setup(x => x.UploadAsync(It.IsAny<Stream>(), overwrite, cancellation.Token))
            .Callback<Stream, bool, CancellationToken>((stream, _, _) => uploaded = stream)
            .ReturnsAsync(Response.FromValue(BlobsModelFactory.BlobContentInfo(new ETag("etag"), DateTimeOffset.UtcNow, null, null, 0), Mock.Of<Response>()));
        using var content = new MemoryStream([1, 2, 3]);

        var result = await provider.SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", content, "bin", overwrite, cancellation.Token));

        Assert.Equal("file", result);
        Assert.Same(content, uploaded);
        Assert.True(content.CanRead);
    }

    [Fact]
    public async Task Save_Should_Reject_Duplicate_Without_Overwrite()
    {
        var (provider, _) = CreateProvider(true);
        using var content = new MemoryStream([1]);
        await Assert.ThrowsAsync<FileAlreadyExistsException>(() => provider
            .SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", content, "bin")));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Exists_Delete_And_Get_Should_Return_Expected_Missing_Or_Content_Result(bool exists)
    {
        var (provider, blob) = CreateProvider(exists);
        blob.Setup(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));
        blob.Setup(x => x.DownloadAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Response.FromValue(BlobsModelFactory.BlobDownloadInfo(content: new MemoryStream([3, 1, 4])), Mock.Of<Response>()));

        Assert.Equal(exists, await provider.ExistsAsync(new FileProviderExistsArgs("bucket", _configuration, "file")));
        Assert.Equal(exists, await provider.DeleteAsync(new FileProviderDeleteArgs("bucket", _configuration, "file")));
        using var result = await provider.GetOrNullAsync(new FileProviderGetArgs("bucket", _configuration, "file"));
        if (exists)
        {
            Assert.Equal(new byte[] { 3, 1, 4 }, ((MemoryStream)result).ToArray());
            Assert.Equal(0, result.Position);
        }
        else
        {
            Assert.Null(result);
            Assert.False(await provider.DownloadAsync(new FileProviderDownloadArgs("bucket", _configuration, "file", "unused")));
        }
    }

    [Fact]
    public async Task Access_Url_Should_Respect_Public_Access_Existence_And_Read_Only_Sas()
    {
        var (provider, blob) = CreateProvider(true);
        blob.SetupGet(x => x.Uri).Returns(new Uri("https://example.invalid/bucket/file"));
        BlobSasBuilder signed = null;
        blob.Setup(x => x.GenerateSasUri(It.IsAny<BlobSasBuilder>()))
            .Callback<BlobSasBuilder>(builder => signed = builder)
            .Returns(new Uri("https://example.invalid/bucket/file?sig=test"));
        Assert.Equal("https://example.invalid/bucket/file", await provider.GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file")));
        var expires = GetRequiredService<IClock>().Now.AddHours(1);
        Assert.Equal("https://example.invalid/bucket/file?sig=test", await provider.GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file", expires, true)));
        Assert.Equal("bucket", signed.BlobContainerName);
        Assert.Equal("file", signed.BlobName);
        Assert.Equal("b", signed.Resource);
        Assert.Equal("r", signed.Permissions);
        Assert.Equal(new DateTimeOffset(expires), signed.ExpiresOn);
        var (missing, _) = CreateProvider(false);
        Assert.Equal("", await missing.GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file", checkFileExist: true)));
        _configuration.HttpAccess = false;
        Assert.Equal("", await provider.GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file")));
    }

    [Fact]
    public async Task Missing_Container_Is_Created_When_Configured_And_Download_Streams_To_Destination()
    {
        var container = new Mock<BlobContainerClient>(MockBehavior.Strict);
        var blob = new Mock<BlobClient>(MockBehavior.Strict);
        container.Setup(client => client.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(false, Mock.Of<Response>()));
        container.Setup(client => client.CreateIfNotExistsAsync(It.IsAny<PublicAccessType>(), It.IsAny<System.Collections.Generic.IDictionary<string, string>>(),
            It.IsAny<BlobContainerEncryptionScopeOptions>(), It.IsAny<CancellationToken>())).ReturnsAsync((Response<BlobContainerInfo>)null);
        container.Setup(client => client.GetBlobClient("file")).Returns(blob.Object);
        blob.Setup(client => client.UploadAsync(It.IsAny<Stream>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(BlobsModelFactory.BlobContentInfo(new ETag("etag"), DateTimeOffset.UtcNow, null, null, 0), Mock.Of<Response>()));
        var provider = new TestAzureProvider(ServiceProvider, container.Object);
        _configuration.GetAzureConfiguration().CreateContainerIfNotExists = true;
        using var stream = new MemoryStream([1]);
        Assert.Equal("file", await provider.SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", stream, "bin")));
        container.Verify(client => client.CreateIfNotExistsAsync(It.IsAny<PublicAccessType>(), It.IsAny<System.Collections.Generic.IDictionary<string, string>>(),
            It.IsAny<BlobContainerEncryptionScopeOptions>(), It.IsAny<CancellationToken>()), Times.Once);

        container.Setup(client => client.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));
        blob.Setup(client => client.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));
        var response = new Mock<Response>();
        response.SetupGet(value => value.IsError).Returns(false);
        using var cancellation = new CancellationTokenSource();
        var destination = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        blob.Setup(client => client.DownloadToAsync(It.IsAny<Stream>(), cancellation.Token))
            .Returns<Stream, CancellationToken>(async (output, token) =>
            {
                await output.WriteAsync(new byte[] { 1, 2, 3 }, token);
                return response.Object;
            });
        try
        {
            Assert.True(await provider.DownloadAsync(new FileProviderDownloadArgs("bucket", _configuration, "file", destination, cancellation.Token)));
            Assert.Equal(new byte[] { 1, 2, 3 }, await File.ReadAllBytesAsync(destination));
        }
        finally
        {
            File.Delete(destination);
        }
    }

    [Fact]
    public async Task Sdk_Read_Stream_Is_Disposed_After_Copy_And_Past_Expiry_Uses_A_Short_Read_Only_Sas()
    {
        var (provider, blob) = CreateProvider(true);
        var source = new MemoryStream([1, 2, 3]);
        blob.Setup(client => client.DownloadAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(BlobsModelFactory.BlobDownloadInfo(content: source), Mock.Of<Response>()));
        using var content = await provider.GetOrNullAsync(new FileProviderGetArgs("bucket", _configuration, "file"));
        Assert.False(source.CanRead);
        Assert.True(content.CanRead);
        var now = GetRequiredService<IClock>().Now;
        BlobSasBuilder signed = null;
        blob.Setup(client => client.GenerateSasUri(It.IsAny<BlobSasBuilder>()))
            .Callback<BlobSasBuilder>(builder => signed = builder).Returns(new Uri("https://storage.test/file?sig=test"));
        await provider.GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file", now.AddMinutes(-1)));
        Assert.InRange(signed.ExpiresOn, new DateTimeOffset(now.AddMinutes(9)), new DateTimeOffset(now.AddMinutes(11)));
        Assert.Equal("r", signed.Permissions);
    }

    [Fact]
    public async Task Container_Client_Cache_Uses_The_Connection_Identity_And_Normalizes_The_Configured_Name()
    {
        var cache = new Mock<FileProviderClientCache<BlobServiceClient>>();
        var service = new Mock<BlobServiceClient>(MockBehavior.Strict);
        var container = new Mock<BlobContainerClient>(MockBehavior.Strict);
        var blob = new Mock<BlobClient>(MockBehavior.Strict);
        var keys = new System.Collections.Generic.List<string>();
        cache.Setup(value => value.GetOrAdd(It.IsAny<string>(), It.IsAny<Func<BlobServiceClient>>()))
            .Callback<string, Func<BlobServiceClient>>((key, _) => keys.Add(key)).Returns(service.Object);
        service.Setup(value => value.GetBlobContainerClient("bucket")).Returns(container.Object);
        container.Setup(value => value.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));
        container.Setup(value => value.GetBlobClient("file")).Returns(blob.Object);
        blob.Setup(value => value.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));
        var provider = new AzureFileProvider(GetRequiredService<ILogger<AzureFileProvider>>(), cache.Object,
            GetRequiredService<IAzureFileNameCalculator>(), GetRequiredService<IFileNormalizeNamingService>(), GetRequiredService<IClock>());
        var configuration = _configuration.GetAzureConfiguration();
        configuration.ConnectionString = "UseDevelopmentStorage=true";
        configuration.ContainerName = "Bucket!";
        Assert.True(await provider.ExistsAsync(new FileProviderExistsArgs("fallback", _configuration, "file")));
        var initialKey = keys[0];
        configuration.ConnectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://127.0.0.1";
        Assert.True(await provider.ExistsAsync(new FileProviderExistsArgs("fallback", _configuration, "file")));
        Assert.DoesNotContain(initialKey, keys.GetRange(2, keys.Count - 2));
        Assert.DoesNotContain("UseDevelopmentStorage", initialKey);
        _configuration.SetConfiguration(AzureFileProviderConfigurationNames.ContainerName, "");
        Assert.True(await provider.ExistsAsync(new FileProviderExistsArgs("bucket", _configuration, "file")));
    }

    private (AzureFileProvider Provider, Mock<BlobClient> Blob) CreateProvider(bool exists)
    {
        var container = new Mock<BlobContainerClient>(MockBehavior.Strict);
        var blob = new Mock<BlobClient>(MockBehavior.Strict);
        container.Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));
        container.Setup(x => x.GetBlobClient("file")).Returns(blob.Object);
        blob.Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(exists, Mock.Of<Response>()));
        return (new TestAzureProvider(ServiceProvider, container.Object), blob);
    }

    [DisableConventionalRegistration]
    private sealed class TestAzureProvider(IServiceProvider services, BlobContainerClient container) : AzureFileProvider(
        services.GetRequiredService<ILogger<AzureFileProvider>>(), services.GetRequiredService<FileProviderClientCache<BlobServiceClient>>(),
        services.GetRequiredService<IAzureFileNameCalculator>(), services.GetRequiredService<IFileNormalizeNamingService>(), services.GetRequiredService<IClock>())
    {
        protected override BlobContainerClient GetBlobContainerClient(FileProviderArgs args) => container;
    }
}

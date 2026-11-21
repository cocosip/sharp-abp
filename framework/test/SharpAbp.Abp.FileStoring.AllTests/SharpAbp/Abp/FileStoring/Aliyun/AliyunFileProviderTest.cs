using Aliyun.OSS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;
using Xunit;

namespace SharpAbp.Abp.FileStoring.Aliyun;

public class AliyunFileProviderTest : AbpFileStoringAllTestBase
{
    private readonly FileContainerConfiguration _configuration = new();

    public AliyunFileProviderTest()
    {
        _configuration.UseAliyun(configuration => configuration.BucketName = "bucket");
        GetRequiredService<IOptions<AbpFileStoringAbstractionsOptions>>().Value.FilePathStrategy = FilePathGenerationStrategy.DirectFileId;
    }

    [Fact]
    public async Task Save_Should_Forward_Content_Through_Apm_Client_Boundary()
    {
        var client = CreateClient(false);
        PutObjectRequest sent = null;
        client.Setup(x => x.BeginPutObject(It.IsAny<PutObjectRequest>(), It.IsAny<AsyncCallback>(), It.IsAny<object>()))
            .Returns<PutObjectRequest, AsyncCallback, object>((request, callback, state) =>
            {
                sent = request;
                return Complete(CreateSdkResult<PutObjectResult>(), callback, state);
            });
        client.Setup(x => x.EndPutObject(It.IsAny<IAsyncResult>())).Returns<IAsyncResult>(result => ((Task<PutObjectResult>)result).Result);
        using var content = new MemoryStream([1, 2, 3]);

        var result = await new TestAliyunProvider(ServiceProvider, client.Object)
            .SaveAsync(new FileProviderSaveArgs("fallback", _configuration, "file", content, "bin"));

        Assert.Equal("file", result);
        Assert.Equal("bucket", sent.BucketName);
        Assert.Equal("file", sent.Key);
        Assert.Equal("true", sent.Metadata.HttpMetadata["x-oss-forbid-overwrite"]);
        Assert.Same(content, sent.Content);
        Assert.True(content.CanRead);
    }

    [Fact]
    public async Task Save_Should_Reject_Duplicate_Without_Overwrite()
    {
        using var content = new MemoryStream([1]);
        await Assert.ThrowsAsync<FileAlreadyExistsException>(() => new TestAliyunProvider(ServiceProvider, CreateClient(true).Object)
            .SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", content, "bin")));
    }

    [Theory]
    [InlineData(VersioningStatus.Enabled)]
    [InlineData(VersioningStatus.Suspended)]
    public async Task Save_Rejects_No_Overwrite_When_Versioning_Has_Been_Enabled(VersioningStatus status)
    {
        var client = CreateClient(false);
        client.Setup(x => x.GetBucketVersioning("bucket"))
            .Returns(CreateSdkResult<GetBucketVersioningResult>((nameof(GetBucketVersioningResult.Status), status)));
        using var input = new MemoryStream([1]);

        await Assert.ThrowsAsync<NotSupportedException>(() => new TestAliyunProvider(ServiceProvider, client.Object)
            .SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", input, "bin")));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task File_Operations_Should_Return_Missing_Or_Copied_Content(bool exists)
    {
        var client = CreateClient(exists);
        GetObjectRequest requested = null;
        client.Setup(x => x.BeginGetObject(It.IsAny<GetObjectRequest>(), It.IsAny<AsyncCallback>(), It.IsAny<object>()))
            .Returns<GetObjectRequest, AsyncCallback, object>((request, callback, state) =>
            {
                requested = request;
                return Complete(CreateSdkResult<OssObject>((nameof(OssObject.ResponseStream), new MemoryStream([3, 1, 4]))), callback, state);
            });
        client.Setup(x => x.EndGetObject(It.IsAny<IAsyncResult>())).Returns<IAsyncResult>(result => ((Task<OssObject>)result).Result);
        var deleted = false;
        client.Setup(x => x.DeleteObject("bucket", "file")).Callback(() => deleted = true).Returns(CreateSdkResult<DeleteObjectResult>());
        var provider = new TestAliyunProvider(ServiceProvider, client.Object);
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        try
        {
            Assert.Equal(exists, await provider.ExistsAsync(new FileProviderExistsArgs("bucket", _configuration, "file")));
            using var result = await provider.GetOrNullAsync(new FileProviderGetArgs("bucket", _configuration, "file"));
            Assert.Equal(exists, await provider.DownloadAsync(new FileProviderDownloadArgs("bucket", _configuration, "file", path)));
            Assert.Equal(exists, await provider.DeleteAsync(new FileProviderDeleteArgs("bucket", _configuration, "file")));
            Assert.Equal(exists, deleted);
            if (exists)
            {
                Assert.Equal("bucket", requested.BucketName);
                Assert.Equal("file", requested.Key);
                Assert.Equal(new byte[] { 3, 1, 4 }, ((MemoryStream)result).ToArray());
                Assert.Equal(0, result.Position);
                Assert.Equal(new byte[] { 3, 1, 4 }, await File.ReadAllBytesAsync(path));
            }
            else
            {
                Assert.Null(result);
                Assert.False(File.Exists(path));
            }
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task Access_Should_Sign_Expected_File_And_Expiration_And_Respect_Missing_File()
    {
        var client = CreateClient(true);
        var expires = GetRequiredService<IClock>().Now.AddHours(1);
        DateTime? signedExpiration = null;
        client.Setup(x => x.GeneratePresignedUri("bucket", "file", It.IsAny<DateTime>()))
            .Callback<string, string, DateTime>((_, _, value) => signedExpiration = value)
            .Returns(new Uri("https://example.invalid/file?sig=test"));
        var provider = new TestAliyunProvider(ServiceProvider, client.Object);

        Assert.Equal("https://example.invalid/file?sig=test", await provider.GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file", expires, true)));
        Assert.Equal(expires, signedExpiration);
        Assert.Equal("", await new TestAliyunProvider(ServiceProvider, CreateClient(false).Object)
            .GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file", checkFileExist: true)));
        _configuration.HttpAccess = false;
        Assert.Equal("", await provider.GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file")));
    }

    [Fact]
    public async Task Multipart_Should_Send_Remaining_Bytes_And_Ordered_ETags()
    {
        _configuration.EnableAutoMultiPartUpload = true;
        _configuration.MultiPartUploadMinFileSize = 1;
        _configuration.MultiPartUploadShardingSize = 2;
        using var input = new MemoryStream([0, 1, 2, 3, 4, 5]);
        input.Position = 1;
        var client = CreateClient(false);
        client.Setup(x => x.InitiateMultipartUpload(It.IsAny<InitiateMultipartUploadRequest>()))
            .Returns(CreateSdkResult<InitiateMultipartUploadResult>((nameof(InitiateMultipartUploadResult.UploadId), "upload")));
        var parts = new List<byte[]>();
        client.Setup(x => x.BeginUploadPart(It.IsAny<UploadPartRequest>(), It.IsAny<AsyncCallback>(), It.IsAny<object>()))
            .Returns<UploadPartRequest, AsyncCallback, object>((request, callback, state) =>
            {
                Assert.Equal("upload", request.UploadId);
                Assert.Equal(parts.Count + 1, request.PartNumber);
                using var content = new MemoryStream();
                request.InputStream.CopyTo(content);
                parts.Add(content.ToArray());
                return Complete(CreateSdkResult<UploadPartResult>((nameof(UploadPartResult.PartNumber), request.PartNumber.Value),
                    (nameof(UploadPartResult.ETag), $"etag-{request.PartNumber}")), callback, state);
            });
        client.Setup(x => x.EndUploadPart(It.IsAny<IAsyncResult>())).Returns<IAsyncResult>(result => ((Task<UploadPartResult>)result).Result);
        CompleteMultipartUploadRequest completed = null;
        client.Setup(x => x.CompleteMultipartUpload(It.IsAny<CompleteMultipartUploadRequest>()))
            .Callback<CompleteMultipartUploadRequest>(request => completed = request)
            .Returns(CreateSdkResult<CompleteMultipartUploadResult>());

        Assert.Equal("file", await new TestAliyunProvider(ServiceProvider, client.Object)
            .SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", input, "bin")));
        Assert.Equal(3, parts.Count);
        Assert.Equal(new byte[] { 1, 2 }, parts[0]);
        Assert.Equal(new byte[] { 3, 4 }, parts[1]);
        Assert.Equal(new byte[] { 5 }, parts[2]);
        Assert.Equal("true", completed.Metadata.HttpMetadata["x-oss-forbid-overwrite"]);
        Assert.Equal(new[] { "etag-1", "etag-2", "etag-3" }, completed.PartETags.Select(part => part.ETag));
    }

    private static IAsyncResult Complete<T>(T value, AsyncCallback callback, object state)
    {
        var completion = new TaskCompletionSource<T>(state);
        completion.SetResult(value);
        callback(completion.Task);
        return completion.Task;
    }

    private static T CreateSdkResult<T>(params (string Name, object Value)[] properties)
    {
        // Aliyun keeps response constructors and setters internal and provides no model factory.
        var result = (T)Activator.CreateInstance(typeof(T), nonPublic: true);
        foreach (var (name, value) in properties)
        {
            typeof(T).GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(result, value);
        }
        return result;
    }

    private static Mock<IOss> CreateClient(bool exists)
    {
        var client = new Mock<IOss>(MockBehavior.Strict);
        client.Setup(x => x.DoesBucketExist("bucket")).Returns(true);
        client.Setup(x => x.DoesObjectExist("bucket", "file")).Returns(exists);
        client.Setup(x => x.GetBucketVersioning("bucket"))
            .Returns(CreateSdkResult<GetBucketVersioningResult>((nameof(GetBucketVersioningResult.Status), VersioningStatus.Off)));
        return client;
    }

    [DisableConventionalRegistration]
    private sealed class TestAliyunProvider(IServiceProvider services, IOss client) : AliyunFileProvider(
        services.GetRequiredService<ILogger<AliyunFileProvider>>(), services.GetRequiredService<IServiceScopeFactory>(), services.GetRequiredService<IClock>(),
        services.GetRequiredService<FileProviderClientCache<IOss>>(), services.GetRequiredService<IAliyunFileNameCalculator>(), services.GetRequiredService<IFileNormalizeNamingService>())
    {
        protected override OssClientLease GetOssClient(AliyunFileProviderConfiguration configuration) => new(client, false);
    }
}

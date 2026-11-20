using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;
using Xunit;

namespace SharpAbp.Abp.FileStoring.Aws;

public class AwsFileProviderTest : AbpFileStoringAllTestBase
{
    private readonly FileContainerConfiguration _configuration = new();

    public AwsFileProviderTest()
    {
        _configuration.UseAws(configuration => configuration.ContainerName = "bucket");
        GetRequiredService<IOptions<AbpFileStoringAbstractionsOptions>>().Value.FilePathStrategy = FilePathGenerationStrategy.DirectFileId;
    }

    [Fact]
    public async Task Save_Should_Send_File_Content_And_Cancellation_To_Sdk()
    {
        using var cancellation = new CancellationTokenSource();
        using var input = new MemoryStream([1, 2, 3]);
        PutObjectRequest sent = null;
        var client = new Mock<IAmazonS3>(MockBehavior.Strict);
        client.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), cancellation.Token))
            .Callback<PutObjectRequest, CancellationToken>((request, _) => sent = request)
            .ReturnsAsync(new PutObjectResponse());
        var disposed = 0;
        client.Setup(x => x.Dispose()).Callback(() => disposed++);
        var provider = new TestAwsProvider(ServiceProvider, client.Object, false);

        var result = await provider.SaveAsync(new FileProviderSaveArgs("fallback", _configuration, "file", input, "bin", cancellationToken: cancellation.Token));

        Assert.Equal("file", result);
        Assert.Equal("bucket", sent.BucketName);
        Assert.Equal("file", sent.Key);
        Assert.Same(input, sent.InputStream);
        Assert.True(input.CanRead);
        Assert.Equal(1, disposed);
    }

    [Fact]
    public async Task Existing_File_Should_Reject_Save_And_Return_Client_Lease()
    {
        var client = new Mock<IAmazonS3>(MockBehavior.Strict);
        var disposed = 0;
        client.Setup(x => x.Dispose()).Callback(() => disposed++);
        using var input = new MemoryStream([1]);

        await Assert.ThrowsAsync<FileAlreadyExistsException>(() => new TestAwsProvider(ServiceProvider, client.Object, true)
            .SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", input, "bin")));

        Assert.Equal(1, disposed);
    }

    [Fact]
    public async Task Multipart_Save_Should_Start_At_Current_Position_And_Complete_In_Order()
    {
        _configuration.EnableAutoMultiPartUpload = true;
        _configuration.MultiPartUploadMinFileSize = 1;
        _configuration.MultiPartUploadShardingSize = 3;
        using var input = new MemoryStream([0, 1, 2, 3, 4, 5, 6, 7, 8]);
        input.Position = 2;
        var parts = new List<byte[]>();
        CompleteMultipartUploadRequest completion = null;
        var client = new Mock<IAmazonS3>(MockBehavior.Strict);
        client.Setup(x => x.InitiateMultipartUploadAsync(It.IsAny<InitiateMultipartUploadRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InitiateMultipartUploadResponse { UploadId = "upload" });
        client.Setup(x => x.UploadPartAsync(It.IsAny<UploadPartRequest>(), It.IsAny<CancellationToken>()))
            .Returns<UploadPartRequest, CancellationToken>((request, _) =>
            {
                Assert.Equal("bucket", request.BucketName);
                Assert.Equal("file", request.Key);
                Assert.Equal("upload", request.UploadId);
                Assert.Equal(parts.Count + 1, request.PartNumber);
                using var data = new MemoryStream();
                request.InputStream.CopyTo(data);
                Assert.Equal(data.Length, request.PartSize);
                parts.Add(data.ToArray());
                return Task.FromResult(new UploadPartResponse { ETag = $"etag-{request.PartNumber}" });
            });
        client.Setup(x => x.CompleteMultipartUploadAsync(It.IsAny<CompleteMultipartUploadRequest>(), It.IsAny<CancellationToken>()))
            .Callback<CompleteMultipartUploadRequest, CancellationToken>((request, _) => completion = request)
            .ReturnsAsync(new CompleteMultipartUploadResponse { Key = "file", ETag = "complete" });
        client.Setup(x => x.Dispose());

        var result = await new TestAwsProvider(ServiceProvider, client.Object, false)
            .SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", input, "bin", overrideExisting: true));

        Assert.Equal("file", result);
        Assert.Equal(3, parts.Count);
        Assert.Equal(new byte[] { 2, 3, 4 }, parts[0]);
        Assert.Equal(new byte[] { 5, 6, 7 }, parts[1]);
        Assert.Equal(new byte[] { 8 }, parts[2]);
        Assert.Equal("upload", completion.UploadId);
        Assert.Equal(new int?[] { 1, 2, 3 }, completion.PartETags.Select(x => x.PartNumber));
        Assert.Equal(new[] { "etag-1", "etag-2", "etag-3" }, completion.PartETags.Select(x => x.ETag));
    }

    [Fact]
    public async Task Failed_Multipart_Should_Abort_And_Preserve_Original_Error_When_Abort_Fails()
    {
        _configuration.EnableAutoMultiPartUpload = true;
        _configuration.MultiPartUploadMinFileSize = 1;
        _configuration.MultiPartUploadShardingSize = 3;
        using var input = new MemoryStream([1, 2, 3, 4]);
        var failure = new IOException("Upload failed");
        AbortMultipartUploadRequest aborted = null;
        var client = new Mock<IAmazonS3>(MockBehavior.Strict);
        client.Setup(x => x.InitiateMultipartUploadAsync(It.IsAny<InitiateMultipartUploadRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InitiateMultipartUploadResponse { UploadId = "upload" });
        client.Setup(x => x.UploadPartAsync(It.IsAny<UploadPartRequest>(), It.IsAny<CancellationToken>())).ThrowsAsync(failure);
        client.Setup(x => x.AbortMultipartUploadAsync(It.IsAny<AbortMultipartUploadRequest>(), It.IsAny<CancellationToken>()))
            .Callback<AbortMultipartUploadRequest, CancellationToken>((request, _) => aborted = request)
            .ThrowsAsync(new IOException("Abort failed"));
        client.Setup(x => x.Dispose());

        var actual = await Assert.ThrowsAsync<IOException>(() => new TestAwsProvider(ServiceProvider, client.Object, false)
            .SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", input, "bin")));

        Assert.Same(failure, actual);
        Assert.Equal("upload", aborted.UploadId);
        Assert.Equal("bucket", aborted.BucketName);
        Assert.Equal("file", aborted.Key);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Delete_And_Exists_Should_Respect_Missing_Files(bool exists)
    {
        DeleteObjectRequest deleted = null;
        var client = new Mock<IAmazonS3>(MockBehavior.Strict);
        client.Setup(x => x.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()))
            .Callback<DeleteObjectRequest, CancellationToken>((request, _) => deleted = request)
            .ReturnsAsync(new DeleteObjectResponse());
        client.Setup(x => x.Dispose());
        var provider = new TestAwsProvider(ServiceProvider, client.Object, exists);

        Assert.Equal(exists, await provider.ExistsAsync(new FileProviderExistsArgs("bucket", _configuration, "file")));
        Assert.Equal(exists, await provider.DeleteAsync(new FileProviderDeleteArgs("bucket", _configuration, "file")));
        if (exists)
        {
            Assert.Equal("bucket", deleted.BucketName);
            Assert.Equal("file", deleted.Key);
        }
        else
        {
            Assert.Null(deleted);
        }
    }

    [Fact]
    public async Task Get_And_Download_Should_Copy_Response_Before_Disposing_It()
    {
        var client = new Mock<IAmazonS3>(MockBehavior.Strict);
        client.Setup(x => x.GetObjectAsync(It.Is<GetObjectRequest>(r => r.BucketName == "bucket" && r.Key == "file"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new GetObjectResponse { ResponseStream = new MemoryStream([3, 1, 4]) });
        client.Setup(x => x.Dispose());
        var provider = new TestAwsProvider(ServiceProvider, client.Object, true);
        using var result = await provider.GetOrNullAsync(new FileProviderGetArgs("bucket", _configuration, "file"));
        Assert.Equal(new byte[] { 3, 1, 4 }, ((MemoryStream)result).ToArray());
        Assert.Equal(0, result.Position);
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        try
        {
            Assert.True(await provider.DownloadAsync(new FileProviderDownloadArgs("bucket", _configuration, "file", path)));
            Assert.Equal(new byte[] { 3, 1, 4 }, await File.ReadAllBytesAsync(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task Missing_File_Should_Return_Empty_Results_Without_Sdk_Download()
    {
        var client = new Mock<IAmazonS3>(MockBehavior.Strict);
        client.Setup(x => x.Dispose());
        var provider = new TestAwsProvider(ServiceProvider, client.Object, false);
        Assert.Null(await provider.GetOrNullAsync(new FileProviderGetArgs("bucket", _configuration, "file")));
        Assert.False(await provider.DownloadAsync(new FileProviderDownloadArgs("bucket", _configuration, "file", "unused")));
        Assert.Equal("", await provider.GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file", checkFileExist: true)));
        _configuration.HttpAccess = false;
        Assert.Equal("", await provider.GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file")));
    }

    [Fact]
    public async Task Access_Url_Should_Pass_Object_And_Explicit_Expiration_To_Signer()
    {
        var expires = new DateTime(2030, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        GetPreSignedUrlRequest signed = null;
        var client = new Mock<IAmazonS3>(MockBehavior.Strict);
        client.Setup(x => x.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>()))
            .Callback<GetPreSignedUrlRequest>(request => signed = request)
            .Returns("https://example.invalid/file");
        client.Setup(x => x.Dispose());

        var result = await new TestAwsProvider(ServiceProvider, client.Object, true)
            .GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file", expires, checkFileExist: true));

        Assert.Equal("https://example.invalid/file", result);
        Assert.Equal("bucket", signed.BucketName);
        Assert.Equal("file", signed.Key);
        Assert.Equal(expires, signed.Expires);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Exists_Should_Distinguish_Missing_Object_From_Available_Object(bool missing)
    {
        var client = new Mock<IAmazonS3>(MockBehavior.Strict);
        client.Setup(x => x.GetBucketAclAsync(It.IsAny<GetBucketAclRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetBucketAclResponse());
        var metadata = client.Setup(x => x.GetObjectMetadataAsync("bucket", "file", It.IsAny<CancellationToken>()));
        if (missing)
        {
            metadata.ThrowsAsync(new AmazonS3Exception("Missing") { StatusCode = HttpStatusCode.NotFound });
        }
        else
        {
            metadata.ReturnsAsync(new GetObjectMetadataResponse { HttpStatusCode = HttpStatusCode.OK });
        }
        client.Setup(x => x.Dispose());

        var result = await new TestAwsProvider(ServiceProvider, client.Object, null)
            .ExistsAsync(new FileProviderExistsArgs("bucket", _configuration, "file"));

        Assert.Equal(!missing, result);
    }

    [DisableConventionalRegistration]
    private sealed class TestAwsProvider(IServiceProvider services, IAmazonS3 client, bool? exists) : AwsFileProvider(
        services.GetRequiredService<ILogger<AwsFileProvider>>(), services.GetRequiredService<IServiceScopeFactory>(),
        services.GetRequiredService<IClock>(), services.GetRequiredService<FileProviderClientCache<IAmazonS3>>(),
        services.GetRequiredService<IAwsFileNameCalculator>(), services.GetRequiredService<IFileNormalizeNamingService>())
    {
        protected override Task<AmazonS3ClientLease> GetAmazonS3ClientAsync(AwsFileProviderConfiguration configuration) => Task.FromResult(new AmazonS3ClientLease(client, true));
        protected override Task<bool> FileExistsAsync(IAmazonS3 sdk, string containerName, string fileName) => exists.HasValue
            ? Task.FromResult(exists.Value)
            : base.FileExistsAsync(sdk, containerName, fileName);
    }
}

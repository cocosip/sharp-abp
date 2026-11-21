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

namespace SharpAbp.Abp.FileStoring.S3;

public class S3FileProviderTest : AbpFileStoringAllTestBase
{
    private readonly FileContainerConfiguration _configuration = new();

    public S3FileProviderTest()
    {
        _configuration.UseS3(configuration =>
        {
            configuration.BucketName = "bucket";
            configuration.UseChunkEncoding = true;
            configuration.Protocol = (int)Protocol.HTTPS;
        });
        GetRequiredService<IOptions<AbpFileStoringAbstractionsOptions>>().Value.FilePathStrategy = FilePathGenerationStrategy.DirectFileId;
    }

    [Fact]
    public async Task Single_Save_Should_Keep_Caller_Stream_Open_And_Apply_Chunk_Encoding()
    {
        var client = CreateClient(false);
        PutObjectRequest sent = null;
        client.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .Callback<PutObjectRequest, CancellationToken>((request, _) => sent = request)
            .ReturnsAsync(new PutObjectResponse());
        using var input = new MemoryStream([1, 2, 3]);

        var result = await new TestS3Provider(ServiceProvider, client.Object)
            .SaveAsync(new FileProviderSaveArgs("fallback", _configuration, "file", input, "bin"));

        Assert.Equal("file", result);
        Assert.Equal("bucket", sent.BucketName);
        Assert.Equal("file", sent.Key);
        Assert.Equal("*", sent.IfNoneMatch);
        Assert.Same(input, sent.InputStream);
        Assert.False(sent.AutoCloseStream);
        Assert.True(sent.UseChunkEncoding);
    }

    [Fact]
    public async Task Existing_File_Should_Require_Explicit_Overwrite()
    {
        using var input = new MemoryStream([1]);
        var provider = new TestS3Provider(ServiceProvider, CreateClient(true).Object);
        await Assert.ThrowsAsync<FileAlreadyExistsException>(() => provider
            .SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", input, "bin")));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task File_Operations_Should_Return_Missing_Status_Or_Stored_Content(bool exists)
    {
        var client = CreateClient(exists);
        client.Setup(x => x.GetObjectAsync("bucket", "file", It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new GetObjectResponse { ResponseStream = new MemoryStream([7, 8, 9]) });
        (string Bucket, string Key)? deleted = null;
        client.Setup(x => x.DeleteAsync("bucket", "file", null, It.IsAny<CancellationToken>()))
            .Callback<string, string, IDictionary<string, object>, CancellationToken>((bucket, key, _, _) => deleted = (bucket, key))
            .Returns(Task.CompletedTask);
        var provider = new TestS3Provider(ServiceProvider, client.Object);
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        try
        {
            Assert.Equal(exists, await provider.ExistsAsync(new FileProviderExistsArgs("bucket", _configuration, "file")));
            using var content = await provider.GetOrNullAsync(new FileProviderGetArgs("bucket", _configuration, "file"));
            Assert.Equal(exists, await provider.DownloadAsync(new FileProviderDownloadArgs("bucket", _configuration, "file", path)));
            Assert.Equal(exists, await provider.DeleteAsync(new FileProviderDeleteArgs("bucket", _configuration, "file")));
            if (exists)
            {
                Assert.Equal(new byte[] { 7, 8, 9 }, ((MemoryStream)content).ToArray());
                Assert.Equal(0, content.Position);
                Assert.Equal(new byte[] { 7, 8, 9 }, await File.ReadAllBytesAsync(path));
                Assert.Equal("bucket", deleted.Value.Bucket);
                Assert.Equal("file", deleted.Value.Key);
            }
            else
            {
                Assert.Null(content);
                Assert.Null(deleted);
                Assert.False(File.Exists(path));
            }
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task Access_Url_Should_Respect_Http_Access_Existence_And_Signing_Options()
    {
        var client = CreateClient(true);
        GetPreSignedUrlRequest signed = null;
        client.Setup(x => x.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>()))
            .Callback<GetPreSignedUrlRequest>(request => signed = request)
            .Returns("https://example.invalid/file");
        var provider = new TestS3Provider(ServiceProvider, client.Object);
        var expires = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        Assert.Equal("https://example.invalid/file", await provider.GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file", expires, true)));
        Assert.Equal("bucket", signed.BucketName);
        Assert.Equal("file", signed.Key);
        Assert.Equal(expires, signed.Expires);
        Assert.Equal(Protocol.HTTPS, signed.Protocol);
        Assert.Equal("", await new TestS3Provider(ServiceProvider, CreateClient(false).Object)
            .GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file", checkFileExist: true)));
        _configuration.HttpAccess = false;
        Assert.Equal("", await provider.GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file")));
    }

    [Fact]
    public async Task Multipart_Save_Should_Upload_Remaining_Bytes_And_Complete_With_Ordered_ETags()
    {
        _configuration.EnableAutoMultiPartUpload = true;
        _configuration.MultiPartUploadMinFileSize = 1;
        _configuration.MultiPartUploadShardingSize = 2;
        using var input = new MemoryStream([0, 1, 2, 3, 4, 5]);
        input.Position = 1;
        var parts = new List<byte[]>();
        CompleteMultipartUploadRequest completed = null;
        var client = CreateClient(false);
        client.Setup(x => x.InitiateMultipartUploadAsync("bucket", "file", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InitiateMultipartUploadResponse { UploadId = "upload" });
        client.Setup(x => x.UploadPartAsync(It.IsAny<UploadPartRequest>(), It.IsAny<CancellationToken>()))
            .Returns<UploadPartRequest, CancellationToken>((request, _) =>
            {
                Assert.Equal(parts.Count + 1, request.PartNumber);
                Assert.Equal("upload", request.UploadId);
                Assert.True(request.UseChunkEncoding);
                using var content = new MemoryStream();
                request.InputStream.CopyTo(content);
                parts.Add(content.ToArray());
                return Task.FromResult(new UploadPartResponse { ETag = $"etag-{request.PartNumber}" });
            });
        client.Setup(x => x.CompleteMultipartUploadAsync(It.IsAny<CompleteMultipartUploadRequest>(), It.IsAny<CancellationToken>()))
            .Callback<CompleteMultipartUploadRequest, CancellationToken>((request, _) => completed = request)
            .ReturnsAsync(new CompleteMultipartUploadResponse { Key = "file", ETag = "complete" });

        Assert.Equal("file", await new TestS3Provider(ServiceProvider, client.Object)
            .SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", input, "bin")));
        Assert.Equal(3, parts.Count);
        Assert.Equal(new byte[] { 1, 2 }, parts[0]);
        Assert.Equal(new byte[] { 3, 4 }, parts[1]);
        Assert.Equal(new byte[] { 5 }, parts[2]);
        Assert.Equal("upload", completed.UploadId);
        Assert.Equal("*", completed.IfNoneMatch);
        Assert.Equal(new[] { "etag-1", "etag-2", "etag-3" }, completed.PartETags.Select(x => x.ETag));
    }

    [Fact]
    public async Task Cancelled_Multipart_Should_Abort_With_An_Independent_Token()
    {
        _configuration.EnableAutoMultiPartUpload = true;
        _configuration.MultiPartUploadMinFileSize = 1;
        _configuration.MultiPartUploadShardingSize = 2;
        using var input = new MemoryStream([1, 2, 3]);
        using var cancellation = new CancellationTokenSource();
        var client = CreateClient(false);
        client.Setup(x => x.InitiateMultipartUploadAsync("bucket", "file", cancellation.Token))
            .ReturnsAsync(new InitiateMultipartUploadResponse { UploadId = "upload" });
        client.Setup(x => x.UploadPartAsync(It.IsAny<UploadPartRequest>(), cancellation.Token))
            .Returns(() =>
            {
                cancellation.Cancel();
                return Task.FromCanceled<UploadPartResponse>(cancellation.Token);
            });
        var aborted = false;
        client.Setup(x => x.AbortMultipartUploadAsync(It.IsAny<AbortMultipartUploadRequest>(), It.IsAny<CancellationToken>()))
            .Returns((AbortMultipartUploadRequest request, CancellationToken cleanupToken) =>
            {
                cleanupToken.ThrowIfCancellationRequested();
                aborted = request.UploadId == "upload";
                return Task.FromResult(new AbortMultipartUploadResponse());
            });

        var error = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => new TestS3Provider(ServiceProvider, client.Object)
            .SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", input, "bin", cancellationToken: cancellation.Token)));

        Assert.Equal(cancellation.Token, error.CancellationToken);
        Assert.True(aborted);
    }

    [Fact]
    public async Task Metadata_Transport_Error_Should_Propagate()
    {
        var client = CreateClient(true);
        var error = new IOException("Connection lost");
        client.Setup(x => x.GetObjectMetadataAsync("bucket", "file", It.IsAny<CancellationToken>())).ThrowsAsync(error);

        var actual = await Assert.ThrowsAsync<IOException>(() => new TestS3Provider(ServiceProvider, client.Object)
            .ExistsAsync(new FileProviderExistsArgs("bucket", _configuration, "file")));

        Assert.Same(error, actual);
    }

    private static Mock<IAmazonS3> CreateClient(bool exists)
    {
        var client = new Mock<IAmazonS3>(MockBehavior.Strict);
        client.Setup(x => x.GetBucketAclAsync(It.IsAny<GetBucketAclRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetBucketAclResponse());
        var metadata = client.Setup(x => x.GetObjectMetadataAsync("bucket", "file", It.IsAny<CancellationToken>()));
        if (exists)
        {
            metadata.ReturnsAsync(new GetObjectMetadataResponse { HttpStatusCode = HttpStatusCode.OK });
        }
        else
        {
            metadata.ThrowsAsync(new AmazonS3Exception("Missing") { StatusCode = HttpStatusCode.NotFound });
        }
        return client;
    }

    [DisableConventionalRegistration]
    private sealed class TestS3Provider(IServiceProvider services, IAmazonS3 client) : S3FileProvider(
        services.GetRequiredService<ILogger<S3FileProvider>>(), services.GetRequiredService<IServiceScopeFactory>(),
        services.GetRequiredService<IClock>(), services.GetRequiredService<FileProviderClientCache<IAmazonS3>>(),
        services.GetRequiredService<IS3FileNameCalculator>(), services.GetRequiredService<IFileNormalizeNamingService>())
    {
        protected override IAmazonS3 GetS3Client(S3FileProviderConfiguration configuration) => client;
    }
}

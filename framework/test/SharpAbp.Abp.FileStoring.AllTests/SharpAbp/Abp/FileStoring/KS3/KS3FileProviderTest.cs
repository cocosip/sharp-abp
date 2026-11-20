using KS3;
using KS3.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;
using Xunit;

namespace SharpAbp.Abp.FileStoring.KS3;

public class KS3FileProviderTest : AbpFileStoringAllTestBase
{
    private readonly FileContainerConfiguration _configuration = new();

    public KS3FileProviderTest()
    {
        _configuration.UseKS3(configuration => configuration.BucketName = "bucket");
        GetRequiredService<IOptions<AbpFileStoringAbstractionsOptions>>().Value.FilePathStrategy = FilePathGenerationStrategy.DirectFileId;
    }

    [Fact]
    public async Task Save_Should_Create_Missing_Bucket_And_Send_Caller_Content()
    {
        _configuration.GetKS3Configuration().CreateContainerIfNotExists = true;
        var client = CreateClient(false, false);
        CreateBucketRequest created = null;
        PutObjectRequest sent = null;
        client.Setup(x => x.CreateBucket(It.IsAny<CreateBucketRequest>())).Callback<CreateBucketRequest>(request => created = request).Returns(new Bucket());
        client.Setup(x => x.PutObject(It.IsAny<PutObjectRequest>())).Callback<PutObjectRequest>(request => sent = request).Returns(new PutObjectResult());
        using var content = new MemoryStream([1, 2, 3]);

        Assert.Equal("file", await new TestKS3Provider(ServiceProvider, client.Object)
            .SaveAsync(new FileProviderSaveArgs("fallback", _configuration, "file", content, "bin")));

        Assert.Equal("bucket", created.BucketName);
        Assert.Equal("bucket", sent.BucketName);
        Assert.Equal("file", sent.Key);
        Assert.Same(content, sent.InputStream);
        Assert.True(content.CanRead);
    }

    [Fact]
    public async Task Save_Should_Reject_Existing_File_Without_Overwrite()
    {
        using var content = new MemoryStream([1]);
        await Assert.ThrowsAsync<FileAlreadyExistsException>(() => new TestKS3Provider(ServiceProvider, CreateClient(true, true).Object)
            .SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", content, "bin")));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task File_Operations_Should_Return_Missing_Or_Copied_Content(bool exists)
    {
        var client = CreateClient(true, exists);
        GetObjectRequest requested = null;
        var responseStreams = new List<MemoryStream>();
        client.Setup(x => x.GetObject(It.IsAny<GetObjectRequest>())).Returns<GetObjectRequest>(request =>
        {
            requested = request;
            var response = new MemoryStream([3, 1, 4]);
            responseStreams.Add(response);
            return new KS3Object { ObjectContent = response };
        });
        DeleteObjectRequest deleted = null;
        client.Setup(x => x.DeleteObject(It.IsAny<DeleteObjectRequest>())).Callback<DeleteObjectRequest>(request => deleted = request);
        var provider = new TestKS3Provider(ServiceProvider, client.Object);
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        try
        {
            Assert.Equal(exists, await provider.ExistsAsync(new FileProviderExistsArgs("bucket", _configuration, "file")));
            using var content = await provider.GetOrNullAsync(new FileProviderGetArgs("bucket", _configuration, "file"));
            Assert.Equal(exists, await provider.DownloadAsync(new FileProviderDownloadArgs("bucket", _configuration, "file", path)));
            Assert.Equal(exists, await provider.DeleteAsync(new FileProviderDeleteArgs("bucket", _configuration, "file")));
            if (exists)
            {
                Assert.Equal("bucket", requested.BucketName);
                Assert.Equal("file", requested.Key);
                Assert.Equal("bucket", deleted.BucketName);
                Assert.Equal("file", deleted.Key);
                Assert.Equal(new byte[] { 3, 1, 4 }, ((MemoryStream)content).ToArray());
                Assert.Equal(0, content.Position);
                Assert.Equal(new byte[] { 3, 1, 4 }, await File.ReadAllBytesAsync(path));
                Assert.All(responseStreams, stream => Assert.False(stream.CanRead));
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
    public async Task Multipart_Should_Send_Remaining_Bytes_And_Complete_With_Ordered_Xml_Parts()
    {
        _configuration.EnableAutoMultiPartUpload = true;
        _configuration.MultiPartUploadMinFileSize = 1;
        _configuration.MultiPartUploadShardingSize = 2;
        using var input = new MemoryStream([0, 1, 2, 3, 4, 5]);
        input.Position = 1;
        var client = CreateClient(true, false);
        client.Setup(x => x.InitiateMultipartUpload(It.IsAny<InitiateMultipartUploadRequest>())).Returns(new InitiateMultipartUploadResult { UploadId = "upload" });
        var parts = new List<byte[]>();
        client.Setup(x => x.UploadPart(It.IsAny<UploadPartRequest>())).Returns<UploadPartRequest>(request =>
        {
            Assert.Equal("upload", request.UploadId);
            Assert.Equal(parts.Count + 1, request.PartNumber);
            using var content = new MemoryStream();
            request.InputStream.CopyTo(content);
            parts.Add(content.ToArray());
            return new PartETag(request.PartNumber, $"etag-{request.PartNumber}");
        });
        XDocument completion = null;
        client.Setup(x => x.CompleteMultipartUpload(It.IsAny<CompleteMultipartUploadRequest>())).Returns<CompleteMultipartUploadRequest>(request =>
        {
            Assert.Equal("upload", request.UploadId);
            completion = XDocument.Load(request.Content);
            return new CompleteMultipartUploadResult();
        });

        Assert.Equal("file", await new TestKS3Provider(ServiceProvider, client.Object)
            .SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", input, "bin", true)));
        Assert.Equal(3, parts.Count);
        Assert.Equal(new byte[] { 1, 2 }, parts[0]);
        Assert.Equal(new byte[] { 3, 4 }, parts[1]);
        Assert.Equal(new byte[] { 5 }, parts[2]);
        Assert.Equal(new[] { "1", "2", "3" }, completion.Root.Elements("Part").Select(part => part.Element("PartNumber").Value));
        Assert.Equal(new[] { "etag-1", "etag-2", "etag-3" }, completion.Root.Elements("Part").Select(part => part.Element("ETag").Value));
    }

    [Fact]
    public async Task Multipart_Failure_Should_Abort_And_Preserve_Upload_Error()
    {
        _configuration.EnableAutoMultiPartUpload = true;
        _configuration.MultiPartUploadMinFileSize = 1;
        _configuration.MultiPartUploadShardingSize = 2;
        using var input = new MemoryStream([1, 2, 3]);
        var error = new IOException("Upload failed");
        var client = CreateClient(true, false);
        client.Setup(x => x.InitiateMultipartUpload(It.IsAny<InitiateMultipartUploadRequest>())).Returns(new InitiateMultipartUploadResult { UploadId = "upload" });
        client.Setup(x => x.UploadPart(It.IsAny<UploadPartRequest>())).Throws(error);
        AbortMultipartUploadRequest aborted = null;
        client.Setup(x => x.AbortMultipartUpload(It.IsAny<AbortMultipartUploadRequest>())).Callback<AbortMultipartUploadRequest>(request => aborted = request).Throws(new IOException("Abort failed"));

        var actual = await Assert.ThrowsAsync<IOException>(() => new TestKS3Provider(ServiceProvider, client.Object)
            .SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", input, "bin")));

        Assert.Same(error, actual);
        Assert.Equal("bucket", aborted.BucketName);
        Assert.Equal("file", aborted.ObjectKey);
        Assert.Equal("upload", aborted.UploadId);
    }

    private static Mock<IKS3> CreateClient(bool bucketExists, bool objectExists)
    {
        var client = new Mock<IKS3>(MockBehavior.Strict);
        client.Setup(x => x.HeadBucket(It.IsAny<HeadBucketRequest>())).Returns(new HeadBucketResult { StatueCode = bucketExists ? HttpStatusCode.OK : HttpStatusCode.NotFound });
        var head = client.Setup(x => x.HeadObject(It.IsAny<HeadObjectRequest>()));
        if (objectExists)
        {
            head.Returns(new HeadObjectResult());
        }
        else
        {
            head.Throws(new IOException("Missing object"));
        }
        return client;
    }

    [DisableConventionalRegistration]
    private sealed class TestKS3Provider(IServiceProvider services, IKS3 client) : KS3FileProvider(
        services.GetRequiredService<ILogger<KS3FileProvider>>(), services.GetRequiredService<IServiceScopeFactory>(), services.GetRequiredService<IClock>(),
        services.GetRequiredService<FileProviderClientCache<IKS3>>(), services.GetRequiredService<IKS3FileNameCalculator>(), services.GetRequiredService<IFileNormalizeNamingService>())
    {
        protected override KS3ClientLease GetKS3ClientLease(KS3FileProviderConfiguration configuration) => new(client);
    }
}

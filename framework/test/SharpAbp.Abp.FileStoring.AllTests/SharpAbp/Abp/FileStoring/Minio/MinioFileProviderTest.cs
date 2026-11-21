using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;
using Moq;
using Volo.Abp.Timing;
using Xunit;

namespace SharpAbp.Abp.FileStoring.Minio;

public class MinioFileProviderTest : AbpFileStoringAllTestBase
{
    private readonly FileContainerConfiguration _configuration = new();

    public MinioFileProviderTest()
    {
        _configuration.UseMinio(configuration =>
        {
            configuration.BucketName = "Bucket!";
            configuration.EndPoint = "storage.test:9000";
            configuration.AccessKey = "unit-access";
            configuration.SecretKey = "unit-secret";
        });
        GetRequiredService<IOptions<AbpFileStoringAbstractionsOptions>>().Value.FilePathStrategy = FilePathGenerationStrategy.DirectFileId;
    }

    private MinioFileProvider Provider(IMinioClient client)
    {
        var cache = new Mock<FileProviderClientCache<IMinioClient>>();
        cache.Setup(value => value.GetOrAdd(It.IsAny<string>(), It.IsAny<Func<IMinioClient>>())).Returns(client);
        return new MinioFileProvider(GetRequiredService<ILogger<MinioFileProvider>>(), GetRequiredService<IClock>(),
            cache.Object, GetRequiredService<IMinioFileNameCalculator>(), GetRequiredService<IFileNormalizeNamingService>());
    }

    private static IMinioClient Client(HttpClient http) => new MinioClient().WithEndpoint("storage.test:9000")
        .WithCredentials("unit-access", "unit-secret").WithRegion("us-east-1").WithHttpClient(http).Build();

    [Fact]
    public async Task Save_Sends_The_Normalized_Bucket_And_Content_And_Preserves_Input()
    {
        using var transport = new StorageTransport();
        using var http = new HttpClient(transport);
        using var client = Client(http);
        using var input = new MemoryStream([1, 2, 3]);
        var provider = Provider(client);

        Assert.Equal("file", await provider.SaveAsync(new FileProviderSaveArgs("fallback", _configuration, "file", input, "bin", true)));

        var request = Assert.Single(transport.Requests, request => request.Method == HttpMethod.Put);
        Assert.Equal("/bucket/file", request.Path);
        Assert.Equal(new byte[] { 1, 2, 3 }, request.Body);
        Assert.True(input.CanRead);
        Assert.False(transport.Disposed);
    }

    [Fact]
    public async Task Save_Rejects_Unsupported_Conditional_Create_Without_A_Request()
    {
        using var transport = new StorageTransport();
        using var http = new HttpClient(transport);
        using var client = Client(http);
        using var input = new MemoryStream([1]);
        await Assert.ThrowsAsync<NotSupportedException>(() => Provider(client)
            .SaveAsync(new FileProviderSaveArgs("fallback", _configuration, "file", input, "bin")));
        Assert.Empty(transport.Requests);
        Assert.True(input.CanRead);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Save_Creates_A_Bucket_Only_When_Requested_And_Missing(bool bucketExists)
    {
        using var transport = new StorageTransport { BucketExists = bucketExists };
        using var http = new HttpClient(transport);
        using var client = Client(http);
        using var input = new MemoryStream([1]);
        _configuration.GetMinioConfiguration().CreateBucketIfNotExists = true;
        await Provider(client).SaveAsync(new FileProviderSaveArgs("fallback", _configuration, "file", input, "bin", true));
        Assert.Equal(bucketExists ? 0 : 1, transport.Requests.Count(request => request.Method == HttpMethod.Put && request.Path == "/bucket"));
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public async Task Read_Delete_And_Exists_Respect_Actual_Sdk_Not_Found_Responses(bool bucketExists, bool objectExists)
    {
        using var transport = new StorageTransport { BucketExists = bucketExists, ObjectExists = objectExists };
        using var http = new HttpClient(transport);
        using var client = Client(http);
        var provider = Provider(client);
        var exists = bucketExists && objectExists;
        Assert.Equal(exists, await provider.ExistsAsync(new FileProviderExistsArgs("fallback", _configuration, "file")));
        using var content = await provider.GetOrNullAsync(new FileProviderGetArgs("fallback", _configuration, "file"));
        if (exists)
        {
            Assert.Equal(new byte[] { 3, 1, 4 }, ((MemoryStream)content).ToArray());
            Assert.Equal(0, content.Position);
        }
        else
        {
            Assert.Null(content);
        }
        Assert.Equal(exists, await provider.DeleteAsync(new FileProviderDeleteArgs("fallback", _configuration, "file")));
        Assert.Equal(exists ? 1 : 0, transport.Requests.Count(request => request.Method == HttpMethod.Delete));
        Assert.False(transport.Disposed);
        _configuration.SetConfiguration(MinioFileProviderConfigurationNames.BucketName, "");
        Assert.Equal(exists, await provider.ExistsAsync(new FileProviderExistsArgs("bucket", _configuration, "file")));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Download_Writes_Sdk_Content_Only_For_Existing_Objects(bool exists)
    {
        using var transport = new StorageTransport { ObjectExists = exists };
        using var http = new HttpClient(transport);
        using var client = Client(http);
        var directory = Path.Combine(Path.GetTempPath(), "sharp-abp-minio-" + Guid.NewGuid().ToString("N"));
        var target = Path.Combine(directory, "file.bin");
        try
        {
            Assert.Equal(exists, await Provider(client).DownloadAsync(new FileProviderDownloadArgs("fallback", _configuration, "file", target)));
            Assert.Equal(exists, File.Exists(target));
            if (exists) Assert.Equal(new byte[] { 3, 1, 4 }, await File.ReadAllBytesAsync(target));
        }
        finally
        {
            if (File.Exists(target)) File.Delete(target);
            if (Directory.Exists(directory)) Directory.Delete(directory);
        }
    }

    [Fact]
    public async Task Unexpected_Sdk_Errors_Propagate_And_Keep_The_Cached_Client_Usable()
    {
        using var transport = new StorageTransport { Forbidden = true };
        using var http = new HttpClient(transport);
        using var client = Client(http);
        var provider = Provider(client);
        await Assert.ThrowsAnyAsync<MinioException>(() => provider.ExistsAsync(new FileProviderExistsArgs("fallback", _configuration, "file")));
        Assert.False(transport.Disposed);
        transport.Forbidden = false;
        Assert.True(await provider.ExistsAsync(new FileProviderExistsArgs("fallback", _configuration, "file")));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Access_Urls_Are_Signed_With_The_Requested_Expiry_And_Optional_Existence_Check(bool future)
    {
        using var transport = new StorageTransport();
        using var http = new HttpClient(transport);
        using var client = Client(http);
        var provider = Provider(client);
        var now = GetRequiredService<IClock>().Now;
        var expires = now.AddSeconds(future ? 1200 : -1);
        var url = await provider.GetAccessUrlAsync(new FileProviderAccessArgs("fallback", _configuration, "file", expires, true));
        Assert.Contains("/bucket/file?", url);
        Assert.Contains(future ? "X-Amz-Expires=1200" : "X-Amz-Expires=600", url, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("X-Amz-Signature=", url, StringComparison.OrdinalIgnoreCase);
        transport.ObjectExists = false;
        Assert.Equal("", await provider.GetAccessUrlAsync(new FileProviderAccessArgs("fallback", _configuration, "file", checkFileExist: true)));
        _configuration.HttpAccess = false;
        Assert.Equal("", await provider.GetAccessUrlAsync(new FileProviderAccessArgs("fallback", _configuration, "file")));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Client_Policy_Applies_Endpoint_Credentials_And_Tls_Without_Opening_A_Connection(bool ssl)
    {
        var configuration = _configuration.GetMinioConfiguration();
        configuration.WithSSL = ssl;
        var policy = new MinioClientPolicy(configuration);
        using var client = policy.Create();
        Assert.Equal(ssl, client.Config.Secure);
        Assert.Equal("unit-access", client.Config.AccessKey);
        Assert.Equal("unit-secret", client.Config.SecretKey);
        Assert.Contains("storage.test", client.Config.BaseUrl);
        Assert.True(policy.Return(client));
        Assert.False(policy.Return(null));
    }

    private sealed class StorageTransport : HttpMessageHandler
    {
        public bool BucketExists { get; set; } = true;
        public bool ObjectExists { get; set; } = true;
        public bool Forbidden { get; set; }
        public bool Disposed { get; private set; }
        public List<(HttpMethod Method, string Path, byte[] Body)> Requests { get; } = new();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = request.RequestUri.AbsolutePath.TrimEnd('/');
            var body = request.Content == null ? [] : await request.Content.ReadAsByteArrayAsync(cancellationToken);
            Requests.Add((request.Method, path, body));
            var isBucket = path == "/bucket";
            var missing = request.Method == HttpMethod.Head && (!BucketExists || (!isBucket && !ObjectExists));
            var response = new HttpResponseMessage(Forbidden ? HttpStatusCode.Forbidden : missing ? HttpStatusCode.NotFound : HttpStatusCode.OK)
            {
                RequestMessage = request,
                Content = new ByteArrayContent(Forbidden || missing || isBucket || request.Method != HttpMethod.Get ? [] : [3, 1, 4])
            };
            response.Headers.TryAddWithoutValidation("ETag", "\"test-etag\"");
            response.Headers.TryAddWithoutValidation("x-amz-request-id", "test-request");
            if (!isBucket && !missing && !Forbidden)
            {
                response.Content.Headers.ContentLength = 3;
                response.Content.Headers.LastModified = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
            }
            if (request.Method == HttpMethod.Put && isBucket) BucketExists = true;
            return response;
        }

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }
    }
}

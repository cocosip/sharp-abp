using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OBS;
using OBS.Model;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;
using Xunit;

namespace SharpAbp.Abp.FileStoring.Obs;

public class ObsFileProviderTest : AbpFileStoringAllTestBase
{
    private readonly FileContainerConfiguration _configuration = new();

    public ObsFileProviderTest()
    {
        _configuration.UseObs(configuration => configuration.BucketName = "bucket");
        GetRequiredService<IOptions<AbpFileStoringAbstractionsOptions>>().Value.FilePathStrategy = FilePathGenerationStrategy.DirectFileId;
    }

    [Fact]
    public void Factory_Should_Pass_Credentials_And_Endpoint_In_Sdk_Constructor_Order()
    {
        var configuration = _configuration.GetObsConfiguration();
        configuration.Endpoint = "https://obs.example.invalid";
        configuration.AccessKeyId = "test-access";
        configuration.AccessKeySecret = "test-secret";

        var client = GetRequiredService<IObsClientFactory>().Create(configuration);
        var signed = new Uri(client.CreateTemporarySignature(new CreateTemporarySignatureRequest
        {
            BucketName = "bucket",
            ObjectKey = "file",
            Expires = 60
        }).SignUrl);

        Assert.Equal("bucket.obs.example.invalid", signed.Host);
        Assert.Contains("test-access", signed.Query);
        Assert.DoesNotContain("test-secret", signed.AbsoluteUri);
    }

    [Fact]
    public async Task Sdk_Loopback_Should_Create_Save_Copy_Download_And_Delete_File()
    {
        await using var server = new ObsLoopbackServer();
        var client = CreateClient(server);
        var provider = new TestObsProvider(ServiceProvider, client);
        _configuration.GetObsConfiguration().CreateContainerIfNotExists = true;
        using var content = new MemoryStream([1, 2, 3, 4]);
        Assert.False(await provider.ExistsAsync(new FileProviderExistsArgs("bucket", _configuration, "file")));
        Assert.Equal("file", await provider.SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", content, "bin")));
        Assert.Equal(new byte[] { 1, 2, 3, 4 }, server.Objects["file"]);
        Assert.True(await provider.ExistsAsync(new FileProviderExistsArgs("bucket", _configuration, "file")));
        using var duplicate = new MemoryStream([9]);
        await Assert.ThrowsAsync<FileAlreadyExistsException>(() => provider.SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", duplicate, "bin")));
        Assert.Equal(new byte[] { 1, 2, 3, 4 }, server.Objects["file"]);
        using var copied = await provider.GetOrNullAsync(new FileProviderGetArgs("bucket", _configuration, "file"));
        Assert.Equal(new byte[] { 1, 2, 3, 4 }, ((MemoryStream)copied).ToArray());
        Assert.Equal(0, copied.Position);
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        try
        {
            Assert.True(await provider.DownloadAsync(new FileProviderDownloadArgs("bucket", _configuration, "file", path)));
            Assert.Equal(new byte[] { 1, 2, 3, 4 }, await File.ReadAllBytesAsync(path));
        }
        finally
        {
            File.Delete(path);
        }
        Assert.True(await provider.DeleteAsync(new FileProviderDeleteArgs("bucket", _configuration, "file")));
        Assert.False(await provider.DeleteAsync(new FileProviderDeleteArgs("bucket", _configuration, "file")));
        Assert.Null(await provider.GetOrNullAsync(new FileProviderGetArgs("bucket", _configuration, "file")));
        Assert.False(await provider.DownloadAsync(new FileProviderDownloadArgs("bucket", _configuration, "file", "unused")));
        Assert.Null(server.Failure);
    }

    [Fact]
    public async Task Explicit_Overwrite_Uploads_Without_An_Existence_Request()
    {
        await using var server = new ObsLoopbackServer { BucketExists = true };
        server.Objects["file"] = [9];
        var client = CreateClient(server);
        var provider = new TestObsProvider(ServiceProvider, client);
        using var content = new MemoryStream([1, 2, 3]);

        Assert.Equal("file", await provider.SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", content, "bin", true)));
        Assert.Equal(new byte[] { 1, 2, 3 }, server.Objects["file"]);
        Assert.Equal(0, server.HeadRequests);
        Assert.Null(server.Failure);
    }

    [Fact]
    public async Task Sdk_Loopback_Multipart_Should_Send_Bytes_From_Current_Position_And_Complete()
    {
        await using var server = new ObsLoopbackServer { BucketExists = true };
        var client = CreateClient(server);
        var provider = new TestObsProvider(ServiceProvider, client);
        _configuration.EnableAutoMultiPartUpload = true;
        _configuration.MultiPartUploadMinFileSize = 1;
        _configuration.MultiPartUploadShardingSize = 2;
        using var content = new MemoryStream([0, 1, 2, 3, 4, 5]);
        content.Position = 1;

        Assert.Equal("file", await provider.SaveAsync(new FileProviderSaveArgs("bucket", _configuration, "file", content, "bin")));

        Assert.Equal(new byte[] { 1, 2 }, server.Parts[1]);
        Assert.Equal(new byte[] { 3, 4 }, server.Parts[2]);
        Assert.Equal(new byte[] { 5 }, server.Parts[3]);
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, server.Objects["file"]);
        Assert.Contains("etag-1", server.CompletionXml);
        Assert.Contains("etag-2", server.CompletionXml);
        Assert.Contains("etag-3", server.CompletionXml);
        Assert.Null(server.Failure);
    }

    [Fact]
    public async Task Access_Should_Create_Signed_Url_And_Respect_Missing_Or_Disabled_Access()
    {
        await using var server = new ObsLoopbackServer { BucketExists = true };
        var client = CreateClient(server);
        var provider = new TestObsProvider(ServiceProvider, client);
        var url = await provider.GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file", GetRequiredService<IClock>().Now.AddHours(1)));
        var uri = new Uri(url);
        Assert.Equal("/bucket/file", uri.AbsolutePath);
        Assert.Contains("Signature=", uri.Query);
        Assert.Equal("", await provider.GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file", checkFileExist: true)));
        _configuration.HttpAccess = false;
        Assert.Equal("", await provider.GetAccessUrlAsync(new FileProviderAccessArgs("bucket", _configuration, "file")));
        Assert.Null(server.Failure);
    }

    private static ObsClient CreateClient(ObsLoopbackServer server) => new("test-access", "test-secret", new ObsConfig
    {
        Endpoint = server.Url,
        AuthTypeNegotiation = false,
        MaxErrorRetry = 0,
        Timeout = 10000
    });

    [DisableConventionalRegistration]
    private sealed class TestObsProvider(IServiceProvider services, ObsClient client) : ObsFileProvider(
        services.GetRequiredService<ILogger<ObsFileProvider>>(), services.GetRequiredService<IServiceScopeFactory>(), services.GetRequiredService<IClock>(),
        services.GetRequiredService<FileProviderClientCache<ObsClient>>(), services.GetRequiredService<IObsFileNameCalculator>(), services.GetRequiredService<IFileNormalizeNamingService>())
    {
        protected override ObsClientLease GetObsClientLease(ObsFileProviderConfiguration configuration) => new(client);
    }

    private sealed class ObsLoopbackServer : IAsyncDisposable
    {
        private readonly HttpListener _listener = new();
        private readonly CancellationTokenSource _cancellation = new();
        private readonly Task _loop;
        public string Url { get; }
        public bool BucketExists { get; set; }
        public int HeadRequests { get; private set; }
        public ConcurrentDictionary<string, byte[]> Objects { get; } = new();
        public ConcurrentDictionary<int, byte[]> Parts { get; } = new();
        public string CompletionXml { get; private set; }
        public Exception Failure { get; private set; }

        public ObsLoopbackServer()
        {
            var reservation = new TcpListener(IPAddress.Loopback, 0);
            reservation.Start();
            var port = ((IPEndPoint)reservation.LocalEndpoint).Port;
            reservation.Stop();
            Url = $"http://127.0.0.1:{port}/";
            _listener.Prefixes.Add(Url);
            _listener.Start();
            _loop = RunAsync();
        }

        private async Task RunAsync()
        {
            try
            {
                while (!_cancellation.IsCancellationRequested)
                {
                    var context = await _listener.GetContextAsync().WaitAsync(_cancellation.Token);
                    try
                    {
                        await HandleAsync(context);
                    }
                    catch (Exception exception)
                    {
                        Failure = exception;
                        context.Response.StatusCode = 500;
                    }
                    finally
                    {
                        context.Response.Close();
                    }
                }
            }
            catch (OperationCanceledException) when (_cancellation.IsCancellationRequested) { }
            catch (HttpListenerException) when (_cancellation.IsCancellationRequested) { }
            catch (ObjectDisposedException) when (_cancellation.IsCancellationRequested) { }
        }

        private async Task HandleAsync(HttpListenerContext context)
        {
            var request = context.Request;
            var key = request.Url.AbsolutePath.TrimStart('/').Split('/', 2).ElementAtOrDefault(1);
            using var body = new MemoryStream();
            await request.InputStream.CopyToAsync(body);
            var response = context.Response;
            response.Headers["ETag"] = "\"etag\"";
            response.Headers["x-obs-request-id"] = "unit-test";
            response.ContentType = "application/xml";
            string xml = null;
            if (request.HttpMethod == "HEAD")
            {
                HeadRequests++;
                response.StatusCode = BucketExists && (key == null || Objects.ContainsKey(key)) ? 200 : 404;
            }
            else if (request.HttpMethod == "PUT" && key == null)
            {
                BucketExists = true;
            }
            else if (request.HttpMethod == "POST" && request.Url.Query.Contains("uploads"))
            {
                xml = "<InitiateMultipartUploadResult><Bucket>bucket</Bucket><Key>file</Key><UploadId>upload</UploadId></InitiateMultipartUploadResult>";
            }
            else if (request.HttpMethod == "PUT" && request.QueryString["uploadId"] != null)
            {
                var part = int.Parse(request.QueryString["partNumber"]);
                Parts[part] = body.ToArray();
                response.Headers["ETag"] = $"\"etag-{part}\"";
            }
            else if (request.HttpMethod == "POST" && request.QueryString["uploadId"] != null)
            {
                CompletionXml = Encoding.UTF8.GetString(body.ToArray());
                Objects[key] = Parts.OrderBy(part => part.Key).SelectMany(part => part.Value).ToArray();
                xml = "<CompleteMultipartUploadResult><Bucket>bucket</Bucket><Key>file</Key><ETag>complete</ETag></CompleteMultipartUploadResult>";
            }
            else if (request.HttpMethod == "PUT")
            {
                Objects[key] = body.ToArray();
            }
            else if (request.HttpMethod == "GET" && Objects.TryGetValue(key, out var content))
            {
                response.ContentLength64 = content.Length;
                await response.OutputStream.WriteAsync(content);
            }
            else if (request.HttpMethod == "DELETE")
            {
                Objects.TryRemove(key, out _);
                response.StatusCode = 204;
            }
            else
            {
                throw new InvalidOperationException($"Unexpected request {request.HttpMethod} {request.RawUrl}");
            }
            if (xml != null)
            {
                var bytes = Encoding.UTF8.GetBytes(xml);
                response.ContentLength64 = bytes.Length;
                await response.OutputStream.WriteAsync(bytes);
            }
        }

        public async ValueTask DisposeAsync()
        {
            _cancellation.Cancel();
            _listener.Close();
            await _loop;
            _cancellation.Dispose();
        }
    }
}

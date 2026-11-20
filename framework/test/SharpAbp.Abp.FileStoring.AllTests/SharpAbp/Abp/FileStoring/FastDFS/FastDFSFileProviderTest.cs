using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FastDFS.Client;
using FastDFS.Client.Configuration;
using FastDFS.Client.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace SharpAbp.Abp.FileStoring.FastDFS;

public class FastDFSFileProviderTest : AbpFileStoringAllTestBase
{
    private readonly FileContainerConfiguration _configuration = new();
    private readonly Mock<IFastDFSClient> _client = new(MockBehavior.Strict);
    private readonly Mock<IFastDFSClientFactory> _clients = new(MockBehavior.Strict);
    private readonly DefaultFastDFSFileProviderConfigurationFactory _factory;

    public FastDFSFileProviderTest()
    {
        _configuration.UseFastDFS(configuration =>
        {
            configuration.ClusterName = "cluster";
            configuration.GroupName = "group1";
            configuration.Trackers = " tracker-a:22122, ,tracker-b:22122 ";
            configuration.Charset = "UTF-8";
            configuration.HttpServer = "";
        });
        _clients.Setup(factory => factory.GetClient("cluster")).Returns(_client.Object);
        _clients.Setup(factory => factory.RegisterClient("cluster", It.IsAny<FastDFSConfiguration>())).Returns(_client.Object);
        _factory = new DefaultFastDFSFileProviderConfigurationFactory(NullLogger<DefaultFastDFSFileProviderConfigurationFactory>.Instance, _clients.Object);
        GetRequiredService<IOptions<AbpFileStoringAbstractionsOptions>>().Value.FilePathStrategy = FilePathGenerationStrategy.DirectFileId;
    }

    private FastDFSFileProvider Provider() => new(GetRequiredService<ILogger<FastDFSFileProvider>>(),
        GetRequiredService<IFastDFSFileNameCalculator>(), GetRequiredService<IFileNormalizeNamingService>(), _factory, _clients.Object);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Save_Uses_The_Cluster_And_Normalized_Or_Fallback_Group_Without_Disposing_Input(bool explicitGroup)
    {
        if (explicitGroup)
        {
            _configuration.GetFastDFSConfiguration().GroupName = "Group!1";
        }
        else
        {
            _configuration.SetConfiguration(FastDFSFileProviderConfigurationNames.GroupName, "");
        }
        using var input = new MemoryStream([1, 2, 3]);
        _client.Setup(client => client.UploadAsync(explicitGroup ? "group1" : "fallback", input, "bin", CancellationToken.None))
            .ReturnsAsync("group1/M00/server-generated.bin");

        Assert.Equal("group1/M00/server-generated.bin", await Provider().SaveAsync(new FileProviderSaveArgs("fallback", _configuration, "requested-id", input, "bin")));
        Assert.True(input.CanRead);
        _client.VerifyAll();
        _clients.Verify(factory => factory.RegisterClient("cluster", It.IsAny<FastDFSConfiguration>()), Times.Once);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Delete_Download_And_Read_Translate_Storage_Errors_To_Missing_Results(bool missing)
    {
        var provider = Provider();
        _client.Setup(client => client.DeleteAsync("file", CancellationToken.None))
            .Returns(missing ? Task.FromException(new FastDFSException("not found")) : Task.CompletedTask);
        _client.Setup(client => client.DownloadFileAsync("file", "target", CancellationToken.None))
            .Returns(missing ? Task.FromException(new FastDFSException("not found")) : Task.CompletedTask);
        _client.Setup(client => client.DownloadAsync("file", It.IsAny<Stream>(), CancellationToken.None))
            .Returns<string, Stream, CancellationToken>(async (_, stream, _) =>
            {
                if (missing) throw new FastDFSException("not found");
                await stream.WriteAsync(new byte[] { 3, 1, 4 });
            });
        _client.Setup(client => client.FileExistsAsync("file", CancellationToken.None)).ReturnsAsync(!missing);

        Assert.Equal(!missing, await provider.DeleteAsync(new FileProviderDeleteArgs("group1", _configuration, "file")));
        Assert.Equal(!missing, await provider.DownloadAsync(new FileProviderDownloadArgs("group1", _configuration, "file", "target")));
        Assert.Equal(!missing, await provider.ExistsAsync(new FileProviderExistsArgs("group1", _configuration, "file")));
        using var result = await provider.GetOrNullAsync(new FileProviderGetArgs("group1", _configuration, "file"));
        if (missing)
        {
            Assert.Null(result);
        }
        else
        {
            Assert.Equal(0, result.Position);
            Assert.Equal(new byte[] { 3, 1, 4 }, ((MemoryStream)result).ToArray());
        }
        _client.VerifyAll();
    }

    [Fact]
    public async Task Unexpected_Sdk_Errors_Are_Not_Misreported_As_Missing_Files()
    {
        _client.Setup(client => client.DeleteAsync("file", CancellationToken.None)).ThrowsAsync(new IOException("transport failure"));
        await Assert.ThrowsAsync<IOException>(() => Provider().DeleteAsync(new FileProviderDeleteArgs("group1", _configuration, "file")));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Access_Urls_Use_Token_Settings_Only_When_Enabled(bool antiSteal)
    {
        var configuration = _configuration.GetFastDFSConfiguration();
        configuration.AntiStealCheckToken = antiSteal;
        configuration.DefaultTokenExpireSeconds = 120;
        _client.Setup(client => client.GetFileUrlAsync("file", null, CancellationToken.None)).ReturnsAsync("https://storage.test/file");
        _client.Setup(client => client.GetFileUrlWithTokenAsync("file", 120, null, CancellationToken.None)).ReturnsAsync("https://storage.test/file?token=test");
        var provider = Provider();

        Assert.Equal(antiSteal ? "https://storage.test/file?token=test" : "https://storage.test/file",
            await provider.GetAccessUrlAsync(new FileProviderAccessArgs("group1", _configuration, "file")));
        _configuration.HttpAccess = false;
        Assert.Equal("", await provider.GetAccessUrlAsync(new FileProviderAccessArgs("group1", _configuration, "file")));
        _client.Verify(client => client.GetFileUrlWithTokenAsync("file", 120, null, CancellationToken.None), antiSteal ? Times.Once() : Times.Never());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Configuration_Factory_Registers_Once_And_Preserves_Tracker_Pool_And_Http_Settings(bool httpAccess)
    {
        var configuration = _configuration.GetFastDFSConfiguration();
        configuration.Charset = "UTF-8";
        configuration.NetworkTimeout = 12;
        configuration.MaxConnectionPerServer = 20;
        configuration.MinConnectionPerServer = 2;
        configuration.ConnectionIdleTimeout = 30;
        configuration.ConnectionLifeTime = 60;
        configuration.ConnectionTimeout = 100;
        configuration.SendTimeout = 200;
        configuration.ReceiveTimeout = 300;
        configuration.HttpServer = httpAccess ? "https://storage.test" : null;
        configuration.SecretKey = "test-secret";
        configuration.AntiStealCheckToken = true;
        configuration.DefaultTokenExpireSeconds = 90;
        FastDFSConfiguration registered = null;
        _clients.Setup(factory => factory.RegisterClient("cluster", It.IsAny<FastDFSConfiguration>()))
            .Callback<string, FastDFSConfiguration>((_, value) => registered = value).Returns(_client.Object);

        Assert.True(_factory.AddIfNotContains(configuration));
        Assert.False(_factory.AddIfNotContains(configuration));
        Assert.Same(configuration, _factory.GetConfiguration("cluster"));
        Assert.Null(_factory.GetConfiguration("missing"));
        Assert.Equal(new[] { "tracker-a:22122", "tracker-b:22122" }, registered.TrackerServers);
        Assert.Equal("UTF-8", registered.Charset);
        Assert.Equal(12, registered.NetworkTimeout);
        Assert.Equal(20, registered.ConnectionPool.MaxConnectionPerServer);
        Assert.Equal(2, registered.ConnectionPool.MinConnectionPerServer);
        Assert.Equal(30, registered.ConnectionPool.ConnectionIdleTimeout);
        Assert.Equal(60, registered.ConnectionPool.ConnectionLifetime);
        Assert.Equal(100, registered.ConnectionPool.ConnectionTimeout);
        Assert.Equal(200, registered.ConnectionPool.SendTimeout);
        Assert.Equal(300, registered.ConnectionPool.ReceiveTimeout);
        if (httpAccess)
        {
            Assert.Equal("https://storage.test", registered.HttpConfig.ServerUrls["group1"]);
            Assert.Equal("test-secret", registered.HttpConfig.SecretKey);
            Assert.True(registered.HttpConfig.AntiStealTokenEnabled);
            Assert.Equal(90, registered.HttpConfig.DefaultTokenExpireSeconds);
        }
        _clients.Verify(factory => factory.RegisterClient("cluster", It.IsAny<FastDFSConfiguration>()), Times.Once);
    }
}

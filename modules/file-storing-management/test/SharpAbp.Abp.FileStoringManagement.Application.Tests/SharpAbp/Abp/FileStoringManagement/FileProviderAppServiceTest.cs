using SharpAbp.Abp.FileStoring;
using SharpAbp.Abp.FileStoring.Aws;
using SharpAbp.Abp.FileStoring.S3;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileProviderAppServiceTest : FileStoringManagementApplicationTestBase
    {
        private readonly IFileProviderAppService _fileProviderAppService;

        public FileProviderAppServiceTest()
        {
            _fileProviderAppService = GetRequiredService<IFileProviderAppService>();
        }

        [Fact]
        public async Task Aws_Options_Should_Include_MaxConnectionsPerServer()
        {
            var options = await _fileProviderAppService.GetOptionsAsync(AwsFileProviderConfigurationNames.ProviderName);

            Assert.Contains(options.Values, item =>
                item.Name == AwsFileProviderConfigurationNames.MaxConnectionsPerServer);
        }

        [Fact]
        public async Task S3_Options_Should_Include_MaxConnectionsPerServer()
        {
            var options = await _fileProviderAppService.GetOptionsAsync(S3FileProviderConfigurationNames.ProviderName);

            Assert.Contains(options.Values, item =>
                item.Name == S3FileProviderConfigurationNames.MaxConnectionsPerServer);
        }

        [Fact]
        public void Converter_Should_Skip_Unknown_And_Empty_Optional_Items()
        {
            var converter = GetRequiredService<IFileContainerConfigurationConverter>();

            var container = new FileStoringContainer(
                Guid.NewGuid(),
                null,
                false,
                S3FileProviderConfigurationNames.ProviderName,
                "test",
                "test",
                false,
                0,
                0,
                false);

            container.AddItem(Guid.NewGuid(), S3FileProviderConfigurationNames.ServerUrl, "http://localhost:9000");
            container.AddItem(Guid.NewGuid(), "S3.RemovedOption", "whatever");
            container.AddItem(Guid.NewGuid(), S3FileProviderConfigurationNames.MaxConnectionsPerServer, "");

            var configuration = converter.ToConfiguration(container);

            Assert.Equal("http://localhost:9000",
                configuration.GetConfigurationOrNull(S3FileProviderConfigurationNames.ServerUrl));
            Assert.Null(configuration.GetConfigurationOrNull("S3.RemovedOption"));
            Assert.Null(configuration.GetConfigurationOrNull(S3FileProviderConfigurationNames.MaxConnectionsPerServer));
        }
    }
}

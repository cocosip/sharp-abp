#nullable enable
using System;
using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using SharpAbp.Abp.FileStoring.FileSystem;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    public class FileSystemSecurityTests
    {
        private static FileContainerConfiguration CreateFileSystemConfiguration(string basePath, bool appendContainerName = true)
        {
            var configuration = new FileContainerConfiguration
            {
                HttpAccess = true
            };

            configuration.UseFileSystem(options =>
            {
                options.BasePath = basePath;
                options.HttpServer = "https://cdn.example.com/files";
                options.AppendContainerNameToBasePath = appendContainerName;
            });

            return configuration;
        }

        private static DefaultFilePathCalculator CreateCalculator(
            Guid? tenantId = null,
            string? tenantName = null,
            FilePathContext? context = null,
            Action<AbpFileStoringAbstractionsOptions>? configure = null)
        {
            var currentTenant = new Mock<ICurrentTenant>();
            currentTenant.Setup(x => x.Id).Returns(tenantId);
            currentTenant.Setup(x => x.Name).Returns(tenantName);

            var accessor = new Mock<IFilePathContextAccessor>();
            accessor.Setup(x => x.Current).Returns(context);

            var options = new AbpFileStoringAbstractionsOptions();
            configure?.Invoke(options);

            return new DefaultFilePathCalculator(currentTenant.Object, accessor.Object, Options.Create(options));
        }

        [Theory]
        [InlineData("../../escape.txt")]
        [InlineData("..\\..\\escape.txt")]
        [InlineData("/tmp/escape.txt")]
        [InlineData("C:/temp/escape.txt")]
        public void Calculate_Should_Reject_FileIds_That_Escape_BasePath(string fileId)
        {
            var basePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var configuration = CreateFileSystemConfiguration(basePath);
            var calculator = CreateCalculator();
            var args = new FileProviderSaveArgs("docs", configuration, fileId);

            var exception = Assert.Throws<AbpException>(() => calculator.Calculate(args));

            Assert.Contains("must be a relative path without traversal segments", exception.Message);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAccessUrlAsync_Should_Reuse_Calculated_Path_Segments()
        {
            var tenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var basePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var configuration = CreateFileSystemConfiguration(basePath);
            var context = new FilePathContext { Prefix = "uploads" };
            var calculator = CreateCalculator(
                tenantId: tenantId,
                tenantName: "tenant-a",
                context: context,
                configure: options =>
                {
                    options.FilePathBuilder.TenantsSegment = "orgs";
                    options.FilePathBuilder.TenantIdentifierFactory = (id, name, current) => current?.TenantCode ?? name ?? id.ToString("D");
                });

            var currentTenant = new Mock<ICurrentTenant>();
            currentTenant.Setup(x => x.Id).Returns(tenantId);
            var provider = new FileSystemFileProvider(NullLogger<FileSystemFileProvider>.Instance, currentTenant.Object, calculator);
            var args = new FileProviderAccessArgs("reports", configuration, "daily/summary.txt");

            var url = await provider.GetAccessUrlAsync(args);

            Assert.Equal("https://cdn.example.com/files/uploads/orgs/tenant-a/reports/daily/summary.txt", url);
        }

        [Fact]
        public void ImplementedFileContainerConfigurationProvider_Should_Not_Be_A_Conventionally_Registered_Transient_Service()
        {
            Assert.DoesNotContain(typeof(ITransientDependency), typeof(ImplementedFileContainerConfigurationProvider).GetInterfaces());
        }

        [Fact]
        public void ImplementedFileContainerConfigurationProvider_Should_Throw_Clear_Message_When_Used_Directly()
        {
            var provider = new ImplementedFileContainerConfigurationProvider();

            var exception = Assert.Throws<AbpException>(() => provider.Get("default"));

            Assert.Contains("Reference SharpAbp.Abp.FileStoring", exception.Message);
        }
    }
}

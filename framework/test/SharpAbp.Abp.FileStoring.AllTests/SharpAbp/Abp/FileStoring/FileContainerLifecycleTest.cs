using Microsoft.Extensions.Options;
using SharpAbp.Abp.FileStoring.FileSystem;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace SharpAbp.Abp.FileStoring;

public class FileContainerLifecycleTest : AbpFileStoringAllTestBase
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Typed_Container_Should_Preserve_Context_And_Isolate_Or_Share_Files_As_Configured(bool multiTenant)
    {
        var directory = Directory.CreateTempSubdirectory("sharp-abp-container-");
        try
        {
            GetRequiredService<IOptions<AbpFileStoringOptions>>().Value.Containers.Configure<LifecycleContainer>(configuration =>
            {
                configuration.IsMultiTenant = multiTenant;
                configuration.HttpAccess = true;
                configuration.UseFileSystem(fileSystem =>
                {
                    fileSystem.BasePath = directory.FullName;
                    fileSystem.HttpServer = "https://files.example.invalid";
                    fileSystem.AppendContainerNameToBasePath = true;
                });
            });
            var pathOptions = GetRequiredService<IOptions<AbpFileStoringAbstractionsOptions>>().Value;
            pathOptions.FilePathStrategy = FilePathGenerationStrategy.TenantBased;
            pathOptions.FilePathBuilder.HostSegment = "host";
            pathOptions.FilePathBuilder.TenantsSegment = "tenants";
            var container = GetRequiredService<IFileContainer<LifecycleContainer>>();
            Assert.Equal(multiTenant, container.GetConfiguration().IsMultiTenant);
            var tenant = GetRequiredService<ICurrentTenant>();
            var accessor = GetRequiredService<IFilePathContextAccessor>();
            var firstId = Guid.NewGuid();
            var secondId = Guid.NewGuid();
            var context = new FilePathContext { Prefix = "scoped" };
            using (accessor.Change(context))
            using (tenant.Change(firstId, "first"))
            {
                Assert.Equal("file.bin", await container.SaveAsync("file.bin", new byte[] { 1, 2, 3 }, ".bin"));
                await Assert.ThrowsAsync<FileAlreadyExistsException>(() => container.SaveAsync("file.bin", new byte[] { 9 }, ".bin"));
                Assert.True(await container.ExistsAsync("file.bin"));
                Assert.Equal(new byte[] { 1, 2, 3 }, await container.GetAllBytesAsync("file.bin"));
                Assert.Equal(new byte[] { 1, 2, 3 }, await container.GetAllBytesOrNullAsync("file.bin"));
                using (var optional = await container.GetOrNullAsync("file.bin"))
                {
                    Assert.Equal(0, optional.Position);
                }
                var url = await container.GetAccessUrlAsync("file.bin", checkFileExist: true);
                Assert.Contains(multiTenant ? $"/scoped/tenants/{firstId:D}/unit-lifecycle/file.bin" : "/scoped/host/unit-lifecycle/file.bin", url);
                Assert.Equal(firstId, tenant.Id);
                Assert.Same(context, accessor.Current);
                using (tenant.Change(secondId, "second"))
                {
                    Assert.Equal(!multiTenant, await container.ExistsAsync("file.bin"));
                    if (multiTenant)
                    {
                        Assert.Null(await container.GetOrNullAsync("file.bin"));
                        await Assert.ThrowsAsync<AbpException>(() => container.GetAsync("file.bin"));
                        Assert.Null(await container.GetAllBytesOrNullAsync("file.bin"));
                        Assert.Equal("", await container.GetAccessUrlAsync("file.bin", checkFileExist: true));
                        Assert.False(await container.DownloadAsync("file.bin", Path.Combine(directory.FullName, "missing")));
                        Assert.False(await container.DeleteAsync("file.bin"));
                    }
                    else
                    {
                        Assert.Equal(new byte[] { 1, 2, 3 }, await container.GetAllBytesAsync("file.bin"));
                    }
                    Assert.Equal(secondId, tenant.Id);
                    Assert.Same(context, accessor.Current);
                }
                var downloaded = Path.Combine(directory.FullName, "download", "copy.bin");
                Assert.True(await container.DownloadAsync("file.bin", downloaded));
                Assert.Equal(new byte[] { 1, 2, 3 }, await File.ReadAllBytesAsync(downloaded));
                Assert.Equal("from-path.bin", await container.SaveAsync("from-path.bin", downloaded));
                Assert.Equal(new byte[] { 1, 2, 3 }, await container.GetAllBytesAsync("from-path.bin"));
                Assert.True(await container.DeleteAsync("file.bin"));
                Assert.False(await container.DeleteAsync("file.bin"));
                Assert.True(await container.DeleteAsync("from-path.bin"));
                Assert.Same(context, accessor.Current);
                Assert.Equal(firstId, tenant.Id);
            }
            Assert.Null(accessor.Current);
            Assert.Null(tenant.Id);
        }
        finally
        {
            directory.Delete(true);
        }
    }

    [Fact]
    public async Task Missing_Resolved_File_Context_Should_Be_Restored_After_Failure()
    {
        var directory = Directory.CreateTempSubdirectory("sharp-abp-container-");
        try
        {
            GetRequiredService<IOptions<AbpFileStoringOptions>>().Value.Containers.Configure<LifecycleContainer>(configuration =>
                configuration.UseFileSystem(fileSystem => fileSystem.BasePath = directory.FullName));
            var container = GetRequiredService<IFileContainer<LifecycleContainer>>();
            var accessor = GetRequiredService<IFilePathContextAccessor>();

            Assert.Null(await container.GetOrNullAsync("missing.bin"));
            await Assert.ThrowsAsync<AbpException>(() => container.GetAsync("missing.bin"));

            Assert.Null(accessor.Current);
        }
        finally
        {
            directory.Delete(true);
        }
    }

    [FileContainerName("unit-lifecycle")]
    public class LifecycleContainer;
}

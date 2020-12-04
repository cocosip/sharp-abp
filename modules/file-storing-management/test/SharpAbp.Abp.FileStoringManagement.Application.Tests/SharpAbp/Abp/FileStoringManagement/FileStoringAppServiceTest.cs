using SharpAbp.Abp.FileStoring.Aliyun;
using SharpAbp.Abp.FileStoring.FileSystem;
using SharpAbp.Abp.FileStoring.Minio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringAppServiceTest : FileStoringManagementApplicationTestBase
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly IFileStoringAppService _fileStoringAppService;
        public FileStoringAppServiceTest()
        {
            _currentTenant = GetRequiredService<ICurrentTenant>();
            _fileStoringAppService = GetRequiredService<IFileStoringAppService>();
        }

        [Fact]
        public void GetProviders_Test()
        {
            var providers = _fileStoringAppService.GetProviders();
            Assert.Equal(6, providers.Count);
        }

        [Fact]
        public void HasProvider_Test()
        {
            Assert.True(_fileStoringAppService.HasProvider(AliyunFileProviderConfigurationNames.ProviderName));
            Assert.False(_fileStoringAppService.HasProvider("name1"));
        }

        [Fact]
        public void GetProviderOptions_Test()
        {
            var providerOptions = _fileStoringAppService.GetProviderOptions(MinioFileProviderConfigurationNames.ProviderName);
            Assert.Equal(MinioFileProviderConfigurationNames.ProviderName, providerOptions.Provider);
            Assert.Equal(6, providerOptions.Properties.Count);
        }

        [Fact]
        public async Task CreateAsync_UpdateAsync_Test()
        {
            var tenantId = new Guid("42645233-3d72-4339-9adc-845321f8ada3");
            var id = await _fileStoringAppService.CreateAsync(new CreateContainerInput()
            {
                Provider = MinioFileProviderConfigurationNames.ProviderName,
                Name = "default1",
                TenantId = tenantId,
                IsMultiTenant = true,
                HttpAccess = true,
                Title = "test-container1",
                Items = new List<CreateContainerItemInput>()
                {
                    new CreateContainerItemInput(MinioFileProviderConfigurationNames.BucketName,"bucket1"),
                    new CreateContainerItemInput(MinioFileProviderConfigurationNames.EndPoint,"http://192.168.0.4:9000"),
                    new CreateContainerItemInput(MinioFileProviderConfigurationNames.AccessKey,"minioadmin"),
                    new CreateContainerItemInput(MinioFileProviderConfigurationNames.SecretKey,"minioadmin"),
                    new CreateContainerItemInput(MinioFileProviderConfigurationNames.WithSSL,"false"),
                    new CreateContainerItemInput(MinioFileProviderConfigurationNames.CreateBucketIfNotExists,"false")
                }
            });

            var container2 = await _fileStoringAppService.GetAsync(id, true);
            Assert.Equal("default1", container2.Name);
            Assert.Equal("Minio", container2.Provider);
            Assert.Equal(tenantId, container2.TenantId);
            Assert.True(container2.IsMultiTenant);
            Assert.True(container2.HttpAccess);
            Assert.Equal("test-container1", container2.Title);
            Assert.Equal(6, container2.Items.Count);

            await _fileStoringAppService.UpdateAsync(new UpdateContainerInput()
            {
                Id = id,
                Provider = "FileSystem",
                Name = "default2",
                IsMultiTenant = false,
                HttpAccess = false,
                Title = "test-container2",
                Items = new List<UpdateContainerItemInput>()
                {
                    new UpdateContainerItemInput(FileSystemFileProviderConfigurationNames.BasePath,"D:\\files"),
                    new UpdateContainerItemInput(FileSystemFileProviderConfigurationNames.AppendContainerNameToBasePath,"true"),
                    new UpdateContainerItemInput(FileSystemFileProviderConfigurationNames.HttpServer,"")
                }
            });

            using (_currentTenant.Change(tenantId))
            {
                var container3 = await _fileStoringAppService.GetAsync(id, true);
                Assert.Equal("default2", container3.Name);
                Assert.Equal("FileSystem", container3.Provider);
                Assert.Equal(tenantId, container3.TenantId);
                Assert.False(container3.IsMultiTenant);
                Assert.False(container3.HttpAccess);
                Assert.Equal("test-container2", container3.Title);
                Assert.Equal(3, container3.Items.Count);
            }
        }

        [Fact]
        public async Task Create_Delete_Test()
        {
            var tenantId = new Guid("42124112-3d72-4339-9adc-845321f8a2a0");
            var id = await _fileStoringAppService.CreateAsync(new CreateContainerInput()
            {
                Provider = MinioFileProviderConfigurationNames.ProviderName,
                Name = "default22",
                TenantId = tenantId,
                IsMultiTenant = true,
                HttpAccess = true,
                Title = "test-container22",
                Items = new List<CreateContainerItemInput>()
                {
                    new CreateContainerItemInput(MinioFileProviderConfigurationNames.BucketName,"bucket22"),
                    new CreateContainerItemInput(MinioFileProviderConfigurationNames.EndPoint,"http://192.168.0.4:9000"),
                    new CreateContainerItemInput(MinioFileProviderConfigurationNames.AccessKey,"minioadmin"),
                    new CreateContainerItemInput(MinioFileProviderConfigurationNames.SecretKey,"minioadmin"),
                    new CreateContainerItemInput(MinioFileProviderConfigurationNames.WithSSL,"false"),
                    new CreateContainerItemInput(MinioFileProviderConfigurationNames.CreateBucketIfNotExists,"false")
                }
            });

            using (_currentTenant.Change(tenantId))
            {
                var container = await _fileStoringAppService.GetByNameAsync("default22");
                Assert.NotNull(container);
            }

            var container2 = await _fileStoringAppService.GetAsync(id);
            Assert.NotNull(container2);

            await _fileStoringAppService.DeleteAsync(id);

            var container3 = await _fileStoringAppService.GetAsync(id);
            Assert.Null(container3);
        }



    }
}

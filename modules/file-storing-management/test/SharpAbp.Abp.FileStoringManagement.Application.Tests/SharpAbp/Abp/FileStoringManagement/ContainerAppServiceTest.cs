using SharpAbp.Abp.FileStoring;
using SharpAbp.Abp.FileStoring.FileSystem;
using SharpAbp.Abp.FileStoring.Minio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class ContainerAppServiceTest : FileStoringManagementApplicationTestBase
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly IDataFilter<IMultiTenant> _dataFilter;
        private readonly IContainerAppService _containerAppService;
        private readonly IFileContainerFactory _fileContainerFactory;
        public ContainerAppServiceTest()
        {
            _currentTenant = GetRequiredService<ICurrentTenant>();
            _dataFilter = GetRequiredService<IDataFilter<IMultiTenant>>();
            _containerAppService = GetRequiredService<IContainerAppService>();
            _fileContainerFactory = GetRequiredService<IFileContainerFactory>();
        }


        [Fact]
        public async Task CreateAsync_UpdateAsync_Test()
        {
            var tenantId = new Guid("42645233-3d72-4339-9adc-845321f8ada3");
            var tenantId2 = new Guid("d0ad04d5-2839-2c2a-1078-6b253678dceb");

            ContainerDto c1;
            using (_currentTenant.Change(tenantId))
            {
                c1 = await _containerAppService.CreateAsync(new CreateOrUpdateContainerDto()
                {
                    Provider = MinioFileProviderConfigurationNames.ProviderName,
                    Name = "default1",
                    IsMultiTenant = true,
                    HttpAccess = true,
                    Title = "test-container1",
                    Items = new List<CreateOrUpdateContainerItemDto>()
                    {
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.BucketName,"bucket1"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.EndPoint,"http://192.168.0.4:9000"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.AccessKey,"minioadmin"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.SecretKey,"minioadmin"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.WithSSL,"false"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.CreateBucketIfNotExists,"false")
                    }
                });
            }

            ContainerDto c2;
            using (_currentTenant.Change(tenantId2))
            {
                c2 = await _containerAppService.CreateAsync(new CreateOrUpdateContainerDto()
                {
                    Provider = MinioFileProviderConfigurationNames.ProviderName,
                    Name = "default2",
                    IsMultiTenant = true,
                    HttpAccess = true,
                    Title = "test-container2",
                    Items = new List<CreateOrUpdateContainerItemDto>()
                    {
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.BucketName,"bucket2"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.EndPoint,"http://192.168.0.4:9000"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.AccessKey,"minioadmin"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.SecretKey,"minioadmin"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.WithSSL,"false"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.CreateBucketIfNotExists,"false")
                    }
                });
            }

            using (_dataFilter.Disable())
            {
                var pagedContainers = await _containerAppService.GetPagedListAsync(new FileStoringContainerPagedRequestDto()
                {
                    SkipCount = 0,
                    MaxResultCount = 10
                });

                Assert.Equal(2, pagedContainers.TotalCount);
            }

            using (_currentTenant.Change(tenantId))
            {
                var pagedContainers = await _containerAppService.GetPagedListAsync(new FileStoringContainerPagedRequestDto()
                {
                    SkipCount = 0,
                    MaxResultCount = 10
                });

                Assert.Equal(1, pagedContainers.TotalCount);


                var container2 = await _containerAppService.GetAsync(c1.Id);
                Assert.Equal("default1", container2.Name);
                Assert.Equal("Minio", container2.Provider);
                Assert.Equal(tenantId, container2.TenantId);
                Assert.True(container2.IsMultiTenant);
                Assert.True(container2.HttpAccess);
                Assert.Equal("test-container1", container2.Title);
                Assert.Equal(6, container2.Items.Count);

                await _containerAppService.UpdateAsync(c1.Id, new CreateOrUpdateContainerDto()
                {
                    Provider = "FileSystem",
                    Name = "default2",
                    IsMultiTenant = false,
                    HttpAccess = false,
                    Title = "test-container2",
                    Items = new List<CreateOrUpdateContainerItemDto>()
                    {
                        new CreateOrUpdateContainerItemDto(FileSystemFileProviderConfigurationNames.BasePath,"D:\\files"),
                        new CreateOrUpdateContainerItemDto(FileSystemFileProviderConfigurationNames.AppendContainerNameToBasePath,"true"),
                        new CreateOrUpdateContainerItemDto(FileSystemFileProviderConfigurationNames.HttpServer,"")
                    }
                });

                var container3 = await _containerAppService.GetAsync(c1.Id);
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
        public async Task CreateAsync_DeleteAsync_Test()
        {
            var tenantId = new Guid("42124112-3d72-4339-9adc-845321f8a2a0");
            ContainerDto c1;
            using (_currentTenant.Change(tenantId))
            {
                c1 = await _containerAppService.CreateAsync(new CreateOrUpdateContainerDto()
                {
                    Provider = MinioFileProviderConfigurationNames.ProviderName,
                    Name = "default22",
                    IsMultiTenant = true,
                    HttpAccess = true,
                    Title = "test-container22",
                    Items = new List<CreateOrUpdateContainerItemDto>()
                    {
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.BucketName,"bucket22"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.EndPoint,"http://192.168.0.4:9000"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.AccessKey,"minioadmin"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.SecretKey,"minioadmin"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.WithSSL,"false"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.CreateBucketIfNotExists,"false")
                    }
                });
            }

            await Assert.ThrowsAsync<EntityNotFoundException<FileStoringContainer>>(() =>
            {
                return _containerAppService.GetAsync(c1.Id);
            });


            using (_currentTenant.Change(tenantId))
            {
                var container1 = await _containerAppService.FindByNameAsync("default22");
                Assert.NotNull(container1);

                var container2 = await _containerAppService.GetAsync(c1.Id);
                Assert.NotNull(container2);

                await _containerAppService.DeleteAsync(c1.Id);

                await Assert.ThrowsAsync<EntityNotFoundException<FileStoringContainer>>(() =>
                {
                    return _containerAppService.GetAsync(c1.Id);
                });
            }
        }

        [Fact]
        public async Task Get_Container_Test()
        {
            var tenantId = new Guid("42124112-3d72-2232-85a3-845321f8a2a0");
            using (_currentTenant.Change(tenantId))
            {
                var id = await _containerAppService.CreateAsync(new CreateOrUpdateContainerDto()
                {
                    Provider = MinioFileProviderConfigurationNames.ProviderName,
                    Name = "default22",
                    IsMultiTenant = false,
                    HttpAccess = true,
                    Title = "test-container22",
                    Items = new List<CreateOrUpdateContainerItemDto>()
                    {
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.BucketName,"bucket22"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.EndPoint,"http://192.168.0.4:9000"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.AccessKey,"minioadmin"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.SecretKey,"minioadmin"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.WithSSL,"false"),
                        new CreateOrUpdateContainerItemDto(MinioFileProviderConfigurationNames.CreateBucketIfNotExists,"false")
                    }
                });

                var container = _fileContainerFactory.Create("default22");

                var configuration = container.GetConfiguration();
                var minioConfiguration = configuration.GetMinioConfiguration();
                Assert.Equal("bucket22", minioConfiguration.BucketName);
                Assert.Equal("minioadmin", minioConfiguration.AccessKey);
                Assert.Equal("minioadmin", minioConfiguration.SecretKey);
                Assert.False(minioConfiguration.WithSSL);
                Assert.False(minioConfiguration.CreateBucketIfNotExists);

                var container2 = _fileContainerFactory.Create("default22");
                var configuration2 = container2.GetConfiguration();
                var minioConfiguration2 = configuration2.GetMinioConfiguration();
                Assert.Equal("bucket22", minioConfiguration2.BucketName);
                Assert.Equal("minioadmin", minioConfiguration2.AccessKey);
                Assert.Equal("minioadmin", minioConfiguration2.SecretKey);
                Assert.False(minioConfiguration2.WithSSL);
                Assert.False(minioConfiguration2.CreateBucketIfNotExists);
            }

        }

    }
}

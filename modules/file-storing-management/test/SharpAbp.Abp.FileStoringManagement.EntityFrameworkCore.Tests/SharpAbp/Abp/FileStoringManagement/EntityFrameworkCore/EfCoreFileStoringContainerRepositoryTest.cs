using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace SharpAbp.Abp.FileStoringManagement.EntityFrameworkCore
{
    public class EfCoreFileStoringContainerRepositoryTest : FileStoringManagementEntityFrameworkCoreTestBase
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly IDataFilter<IMultiTenant> _dataFilter;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IFileStoringContainerRepository _fileStoringContainerRepository;
        public EfCoreFileStoringContainerRepositoryTest()
        {
            _currentTenant = GetRequiredService<ICurrentTenant>();
            _dataFilter = GetRequiredService<IDataFilter<IMultiTenant>>();
            _fileStoringContainerRepository = GetRequiredService<IFileStoringContainerRepository>();
            _guidGenerator = GetRequiredService<IGuidGenerator>();
        }

        [Fact]
        public async Task FindByNameAsync_Test()
        {

            var tenantId = new Guid("446a5211-3d72-4339-9adc-845151f8ada0");
            var container = new FileStoringContainer(_guidGenerator.Create())
            {
                TenantId = tenantId,
                Name = "default",
                Title = "test-container",
                ProviderName = "Minio",
                HttpSupport = true,
                State = 1,
                Describe = ""
            };
            container.Items.Add(new FileStoringContainerItem(_guidGenerator.Create())
            {
                Name = "Minio.BucketName",
                TypeName = typeof(string).FullName,
                Value = "bucket1",
                ContainerId = container.Id
            });

            container.Items.Add(new FileStoringContainerItem(_guidGenerator.Create())
            {
                Name = "Minio.EndPoint",
                TypeName = typeof(string).FullName,
                Value = "http://127.0.0.1:9094",
                ContainerId = container.Id
            });

            container.Items.Add(new FileStoringContainerItem(_guidGenerator.Create())
            {
                Name = "Minio.AccessKey",
                TypeName = typeof(string).FullName,
                Value = "minioadmin",
                ContainerId = container.Id
            });

            container.Items.Add(new FileStoringContainerItem(_guidGenerator.Create())
            {
                Name = "Minio.SecretKey",
                TypeName = typeof(string).FullName,
                Value = "minioadmin",
                ContainerId = container.Id
            });

            container.Items.Add(new FileStoringContainerItem(_guidGenerator.Create())
            {
                Name = "Minio.WithSSL",
                TypeName = typeof(bool).FullName,
                Value = "true",
                ContainerId = container.Id
            });

            container.Items.Add(new FileStoringContainerItem(_guidGenerator.Create())
            {
                Name = "Minio.CreateBucketIfNotExists",
                TypeName = typeof(bool).FullName,
                Value = "true",
                ContainerId = container.Id
            });

            using (_currentTenant.Change(tenantId))
            {
                await _fileStoringContainerRepository.InsertAsync(container, true);
            }

            using (_currentTenant.Change(null))
            {
                var queryContainer1 = await _fileStoringContainerRepository.FindByNameAsync("default");
                Assert.Null(queryContainer1);
            }

            using (_currentTenant.Change(tenantId))
            {
                var queryContainer1 = await _fileStoringContainerRepository.FindByNameAsync("default");
                Assert.Equal(container.Id, queryContainer1.Id);
                Assert.Equal(container.Name, queryContainer1.Name);
                Assert.Equal(container.Items.Count, queryContainer1.Items.Count);
            }

            using (_dataFilter.Disable())
            {
                var queryContainer1 = await _fileStoringContainerRepository.FindByNameAsync("default");
                Assert.Equal(container.Id, queryContainer1.Id);
                Assert.Equal(container.Name, queryContainer1.Name);
                Assert.Equal(container.Items.Count, queryContainer1.Items.Count);
            }

        }


    }
}

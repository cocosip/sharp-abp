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
            var container = new FileStoringContainer(
                _guidGenerator.Create(),
                null,
                true,
                "Minio",
                "default",
                "test-container",
                true);

            container.Items.Add(new FileStoringContainerItem(
                _guidGenerator.Create(),
                "Minio.BucketName",
                "bucket1",
                container.Id));

            container.Items.Add(new FileStoringContainerItem(
                _guidGenerator.Create(),
                "Minio.EndPoint",
                "http://127.0.0.1:9094",
                container.Id));

            container.Items.Add(new FileStoringContainerItem(
                _guidGenerator.Create(),
                "Minio.AccessKey",
                "minioadmin",
                container.Id));

            container.Items.Add(new FileStoringContainerItem(
                _guidGenerator.Create(),
                "Minio.SecretKey",
                "minioadmin",
                container.Id));

            container.Items.Add(new FileStoringContainerItem(
                _guidGenerator.Create(), 
                "Minio.WithSSL", 
                "true", 
                container.Id));

            container.Items.Add(new FileStoringContainerItem(
                _guidGenerator.Create(), 
                "Minio.CreateBucketIfNotExists",
                "true",
                container.Id));

            using (_currentTenant.Change(tenantId))
            {
                await _fileStoringContainerRepository.InsertAsync(container, true);
            }

            using (_currentTenant.Change(null))
            {
                var queryContainer1 = await _fileStoringContainerRepository.FindAsync("default");
                Assert.Null(queryContainer1);
            }

            using (_currentTenant.Change(tenantId))
            {
                var queryContainer1 = await _fileStoringContainerRepository.FindAsync("default");
                Assert.Equal(container.Id, queryContainer1.Id);
                Assert.Equal(container.Name, queryContainer1.Name);
                Assert.Equal(container.Items.Count, queryContainer1.Items.Count);

                var queryContainer2 = await _fileStoringContainerRepository.GetAsync(container.Id, includeDetails: true);
                Assert.NotNull(queryContainer2);
                Assert.Equal(container.Items.Count, queryContainer2.Items.Count);

            }

            using (_dataFilter.Disable())
            {
                var queryContainer1 = await _fileStoringContainerRepository.FindAsync("default");
                Assert.Equal(container.Id, queryContainer1.Id);
                Assert.Equal(container.Name, queryContainer1.Name);
                Assert.Equal(container.Items.Count, queryContainer1.Items.Count);
            }

        }


    }
}

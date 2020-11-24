using System.Threading.Tasks;
using Volo.Abp.Guids;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.Abp.FileStoringManagement.EntityFrameworkCore
{
    public class EfCoreFileStoringContainerRepositoryTest : FileStoringManagementEntityFrameworkCoreTestBase
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IFileStoringContainerRepository _fileStoringContainerRepository;
        public EfCoreFileStoringContainerRepositoryTest()
        {
            _fileStoringContainerRepository = GetRequiredService<IFileStoringContainerRepository>();
            _guidGenerator = GetRequiredService<IGuidGenerator>();
        }

        [Fact]
        public async Task FindByNameAsync_Test()
        {
            var container = new FileStoringContainer(_guidGenerator.Create())
            {
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

            await _fileStoringContainerRepository.InsertAsync(container, true);
            var queryContainer = await _fileStoringContainerRepository.FindByNameAsync("default");
            Assert.Equal(container.Id, queryContainer.Id);
            Assert.Equal(container.Name, queryContainer.Name);
            Assert.Equal(container.Items.Count, queryContainer.Items.Count);
        }

    }
}

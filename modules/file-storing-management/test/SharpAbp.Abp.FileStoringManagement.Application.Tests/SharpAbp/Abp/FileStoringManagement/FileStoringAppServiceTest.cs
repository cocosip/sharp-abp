using Xunit;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringAppServiceTest : FileStoringManagementApplicationTestBase
    {
        private readonly IFileStoringAppService _fileStoringAppService;
        public FileStoringAppServiceTest()
        {
            _fileStoringAppService = GetRequiredService<IFileStoringAppService>();
        }

        [Fact]
        public void GetProviders_Test()
        {
            var providers = _fileStoringAppService.GetProviders();
            Assert.Equal(6, providers.Count);
        }

    }
}

using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderDependencyInjectionTest : AbpFileStoringAllTestBase
    {
        [Theory]
        [InlineData("default")]
        [InlineData("obs-container")]
        [InlineData("azure-container")]
        [InlineData("aws-container")]
        [InlineData("fastdfs-container")]
        [InlineData("filesystem-container")]
        [InlineData("minio-container")]
        [InlineData("ks3-container")]
        [InlineData("s3-container")]
        public void Providers_Should_Resolve_With_The_Client_Cache(string containerName)
        {
            var provider = GetRequiredService<IFileProviderSelector>().Get(containerName);

            Assert.NotNull(provider);
        }
    }
}

using SharpAbp.Abp.FileStoring;
using SharpAbp.Abp.FileStoring.Aws;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class AwsContainerConfigurationTest : FileStoringManagementApplicationTestBase
    {
        private readonly IContainerAppService _containerAppService;
        private readonly IFileContainerFactory _fileContainerFactory;

        public AwsContainerConfigurationTest()
        {
            _containerAppService = GetRequiredService<IContainerAppService>();
            _fileContainerFactory = GetRequiredService<IFileContainerFactory>();
        }

        [Fact]
        public async Task MaxConnectionsPerServer_Should_Round_Trip_From_Database()
        {
            await _containerAppService.CreateAsync(new CreateOrUpdateContainerDto
            {
                Provider = AwsFileProviderConfigurationNames.ProviderName,
                Name = "aws-max-connections",
                IsMultiTenant = false,
                HttpAccess = true,
                Title = "aws-max-connections",
                Items = new List<CreateOrUpdateContainerItemDto>
                {
                    new(AwsFileProviderConfigurationNames.ContainerName, "bucket"),
                    new(AwsFileProviderConfigurationNames.AccessKeyId, "ak"),
                    new(AwsFileProviderConfigurationNames.SecretAccessKey, "sk"),
                    new(AwsFileProviderConfigurationNames.UseCredentials, "false"),
                    new(AwsFileProviderConfigurationNames.UseTemporaryCredentials, "false"),
                    new(AwsFileProviderConfigurationNames.UseTemporaryFederatedCredentials, "false"),
                    new(AwsFileProviderConfigurationNames.ProfileName, ""),
                    new(AwsFileProviderConfigurationNames.ProfilesLocation, ""),
                    new(AwsFileProviderConfigurationNames.DurationSeconds, "3600"),
                    new(AwsFileProviderConfigurationNames.Name, "bucket"),
                    new(AwsFileProviderConfigurationNames.Policy, ""),
                    new(AwsFileProviderConfigurationNames.Region, "us-east-1"),
                    new(AwsFileProviderConfigurationNames.CreateContainerIfNotExists, "false"),
                    new(AwsFileProviderConfigurationNames.MaxConnectionsPerServer, "77"),
                    new(AwsFileProviderConfigurationNames.TemporaryCredentialsCacheKey, "cache")
                }
            });

            var configuration = _fileContainerFactory
                .Create("aws-max-connections")
                .GetConfiguration()
                .GetAwsConfiguration();

            Assert.Equal(77, configuration.MaxConnectionsPerServer);
        }
    }
}

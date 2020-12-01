using SharpAbp.Abp.FileStoring;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class DatabaseFileContainerConfigurationProvider : IFileContainerConfigurationProvider
    {
        protected IFileContainerConfigurationConverter FileContainerConfigurationConverter { get; }
        protected IDistributedCache<FileContainerConfiguration> ConfigurationCache { get; }
        protected IFileStoringContainerRepository FileStoringContainerRepository { get; }

        public DatabaseFileContainerConfigurationProvider(
            IFileContainerConfigurationConverter fileContainerConfigurationConverter,
            IDistributedCache<FileContainerConfiguration> configurationCache,
            IFileStoringContainerRepository fileStoringContainerRepository)
        {
            FileContainerConfigurationConverter = fileContainerConfigurationConverter;
            ConfigurationCache = configurationCache;
            FileStoringContainerRepository = fileStoringContainerRepository;
        }


        public FileContainerConfiguration Get(string name)
        {
            return AsyncHelper.RunSync(() =>
            {
                return GetConfigurationInternal(name);
            });
        }

        protected virtual async Task<FileContainerConfiguration> GetConfigurationInternal(string name)
        {
            var configuration = await ConfigurationCache.GetAsync(name);
            if (configuration == null)
            {
                //Read from db
                var container = await FileStoringContainerRepository.FindByNameAsync(name);
                if (container != null)
                {
                    configuration = FileContainerConfigurationConverter.ToConfiguration(container);
                    await ConfigurationCache.SetAsync(name, configuration);
                }
            }

            return configuration;
        }

    }
}

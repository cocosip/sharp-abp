using SharpAbp.Abp.FileStoring;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class DatabaseFileContainerConfigurationProvider : IFileContainerConfigurationProvider
    {
        protected IFileContainerConfigurationConverter FileContainerConfigurationConverter { get; }
        protected IDistributedCache<FileStoringContainer> ContainerCache { get; }
        protected IFileStoringContainerRepository FileStoringContainerRepository { get; }

        public DatabaseFileContainerConfigurationProvider(
            IFileContainerConfigurationConverter fileContainerConfigurationConverter,
            IDistributedCache<FileStoringContainer> containerCache,
            IFileStoringContainerRepository fileStoringContainerRepository)
        {
            FileContainerConfigurationConverter = fileContainerConfigurationConverter;
            ContainerCache = containerCache;
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
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var container = await ContainerCache.GetOrAddAsync(
                name,
                async () => await FileStoringContainerRepository.FindAsync(name)
                );

            var configuration = FileContainerConfigurationConverter.ToConfiguration(container);

            return configuration;
        }

        //protected virtual async Task<FileContainerConfiguration> GetConfigurationInternal(string name)
        //{
        //    var configuration = await ConfigurationCache.GetAsync(name, hideErrors: false);
        //    if (configuration == null)
        //    {
        //        //Read from db
        //        var container = await FileStoringContainerRepository.FindAsync(name);
        //        if (container != null)
        //        {
        //            configuration = FileContainerConfigurationConverter.ToConfiguration(container);
        //            await ConfigurationCache.SetAsync(name, configuration, hideErrors: false);
        //        }
        //    }

        //    return configuration;
        //}
         
    }
}

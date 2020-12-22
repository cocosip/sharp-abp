using SharpAbp.Abp.FileStoring;
using Volo.Abp;
using Volo.Abp.Caching;

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
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return GetConfigurationInternal(name);
        }

        protected virtual FileContainerConfiguration GetConfigurationInternal(string name)
        {
            var container = ContainerCache.GetOrAdd(
                name,
                () =>
                {
                    return FileStoringContainerRepository.Find(name, true);
                },
                hideErrors: false);

            if (container != null)
            {
                return FileContainerConfigurationConverter.ToConfiguration(container);
            }

            return default;
        }


    }
}

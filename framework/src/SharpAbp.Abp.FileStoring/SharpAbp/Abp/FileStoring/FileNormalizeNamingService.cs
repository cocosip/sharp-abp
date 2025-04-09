using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring
{
    public class FileNormalizeNamingService : IFileNormalizeNamingService, ITransientDependency
    {
        protected IServiceProvider ServiceProvider { get; }
        public FileNormalizeNamingService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public virtual FileNormalizeNaming NormalizeNaming(
            FileContainerConfiguration configuration,
            string? containerName,
            string? fileName)
        {

            if (!configuration.NamingNormalizers.Any())
            {
                return new FileNormalizeNaming(containerName, fileName);
            }

            using var scope = ServiceProvider.CreateScope();
            foreach (var normalizerType in configuration.NamingNormalizers)
            {
                var normalizer = scope.ServiceProvider
                    .GetRequiredService(normalizerType)
                    .As<IFileNamingNormalizer>();

                containerName = containerName.IsNullOrWhiteSpace() ? containerName : normalizer.NormalizeContainerName(containerName);
                fileName = fileName.IsNullOrWhiteSpace() ? fileName : normalizer.NormalizeFileName(fileName);
            }

            return new FileNormalizeNaming(containerName, fileName);
        }

        public string NormalizeContainerName(FileContainerConfiguration configuration, string containerName)
        {
            if (!configuration.NamingNormalizers.Any())
            {
                return containerName;
            }

            return NormalizeNaming(configuration, containerName, null).ContainerName!;
        }

        public string NormalizeFileName(FileContainerConfiguration configuration, string fileName)
        {
            if (!configuration.NamingNormalizers.Any())
            {
                return fileName;
            }

            return NormalizeNaming(configuration, null, fileName).FileName!;
        }
    }
}

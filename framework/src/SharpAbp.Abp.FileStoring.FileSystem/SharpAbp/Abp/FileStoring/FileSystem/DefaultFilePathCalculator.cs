using System.IO;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring.FileSystem
{
    public class DefaultFilePathCalculator : IFilePathCalculator, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }
        protected AbpFileStoringAbstractionsOptions Options { get; }

        public DefaultFilePathCalculator(
            ICurrentTenant currentTenant,
            IOptions<AbpFileStoringAbstractionsOptions> options)
        {
            CurrentTenant = currentTenant;
            Options = options.Value;
        }

        public virtual string Calculate(FileProviderArgs args)
        {
            var fileSystemConfiguration = args.Configuration.GetFileSystemConfiguration();
            var filePath = fileSystemConfiguration.BasePath;

            if (Options.FilePathStrategy == FilePathGenerationStrategy.DirectFileId)
            {
                // Use FileId directly without tenant path prefix
                if (fileSystemConfiguration.AppendContainerNameToBasePath)
                {
                    filePath = Path.Combine(filePath, args.ContainerName);
                }

                filePath = Path.Combine(filePath, args.FileId);
            }
            else
            {
                // Use tenant-based path structure (default behavior)
                if (CurrentTenant.Id == null)
                {
                    filePath = Path.Combine(filePath, "host");
                }
                else
                {
                    filePath = Path.Combine(filePath, "tenants", CurrentTenant.Id.Value.ToString("D"));
                }

                if (fileSystemConfiguration.AppendContainerNameToBasePath)
                {
                    filePath = Path.Combine(filePath, args.ContainerName);
                }

                filePath = Path.Combine(filePath, args.FileId);
            }

            return filePath;
        }
    }
}

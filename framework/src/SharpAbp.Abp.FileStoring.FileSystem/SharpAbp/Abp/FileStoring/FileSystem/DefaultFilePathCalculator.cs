using System.IO;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring.FileSystem
{
    public class DefaultFilePathCalculator : IFilePathCalculator, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }
        protected IFilePathContextAccessor FilePathContextAccessor { get; }
        protected AbpFileStoringAbstractionsOptions Options { get; }

        public DefaultFilePathCalculator(
            ICurrentTenant currentTenant,
            IFilePathContextAccessor filePathContextAccessor,
            IOptions<AbpFileStoringAbstractionsOptions> options)
        {
            CurrentTenant = currentTenant;
            FilePathContextAccessor = filePathContextAccessor;
            Options = options.Value;
        }

        public virtual string Calculate(FileProviderArgs args)
        {
            var fileSystemConfiguration = args.Configuration.GetFileSystemConfiguration();
            var filePath = fileSystemConfiguration.BasePath;
            var builderOptions = Options.FilePathBuilder;
            var context = FilePathContextAccessor.Current;

            if (Options.FilePathStrategy == FilePathGenerationStrategy.DirectFileId)
            {
                if (fileSystemConfiguration.AppendContainerNameToBasePath)
                {
                    filePath = Path.Combine(filePath, args.ContainerName);
                }

                filePath = Path.Combine(filePath, args.FileId);
            }
            else
            {
                // Optional prefix
                var prefix = ResolvePrefixSegment(builderOptions, context);
                if (!string.IsNullOrEmpty(prefix))
                {
                    filePath = Path.Combine(filePath, prefix!);
                }

                // Tenant segment
                if (CurrentTenant.Id == null)
                {
                    filePath = Path.Combine(filePath, builderOptions.HostSegment);
                }
                else
                {
                    var identifier = builderOptions.TenantIdentifierFactory != null
                        ? builderOptions.TenantIdentifierFactory(CurrentTenant.Id.Value, CurrentTenant.Name, context)
                        : CurrentTenant.Id.Value.ToString("D");

                    filePath = Path.Combine(filePath, builderOptions.TenantsSegment, identifier);
                }

                if (fileSystemConfiguration.AppendContainerNameToBasePath)
                {
                    filePath = Path.Combine(filePath, args.ContainerName);
                }

                filePath = Path.Combine(filePath, args.FileId);
            }

            return filePath;
        }

        protected virtual string? ResolvePrefixSegment(FilePathBuilderOptions options, FilePathContext? context)
        {
            if (options.PrefixFactory != null)
            {
                return options.PrefixFactory(context);
            }

            if (!string.IsNullOrEmpty(context?.Prefix))
            {
                return context!.Prefix;
            }

            return options.Prefix;
        }
    }
}

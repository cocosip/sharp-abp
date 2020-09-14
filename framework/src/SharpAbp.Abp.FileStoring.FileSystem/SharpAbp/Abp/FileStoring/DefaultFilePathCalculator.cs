using System.IO;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring
{
    public class DefaultFilePathCalculator : IFilePathCalculator, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }

        public DefaultFilePathCalculator(ICurrentTenant currentTenant)
        {
            CurrentTenant = currentTenant;
        }

        public virtual string Calculate(FileProviderArgs args)
        {
            var fileSystemConfiguration = args.Configuration.GetFileSystemConfiguration();
            var filePath = fileSystemConfiguration.BasePath;

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

            return filePath;
        }
    }
}

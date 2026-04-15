using JetBrains.Annotations;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public class ImplementedFileContainerConfigurationProvider : IFileContainerConfigurationProvider
    {
        public FileContainerConfiguration Get([NotNull] string name)
        {
            throw new AbpException("IFileContainerConfigurationProvider is not available in SharpAbp.Abp.FileStoring.Abstractions. Reference SharpAbp.Abp.FileStoring to resolve container configurations.");
        }
    }
}

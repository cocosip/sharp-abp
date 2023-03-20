using JetBrains.Annotations;
using System;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring
{
    public class ImplementedFileContainerConfigurationProvider : IFileContainerConfigurationProvider, ITransientDependency
    {
        public FileContainerConfiguration Get([NotNull] string name)
        {
            throw new NotImplementedException();
        }
    }
}

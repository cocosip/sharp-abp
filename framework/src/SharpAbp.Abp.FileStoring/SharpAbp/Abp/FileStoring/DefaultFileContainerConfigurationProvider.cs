using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring
{
    public class DefaultFileContainerConfigurationProvider : IFileContainerConfigurationProvider, ITransientDependency
    {
        protected AbpFileStoringOptions Options { get; }

        public DefaultFileContainerConfigurationProvider(IOptions<AbpFileStoringOptions> options)
        {
            Options = options.Value;
        }
        
        public virtual FileContainerConfiguration Get(string name)
        {
            return Options.Containers.GetConfiguration(name);
        }
    }
}
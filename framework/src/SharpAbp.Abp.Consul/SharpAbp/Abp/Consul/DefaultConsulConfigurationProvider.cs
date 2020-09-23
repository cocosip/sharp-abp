using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Consul
{
    public class DefaultConsulConfigurationProvider : IConsulConfigurationProvider, ITransientDependency
    {
        protected AbpConsulOptions Options { get; }

        public DefaultConsulConfigurationProvider(IOptions<AbpConsulOptions> options)
        {
            Options = options.Value;
        }

        [NotNull]
        public virtual ConsulConfiguration Get([NotNull] string name)
        {
            return Options.Consuls.GetConfiguration(name);
        }
    }
}

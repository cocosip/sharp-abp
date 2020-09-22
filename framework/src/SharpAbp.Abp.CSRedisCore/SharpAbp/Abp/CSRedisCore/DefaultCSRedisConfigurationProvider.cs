using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.CSRedisCore
{
    public class DefaultCSRedisConfigurationProvider : ICSRedisConfigurationProvider, ITransientDependency
    {
        protected AbpCSRedisOptions Options { get; }

        public DefaultCSRedisConfigurationProvider(IOptions<AbpCSRedisOptions> options)
        {
            Options = options.Value;
        }


        [NotNull]
        public virtual CSRedisConfiguration Get([NotNull] string name)
        {
            return Options.Clients.GetConfiguration(name);
        }

    }
}

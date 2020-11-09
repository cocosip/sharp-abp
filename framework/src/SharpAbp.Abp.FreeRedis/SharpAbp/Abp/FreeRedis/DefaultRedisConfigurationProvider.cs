using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FreeRedis
{
    public class DefaultRedisConfigurationProvider : IRedisConfigurationProvider, ITransientDependency
    {
        protected AbpFreeRedisOptions Options { get; }

        public DefaultRedisConfigurationProvider(IOptions<AbpFreeRedisOptions> options)
        {
            Options = options.Value;
        }


        [NotNull]
        public virtual FreeRedisConfiguration Get([NotNull] string name)
        {
            return Options.Clients.GetConfiguration(name);
        }

    }
}

using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.CSRedisCore
{
    public class DefaultCSRedisConfigurationSelector : ICSRedisConfigurationSelector, ITransientDependency
    {
        protected AbpCSRedisOptions Options { get; }

        public DefaultCSRedisConfigurationSelector(IOptions<AbpCSRedisOptions> options)
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

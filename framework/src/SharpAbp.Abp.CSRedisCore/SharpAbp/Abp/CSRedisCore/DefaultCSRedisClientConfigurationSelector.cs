using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using System.Linq;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.CSRedisCore
{
    public class DefaultCSRedisClientConfigurationSelector : ICSRedisClientConfigurationSelector, ITransientDependency
    {
        private readonly CSRedisOptions _options;

        public DefaultCSRedisClientConfigurationSelector(IOptions<CSRedisOptions> options)
        {
            _options = options.Value;
        }


        [NotNull]
        public virtual CSRedisClientConfiguration Get([NotNull] string name)
        {
            var configuration = _options.Configurations.FirstOrDefault(x => x.Name == name);

            if (configuration == null)
            {
                throw new AbpException($"Could not find 'CSRedisCoreConfiguration' by name '{name}'");
            }

            return configuration;
        }

    }
}

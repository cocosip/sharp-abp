using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using System.Linq;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.CSRedisCore
{
    public class DefaultCSRedisClientConfigurationSelector : ICSRedisClientConfigurationSelector, ITransientDependency
    {
        private readonly CSRedisOption _option;

        public DefaultCSRedisClientConfigurationSelector(IOptions<CSRedisOption> options)
        {
            _option = options.Value;
        }


        [NotNull]
        public virtual CSRedisClientConfiguration Get([NotNull] string name)
        {
            var configuration = _option.Configurations.FirstOrDefault(x => x.Name == name);

            if (configuration == null)
            {
                throw new AbpException($"Could not find 'CSRedisCoreConfiguration' by name '{name}'");
            }

            return configuration;
        }

    }
}

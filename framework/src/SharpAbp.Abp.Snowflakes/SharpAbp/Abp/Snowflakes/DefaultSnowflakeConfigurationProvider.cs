using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Snowflakes
{
    public class DefaultSnowflakeConfigurationProvider : ISnowflakeConfigurationProvider, ITransientDependency
    {
        protected AbpSnowflakesOptions Options { get; }
        public DefaultSnowflakeConfigurationProvider(IOptions<AbpSnowflakesOptions> options)
        {
            Options = options.Value;
        }

        [NotNull]
        public virtual SnowflakeConfiguration Get([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return Options.Snowflakes.GetConfiguration(name);
        }
    }
}

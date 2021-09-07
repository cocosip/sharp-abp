using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Snowflakes
{
    public class DefaultSnowflakeFactory : ISnowflakeFactory, ISingletonDependency
    {
        private readonly ConcurrentDictionary<string, Snowflake> _snowflakes;
        
        protected ILogger Logger { get; }
        protected ISnowflakeConfigurationProvider ConfigurationProvider { get; }
        public DefaultSnowflakeFactory(
            ILogger<DefaultSnowflakeFactory> logger,
            ISnowflakeConfigurationProvider configurationProvider)
        {
            Logger = logger;
            ConfigurationProvider = configurationProvider;

            _snowflakes = new ConcurrentDictionary<string, Snowflake>();
        }

        /// <summary>
        /// Get or create a snowflake by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Snowflake Get([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var snowflake = _snowflakes.GetOrAdd(name, n => Create(n));
            
            if (snowflake == null)
            {
                throw new AbpException($"Could not find snowflake by name '{name}'.");
            }

            return snowflake;
        }

        /// <summary>
        /// Get default snowflake
        /// </summary>
        /// <returns></returns>
        public virtual Snowflake GetDefault()
        {
            return Get(DefaultSnowflake.Name);
        }


        /// <summary>
        /// Create snowflake by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected virtual Snowflake Create(string name)
        {
            var configuration = ConfigurationProvider.Get(name);
            var snowflake = new Snowflake(
                configuration.Twepoch,
                configuration.WorkerIdBits,
                configuration.DatacenterIdBits,
                configuration.SequenceBits,
                configuration.WorkerId,
                configuration.DatacenterId);
            return snowflake;
        }

    }
}

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.Snowflakes
{
    public class SnowflakeConfigurations
    {
        private SnowflakeConfiguration Default => GetConfiguration<DefaultSnowflake>();

        private readonly Dictionary<string, SnowflakeConfiguration> _snowflakes;

        public SnowflakeConfigurations()
        {
            _snowflakes = new Dictionary<string, SnowflakeConfiguration>
            {
                //Add default snowflake
                [SnowflakeNameAttribute.GetSnowflakeName<DefaultSnowflake>()] = new SnowflakeConfiguration()
            };
        }

        public SnowflakeConfigurations Configure<TSnowflake>(
            Action<SnowflakeConfiguration> configureAction)
        {
            return Configure(
                SnowflakeNameAttribute.GetSnowflakeName<TSnowflake>(),
                configureAction
            );
        }

        public SnowflakeConfigurations Configure(
            [NotNull] string name,
            [NotNull] Action<SnowflakeConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _snowflakes.GetOrAdd(
                    name,
                    () => new SnowflakeConfiguration()
                )
            );

            return this;
        }

        public SnowflakeConfigurations ConfigureDefault(Action<SnowflakeConfiguration> configureAction)
        {
            configureAction(Default);
            return this;
        }

        public SnowflakeConfigurations ConfigureAll(Action<string, SnowflakeConfiguration> configureAction)
        {
            foreach (var keyValuePair in _snowflakes)
            {
                configureAction(keyValuePair.Key, keyValuePair.Value);
            }

            return this;
        }

        [NotNull]
        public SnowflakeConfiguration GetConfiguration<TSnowflake>()
        {
            return GetConfiguration(SnowflakeNameAttribute.GetSnowflakeName<TSnowflake>());
        }

        [NotNull]
        public SnowflakeConfiguration GetConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            return _snowflakes.GetOrDefault(name) ??
                   Default;
        }
    }
}

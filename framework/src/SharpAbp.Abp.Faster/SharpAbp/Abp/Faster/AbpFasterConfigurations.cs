using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.Faster
{
    public class AbpFasterConfigurations
    {
        private AbpFasterConfiguration Default => GetConfiguration<DefaultFasterLog>();

        private readonly Dictionary<string, AbpFasterConfiguration> _configurations;

        public AbpFasterConfigurations()
        {
            _configurations = [];
        }

        public AbpFasterConfigurations Configure<TLog>(Action<AbpFasterConfiguration> configureAction)
        {
            return Configure(
                FasterLogNameAttribute.GetLogName<TLog>(),
                configureAction
            );
        }

        public AbpFasterConfigurations Configure(
            [NotNull] string name,
            [NotNull] Action<AbpFasterConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _configurations.GetOrAdd(
                    name,
                    () => new AbpFasterConfiguration()
                    )
                );

            return this;
        }

        public AbpFasterConfigurations ConfigureDefault(Action<AbpFasterConfiguration> configureAction)
        {
            configureAction(Default);
            return this;
        }

        public AbpFasterConfigurations ConfigureAll(Action<string, AbpFasterConfiguration> configureAction)
        {
            foreach (var keyValuePair in _configurations)
            {
                configureAction(keyValuePair.Key, keyValuePair.Value);
            }

            return this;
        }

        [NotNull]
        public AbpFasterConfiguration GetConfiguration<TLog>()
        {
            return GetConfiguration(FasterLogNameAttribute.GetLogName<TLog>());
        }

        [NotNull]
        public AbpFasterConfiguration GetConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return _configurations.GetOrDefault(name) ??
                   Default;
        }
    }
}

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.CSRedisCore
{
    public class CSRedisConfigurations
    {
        private CSRedisConfiguration Default => GetConfiguration<DefaultClient>();

        private readonly Dictionary<string, CSRedisConfiguration> _clients;

        public CSRedisConfigurations()
        {
            _clients = new Dictionary<string, CSRedisConfiguration>()
            {
                [CSRedisClientNameAttribute.GetClientName<DefaultClient>()] = new CSRedisConfiguration()
            };
        }

        public CSRedisConfigurations Configure<TContainer>(
          Action<CSRedisConfiguration> configureAction)
        {
            return Configure(
                CSRedisClientNameAttribute.GetClientName<TContainer>(),
                configureAction
            );
        }

        public CSRedisConfigurations Configure(
            [NotNull] string name,
            [NotNull] Action<CSRedisConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _clients.GetOrAdd(
                    name,
                    () => new CSRedisConfiguration()
                    {
                        ConnectionString = Default.ConnectionString,
                        Mode = Default.Mode,
                        Sentinels = Default.Sentinels,
                        ReadOnly = Default.ReadOnly
                    }
                )
            );

            return this;
        }

        public CSRedisConfigurations ConfigureDefault(Action<CSRedisConfiguration> configureAction)
        {
            configureAction(Default);
            return this;
        }

        public CSRedisConfigurations ConfigureAll(Action<string, CSRedisConfiguration> configureAction)
        {
            foreach (var client in _clients)
            {
                configureAction(client.Key, client.Value);
            }

            return this;
        }

        [NotNull]
        public CSRedisConfiguration GetConfiguration<TContainer>()
        {
            return GetConfiguration(CSRedisClientNameAttribute.GetClientName<TContainer>());
        }

        [NotNull]
        public CSRedisConfiguration GetConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            return _clients.GetOrDefault(name) ??
                   Default;
        }

    }
}

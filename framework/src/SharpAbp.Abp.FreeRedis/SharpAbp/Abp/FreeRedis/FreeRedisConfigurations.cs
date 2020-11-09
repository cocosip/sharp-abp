using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.FreeRedis
{
    public class FreeRedisConfigurations
    {
        private FreeRedisConfiguration Default => GetConfiguration<DefaultClient>();

        private readonly Dictionary<string, FreeRedisConfiguration> _clients;

        public FreeRedisConfigurations()
        {
            _clients = new Dictionary<string, FreeRedisConfiguration>()
            {
                [RedisClientNameAttribute.GetClientName<DefaultClient>()] = new FreeRedisConfiguration()
            };
        }

        public FreeRedisConfigurations Configure<TContainer>(
          Action<FreeRedisConfiguration> configureAction)
        {
            return Configure(
                RedisClientNameAttribute.GetClientName<TContainer>(),
                configureAction
            );
        }

        public FreeRedisConfigurations Configure(
            [NotNull] string name,
            [NotNull] Action<FreeRedisConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _clients.GetOrAdd(
                    name,
                    () => new FreeRedisConfiguration()
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

        public FreeRedisConfigurations ConfigureDefault(Action<FreeRedisConfiguration> configureAction)
        {
            configureAction(Default);
            return this;
        }

        public FreeRedisConfigurations ConfigureAll(Action<string, FreeRedisConfiguration> configureAction)
        {
            foreach (var client in _clients)
            {
                configureAction(client.Key, client.Value);
            }

            return this;
        }

        [NotNull]
        public FreeRedisConfiguration GetConfiguration<TContainer>()
        {
            return GetConfiguration(RedisClientNameAttribute.GetClientName<TContainer>());
        }

        [NotNull]
        public FreeRedisConfiguration GetConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            return _clients.GetOrDefault(name) ??
                   Default;
        }

    }
}

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.Consul
{
    public class ConsulConfigurations
    {
        private ConsulConfiguration Default => GetConfiguration<DefaultConsul>();

        private readonly Dictionary<string, ConsulConfiguration> _consuls;

        public ConsulConfigurations()
        {
            _consuls = new Dictionary<string, ConsulConfiguration>()
            {
                [ConsulNameAttribute.GetConsulName<DefaultConsul>()] = new ConsulConfiguration()

            };
        }


        public ConsulConfigurations Configure<TContainer>(Action<ConsulConfiguration> configureAction)
        {
            return Configure(
                ConsulNameAttribute.GetConsulName<TContainer>(),
                configureAction
            );
        }

        public ConsulConfigurations Configure(
            [NotNull] string name,
            [NotNull] Action<ConsulConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _consuls.GetOrAdd(
                    name,
                    () => new ConsulConfiguration()
                    {
                        Address = Default.Address,
                        DataCenter = Default.DataCenter,
                        Token = Default.Token,
                        WaitTime = Default.WaitTime
                    }
                )
            );

            return this;
        }

        public ConsulConfigurations ConfigureDefault(Action<ConsulConfiguration> configureAction)
        {
            configureAction(Default);
            return this;
        }

        public ConsulConfigurations ConfigureAll(Action<string, ConsulConfiguration> configureAction)
        {
            foreach (var _consul in _consuls)
            {
                configureAction(_consul.Key, _consul.Value);
            }

            return this;
        }

        [NotNull]
        public ConsulConfiguration GetConfiguration<TContainer>()
        {
            return GetConfiguration(ConsulNameAttribute.GetConsulName<TContainer>());
        }

        [NotNull]
        public ConsulConfiguration GetConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            return _consuls.GetOrDefault(name) ??
                   Default;
        }

    }
}

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class AddressTableConfigurations
    {
        private readonly Dictionary<string, AddressTableConfiguration> _configurations;
        public AddressTableConfigurations()
        {
            _configurations = new Dictionary<string, AddressTableConfiguration>();
        }

        public AddressTableConfigurations Configure(
          [NotNull] string name,
          [NotNull] Action<AddressTableConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(configureAction, nameof(configureAction));

            AddressTableConfiguration configureServiceName(AddressTableConfiguration configuration)
            {
                configuration.Service = name;
                return configuration;
            }

            var nameConfiguration = configureServiceName(_configurations.GetOrAdd(
                    name,
                    () => new AddressTableConfiguration()
                ));

            configureAction(nameConfiguration);

            return this;
        }


        public AddressTableConfigurations ConfigureAll(Action<string, AddressTableConfiguration> configureAction)
        {
            foreach (var _configuration in _configurations)
            {
                configureAction(_configuration.Key, _configuration.Value);
            }

            return this;
        }

        public AddressTableConfiguration GetConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            return _configurations.GetOrDefault(name);
        }
    }
}

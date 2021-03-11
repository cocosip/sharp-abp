using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.MapTenancy
{
    public class MapTenancyConfigurations
    {
        private MapTenancyConfiguration Default => new(null, "");
        private readonly Dictionary<string, MapTenancyConfiguration> _mappers;

        public MapTenancyConfigurations()
        {
            _mappers = new Dictionary<string, MapTenancyConfiguration>();
        }

        public MapTenancyConfigurations Configure(
            [NotNull] string code,
            [NotNull] Action<MapTenancyConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _mappers.GetOrAdd(
                    code,
                    () => new MapTenancyConfiguration()
                    )
                );
            return this;
        }

        public MapTenancyConfigurations ConfigureAll(Action<string, MapTenancyConfiguration> configureAction)
        {
            foreach (var mapper in _mappers)
            {
                configureAction(mapper.Key, mapper.Value);
            }

            return this;
        }

        [NotNull]
        public MapTenancyConfiguration GetConfiguration([NotNull] string code)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            return _mappers.GetOrDefault(code) ??
                   Default;
        }

    }
}

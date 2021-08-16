using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.DotCommon.Performance
{
    public class PerformanceConfigurations
    {
        private PerformanceConfiguration Default => GetConfiguration("default");

        private readonly Dictionary<string, PerformanceConfiguration> _performances;

        public PerformanceConfigurations()
        {
            _performances = new Dictionary<string, PerformanceConfiguration>
            {
                //Add default container
                ["default"] = new PerformanceConfiguration()
            };
        }

        public PerformanceConfigurations Configure(
            [NotNull] string name,
            [NotNull] Action<PerformanceConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _performances.GetOrAdd(
                    name,
                    () => new PerformanceConfiguration()
                )
            );

            return this;
        }


        public PerformanceConfigurations ConfigureAll(Action<string, PerformanceConfiguration> configureAction)
        {
            foreach (var performance in _performances)
            {
                configureAction(performance.Key, performance.Value);
            }
            return this;
        }

        [NotNull]
        public PerformanceConfiguration GetConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            return _performances.GetOrDefault(name) ??
                   Default;
        }
    }
}

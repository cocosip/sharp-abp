using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public class ConcurrencyLimitingConfiguration : IConcurrencyLimitingConfiguration, ISingletonDependency
    {
        public bool IsEnabled { get; }
        private readonly Dictionary<string, Func<IServiceProvider, ConcurrencyLimitingContext, string>> _resolvers;
        public ConcurrencyLimitingConfiguration(IOptions<SharpAbpConcurrencyLimitingOptions> options)
        {
            IsEnabled = options.Value.IsEnabled;
            _resolvers = new Dictionary<string, Func<IServiceProvider, ConcurrencyLimitingContext, string>>(options.Value.Resolvers, StringComparer.Ordinal);
        }

        public virtual string Resolve(string name, IServiceProvider services, ConcurrencyLimitingContext context)
        {
            if (!_resolvers.TryGetValue(name, out var resolver))
            {
                throw new ConcurrencyLimitingConfigurationException("Unknown concurrency resolver: " + name);
            }
            return resolver(services, context);
        }
    }

}

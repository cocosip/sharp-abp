using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DotCommon.Performance
{
    public class DefaultPerformanceConfigurationProvider : IPerformanceConfigurationProvider, ITransientDependency
    {
        protected AbpPerformanceOptions Options { get; }
        public DefaultPerformanceConfigurationProvider(IOptions<AbpPerformanceOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Get configuration
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual PerformanceConfiguration Get([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return Options.Configurations.GetConfiguration(name);
        }
    }
}

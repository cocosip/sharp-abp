using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DotCommon.Performance
{
    public class DefaultCountInfoFactory : ICountInfoFactory, ITransientDependency
    {
        protected IServiceProvider ServiceProvider { get; }
        public DefaultCountInfoFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Create countInfo
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <param name="configuration"></param>
        /// <param name="initialCount"></param>
        /// <param name="rtMilliseconds"></param>
        /// <returns></returns>
        public virtual CountInfo Create(
            string name,
            string key,
            PerformanceConfiguration configuration,
            long initialCount,
            double rtMilliseconds)
        {
            var countInfo = ActivatorUtilities.CreateInstance<CountInfo>(
                ServiceProvider,
                name,
                key,
                configuration,
                initialCount,
                rtMilliseconds);

            return countInfo;
        }

    }
}

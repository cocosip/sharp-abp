using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class DefaultLoadBalancerConfigurationProvider : ILoadBalancerConfigurationProvider, ITransientDependency
    {
        protected AbpMicroLoadBalancerOptions Options { get; }

        public DefaultLoadBalancerConfigurationProvider(IOptions<AbpMicroLoadBalancerOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Get loadbalancer configuration by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual LoadBalancerConfiguration Get(string name)
        {
            return Options.Configurations.GetConfiguration(name);
        }

    }
}

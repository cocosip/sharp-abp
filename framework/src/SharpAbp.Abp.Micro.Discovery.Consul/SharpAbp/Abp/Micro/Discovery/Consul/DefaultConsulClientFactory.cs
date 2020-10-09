using Consul;
using Microsoft.Extensions.Options;
using System;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class DefaultConsulClientFactory : IConsulClientFactory, ISingletonDependency
    {
        protected AbpMicroDiscoveryConsulOptions Options { get; }

        public DefaultConsulClientFactory(IOptions<AbpMicroDiscoveryConsulOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Get a consul client
        /// </summary>
        /// <returns></returns>
        public virtual IConsulClient Get()
        {
            return new ConsulClient(c =>
            {
                c.Address = new Uri($"{Options.Scheme}://{Options.Host}:{Options.Port}");
                c.Datacenter = Options.DataCenter;

                if (Options.WaitSeconds.HasValue)
                {
                    c.WaitTime = TimeSpan.FromSeconds(Options.WaitSeconds.Value);
                }

                if (!Options.Token.IsNullOrWhiteSpace())
                {
                    c.Token = Options.Token;
                }
            });
        }

    }
}

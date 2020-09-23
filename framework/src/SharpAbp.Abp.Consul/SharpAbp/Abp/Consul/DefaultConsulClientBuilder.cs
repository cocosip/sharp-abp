using Consul;
using System;
using System.Net.Http;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Consul
{
    public class DefaultConsulClientBuilder : IConsulClientBuilder, ITransientDependency
    {
        /// <summary>
        /// Create consul client
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public virtual IConsulClient CreateClient(ConsulConfiguration configuration)
        {
            void conf(ConsulClientConfiguration consulClientConfiguration)
            {
                consulClientConfiguration.Address = configuration.Address;
                consulClientConfiguration.Datacenter = configuration.DataCenter;
                consulClientConfiguration.Token = configuration.Token;
                consulClientConfiguration.WaitTime = configuration.WaitTime;
            }

            var clientOverride = configuration.ClientOverride ?? new Action<HttpClient>(c => { });

            var handlerOverride = configuration.HandlerOverride ?? new Action<HttpClientHandler>(c => { });

            IConsulClient consulClient = new ConsulClient(conf, clientOverride, handlerOverride);

            return consulClient;
        }
    }
}

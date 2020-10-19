using SharpAbp.Abp.Micro.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class WeightRoundRobinLoadBalancer : ILoadBalancer
    {
        public string Type => LoadBalancerConsts.WeightRoundRobin;

        public string Service { get; }

        protected WeightRoundRobinLoadBalancerConfiguration Configugration { get; }
        protected IServiceDiscoveryProvider DiscoveryProvider { get; }

        private readonly object SyncObject = new object();
        private int _sequence = 1;
        private List<MicroService> _lastServices;
        private List<MicroService> _weightServices;
        public WeightRoundRobinLoadBalancer(string service, WeightRoundRobinLoadBalancerConfiguration configuration, IServiceDiscoveryProvider discoveryProvider)
        {
            Service = service;
            Configugration = configuration;
            DiscoveryProvider = discoveryProvider;
            _lastServices = new List<MicroService>();
            _weightServices = new List<MicroService>();
        }

        public virtual async Task<MicroService> Lease(string tag = "", CancellationToken cancellationToken = default)
        {
            var services = await DiscoveryProvider.GetServices(Service, tag, cancellationToken);
            if (services == null || services.Count == 0)
            {
                throw new AbpException($"There were no services in WeightRoundRobinLoadBalancer '{Service}'");
            }

            lock (SyncObject)
            {
                BuildWeightServices(services);
                var sequence = Interlocked.Exchange(ref _sequence, _sequence + Configugration.Step);
                var index = sequence % _weightServices.Count;
                return services[index];
            }
        }

        private void BuildWeightServices(List<MicroService> services)
        {
            if (!ServiceMatch(services))
            {
                _lastServices = services;
            }

            if (Configugration.Weights.IsNullOrWhiteSpace())
            {
                _weightServices = services;
                return;
            }


        }


        private bool ServiceMatch(List<MicroService> services)
        {
            foreach (var service in services)
            {
                if (!services.Contains(service))
                {
                    return false;
                }
            }

            if (_lastServices.Count != services.Count)
            {
                return false;
            }
            return true;
        }

    }
}

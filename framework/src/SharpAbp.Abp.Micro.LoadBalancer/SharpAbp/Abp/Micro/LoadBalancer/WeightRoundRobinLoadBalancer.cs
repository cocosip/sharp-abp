using SharpAbp.Abp.Micro.Discovery;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class WeightRoundRobinLoadBalancer : ILoadBalancer
    {
        public string BalancerType => LoadBalancerConsts.WeightRoundRobin;

        public string Service { get; }

        protected WeightRoundRobinLoadBalancerConfiguration Configugration { get; }
        protected IServiceDiscoveryProvider DiscoveryProvider { get; }

        private readonly object SyncObject = new object();
        private readonly List<WeightMicroService> _services;
        public WeightRoundRobinLoadBalancer(string service, WeightRoundRobinLoadBalancerConfiguration configuration, IServiceDiscoveryProvider discoveryProvider)
        {
            Service = service;
            Configugration = configuration;
            DiscoveryProvider = discoveryProvider;
            _services = new List<WeightMicroService>();
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
                //Not match,update
                if (!ServiceMatch(services))
                {
                    Initialize(services);
                }

                var weightService = GetNext();

                return weightService?.Service;
            }
        }

        private WeightMicroService GetNext()
        {
            var weightService = _services.Max();
            var totalWeight = _services.Sum(x => x.Weight);

            weightService.CurrentWeight -= totalWeight;

            foreach (var service in _services)
            {
                service.CurrentWeight += service.Weight;
            }

            return weightService;
        }


        private bool ServiceMatch(List<MicroService> services)
        {
            foreach (var service in services)
            {
                var weightService = _services.FirstOrDefault(x => x.Service == service);
                if (weightService == null)
                {
                    return false;
                }
            }

            if (_services.Count != services.Count)
            {
                return false;
            }
            return true;
        }

        private void Initialize(List<MicroService> services)
        {
            var weightHostAndPorts = LoadBalancerUtil.ConvertToWeightHostAndPorts(Configugration.Weights);

            var weightServices = new List<WeightMicroService>();
            foreach (var service in services)
            {
                var weightHostAndPort = weightHostAndPorts.FirstOrDefault(x => x.HostAndPort.Host == service.Address && x.HostAndPort.Port == service.Port);

                var weight = weightHostAndPort?.Weight ?? 1;
                weightServices.Add(new WeightMicroService(weight, service, weight));
            }

            _services.Clear();
            _services.AddRange(weightServices);
        }

    }
}

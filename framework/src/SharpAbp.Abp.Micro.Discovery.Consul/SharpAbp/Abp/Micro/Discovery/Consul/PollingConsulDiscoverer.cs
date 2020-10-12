using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class PollingConsulDiscoverer : IPollingConsulDiscoverer, IDisposable
    {
        protected PollingConsulDiscovererOption Option { get; }
        protected ILogger Logger { get; }
        protected IDistributedCache<List<MicroService>> ServiceCache { get; }
        protected IConsulDiscoveryService ConsulDiscoveryService { get; }

        private Timer _timer = null;
        private bool _polling = false;

        public PollingConsulDiscoverer(PollingConsulDiscovererOption option, ILogger<PollingConsulDiscoverer> logger, IDistributedCache<List<MicroService>> serviceCache, IConsulDiscoveryService consulDiscoveryService)
        {
            Option = option;
            Logger = logger;
            ServiceCache = serviceCache;
            ConsulDiscoveryService = consulDiscoveryService;
        }

        public async Task<List<MicroService>> GetAsync([NotNull] string service, CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(service, nameof(service));
            var key = GetCacheKey();
            var services = await ServiceCache.GetAsync(key, token: cancellationToken);
            if (services == null)
            {
                await Poll();
            }

            services = await ServiceCache.GetAsync(key);

            return services;
        }


        public virtual void Run()
        {
            if (_timer != null)
            {
                Logger.LogDebug("PollingConsulDiscovery is running! Don't run again!");
                return;
            }

            _timer = new Timer(async x =>
            {
                if (_polling)
                {
                    return;
                }

                _polling = true;
                await Poll();
                _polling = false;
            }, null, Option.PollingInterval, Option.PollingInterval);

        }

        private string GetCacheKey()
        {
            return $"{Option.Prefix}{Option.Service}";
        }

        private async Task Poll()
        {
            var services = await ConsulDiscoveryService.GetAsync(Option.Service);
            if (services == null)
            {
                services = new List<MicroService>();
            }

            var key = GetCacheKey();
            await ServiceCache.SetAsync(key, services, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(Option.Expires)
            });
        }


        public void Dispose()
        {
            _timer?.Dispose();
            _timer = null;
        }
    }
}

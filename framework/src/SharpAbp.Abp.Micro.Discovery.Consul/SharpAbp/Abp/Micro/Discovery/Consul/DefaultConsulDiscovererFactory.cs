using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class DefaultConsulDiscovererFactory : IConsulDiscovererFactory, ISingletonDependency
    {
        protected AbpMicroDiscoveryConsulOptions Options { get; }
        protected ILogger Logger { get; }
        protected IServiceProvider ServiceProvider { get; }
        protected IConsulDiscoverer DefaultDiscoverer { get; set; }
        protected ConcurrentDictionary<string, IConsulDiscoverer> PollingDiscoverers { get; }

        protected object SyncObject = new object();
        public DefaultConsulDiscovererFactory(IOptions<AbpMicroDiscoveryConsulOptions> options, ILogger<DefaultConsulDiscovererFactory> logger, IConsulDiscoverer defaultDiscoverer, IServiceProvider serviceProvider)
        {
            Options = options.Value;
            Logger = logger;
            DefaultDiscoverer = defaultDiscoverer;
            ServiceProvider = serviceProvider;
            PollingDiscoverers = new ConcurrentDictionary<string, IConsulDiscoverer>();
        }


        public virtual IConsulDiscoverer GetDiscoverer([NotNull] string service)
        {
            Check.NotNullOrWhiteSpace(service, nameof(service));

            if (!Options.EnablePolling)
            {
                return DefaultDiscoverer;
            }

            if (!PollingDiscoverers.TryGetValue(service, out IConsulDiscoverer consulDiscoverer))
            {
                lock (SyncObject)
                {
                    if (PollingDiscoverers.TryGetValue(service, out consulDiscoverer))
                    {
                        if (PollingDiscoverers.TryAdd(service, consulDiscoverer))
                        {
                            //Run...
                            ((IPollingConsulDiscoverer)consulDiscoverer).Run();
                        }
                        else
                        {
                            Logger.LogDebug("Can't add PollingConsulDiscovery to dict with service '{0}'.", service);
                        }
                    }
                }
            }
            return consulDiscoverer;
        }

        protected virtual IPollingConsulDiscoverer BuildPollingConsulDiscoverer(string service)
        {
            var option = new PollingConsulDiscovererOption()
            {
                Service = service,
                PollingInterval = Options.PollingInterval,
                Prefix = Options.Prefix,
                Expires = Options.Expires
            };
            var logger = ServiceProvider.GetService<ILogger<PollingConsulDiscoverer>>();
            var cache = ServiceProvider.GetService<IDistributedCache<List<MicroService>>>();
            var consulService = ServiceProvider.GetService<IConsulDiscoveryService>();

            return new PollingConsulDiscoverer(option, logger, cache, consulService);
        }

    }
}

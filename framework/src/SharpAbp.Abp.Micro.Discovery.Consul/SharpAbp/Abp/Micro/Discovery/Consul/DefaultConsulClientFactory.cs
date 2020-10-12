using Consul;
using Microsoft.Extensions.Options;
using System;
using Volo.Abp.DependencyInjection;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using System.Threading;
using Volo.Abp;
using JetBrains.Annotations;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class DefaultConsulClientFactory : IConsulClientFactory, ISingletonDependency
    {
        protected ILogger Logger { get; set; }
        protected AbpMicroDiscoveryConsulOptions Options { get; }

        private readonly ConcurrentDictionary<int, IConsulClient> _clientDict;
        private int _sequence = 1;
        private readonly object SyncObject = new object();

        public DefaultConsulClientFactory(ILogger<DefaultConsulClientFactory> logger, IOptions<AbpMicroDiscoveryConsulOptions> options)
        {
            Logger = logger;
            Options = options.Value;
            _clientDict = new ConcurrentDictionary<int, IConsulClient>();
        }

        /// <summary>
        /// Get a consul client
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public virtual IConsulClient Get()
        {
            //序号
            var index = _sequence % Options.MaxConsulClient;

            //无法获取客户端
            if (!_clientDict.TryGetValue(index, out IConsulClient client))
            {
                lock (SyncObject)
                {
                    if (!_clientDict.TryGetValue(index, out client))
                    {

                        client = BuildClient();

                        if (!_clientDict.TryAdd(index, client))
                        {
                            Logger.LogWarning("Can't add consul client to dict!");
                        }
                        Interlocked.Increment(ref _sequence);
                    }
                }
            }
            if (client == null)
            {
                throw new AbpException($"Can't get any consul client!");
            }
            return client;
        }


        private IConsulClient BuildClient()
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

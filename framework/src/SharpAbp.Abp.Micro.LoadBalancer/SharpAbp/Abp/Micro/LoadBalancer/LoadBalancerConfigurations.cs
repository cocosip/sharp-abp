using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class LoadBalancerConfigurations
    {
        private LoadBalancerConfiguration Default => GetConfiguration<DefaultLoadBalancer>();

        private readonly Dictionary<string, LoadBalancerConfiguration> _balancers;

        public LoadBalancerConfigurations()
        {
            _balancers = new Dictionary<string, LoadBalancerConfiguration>
            {
                [LoadBalancerNameAttribute.GetServiceLoadBalancerName<DefaultLoadBalancer>()] = new LoadBalancerConfiguration()
            };
        }

        public LoadBalancerConfigurations Configure<TBalancer>(
            Action<LoadBalancerConfiguration> configureAction)
        {
            return Configure(
                LoadBalancerNameAttribute.GetServiceLoadBalancerName<TBalancer>(),
                configureAction
            );
        }

        public LoadBalancerConfigurations Configure(
            [NotNull] string name,
            [NotNull] Action<LoadBalancerConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _balancers.GetOrAdd(
                    name,
                    () => new LoadBalancerConfiguration(Default)
                )
            );

            return this;
        }

        public LoadBalancerConfigurations ConfigureDefault(Action<LoadBalancerConfiguration> configureAction)
        {
            configureAction(Default);
            return this;
        }

        public LoadBalancerConfigurations ConfigureAll(Action<string, LoadBalancerConfiguration> configureAction)
        {
            foreach (var container in _balancers)
            {
                configureAction(container.Key, container.Value);
            }

            return this;
        }

        [NotNull]
        public LoadBalancerConfiguration GetConfiguration<TBalancer>()
        {
            return GetConfiguration(LoadBalancerNameAttribute.GetServiceLoadBalancerName<TBalancer>());
        }

        [NotNull]
        public LoadBalancerConfiguration GetConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            return _balancers.GetOrDefault(name) ??
                   Default;
        }

    }



}

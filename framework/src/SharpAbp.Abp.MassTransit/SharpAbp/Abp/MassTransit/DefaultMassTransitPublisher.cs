using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using System;

namespace SharpAbp.Abp.MassTransit
{
    public class DefaultMassTransitPublisher : IMassTransitPublisher, ITransientDependency
    {
        protected AbpMassTransitOptions Options { get; }
        protected IEnumerable<IPublishProvider> Providers { get; }
        public DefaultMassTransitPublisher(
            IOptions<AbpMassTransitOptions> options,
            IEnumerable<IPublishProvider> providers)
        {
            Options = options.Value;
            Providers = providers;
        }

        public virtual async Task PublishAsync<T>(
            T message,
            CancellationToken cancellationToken = default) where T : class
        {
            var provider = Providers.FirstOrDefault(x => x.Provider.Equals(Options.Provider, StringComparison.OrdinalIgnoreCase));
            if (provider != null)
            {
                await provider.PublishAsync(message, cancellationToken);
                return;
            }
            throw new AbpException($"Could not find the MassTransit eventBus provider with the type ({Options.Provider}) configured for the eventBus.");
        }

    }
}

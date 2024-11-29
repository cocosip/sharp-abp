using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit
{
    public class DefaultMassTransitPublisher : IMassTransitPublisher, ITransientDependency
    {
        protected AbpMassTransitOptions Options { get; }
        protected IServiceProvider ServiceProvider { get; }
        public DefaultMassTransitPublisher(
            IOptions<AbpMassTransitOptions> options,
            IServiceProvider serviceProvider)
        {
            Options = options.Value;
            ServiceProvider = serviceProvider;
        }

        public virtual async Task PublishAsync<T>(
            T message,
            CancellationToken cancellationToken = default) where T : class
        {
            var provider = ServiceProvider.GetRequiredKeyedService<IPublishProvider>(Options.Provider);
            await provider.PublishAsync(message, cancellationToken);
        }

    }
}

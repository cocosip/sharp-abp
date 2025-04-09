using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit.SqlServer
{
    public class SqlServerProduceService : ISqlServerProduceService, ITransientDependency
    {
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        public SqlServerProduceService(IServiceScopeFactory serviceScopeFactory)
        {
            ServiceScopeFactory = serviceScopeFactory;
        }


        public virtual async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            await publishEndpoint.Publish<T>(message, cancellationToken);
        }

        public virtual async Task PublishAsync(object message, Type? messageType = null, CancellationToken cancellationToken = default)
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            if (messageType == null)
            {
                await publishEndpoint.Publish(message, cancellationToken);
            }
            else
            {
                await publishEndpoint.Publish(message, messageType, cancellationToken);
            }
        }

        public virtual async Task SendAsync<T>(string uriString, T message, CancellationToken cancellationToken = default) where T : class
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var sendEndpointProvider = scope.ServiceProvider.GetRequiredService<ISendEndpointProvider>();
            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri(uriString));
            await endpoint.Send<T>(message, cancellationToken);
        }

        public virtual async Task SendAsync(string uriString, object message, CancellationToken cancellationToken = default)
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var sendEndpointProvider = scope.ServiceProvider.GetRequiredService<ISendEndpointProvider>();
            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri(uriString));
            await endpoint.Send(message, cancellationToken);
        }
    }
}

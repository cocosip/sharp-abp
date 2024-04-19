using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit.PostgreSql
{
    public class PostgreSqlProduceService : IPostgreSqlProduceService, ITransientDependency
    {
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        public PostgreSqlProduceService(IServiceScopeFactory serviceScopeFactory)
        {
            ServiceScopeFactory = serviceScopeFactory;
        }


        public virtual async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var publishEndpoint = scope.ServiceProvider.GetService<IPublishEndpoint>();
            await publishEndpoint.Publish<T>(message, cancellationToken);
        }

        public virtual async Task PublishAsync(object message, Type messageType = null, CancellationToken cancellationToken = default)
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var publishEndpoint = scope.ServiceProvider.GetService<IPublishEndpoint>();
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
            var sendEndpointProvider = scope.ServiceProvider.GetService<ISendEndpointProvider>();
            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri(uriString));
            await endpoint.Send<T>(message, cancellationToken);
        }

        public virtual async Task SendAsync(string uriString, object message, CancellationToken cancellationToken = default)
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var sendEndpointProvider = scope.ServiceProvider.GetService<ISendEndpointProvider>();
            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri(uriString));
            await endpoint.Send(message, cancellationToken);
        }
    }
}
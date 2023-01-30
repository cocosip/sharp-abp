using Apache.NMS.ActiveMQ.Commands;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit.ActiveMQ
{
    public class ActiveMqProduceService : IActiveMqProduceService, ITransientDependency
    {
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        public ActiveMqProduceService(IServiceScopeFactory serviceScopeFactory)
        {
            ServiceScopeFactory = serviceScopeFactory;
        }


        /// <summary>
        /// Publish message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task PublishAsync<T>(
            T message,
            CancellationToken cancellationToken = default) where T : class
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var publishEndpoint = scope.ServiceProvider.GetService<IPublishEndpoint>();
            await publishEndpoint.Publish<T>(message, cancellationToken);
            //using var scope = ServiceScopeFactory.CreateScope();
            //var sendEndpointProvider = scope.ServiceProvider.GetService<ISendEndpointProvider>();
            //await sendEndpointProvider.Send(message, cancellationToken);
        }

        /// <summary>
        /// Publish message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task PublishAsync(
            object message,
            Type messageType = null,
            CancellationToken cancellationToken = default)
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

            //using var scope = ServiceScopeFactory.CreateScope();
            //var sendEndpointProvider = scope.ServiceProvider.GetService<ISendEndpointProvider>();
            //if (messageType == null)
            //{
            //    await sendEndpointProvider.Send(message, cancellationToken);
            //}
            //else
            //{
            //    await sendEndpointProvider.Send(message, messageType, cancellationToken);
            //}
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uriString"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task SendAsync<T>(
            string uriString,
            T message,
            CancellationToken cancellationToken = default) where T : class
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var sendEndpointProvider = scope.ServiceProvider.GetService<ISendEndpointProvider>();
            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri(uriString));
            await endpoint.Send<T>(message, cancellationToken);
        }


        /// <summary>
        /// Send message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var sendEndpointProvider = scope.ServiceProvider.GetService<ISendEndpointProvider>();
            await sendEndpointProvider.Send(message, cancellationToken);
        }


        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="uriString"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task SendAsync(
            string uriString,
            object message,
            CancellationToken cancellationToken = default)
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var sendEndpointProvider = scope.ServiceProvider.GetService<ISendEndpointProvider>();
            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri(uriString));
            await endpoint.Send(message, cancellationToken);
        }



    }
}

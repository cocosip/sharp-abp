using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit.ActiveMQ
{
    [ExposeKeyedService<IPublishProvider>(MassTransitActiveMqConsts.ProviderName)]
    public class ActiveMqPublishProvider : IPublishProvider, ITransientDependency
    {
        public string Provider => MassTransitActiveMqConsts.ProviderName;
        protected IActiveMqProduceService ActiveMqProduceService { get; }

        public ActiveMqPublishProvider(IActiveMqProduceService activeMqProduceService)
        {
            ActiveMqProduceService = activeMqProduceService;
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
            await ActiveMqProduceService.PublishAsync(message, cancellationToken);
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
            System.Type? messageType = null,
            CancellationToken cancellationToken = default)
        {
            await ActiveMqProduceService.PublishAsync(message, messageType, cancellationToken);
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
            await ActiveMqProduceService.SendAsync(uriString, message, cancellationToken);
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
            await ActiveMqProduceService.SendAsync(uriString, message, cancellationToken);
        }
    }
}

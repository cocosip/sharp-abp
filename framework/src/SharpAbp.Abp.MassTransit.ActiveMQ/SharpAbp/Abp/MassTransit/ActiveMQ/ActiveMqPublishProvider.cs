using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit.ActiveMQ
{
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
            await ActiveMqProduceService.SendAsync(message, cancellationToken);
        }
    }
}

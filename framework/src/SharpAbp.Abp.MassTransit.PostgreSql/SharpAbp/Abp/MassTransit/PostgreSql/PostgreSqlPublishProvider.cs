using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit.PostgreSql
{
    [ExposeKeyedService<IPublishProvider>(MassTransitPostgreSqlConsts.ProviderName)]
    public class PostgreSqlPublishProvider : IPublishProvider, ITransientDependency
    {
        public string Provider => MassTransitPostgreSqlConsts.ProviderName;

        protected IPostgreSqlProduceService PostgreSqlProduceService { get; }
        public PostgreSqlPublishProvider(IPostgreSqlProduceService postgreSqlProduceService)
        {
            PostgreSqlProduceService = postgreSqlProduceService;
        }

        /// <summary>
        /// Publish message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            await PostgreSqlProduceService.PublishAsync<T>(message, cancellationToken);
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
            await PostgreSqlProduceService.PublishAsync(message, messageType, cancellationToken);
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
            await PostgreSqlProduceService.SendAsync(uriString, message, cancellationToken);
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
            await PostgreSqlProduceService.SendAsync(uriString, message, cancellationToken);
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit.SqlServer
{
    [ExposeKeyedService<IPublishProvider>(MassTransitSqlServerConsts.ProviderName)]
    public class SqlServerPublishProvider : IPublishProvider, ITransientDependency
    {
        public string Provider => MassTransitSqlServerConsts.ProviderName;

        protected ISqlServerProduceService SqlServerProduceService { get; }
        public SqlServerPublishProvider(ISqlServerProduceService sqlServerProduceService)
        {
            SqlServerProduceService = sqlServerProduceService;
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
            await SqlServerProduceService.PublishAsync<T>(message, cancellationToken);
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
            await SqlServerProduceService.PublishAsync(message, messageType, cancellationToken);
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
            await SqlServerProduceService.SendAsync(uriString, message, cancellationToken);
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
            await SqlServerProduceService.SendAsync(uriString, message, cancellationToken);
        }
    }
}

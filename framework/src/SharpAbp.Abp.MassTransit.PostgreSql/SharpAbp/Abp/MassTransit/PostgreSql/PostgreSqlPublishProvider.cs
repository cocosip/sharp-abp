using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MassTransit.PostgreSql
{
    public class PostgreSqlPublishProvider : IPublishProvider, ITransientDependency
    {
        public string Provider => MassTransitPostgreSqlConsts.ProviderName;
        
        protected IPostgreSqlProduceService PostgreSqlProduceService { get; }
        public PostgreSqlPublishProvider(IPostgreSqlProduceService postgreSqlProduceService)
        {
            PostgreSqlProduceService = postgreSqlProduceService;
        }

        public virtual async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            await PostgreSqlProduceService.PublishAsync<T>(message, cancellationToken);
        }
    }
}

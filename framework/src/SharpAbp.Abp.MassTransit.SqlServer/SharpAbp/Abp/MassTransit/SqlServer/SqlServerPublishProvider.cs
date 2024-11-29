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

        public virtual async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            await SqlServerProduceService.PublishAsync<T>(message, cancellationToken);
        }
    }
}

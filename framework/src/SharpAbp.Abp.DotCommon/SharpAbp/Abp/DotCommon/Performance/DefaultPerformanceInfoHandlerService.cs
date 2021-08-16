using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DotCommon.Performance
{
    public class DefaultPerformanceInfoHandlerService : IPerformanceInfoHandlerService, ITransientDependency
    {
        protected ILogger Logger { get; }
        public DefaultPerformanceInfoHandlerService(ILogger<DefaultPerformanceInfoHandlerService> logger)
        {
            Logger = logger;
        }

        public virtual void Handle(string name, string key, PerformanceInfo performanceInfo)
        {
            Logger.LogInformation("{0}-{1}, totalCount: {2}, throughput: {3}, averageThrughput: {4}, rt: {5:F3}ms, averageRT: {6:F3}ms", name, key, performanceInfo.TotalCount, performanceInfo.Throughput, performanceInfo.AverageThroughput, performanceInfo.RT, performanceInfo.AverageRT);
        }
    }
}

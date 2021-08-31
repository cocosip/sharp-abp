using SharpAbp.Abp.DotCommon.Performance;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace PerformanceSample
{
    public class PerformanceService : ITransientDependency
    {
        protected IPerformanceServiceFactory PerformanceServiceFactory { get; }
        public PerformanceService(IPerformanceServiceFactory performanceServiceFactory)
        {
            PerformanceServiceFactory = performanceServiceFactory;
        }

        public void RunPerformance()
        {

            Task.Factory.StartNew(async () =>
            {
                var performanceService = PerformanceServiceFactory.GetOrCreate("service1");
                var total = 100000;
                var current = 0;

                while (current < total)
                {
                    var rd = new Random();
                    performanceService.IncrementKeyCount("getId", rd.Next(10, 9999));
                    Interlocked.Increment(ref current);
                    await Task.Delay(10);
                }
            });

            Task.Factory.StartNew(async () =>
            {
                var performanceService = PerformanceServiceFactory.GetOrCreate("service1");
                var total = 100000;
                var current = 0;

                while (current < total)
                {
                    var rd = new Random();
                    performanceService.IncrementKeyCount("getName", rd.Next(10, 999));
                    Interlocked.Increment(ref current);
                    await Task.Delay(10);
                }
            });
        }

    }
}

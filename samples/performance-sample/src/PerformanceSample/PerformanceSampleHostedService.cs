using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Volo.Abp;

namespace PerformanceSample
{
    public class PerformanceSampleHostedService : IHostedService
    {
        private readonly IAbpApplicationWithExternalServiceProvider _application;
        private readonly IServiceProvider _serviceProvider;
        private readonly PerformanceService _performanceService;

        public PerformanceSampleHostedService(
            IAbpApplicationWithExternalServiceProvider application,
            IServiceProvider serviceProvider,
            PerformanceService performanceService)
        {
            _application = application;
            _serviceProvider = serviceProvider;
            _performanceService = performanceService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _application.Initialize(_serviceProvider);

            _performanceService.RunPerformance();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _application.Shutdown();

            return Task.CompletedTask;
        }
    }
}

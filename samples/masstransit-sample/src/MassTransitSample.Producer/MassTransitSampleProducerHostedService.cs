using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace MassTransitSample.Producer
{
    public class MassTransitSampleProducerHostedService : IHostedService
    {
        private IAbpApplicationWithExternalServiceProvider _abpApplication;
        private readonly IServiceProvider _serviceProvider;
        private readonly ProducerService _producerService;

        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public MassTransitSampleProducerHostedService(
            IAbpApplicationWithExternalServiceProvider abpApplication,
            IServiceProvider serviceProvider,
            ProducerService producerService,
            IConfiguration configuration,
            IHostEnvironment hostEnvironment)
        {
            _abpApplication = abpApplication;
            _serviceProvider = serviceProvider;
            _producerService = producerService;

            _configuration = configuration;
            _hostEnvironment = hostEnvironment;

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _abpApplication.InitializeAsync(_serviceProvider);
            _producerService.Run();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _abpApplication.ShutdownAsync();
        }
    }
}

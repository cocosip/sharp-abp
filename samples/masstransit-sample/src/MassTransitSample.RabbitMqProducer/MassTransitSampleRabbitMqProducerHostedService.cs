using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace MassTransitSample.RabbitMqProducer
{
    public class MassTransitSampleRabbitMqProducerHostedService : IHostedService
    {
        private IAbpApplicationWithExternalServiceProvider _abpApplication;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMqProducerService _rabbitMqProducerService;

        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public MassTransitSampleRabbitMqProducerHostedService(
            IAbpApplicationWithExternalServiceProvider abpApplication,
            IServiceProvider serviceProvider,
            RabbitMqProducerService rabbitMqProducerService,
            IConfiguration configuration,
            IHostEnvironment hostEnvironment)
        {
            _abpApplication = abpApplication;
            _serviceProvider = serviceProvider;
            _rabbitMqProducerService = rabbitMqProducerService;

            _configuration = configuration;
            _hostEnvironment = hostEnvironment;

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _abpApplication.InitializeAsync(_serviceProvider);
            _rabbitMqProducerService.Run();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _abpApplication.ShutdownAsync();
        }
    }
}

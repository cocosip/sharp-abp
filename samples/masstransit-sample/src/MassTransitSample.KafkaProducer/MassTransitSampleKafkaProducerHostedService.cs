using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace MassTransitSample.KafkaProducer
{
    public class MassTransitSampleKafkaProducerHostedService : IHostedService
    {
        private IAbpApplicationWithExternalServiceProvider _abpApplication;
        private readonly IServiceProvider _serviceProvider;
        private readonly KafkaProducerService _kafkaProducerService;

        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public MassTransitSampleKafkaProducerHostedService(
            IAbpApplicationWithExternalServiceProvider abpApplication,
            IServiceProvider serviceProvider,
            KafkaProducerService kafkaProducerService,
            IConfiguration configuration,
            IHostEnvironment hostEnvironment)
        {
            _abpApplication = abpApplication;
            _serviceProvider = serviceProvider;
            _kafkaProducerService = kafkaProducerService;

            _configuration = configuration;
            _hostEnvironment = hostEnvironment;

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _abpApplication.InitializeAsync(_serviceProvider);
            _kafkaProducerService.Run();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _abpApplication.ShutdownAsync();
        }
    }

}

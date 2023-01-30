using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace MassTransitSample.ActiveMqConsumer
{
    public class MassTransitSampleActiveMqConsumerHostedService : IHostedService
    {
        private IAbpApplicationWithExternalServiceProvider _abpApplication;
        private readonly IServiceProvider _serviceProvider;

        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public MassTransitSampleActiveMqConsumerHostedService(
            IAbpApplicationWithExternalServiceProvider application,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IHostEnvironment hostEnvironment)
        {
            _abpApplication = application;
            _serviceProvider = serviceProvider;

            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _abpApplication.InitializeAsync(_serviceProvider);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _abpApplication.ShutdownAsync();
        }
    }
}

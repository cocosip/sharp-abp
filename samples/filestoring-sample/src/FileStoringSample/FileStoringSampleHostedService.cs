using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Volo.Abp;

namespace FileStoringSample
{
    public class FileStoringSampleHostedService : IHostedService
    {
        private readonly IAbpApplicationWithExternalServiceProvider _application;
        private readonly IServiceProvider _serviceProvider;
        private readonly FileStoringService _fileStoringService;

        public FileStoringSampleHostedService(
            IAbpApplicationWithExternalServiceProvider application,
            IServiceProvider serviceProvider,
            FileStoringService fileStoringService)
        {
            _application = application;
            _serviceProvider = serviceProvider;
            _fileStoringService = fileStoringService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _application.Initialize(_serviceProvider);

            //_helloWorldService.SayHello();
            await _fileStoringService.RunContainers();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _application.Shutdown();

            return Task.CompletedTask;
        }
    }
}

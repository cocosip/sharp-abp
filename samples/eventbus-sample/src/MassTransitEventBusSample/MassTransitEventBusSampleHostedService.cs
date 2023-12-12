using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransitEventBusSample;

public class MassTransitEventBusSampleHostedService : IHostedService
{

    private readonly PublishService _publishService;

    public MassTransitEventBusSampleHostedService(PublishService publishService)
    {
        _publishService = publishService;
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        _publishService.Run();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

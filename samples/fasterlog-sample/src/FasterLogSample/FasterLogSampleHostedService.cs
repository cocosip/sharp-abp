﻿//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Hosting;
//using Volo.Abp;

//namespace FasterLogSample;

//public class FasterLogSampleHostedService : IHostedService
//{
//    private readonly HelloWorldService _helloWorldService;

//    public FasterLogSampleHostedService(HelloWorldService helloWorldService)
//    {
//        _helloWorldService = helloWorldService;
//    }

//    public async Task StartAsync(CancellationToken cancellationToken)
//    {
//        await _helloWorldService.SayHelloAsync();
//    }

//    public Task StopAsync(CancellationToken cancellationToken)
//    {
//        return Task.CompletedTask;
//    }
//}

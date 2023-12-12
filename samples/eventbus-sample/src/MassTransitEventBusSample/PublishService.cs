using Microsoft.Extensions.Logging;
using SharpAbp.Abp.MassTransit;
using System.Threading;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Timing;
using Volo.Abp.Threading;

namespace MassTransitEventBusSample
{
    public class PublishService : ITransientDependency
    {
        static int Event1Sequence = 1;
        static int Event2Sequence = 2;

        protected ILogger Logger { get; }
        protected IClock Clock { get; }
        protected IDistributedEventBus DistributedEventBus { get; }
        protected ICancellationTokenProvider CancellationTokenProvider { get; }
        public PublishService(
            ILogger<PublishService> logger,
            IClock clock,
            IDistributedEventBus distributedEventBus,
            ICancellationTokenProvider cancellationTokenProvider)
        {
            Logger = logger;
            Clock = clock;
            DistributedEventBus = distributedEventBus;
            CancellationTokenProvider = cancellationTokenProvider;
        }



        public void Run()
        {
            Task.Factory.StartNew(async () =>
            {
                while (!CancellationTokenProvider.Token.IsCancellationRequested)
                {
                    try
                    {
                        await PublishMessage1();
                        await PublishMessage2();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "发送消息出错了,异常信息:{Message}", ex.Message);
                    }
                    await Task.Delay(5);
                }
            },TaskCreationOptions.LongRunning);

        }

        public virtual async Task PublishMessage1()
        {
            var sequence = Interlocked.Increment(ref Event1Sequence);
            await DistributedEventBus.PublishAsync(new MassTransitEvent1()
            {
                Sequence = sequence,
                MessageId = Guid.NewGuid().ToString("D"),
                PublishTime = Clock.Now
            });
        }

        public virtual async Task PublishMessage2()
        {
            var sequence = Interlocked.Increment(ref Event2Sequence);
            await DistributedEventBus.PublishAsync(new MassTransitEvent2()
            {
                Sequence = sequence,
                MessageId = Guid.NewGuid().ToString("D"),
                PublishTime = Clock.Now
            });
        }


    }
}

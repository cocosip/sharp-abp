using MassTransitSample.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;
using Volo.Abp.Timing;

namespace MassTransitSample.KafkaProducer
{
    public class KafkaProducerService : ISingletonDependency
    {
        static int Sequence1 = 1;
        protected ILogger Logger { get; }
        protected ICancellationTokenProvider CancellationTokenProvider { get; }
        protected IClock Clock { get; }
        protected IMassTransitPublisher MassTransitPublisher { get; }

        protected AbpMassTransitOptions Options { get; }
        public KafkaProducerService(
            ILogger<KafkaProducerService> logger,
            IOptions<AbpMassTransitOptions> options,
            ICancellationTokenProvider cancellationTokenProvider,
            IClock clock,
            IMassTransitPublisher massTransitPublisher)
        {
            Logger = logger;
            Options = options.Value;

            CancellationTokenProvider = cancellationTokenProvider;
            Clock = clock;
            MassTransitPublisher = massTransitPublisher;
        }

        public virtual void Run()
        {
            Task.Factory.StartNew(async () =>
            {
                while (!CancellationTokenProvider.Token.IsCancellationRequested)
                {
                    try
                    {
                        await PublishAsync();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "发送消息出错了,异常信息:{Message}", ex.Message);
                    }
                    await Task.Delay(200);
                }

            }, TaskCreationOptions.LongRunning);
        }


        public virtual async Task PublishAsync()
        {
            var sequence = Interlocked.Increment(ref Sequence1);
            await MassTransitPublisher.PublishAsync<KafkaMessage>(new KafkaMessage()
            {
                Sequence = sequence,
                MessageId = Guid.NewGuid().ToString("D"),
                PublishTime = Clock.Now
            });
        }

    }
}

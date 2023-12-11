using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.MassTransit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;
using Volo.Abp.Tracing;
using Volo.Abp.Uow;

namespace SharpAbp.Abp.EventBus.MassTransit
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IDistributedEventBus), typeof(MassTransitDistributedEventBus))]
    public class MassTransitDistributedEventBus : DistributedEventBusBase, ISingletonDependency
    {
        protected IMassTransitPublisher MassTransitPublisher { get; }
        public MassTransitDistributedEventBus(
            IServiceScopeFactory serviceScopeFactory,
            ICurrentTenant currentTenant,
            IUnitOfWorkManager unitOfWorkManager,
            IOptions<AbpDistributedEventBusOptions> abpDistributedEventBusOptions,
            IGuidGenerator guidGenerator,
            IClock clock,
            IEventHandlerInvoker eventHandlerInvoker,
            ILocalEventBus localEventBus,
            ICorrelationIdProvider correlationIdProvider,
            IMassTransitPublisher massTransitPublisher) :
            base(serviceScopeFactory, currentTenant, unitOfWorkManager, abpDistributedEventBusOptions, guidGenerator, clock, eventHandlerInvoker, localEventBus, correlationIdProvider)
        {
            MassTransitPublisher = massTransitPublisher;
        }

        public override Task ProcessFromInboxAsync(IncomingEventInfo incomingEvent, InboxConfig inboxConfig)
        {
            throw new NotImplementedException();
        }

        public override Task PublishFromOutboxAsync(OutgoingEventInfo outgoingEvent, OutboxConfig outboxConfig)
        {
            throw new NotImplementedException();
        }

        public override Task PublishManyFromOutboxAsync(IEnumerable<OutgoingEventInfo> outgoingEvents, OutboxConfig outboxConfig)
        {
            throw new NotImplementedException();
        }

        public override IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            throw new NotImplementedException();
        }

        public override void Unsubscribe<TEvent>(Func<TEvent, Task> action)
        {
            throw new NotImplementedException();
        }

        public override void Unsubscribe(Type eventType, IEventHandler handler)
        {
            throw new NotImplementedException();
        }

        public override void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            throw new NotImplementedException();
        }

        public override void UnsubscribeAll(Type eventType)
        {
            throw new NotImplementedException();
        }

        protected override void AddToUnitOfWork(IUnitOfWork unitOfWork, UnitOfWorkEventRecord eventRecord)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
        {
            throw new NotImplementedException();
        }

        protected override Task PublishToEventBusAsync(Type eventType, object eventData)
        {

  

            throw new NotImplementedException();
        }



        protected override byte[] Serialize(object eventData)
        {
            throw new NotImplementedException();
        }


        protected virtual async Task PublishToMassTransitAsync(Type eventType, object eventData)
        {

        }


    }
}

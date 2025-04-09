using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.MassTransit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
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
        protected IMassTransitSerializer Serializer { get; }

        protected ConcurrentDictionary<Type, List<IEventHandlerFactory>> HandlerFactories { get; }
        protected ConcurrentDictionary<string, Type> EventTypes { get; }

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
            IMassTransitPublisher massTransitPublisher,
            IMassTransitSerializer serializer) :
            base(serviceScopeFactory, currentTenant, unitOfWorkManager, abpDistributedEventBusOptions, guidGenerator, clock, eventHandlerInvoker, localEventBus, correlationIdProvider)
        {
            MassTransitPublisher = massTransitPublisher;
            Serializer = serializer;

            HandlerFactories = new ConcurrentDictionary<Type, List<IEventHandlerFactory>>();
            EventTypes = new ConcurrentDictionary<string, Type>();
        }

        public void Initialize()
        {
            SubscribeHandlers(AbpDistributedEventBusOptions.Handlers);
        }

        public virtual async Task ProcessEventAsync(AbpMassTransitEventData message)
        {
            var eventName = message.EventName;
            var eventType = EventTypes.GetOrDefault(eventName);
            if (eventType == null)
            {
                return;
            }

            var messageId = message.MessageId;
            var eventData = Serializer.Deserialize(message.JsonData, eventType);
            var correlationId = message.CorrelationId;

            if (await AddToInboxAsync(messageId, eventName, eventType, eventData, correlationId))
            {
                return;
            }

            using (CorrelationIdProvider.Change(correlationId))
            {
                await TriggerHandlersDirectAsync(eventType, eventData);
            }
        }


        public override async Task ProcessFromInboxAsync(IncomingEventInfo incomingEvent, InboxConfig inboxConfig)
        {
            var eventType = EventTypes.GetOrDefault(incomingEvent.EventName);
            if (eventType == null)
            {
                return;
            }

            var eventData = Serializer.Deserialize(incomingEvent.EventData, eventType);
            var exceptions = new List<Exception>();
            using (CorrelationIdProvider.Change(incomingEvent.GetCorrelationId()))
            {
                await TriggerHandlersFromInboxAsync(eventType, eventData, exceptions, inboxConfig);
            }
            if (exceptions.Any())
            {
                ThrowOriginalExceptions(eventType, exceptions);
            }
        }

        public override async Task PublishFromOutboxAsync(OutgoingEventInfo outgoingEvent, OutboxConfig outboxConfig)
        {
            using (CorrelationIdProvider.Change(outgoingEvent.GetCorrelationId()))
            {
                await TriggerDistributedEventSentAsync(new DistributedEventSent()
                {
                    Source = DistributedEventSource.Outbox,
                    EventName = outgoingEvent.EventName,
                    EventData = outgoingEvent.EventData
                });
            }

            await PublishToMassTransitAsync(outgoingEvent.EventName, outgoingEvent.EventData, outgoingEvent.GetCorrelationId(), outgoingEvent.Id);
        }

        public override async Task PublishManyFromOutboxAsync(IEnumerable<OutgoingEventInfo> outgoingEvents, OutboxConfig outboxConfig)
        {
            var outgoingEventArray = outgoingEvents.ToArray();

            foreach (var outgoingEvent in outgoingEventArray)
            {
                using (CorrelationIdProvider.Change(outgoingEvent.GetCorrelationId()))
                {
                    await TriggerDistributedEventSentAsync(new DistributedEventSent()
                    {
                        Source = DistributedEventSource.Outbox,
                        EventName = outgoingEvent.EventName,
                        EventData = outgoingEvent.EventData
                    });
                }

                await PublishToMassTransitAsync(outgoingEvent.EventName, Serializer.Deserialize(outgoingEvent.EventData, GetEventType(outgoingEvent.EventName)), outgoingEvent.GetCorrelationId(), outgoingEvent.Id);
            }
        }

        public override IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            var handlerFactories = GetOrCreateHandlerFactories(eventType);

            if (factory.IsInFactories(handlerFactories))
            {
                return NullDisposable.Instance;
            }

            handlerFactories.Add(factory);

            return new EventHandlerFactoryUnregistrar(this, eventType, factory);
        }

        public override void Unsubscribe<TEvent>(Func<TEvent, Task> action)
        {
            Check.NotNull(action, nameof(action));

            GetOrCreateHandlerFactories(typeof(TEvent))
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                        {
                            var singleInstanceFactory = factory as SingleInstanceHandlerFactory;
                            if (singleInstanceFactory == null)
                            {
                                return false;
                            }

                            var actionHandler = singleInstanceFactory.HandlerInstance as ActionEventHandler<TEvent>;
                            if (actionHandler == null)
                            {
                                return false;
                            }

                            return actionHandler.Action == action;
                        });
                });
        }

        public override void Unsubscribe(Type eventType, IEventHandler handler)
        {
            GetOrCreateHandlerFactories(eventType)
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                            factory is SingleInstanceHandlerFactory &&
                            (factory as SingleInstanceHandlerFactory)!.HandlerInstance == handler
                    );
                });
        }

        public override void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Remove(factory));
        }

        public override void UnsubscribeAll(Type eventType)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Clear());
        }

        protected override void AddToUnitOfWork(IUnitOfWork unitOfWork, UnitOfWorkEventRecord eventRecord)
        {
            unitOfWork.AddOrReplaceDistributedEvent(eventRecord);
        }

        protected override IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
        {
            var handlerFactoryList = new List<EventTypeWithEventHandlerFactories>();

            foreach (var handlerFactory in
                     HandlerFactories.Where(hf => ShouldTriggerEventForHandler(eventType, hf.Key)))
            {
                handlerFactoryList.Add(
                    new EventTypeWithEventHandlerFactories(handlerFactory.Key, handlerFactory.Value));
            }

            return handlerFactoryList.ToArray();
        }

        protected override async Task PublishToEventBusAsync(Type eventType, object eventData)
        {
            await PublishToMassTransitAsync(eventType, eventData, CorrelationIdProvider.Get(), null);
        }

        protected override byte[] Serialize(object eventData)
        {
            return Serializer.Serialize(eventData);
        }

        public virtual Type GetEventType(string eventName)
        {
            return EventTypes.GetOrDefault(eventName)!;
        }

        private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
        {
            return HandlerFactories.GetOrAdd(
                eventType,
                type =>
                {
                    var eventName = EventNameAttribute.GetNameOrDefault(type);
                    EventTypes.GetOrAdd(eventName, eventType);
                    return new List<IEventHandlerFactory>();
                }
            );
        }

        private static bool ShouldTriggerEventForHandler(Type targetEventType, Type handlerEventType)
        {
            //Should trigger same type
            if (handlerEventType == targetEventType)
            {
                return true;
            }

            //TODO: Support inheritance? But it does not support on subscription to RabbitMq!
            //Should trigger for inherited types
            if (handlerEventType.IsAssignableFrom(targetEventType))
            {
                return true;
            }

            return false;
        }


        protected virtual async Task PublishToMassTransitAsync(Type eventType, object eventData, string? correlationId, Guid? messageId = null, CancellationToken cancellationToken = default)
        {
            await PublishToMassTransitAsync(EventNameAttribute.GetNameOrDefault(eventType), eventData, correlationId, messageId, cancellationToken);
        }

        protected virtual async Task PublishToMassTransitAsync(string eventName, object eventData, string? correlationId, Guid? messageId = null, CancellationToken cancellationToken = default)
        {
            var data = new AbpMassTransitEventData(
                eventName,
                (messageId ?? GuidGenerator.Create()).ToString("N"),
                Serializer.SerializeToString(eventData),
                correlationId);

            await MassTransitPublisher.PublishAsync(data, cancellationToken);
        }

    }
}

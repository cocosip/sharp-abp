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
    /// <summary>
    /// MassTransit-based implementation of distributed event bus that provides event publishing and subscription capabilities.
    /// This class integrates with MassTransit messaging infrastructure to handle distributed events across microservices.
    /// </summary>
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IDistributedEventBus), typeof(MassTransitDistributedEventBus))]
    public class MassTransitDistributedEventBus : DistributedEventBusBase, ISingletonDependency
    {
        /// <summary>
        /// Gets the MassTransit publisher for sending events to the message broker.
        /// </summary>
        protected IMassTransitPublisher MassTransitPublisher { get; }
        
        /// <summary>
        /// Gets the serializer for converting event data to/from JSON format.
        /// </summary>
        protected IMassTransitSerializer Serializer { get; }

        /// <summary>
        /// Gets the concurrent dictionary that stores event handler factories indexed by event type.
        /// </summary>
        protected ConcurrentDictionary<Type, List<IEventHandlerFactory>> HandlerFactories { get; }
        
        /// <summary>
        /// Gets the concurrent dictionary that maps event names to their corresponding types.
        /// </summary>
        protected ConcurrentDictionary<string, Type> EventTypes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MassTransitDistributedEventBus"/> class.
        /// </summary>
        /// <param name="serviceScopeFactory">The service scope factory for creating scoped services.</param>
        /// <param name="currentTenant">The current tenant provider.</param>
        /// <param name="unitOfWorkManager">The unit of work manager for transaction management.</param>
        /// <param name="abpDistributedEventBusOptions">The distributed event bus configuration options.</param>
        /// <param name="guidGenerator">The GUID generator for creating unique identifiers.</param>
        /// <param name="clock">The clock provider for time-related operations.</param>
        /// <param name="eventHandlerInvoker">The event handler invoker for executing event handlers.</param>
        /// <param name="localEventBus">The local event bus for in-process event handling.</param>
        /// <param name="correlationIdProvider">The correlation ID provider for request tracking.</param>
        /// <param name="massTransitPublisher">The MassTransit publisher for sending events.</param>
        /// <param name="serializer">The serializer for event data conversion.</param>
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

        /// <summary>
        /// Initializes the event bus by subscribing to configured event handlers.
        /// This method should be called after the event bus is constructed to set up event subscriptions.
        /// </summary>
        public void Initialize()
        {
            SubscribeHandlers(AbpDistributedEventBusOptions.Handlers);
        }

        /// <summary>
        /// Processes an incoming event message from MassTransit.
        /// This method deserializes the event data and triggers the appropriate event handlers.
        /// </summary>
        /// <param name="message">The MassTransit event data containing event information and payload.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Processes an event from the inbox storage.
        /// This method is used for reliable event processing when events are stored in an inbox before processing.
        /// </summary>
        /// <param name="incomingEvent">The incoming event information from the inbox.</param>
        /// <param name="inboxConfig">The inbox configuration settings.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Throws aggregated exceptions if any handlers fail during processing.</exception>
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

        /// <summary>
        /// Publishes a single event from the outbox storage to MassTransit.
        /// This method is used for reliable event publishing when events are stored in an outbox before sending.
        /// </summary>
        /// <param name="outgoingEvent">The outgoing event information from the outbox.</param>
        /// <param name="outboxConfig">The outbox configuration settings.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Publishes multiple events from the outbox storage to MassTransit in batch.
        /// This method processes multiple outgoing events efficiently for better performance.
        /// </summary>
        /// <param name="outgoingEvents">The collection of outgoing events from the outbox.</param>
        /// <param name="outboxConfig">The outbox configuration settings.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Subscribes an event handler factory to a specific event type.
        /// </summary>
        /// <param name="eventType">The type of event to subscribe to.</param>
        /// <param name="factory">The event handler factory that creates event handlers.</param>
        /// <returns>A disposable object that can be used to unsubscribe the handler.</returns>
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

        /// <summary>
        /// Unsubscribes a specific action-based event handler from an event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of event to unsubscribe from.</typeparam>
        /// <param name="action">The action delegate that handles the event.</param>
        public override void Unsubscribe<TEvent>(Func<TEvent, Task> action)
        {
            Check.NotNull(action, nameof(action));

            GetOrCreateHandlerFactories(typeof(TEvent))
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                        {
                            if (factory is not SingleInstanceHandlerFactory singleInstanceFactory)
                            {
                                return false;
                            }

                            if (singleInstanceFactory.HandlerInstance is not ActionEventHandler<TEvent> actionHandler)
                            {
                                return false;
                            }

                            return actionHandler.Action == action;
                        });
                });
        }

        /// <summary>
        /// Unsubscribes a specific event handler instance from an event type.
        /// </summary>
        /// <param name="eventType">The type of event to unsubscribe from.</param>
        /// <param name="handler">The event handler instance to remove.</param>
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

        /// <summary>
        /// Unsubscribes a specific event handler factory from an event type.
        /// </summary>
        /// <param name="eventType">The type of event to unsubscribe from.</param>
        /// <param name="factory">The event handler factory to remove.</param>
        public override void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Remove(factory));
        }

        /// <summary>
        /// Unsubscribes all event handlers from a specific event type.
        /// </summary>
        /// <param name="eventType">The type of event to unsubscribe all handlers from.</param>
        public override void UnsubscribeAll(Type eventType)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Clear());
        }

        /// <summary>
        /// Adds an event record to the unit of work for distributed event handling.
        /// </summary>
        /// <param name="unitOfWork">The unit of work to add the event to.</param>
        /// <param name="eventRecord">The event record to be added.</param>
        protected override void AddToUnitOfWork(IUnitOfWork unitOfWork, UnitOfWorkEventRecord eventRecord)
        {
            unitOfWork.AddOrReplaceDistributedEvent(eventRecord);
        }

        /// <summary>
        /// Gets the event handler factories for a specific event type, including handlers for base types.
        /// </summary>
        /// <param name="eventType">The type of event to get handlers for.</param>
        /// <returns>An enumerable collection of event type with their corresponding handler factories.</returns>
        protected override IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
        {
            var handlerFactoryList = new List<EventTypeWithEventHandlerFactories>();

            foreach (var handlerFactory in
                     HandlerFactories.Where(hf => ShouldTriggerEventForHandler(eventType, hf.Key)))
            {
                handlerFactoryList.Add(
                    new EventTypeWithEventHandlerFactories(handlerFactory.Key, handlerFactory.Value));
            }

            return [.. handlerFactoryList];
        }

        /// <summary>
        /// Publishes an event to the MassTransit event bus asynchronously.
        /// </summary>
        /// <param name="eventType">The type of the event to publish.</param>
        /// <param name="eventData">The event data to publish.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task PublishToEventBusAsync(Type eventType, object eventData)
        {
            await PublishToMassTransitAsync(eventType, eventData, CorrelationIdProvider.Get(), null);
        }

        /// <summary>
        /// Serializes event data to a byte array using the configured serializer.
        /// </summary>
        /// <param name="eventData">The event data to serialize.</param>
        /// <returns>A byte array containing the serialized event data.</returns>
        protected override byte[] Serialize(object eventData)
        {
            return Serializer.Serialize(eventData);
        }

        /// <summary>
        /// Gets the event type based on the event name.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <returns>The type of the event, or null if not found.</returns>
        public virtual Type GetEventType(string eventName)
        {
            return EventTypes.GetOrDefault(eventName)!;
        }

        /// <summary>
        /// Gets or creates a list of event handler factories for a specific event type.
        /// This method ensures thread-safe access to handler factories and registers the event type mapping.
        /// </summary>
        /// <param name="eventType">The type of event to get or create handlers for.</param>
        /// <returns>A list of event handler factories for the specified event type.</returns>
        private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
        {
            return HandlerFactories.GetOrAdd(
                eventType,
                type =>
                {
                    var eventName = EventNameAttribute.GetNameOrDefault(type);
                    EventTypes.GetOrAdd(eventName, eventType);
                    return [];
                }
            );
        }

        /// <summary>
        /// Determines whether an event should trigger for a specific handler based on type compatibility.
        /// Supports both exact type matches and inheritance-based matching.
        /// </summary>
        /// <param name="targetEventType">The type of the target event.</param>
        /// <param name="handlerEventType">The type of event the handler can process.</param>
        /// <returns>True if the event should trigger for the handler; otherwise, false.</returns>
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


        /// <summary>
        /// Publishes an event to MassTransit using the event type and data.
        /// </summary>
        /// <param name="eventType">The type of the event to publish.</param>
        /// <param name="eventData">The event data to publish.</param>
        /// <param name="correlationId">The correlation ID for request tracking.</param>
        /// <param name="messageId">The optional message ID. If not provided, a new GUID will be generated.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected virtual async Task PublishToMassTransitAsync(Type eventType, object eventData, string? correlationId, Guid? messageId = null, CancellationToken cancellationToken = default)
        {
            await PublishToMassTransitAsync(EventNameAttribute.GetNameOrDefault(eventType), eventData, correlationId, messageId, cancellationToken);
        }

        /// <summary>
        /// Publishes an event to MassTransit using the event name and data.
        /// </summary>
        /// <param name="eventName">The name of the event to publish.</param>
        /// <param name="eventData">The event data to publish.</param>
        /// <param name="correlationId">The correlation ID for request tracking.</param>
        /// <param name="messageId">The optional message ID. If not provided, a new GUID will be generated.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

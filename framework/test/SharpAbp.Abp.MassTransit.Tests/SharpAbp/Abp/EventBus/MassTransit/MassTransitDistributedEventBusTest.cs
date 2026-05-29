using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using SharpAbp.Abp.MassTransit;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;
using Volo.Abp.Json;
using Volo.Abp.Json.SystemTextJson;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Timing;
using Volo.Abp.Tracing;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.Abp.EventBus.MassTransit
{
    public class MassTransitDistributedEventBusTest
    {
        [Fact]
        public void Subscribe_Should_Register_Dynamic_Handler_By_Event_Name()
        {
            var eventBus = CreateEventBus();
            var handlerFactory = new SingleInstanceHandlerFactory(new TestDynamicEventHandler());

            using (eventBus.Subscribe("test.dynamic", handlerFactory))
            {
                var handlerFactories = eventBus.GetDynamicHandlerFactoriesForTest("test.dynamic").ToList();

                Assert.Single(handlerFactories);
                Assert.Same(handlerFactory, handlerFactories[0].EventHandlerFactories.Single());
                Assert.Equal(typeof(DynamicEventData), eventBus.GetEventTypeByEventNameForTest("test.dynamic"));
            }
        }

        [Fact]
        public void Unsubscribe_Should_Remove_Dynamic_Handler_By_Event_Name()
        {
            var eventBus = CreateEventBus();
            var handlerFactory = new SingleInstanceHandlerFactory(new TestDynamicEventHandler());

            eventBus.Subscribe("test.dynamic", handlerFactory);
            eventBus.Unsubscribe("test.dynamic", handlerFactory);

            Assert.Empty(eventBus.GetDynamicHandlerFactoriesForTest("test.dynamic"));
        }

        [Fact]
        public async Task PublishAsync_Should_Publish_Dynamic_Event_By_Event_Name()
        {
            var publisher = new Mock<IMassTransitPublisher>();
            var eventBus = CreateEventBus(publisher.Object);
            var eventData = new DynamicEventData("test.dynamic", new { Value = 42 });

            await eventBus.PublishAsync("test.dynamic", eventData, onUnitOfWorkComplete: false);

            publisher.Verify(
                x => x.PublishAsync(
                    It.Is<AbpMassTransitEventData>(message =>
                        message.EventName == "test.dynamic" &&
                        message.CorrelationId == "correlation-id" &&
                        JsonDocument.Parse(message.JsonData).RootElement.GetProperty("Value").GetInt32() == 42),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private static TestMassTransitDistributedEventBus CreateEventBus(IMassTransitPublisher? publisher = null)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddOptions();
            services.AddTransient<IJsonSerializer, AbpSystemTextJsonSerializer>();
            return new TestMassTransitDistributedEventBus(
                services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>(),
                Mock.Of<ICurrentTenant>(),
                Mock.Of<IUnitOfWorkManager>(),
                Options.Create(new AbpDistributedEventBusOptions()),
                new SimpleGuidGenerator(),
                Mock.Of<IClock>(),
                Mock.Of<IEventHandlerInvoker>(),
                Mock.Of<ILocalEventBus>(),
                Mock.Of<ICorrelationIdProvider>(x => x.Get() == "correlation-id"),
                publisher ?? Mock.Of<IMassTransitPublisher>(),
                new SystemTextJsonMassTransitSerializer());
        }

        private class TestMassTransitDistributedEventBus : MassTransitDistributedEventBus
        {
            public TestMassTransitDistributedEventBus(
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
                IMassTransitSerializer serializer)
                : base(
                    serviceScopeFactory,
                    currentTenant,
                    unitOfWorkManager,
                    abpDistributedEventBusOptions,
                    guidGenerator,
                    clock,
                    eventHandlerInvoker,
                    localEventBus,
                    correlationIdProvider,
                    massTransitPublisher,
                    serializer)
            {
            }

            public System.Collections.Generic.IEnumerable<EventTypeWithEventHandlerFactories> GetDynamicHandlerFactoriesForTest(string eventName)
            {
                return GetDynamicHandlerFactories(eventName);
            }

            public Type GetEventTypeByEventNameForTest(string eventName)
            {
                return GetEventTypeByEventName(eventName);
            }
        }

        private class TestDynamicEventHandler : IDistributedEventHandler<DynamicEventData>
        {
            public Task HandleEventAsync(DynamicEventData eventData)
            {
                return Task.CompletedTask;
            }
        }

        private class SystemTextJsonMassTransitSerializer : IMassTransitSerializer
        {
            public byte[] Serialize(object obj)
            {
                return JsonSerializer.SerializeToUtf8Bytes(obj);
            }

            public string SerializeToString(object obj)
            {
                return JsonSerializer.Serialize(obj);
            }

            public object Deserialize(byte[] value, Type type)
            {
                return JsonSerializer.Deserialize(value, type)!;
            }

            public T Deserialize<T>(byte[] value)
            {
                return JsonSerializer.Deserialize<T>(value)!;
            }

            public object Deserialize(string value, Type type)
            {
                return JsonSerializer.Deserialize(value, type)!;
            }
        }
    }
}

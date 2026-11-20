using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SharpAbp.Abp.MassTransit;
using Volo.Abp;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Testing;
using Volo.Abp.Tracing;
using Volo.Abp.Uow;
using Xunit;

namespace SharpAbp.Abp.EventBus.MassTransit;

public class MassTransitEventBusIntegrationTest : AbpIntegratedTest<MassTransitEventBusIntegrationTestModule>
{
    private MassTransitDistributedEventBus Bus => GetRequiredService<MassTransitDistributedEventBus>();

    [Fact]
    public async Task Should_Deserialize_Typed_Event_And_Restore_Correlation_After_Handling()
    {
        var correlation = GetRequiredService<ICorrelationIdProvider>();
        var before = correlation.Get();
        var values = new List<int>();
        using var subscription = Bus.Subscribe<EventBusTestEvent>(data =>
        {
            values.Add(data.Value);
            Assert.Equal("incoming-correlation", correlation.Get());
            return Task.CompletedTask;
        });

        await Bus.ProcessEventAsync(new AbpMassTransitEventData("tests.typed", "message-1", "{\"value\":42}", "incoming-correlation"));

        Assert.Equal(new[] { 42 }, values);
        Assert.Equal(before, correlation.Get());
    }

    [Theory]
    [InlineData("action")]
    [InlineData("handler")]
    [InlineData("factory")]
    [InlineData("all")]
    [InlineData("disposable")]
    public async Task Should_Stop_Delivering_After_Unsubscription(string removal)
    {
        var calls = 0;
        Func<EventBusTestEvent, Task> action = _ => { calls++; return Task.CompletedTask; };
        var handler = new ActionEventHandler<EventBusTestEvent>(action);
        var factory = new SingleInstanceHandlerFactory(handler);
        var subscription = Bus.Subscribe(typeof(EventBusTestEvent), factory);
        using var duplicate = Bus.Subscribe(typeof(EventBusTestEvent), factory);
        await Bus.ProcessEventAsync(new AbpMassTransitEventData("tests.typed", "first", "{\"value\":1}", null));
        Assert.Equal(1, calls);

        switch (removal)
        {
            case "action": Bus.Unsubscribe(action); break;
            case "handler": Bus.Unsubscribe(typeof(EventBusTestEvent), handler); break;
            case "factory": Bus.Unsubscribe(typeof(EventBusTestEvent), factory); break;
            case "all": Bus.UnsubscribeAll(typeof(EventBusTestEvent)); break;
            case "disposable": subscription.Dispose(); break;
        }
        await Bus.ProcessEventAsync(new AbpMassTransitEventData("tests.typed", "second", "{\"value\":2}", null));

        Assert.Equal(1, calls);
        subscription.Dispose();
    }

    [Fact]
    public async Task Should_Deliver_Derived_Events_To_Base_Handlers_Only_When_Assignable()
    {
        var baseCalls = 0;
        var derivedCalls = 0;
        var unrelatedCalls = 0;
        using var baseSubscription = Bus.Subscribe<EventBusTestEvent>(_ => { baseCalls++; return Task.CompletedTask; });
        using var derivedSubscription = Bus.Subscribe<DerivedEventBusTestEvent>(_ => { derivedCalls++; return Task.CompletedTask; });
        using var otherSubscription = Bus.Subscribe<UnrelatedEventBusTestEvent>(_ => { unrelatedCalls++; return Task.CompletedTask; });

        await Bus.ProcessEventAsync(new AbpMassTransitEventData("tests.derived", "message", "{\"value\":42}", null));

        Assert.Equal(1, baseCalls);
        Assert.Equal(1, derivedCalls);
        Assert.Equal(0, unrelatedCalls);
    }

    [Fact]
    public async Task Should_Defer_Publication_Until_Unit_Of_Work_Completes()
    {
        var output = GetRequiredService<EventBusTestOutput>();
        using var unitOfWork = GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: false);

        await Bus.PublishAsync(new EventBusTestEvent { Value = 42 }, onUnitOfWorkComplete: true, useOutbox: false);
        Assert.Empty(output.Messages);
        await unitOfWork.CompleteAsync();

        var message = Assert.Single(output.Messages);
        Assert.Equal("tests.typed", message.EventName);
        Assert.Equal(42, JsonDocument.Parse(message.JsonData).RootElement.GetProperty("value").GetInt32());
        Assert.True(Guid.TryParseExact(message.MessageId, "N", out _));
    }

    [Fact]
    public async Task Should_Publish_Outbox_Payload_Without_Serializing_Json_Twice()
    {
        var sent = new List<DistributedEventSent>();
        using var subscription = GetRequiredService<ILocalEventBus>().Subscribe<DistributedEventSent>(data =>
        {
            sent.Add(data);
            return Task.CompletedTask;
        });
        var first = new OutgoingEventInfo(Guid.NewGuid(), "tests.first", Encoding.UTF8.GetBytes("{\"value\":1}"), DateTime.UtcNow);
        var second = new OutgoingEventInfo(Guid.NewGuid(), "tests.second", Encoding.UTF8.GetBytes("{\"value\":2}"), DateTime.UtcNow);
        first.SetCorrelationId("first-correlation");

        await Bus.PublishFromOutboxAsync(first, new OutboxConfig("tests"));
        await Bus.PublishManyFromOutboxAsync(new[] { second }, new OutboxConfig("tests"));

        var messages = GetRequiredService<EventBusTestOutput>().Messages;
        Assert.Equal(2, messages.Count);
        Assert.Equal(first.Id.ToString("N"), messages[0].MessageId);
        Assert.Equal("{\"value\":1}", messages[0].JsonData);
        Assert.Equal("first-correlation", messages[0].CorrelationId);
        Assert.Equal(second.Id.ToString("N"), messages[1].MessageId);
        Assert.Equal("{\"value\":2}", messages[1].JsonData);
        Assert.Equal(2, sent.Count);
        Assert.All(sent, item => Assert.Equal(DistributedEventSource.Outbox, item.Source));
    }

    [Fact]
    public async Task Should_Use_Requested_Dynamic_Name_And_Unwrap_Event_Data()
    {
        await Bus.PublishAsync("tests.requested", new DynamicEventData("tests.original", new { Value = 42 }), false, false);

        var message = Assert.Single(GetRequiredService<EventBusTestOutput>().Messages);
        Assert.Equal("tests.requested", message.EventName);
        var payload = JsonDocument.Parse(message.JsonData).RootElement;
        Assert.Equal(42, payload.GetProperty("value").GetInt32());
        Assert.False(payload.TryGetProperty("eventName", out _));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Should_Deliver_Dynamic_Envelope_Name_And_Payload_From_Broker_Or_Inbox(bool fromInbox)
    {
        DynamicEventData received = null;
        var tenant = GetRequiredService<ICurrentTenant>();
        var tenantId = Guid.Parse("bac62db4-25ba-4fe2-b4fe-bbe22ecda350");
        var previousTenant = tenant.Id;
        using var subscription = Bus.Subscribe("tests.dynamic", new SingleInstanceHandlerFactory(
            new ActionEventHandler<DynamicEventData>(data =>
            {
                received = data;
                Assert.Equal(fromInbox ? tenantId : (Guid?)null, tenant.Id);
                return Task.CompletedTask;
            })));

        if (fromInbox)
        {
            var incoming = new IncomingEventInfo(Guid.NewGuid(), "message", "tests.dynamic", Encoding.UTF8.GetBytes("{\"value\":42}"), DateTime.UtcNow);
            incoming.SetTenantId(tenantId);
            await Bus.ProcessFromInboxAsync(incoming, new InboxConfig("tests"));
        }
        else
        {
            await Bus.ProcessEventAsync(new AbpMassTransitEventData("tests.dynamic", "message", "{\"value\":42}", null));
        }

        Assert.NotNull(received);
        Assert.Equal("tests.dynamic", received.EventName);
        var json = GetRequiredService<Volo.Abp.Json.IJsonSerializer>().Serialize(received.Data);
        Assert.Equal(42, JsonDocument.Parse(json).RootElement.GetProperty("value").GetInt32());
        Assert.Equal(previousTenant, tenant.Id);
    }
}

[EventName("tests.typed")]
public class EventBusTestEvent { public int Value { get; set; } }
[EventName("tests.derived")]
public class DerivedEventBusTestEvent : EventBusTestEvent { }
[EventName("tests.unrelated")]
public class UnrelatedEventBusTestEvent { }

public class EventBusTestOutput
{
    public List<AbpMassTransitEventData> Messages { get; } = [];
}

[DependsOn(typeof(AbpEventBusMassTransitModule), typeof(AbpTestBaseModule))]
public class MassTransitEventBusIntegrationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
        => context.Services.AddSingleton<EventBusTestOutput>();

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        var publisher = new Mock<IMassTransitPublisher>();
        context.Services.AddSingleton<IMassTransitPublisher>(services =>
        {
            publisher.Setup(x => x.PublishAsync(It.IsAny<AbpMassTransitEventData>(), It.IsAny<CancellationToken>()))
                .Callback<AbpMassTransitEventData, CancellationToken>((message, _) => services.GetRequiredService<EventBusTestOutput>().Messages.Add(message))
                .Returns(Task.CompletedTask);
            return publisher.Object;
        });
    }
}

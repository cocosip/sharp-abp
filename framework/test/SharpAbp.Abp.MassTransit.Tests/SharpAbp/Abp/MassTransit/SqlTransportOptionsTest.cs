using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using SharpAbp.Abp.MassTransit.PostgreSql;
using SharpAbp.Abp.MassTransit.SqlServer;
using Xunit;

namespace SharpAbp.Abp.MassTransit;

public class SqlTransportOptionsTest
{
    [Fact]
    public void PostgreSql_Should_Bind_Startup_Policy_And_Copy_Independent_Transport_Settings()
    {
        var source = new AbpMassTransitPostgreSqlOptions().PreConfigure(Configuration("PostgreSqlOptions"));
        source.Producers.Add(new PostgreSqlProducerConfiguration());
        source.Consumers.Add(new PostgreSqlConsumerConfiguration());
        source.PostgreSqlPreConfigures.Add((_, _) => { });
        source.PostgreSqlConfigures.Add((_, _) => { });
        source.PostgreSqlPostConfigures.Add((_, _) => { });

        var copy = new AbpMassTransitPostgreSqlOptions().CopyFrom(source);
        source.SqlTransportOptions.Host = "changed.example";
        source.Producers.Clear();
        source.Consumers.Clear();
        source.PostgreSqlConfigures.Clear();

        Assert.False(copy.AutoStart);
        Assert.False(copy.Create);
        Assert.True(copy.Delete);
        Assert.Equal(TimeSpan.FromSeconds(30), copy.AutoDeleteOnIdle);
        Assert.Equal(3, copy.ConcurrentMessageLimit);
        Assert.Equal(12, copy.PrefetchCount);
        Assert.Equal("Host=broker.example;Database=messages;Username=app", copy.ConnectionString);
        AssertTransport(copy.SqlTransportOptions);
        Assert.Single(copy.Producers);
        Assert.Single(copy.Consumers);
        Assert.Single(copy.PostgreSqlPreConfigures);
        Assert.Single(copy.PostgreSqlConfigures);
        Assert.Single(copy.PostgreSqlPostConfigures);
        Assert.Throws<ArgumentNullException>(() => copy.CopyFrom(null));
        Assert.Null(new AbpMassTransitPostgreSqlOptions().CopyFrom(new AbpMassTransitPostgreSqlOptions()).SqlTransportOptions);
    }

    [Fact]
    public void SqlServer_Should_Bind_Startup_Policy_And_Copy_Independent_Transport_Settings()
    {
        var source = new AbpMassTransitSqlServerOptions().PreConfigure(Configuration("SqlServerOptions"));
        source.Producers.Add(new SqlServerProducerConfiguration());
        source.Consumers.Add(new SqlServerConsumerConfiguration());
        source.SqlServerPreConfigures.Add((_, _) => { });
        source.SqlServerConfigures.Add((_, _) => { });
        source.SqlServerPostConfigures.Add((_, _) => { });

        var copy = new AbpMassTransitSqlServerOptions().CopyFrom(source);
        source.SqlTransportOptions.Host = "changed.example";
        source.Producers.Clear();
        source.Consumers.Clear();
        source.SqlServerConfigures.Clear();

        Assert.False(copy.AutoStart);
        Assert.False(copy.Create);
        Assert.True(copy.Delete);
        Assert.Equal(TimeSpan.FromSeconds(30), copy.AutoDeleteOnIdle);
        Assert.Equal(3, copy.ConcurrentMessageLimit);
        Assert.Equal(12, copy.PrefetchCount);
        Assert.Equal("Host=broker.example;Database=messages;Username=app", copy.ConnectionString);
        AssertTransport(copy.SqlTransportOptions);
        Assert.Single(copy.Producers);
        Assert.Single(copy.Consumers);
        Assert.Single(copy.SqlServerPreConfigures);
        Assert.Single(copy.SqlServerConfigures);
        Assert.Single(copy.SqlServerPostConfigures);
        Assert.Throws<ArgumentNullException>(() => copy.CopyFrom(null));
        Assert.Null(new AbpMassTransitSqlServerOptions().CopyFrom(new AbpMassTransitSqlServerOptions()).SqlTransportOptions);
    }

    private static void AssertTransport(global::MassTransit.SqlTransportOptions options)
    {
        Assert.Equal("broker.example", options.Host);
        Assert.Equal(5433, options.Port);
        Assert.Equal("messages", options.Database);
        Assert.Equal("transport", options.Schema);
        Assert.Equal("worker", options.Role);
        Assert.Equal("app", options.Username);
        Assert.Equal("test-password", options.Password);
        Assert.Equal("admin", options.AdminUsername);
        Assert.Equal("test-admin-password", options.AdminPassword);
    }

    private static IConfiguration Configuration(string section)
    {
        var values = new Dictionary<string, string>
        {
            [$"MassTransitOptions:{section}:ConnectionString"] = "Host=broker.example;Database=messages;Username=app",
            [$"MassTransitOptions:{section}:Create"] = "false",
            [$"MassTransitOptions:{section}:Delete"] = "true",
            [$"MassTransitOptions:{section}:AutoStart"] = "false",
            [$"MassTransitOptions:{section}:AutoDeleteOnIdle"] = "00:00:30",
            [$"MassTransitOptions:{section}:ConcurrentMessageLimit"] = "3",
            [$"MassTransitOptions:{section}:PrefetchCount"] = "12",
            [$"MassTransitOptions:{section}:SqlTransportOptions:Host"] = "broker.example",
            [$"MassTransitOptions:{section}:SqlTransportOptions:Port"] = "5433",
            [$"MassTransitOptions:{section}:SqlTransportOptions:Database"] = "messages",
            [$"MassTransitOptions:{section}:SqlTransportOptions:Schema"] = "transport",
            [$"MassTransitOptions:{section}:SqlTransportOptions:Role"] = "worker",
            [$"MassTransitOptions:{section}:SqlTransportOptions:Username"] = "app",
            [$"MassTransitOptions:{section}:SqlTransportOptions:Password"] = "test-password",
            [$"MassTransitOptions:{section}:SqlTransportOptions:AdminUsername"] = "admin",
            [$"MassTransitOptions:{section}:SqlTransportOptions:AdminPassword"] = "test-admin-password"
        };
        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }
}

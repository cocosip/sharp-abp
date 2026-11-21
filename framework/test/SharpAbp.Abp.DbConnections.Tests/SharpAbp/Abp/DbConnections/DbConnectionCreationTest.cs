using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using SharpAbp.Abp.Data;
using Volo.Abp;
using Xunit;

namespace SharpAbp.Abp.DbConnections
{
    public class DbConnectionCreationTest : AbpDbConnectionsTestBase
    {
        [Theory]
        [InlineData(DatabaseProvider.MySql)]
        [InlineData(DatabaseProvider.PostgreSql)]
        [InlineData(DatabaseProvider.SqlServer)]
        [InlineData(DatabaseProvider.Oracle)]
        [InlineData(DatabaseProvider.OceanBase)]
        [InlineData(DatabaseProvider.Sqlite)]
        public void Module_Registers_Creator_Under_Its_Provider_Key(DatabaseProvider provider)
        {
            var creator = ServiceProvider.GetRequiredKeyedService<IInternalDbConnectionCreator>(provider);
            Assert.Equal(provider, creator.DatabaseProvider);
        }

        [Fact]
        public async Task Typed_Factory_Uses_Attribute_Name_And_Creates_Independent_Closed_Connections()
        {
            GetRequiredService<IOptions<AbpDbConnectionsOptions>>().Value.DbConnections.Configure<ReportingConnection>(configuration =>
            {
                configuration.DatabaseProvider = DatabaseProvider.Sqlite;
                configuration.ConnectionString = "Data Source=:memory:";
            });
            var factory = GetRequiredService<IDbConnectionFactory>();

            using var first = await factory.GetDbConnectionAsync<ReportingConnection>();
            using var second = await factory.GetDbConnectionAsync<ReportingConnection>();

            Assert.IsType<SqliteConnection>(first);
            Assert.NotSame(first, second);
            Assert.Equal(ConnectionState.Closed, first.State);
            Assert.Equal(DatabaseProvider.Sqlite, (await factory.GetDbConnectionInfoAsync<ReportingConnection>()).DatabaseProvider);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Public_Named_Operations_Reject_Empty_Names(string name)
        {
            await Assert.ThrowsAnyAsync<ArgumentException>(() => GetRequiredService<IDbConnectionFactory>().GetDbConnectionAsync(name));
            await Assert.ThrowsAnyAsync<ArgumentException>(() => GetRequiredService<IDbConnectionFactory>().GetDbConnectionInfoAsync(name));
            await Assert.ThrowsAnyAsync<ArgumentException>(() => GetRequiredService<IDbConnectionCreateService>().CreateAsync(name));
            await Assert.ThrowsAnyAsync<ArgumentException>(() => GetRequiredService<IDbConnectionInfoResolver>().ResolveAsync(name));
            Assert.ThrowsAny<ArgumentException>(() => new DbConnectionConfigurations().Configure(name, _ => { }));
            Assert.ThrowsAny<ArgumentException>(() => new DbConnectionConfigurations().GetConfiguration(name));
        }

        [Fact]
        public async Task Creation_Rejects_Unsupported_Providers_And_Null_Info()
        {
            var service = GetRequiredService<IDbConnectionCreateService>();
            var options = GetRequiredService<IOptions<AbpDbConnectionsOptions>>().Value;
            options.DbConnections.Configure("unsupported", configuration => configuration.DatabaseProvider = DatabaseProvider.Cosmos);

            await Assert.ThrowsAsync<AbpException>(() => service.CreateAsync("unsupported"));
            await Assert.ThrowsAsync<AbpException>(() => service.CreateAsync(new DbConnectionInfo(DatabaseProvider.Cosmos, "")));
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateAsync((DbConnectionInfo)null));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Explicit_Creator_Takes_Precedence_And_Is_Not_Retried_When_Registered_Twice(bool matches)
        {
            var options = new AbpDbConnectionsOptions();
            options.DatabaseProviders.Add(DatabaseProvider.Sqlite);
            options.Creators.Add<ConfiguredCreator>();
            var configured = new Mock<IDbConnectionCreator>();
            var conventional = new Mock<IDbConnectionCreator>();
            using var configuredConnection = new SqliteConnection("Data Source=:memory:");
            using var conventionalConnection = new SqliteConnection("Data Source=:memory:");
            var info = new DbConnectionInfo(DatabaseProvider.Sqlite, "Data Source=:memory:");
            configured.Setup(creator => creator.IsMatch("reporting", info)).Returns(matches);
            configured.Setup(creator => creator.CreateAsync("reporting", info)).ReturnsAsync(configuredConnection);
            conventional.Setup(creator => creator.IsMatch("reporting", info)).Returns(true);
            conventional.Setup(creator => creator.CreateAsync("reporting", info)).ReturnsAsync(conventionalConnection);
            using var services = new ServiceCollection()
                .AddSingleton(configured)
                .AddTransient<IDbConnectionCreator, ConfiguredCreator>()
                .AddSingleton(conventional.Object)
                .BuildServiceProvider();
            var resolver = new Mock<IDbConnectionInfoResolver>();
            resolver.Setup(value => value.ResolveAsync("reporting")).ReturnsAsync(info);
            var service = new DbConnectionCreateService(Options.Create(options), services, resolver.Object);

            var result = await service.CreateAsync("reporting");

            Assert.Same(matches ? configuredConnection : conventionalConnection, result);
            configured.Verify(creator => creator.IsMatch("reporting", info), Times.Once);
            conventional.Verify(creator => creator.IsMatch("reporting", info), matches ? Times.Never() : Times.Once());
        }

        [Fact]
        public async Task Explicit_Creators_Are_Disposed_After_Each_Attempt()
        {
            var options = new AbpDbConnectionsOptions();
            options.DatabaseProviders.Add(DatabaseProvider.Sqlite);
            options.Creators.Add<UnmatchedCreator>();
            options.Creators.Add<AsyncCreator>();
            var lifetime = new CreatorLifetime();
            var creator = new Mock<IDbConnectionCreator>();
            using var services = new ServiceCollection()
                .AddSingleton(lifetime)
                .AddSingleton(creator.Object)
                .BuildServiceProvider();
            var info = new DbConnectionInfo(DatabaseProvider.Sqlite, "Data Source=:memory:");
            var resolver = new Mock<IDbConnectionInfoResolver>();
            resolver.Setup(value => value.ResolveAsync("reporting")).ReturnsAsync(info);
            var service = new DbConnectionCreateService(Options.Create(options), services, resolver.Object);
            var connection = new Mock<IDbConnection>();
            var error = new InvalidOperationException("Creation failed.");
            creator.SetupSequence(value => value.CreateAsync("reporting", info))
                .ThrowsAsync(error)
                .ReturnsAsync(connection.Object);

            Assert.Same(error, await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync("reporting")));
            Assert.Equal(1, lifetime.Disposed);
            Assert.Equal(1, lifetime.DisposedAsync);

            Assert.Same(connection.Object, await service.CreateAsync("reporting"));
            Assert.Equal(2, lifetime.Disposed);
            Assert.Equal(2, lifetime.DisposedAsync);
            connection.Verify(value => value.Dispose(), Times.Never);
        }

        [Fact]
        public async Task Creation_Disposes_Connection_If_Creator_Cleanup_Fails()
        {
            var options = new AbpDbConnectionsOptions();
            options.DatabaseProviders.Add(DatabaseProvider.Sqlite);
            options.Creators.Add<AsyncCreator>();
            var error = new InvalidOperationException("Creator cleanup failed.");
            var lifetime = new CreatorLifetime { CleanupError = error };
            var creator = new Mock<IDbConnectionCreator>();
            using var services = new ServiceCollection()
                .AddSingleton(lifetime)
                .AddSingleton(creator.Object)
                .BuildServiceProvider();
            var info = new DbConnectionInfo(DatabaseProvider.Sqlite, "Data Source=:memory:");
            var resolver = new Mock<IDbConnectionInfoResolver>();
            resolver.Setup(value => value.ResolveAsync("reporting")).ReturnsAsync(info);
            var service = new DbConnectionCreateService(Options.Create(options), services, resolver.Object);
            var connection = new Mock<IDbConnection>();
            creator.Setup(value => value.CreateAsync("reporting", info)).ReturnsAsync(connection.Object);

            Assert.Same(error, await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync("reporting")));
            connection.Verify(value => value.Dispose(), Times.Once);
        }

        [Fact]
        public async Task Creation_Reports_Missing_Info_And_Requires_A_Keyed_Creator_For_Named_Connections()
        {
            var options = new AbpDbConnectionsOptions();
            options.DatabaseProviders.Add(DatabaseProvider.Sqlite);
            var resolver = new Mock<IDbConnectionInfoResolver>();
            using var services = new ServiceCollection().BuildServiceProvider();
            var service = new DbConnectionCreateService(Options.Create(options), services, resolver.Object);

            await Assert.ThrowsAsync<AbpException>(() => service.CreateAsync("missing"));
            var info = new DbConnectionInfo(DatabaseProvider.Sqlite, "Data Source=:memory:");
            resolver.Setup(value => value.ResolveAsync("reporting")).ReturnsAsync(info);
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync("reporting"));
            Assert.Null(await service.CreateAsync(info));
        }

        [Fact]
        public async Task Factory_Rejects_Null_Resolver_And_Creator_Results()
        {
            var factory = new DefaultDbConnectionFactory(new Mock<IDbConnectionInfoResolver>().Object,
                new Mock<IDbConnectionCreateService>().Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => factory.GetDbConnectionInfoAsync("missing"));
            await Assert.ThrowsAsync<ArgumentNullException>(() => factory.GetDbConnectionAsync("missing"));
        }

        [DbConnectionName("reporting")]
        private class ReportingConnection { }

        public class ConfiguredCreator(Mock<IDbConnectionCreator> creator) : IDbConnectionCreator
        {
            public bool IsMatch(string name, DbConnectionInfo info) => creator.Object.IsMatch(name, info);
            public Task<IDbConnection> CreateAsync(string name, DbConnectionInfo info) => creator.Object.CreateAsync(name, info);
        }

        private sealed class CreatorLifetime
        {
            public int Disposed { get; set; }
            public int DisposedAsync { get; set; }
            public Exception CleanupError { get; init; }
        }

        private sealed class UnmatchedCreator(CreatorLifetime lifetime) : IDbConnectionCreator, IDisposable
        {
            public bool IsMatch(string dbConnectionName, DbConnectionInfo dbConnectionInfo) => false;
            public Task<IDbConnection> CreateAsync(string dbConnectionName, DbConnectionInfo dbConnectionInfo)
                => throw new InvalidOperationException("An unmatched creator cannot create a connection.");
            public void Dispose() => lifetime.Disposed++;
        }

        private sealed class AsyncCreator(CreatorLifetime lifetime, IDbConnectionCreator creator)
            : IDbConnectionCreator, IAsyncDisposable
        {
            public bool IsMatch(string dbConnectionName, DbConnectionInfo dbConnectionInfo) => true;
            public Task<IDbConnection> CreateAsync(string dbConnectionName, DbConnectionInfo dbConnectionInfo)
                => creator.CreateAsync(dbConnectionName, dbConnectionInfo);

            public ValueTask DisposeAsync()
            {
                lifetime.DisposedAsync++;
                return lifetime.CleanupError == null ? ValueTask.CompletedTask : ValueTask.FromException(lifetime.CleanupError);
            }
        }
    }
}

using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.Data;
using SharpAbp.Abp.DbConnections.DM;
using SharpAbp.Abp.DbConnections.GaussDB;
using SharpAbp.Abp.DbConnections.Oracle.Devart;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Xunit;

namespace SharpAbp.Abp.DbConnections
{
    public class AdditionalProviderTest : AbpIntegratedTest<AdditionalProviderTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
            => options.UseAutofac();

        [Theory]
        [InlineData(DatabaseProvider.Dm, "Server=localhost;User Id=app;Password=unused", "Dm.DmConnection")]
        [InlineData(DatabaseProvider.GaussDB, "Host=localhost;Database=app;Username=app", "GaussDB.GaussDBConnection")]
        [InlineData(DatabaseProvider.Oracle, "Server=localhost;User Id=app;Password=unused", "Devart.Data.Oracle.OracleConnection")]
        public async Task Module_Registers_Provider_And_Creates_Closed_Connections(DatabaseProvider provider, string connectionString, string connectionType)
        {
            Assert.Contains(provider, GetRequiredService<IOptions<AbpDbConnectionsOptions>>().Value.DatabaseProviders);
            var creator = ServiceProvider.GetRequiredKeyedService<IInternalDbConnectionCreator>(provider);
            Assert.Equal(provider, creator.DatabaseProvider);

            using var connection = await GetRequiredService<IDbConnectionCreateService>()
                .CreateAsync(new DbConnectionInfo(provider, connectionString));

            Assert.Equal(connectionType, connection.GetType().FullName);
            Assert.Equal(ConnectionState.Closed, connection.State);
            Assert.Contains("localhost", connection.ConnectionString);
        }
    }

    [DependsOn(typeof(AbpDbConnectionsDmModule), typeof(AbpDbConnectionsGaussDBModule),
        typeof(AbpDbConnectionsOracleDevartModule), typeof(AbpTestBaseModule), typeof(AbpAutofacModule))]
    public class AdditionalProviderTestModule : AbpModule { }
}

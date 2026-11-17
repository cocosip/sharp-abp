using SharpAbp.Abp.Data;
using System.Threading.Tasks;
using Volo.Abp;
using Xunit;

namespace SharpAbp.Abp.DbConnections
{
    public class DefaultDbConnectionInfoResolverTest : AbpDbConnectionsTestBase
    {
        private readonly IDbConnectionInfoResolver _resolver;

        public DefaultDbConnectionInfoResolverTest()
        {
            _resolver = GetRequiredService<IDbConnectionInfoResolver>();
        }

        [Fact]
        public async Task ResolveAsync_Should_Return_Configured_Connection_Info()
        {
            var result = await _resolver.ResolveAsync("db1");

            Assert.Equal(DatabaseProvider.MySql, result.DatabaseProvider);
            Assert.Equal("Server=127.0.0.1;Port=3306;Database=demo1;User=root;Charset=utf8;", result.ConnectionString);
        }

        [Fact]
        public async Task ResolveAsync_Should_Throw_When_Name_Is_Unknown()
        {
            await Assert.ThrowsAsync<AbpException>(() => _resolver.ResolveAsync("missing"));
        }
    }
}

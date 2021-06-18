using Devart.Data.Oracle;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using Npgsql;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.Data.DbConnections
{
    public class DbConnectionFactoryTest : AbpDataDbConnectionsTestBase
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public DbConnectionFactoryTest()
        {
            _dbConnectionFactory = GetRequiredService<IDbConnectionFactory>();
        }

        [Fact]
        public async Task GetDbConnectionAsync_Test()
        {
            var dbConnection1 = await _dbConnectionFactory.GetDbConnectionAsync("db1");
            Assert.Equal(typeof(MySqlConnection), dbConnection1.GetType());
            Assert.Equal("Server=127.0.0.1;Port=3306;Database=demo1;User=root;Charset=utf8;", dbConnection1.ConnectionString);

            var dbConnection2 = await _dbConnectionFactory.GetDbConnectionAsync("db2");
            Assert.Equal(typeof(NpgsqlConnection), dbConnection2.GetType());
            Assert.Equal("Server=127.0.0.1;Port=9432;Username=root;Password=123456;Database=demo2;", dbConnection2.ConnectionString);

            var dbConnection3 = await _dbConnectionFactory.GetDbConnectionAsync("db3");
            Assert.Equal(typeof(SqlConnection), dbConnection3.GetType());
            Assert.Equal("server=.;database=demo3;integrated security=SSPI", dbConnection3.ConnectionString);

            var dbConnection4 = await _dbConnectionFactory.GetDbConnectionAsync("db4");
            Assert.Equal(typeof(OracleConnection), dbConnection4.GetType());
            Assert.Equal("User ID=root;Password=123456;Data Source=(DESCRIPTION = (ADDRESS_LIST= (ADDRESS = (PROTOCOL = TCP)(HOST = 127.0.0.1)(PORT = 1521))) (CONNECT_DATA = (SERVICE_NAME = demo4)));", dbConnection4.ConnectionString);

        }
    }
}

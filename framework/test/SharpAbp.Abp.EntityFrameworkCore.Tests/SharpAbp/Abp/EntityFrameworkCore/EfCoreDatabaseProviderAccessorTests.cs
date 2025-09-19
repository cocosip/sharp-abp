using SharpAbp.Abp.Data;
using Volo.Abp.DependencyInjection;
using Xunit;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    /// <summary>
    /// Unit tests for "EfCoreDatabaseProviderAccessor" class.
    /// Tests the mapping of database provider names to DatabaseProvider enum values.
    /// </summary>
    public class EfCoreDatabaseProviderAccessorTests : AbpEntityFrameworkCoreTestBase
    {
        private readonly IEfCoreDatabaseProviderAccessor _databaseProviderAccessor;

        /// <summary>
        /// Initializes a new instance of the "EfCoreDatabaseProviderAccessorTests" class.
        /// </summary>
        public EfCoreDatabaseProviderAccessorTests()
        {
            _databaseProviderAccessor = GetRequiredService<IEfCoreDatabaseProviderAccessor>();
        }

        /// <summary>
        /// Tests that SQL Server provider names are correctly mapped to DatabaseProvider.SqlServer.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNull_SqlServer_ShouldReturnSqlServer()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.SqlServer, _databaseProviderAccessor.GetDatabaseProviderOrNull("SQLSERVER"));
            Assert.Equal(DatabaseProvider.SqlServer, _databaseProviderAccessor.GetDatabaseProviderOrNull("sqlserver"));
            Assert.Equal(DatabaseProvider.SqlServer, _databaseProviderAccessor.GetDatabaseProviderOrNull("SqlServer"));
        }

        /// <summary>
        /// Tests that PostgreSQL provider names are correctly mapped to DatabaseProvider.PostgreSql.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNull_PostgreSql_ShouldReturnPostgreSql()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.PostgreSql, _databaseProviderAccessor.GetDatabaseProviderOrNull("POSTGRESQL"));
            Assert.Equal(DatabaseProvider.PostgreSql, _databaseProviderAccessor.GetDatabaseProviderOrNull("postgresql"));
            Assert.Equal(DatabaseProvider.PostgreSql, _databaseProviderAccessor.GetDatabaseProviderOrNull("PostgreSql"));
        }

        /// <summary>
        /// Tests that MySQL provider names are correctly mapped to DatabaseProvider.MySql.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNull_MySql_ShouldReturnMySql()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.MySql, _databaseProviderAccessor.GetDatabaseProviderOrNull("MYSQL"));
            Assert.Equal(DatabaseProvider.MySql, _databaseProviderAccessor.GetDatabaseProviderOrNull("mysql"));
            Assert.Equal(DatabaseProvider.MySql, _databaseProviderAccessor.GetDatabaseProviderOrNull("MySql"));
        }

        /// <summary>
        /// Tests that Oracle provider names are correctly mapped to DatabaseProvider.Oracle.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNull_Oracle_ShouldReturnOracle()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNull("ORACLE"));
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNull("oracle"));
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNull("Oracle"));
        }

        /// <summary>
        /// Tests that Devart Oracle provider names are correctly mapped to DatabaseProvider.Oracle.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNull_DevartOracle_ShouldReturnOracle()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNull("DEVART.ORACLE"));
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNull("devart.oracle"));
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNull("Devart.Oracle"));
        }

        /// <summary>
        /// Tests that SQLite provider names are correctly mapped to DatabaseProvider.Sqlite.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNull_Sqlite_ShouldReturnSqlite()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Sqlite, _databaseProviderAccessor.GetDatabaseProviderOrNull("SQLITE"));
            Assert.Equal(DatabaseProvider.Sqlite, _databaseProviderAccessor.GetDatabaseProviderOrNull("sqlite"));
            Assert.Equal(DatabaseProvider.Sqlite, _databaseProviderAccessor.GetDatabaseProviderOrNull("Sqlite"));
        }

        /// <summary>
        /// Tests that InMemory provider names are correctly mapped to DatabaseProvider.InMemory.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNull_InMemory_ShouldReturnInMemory()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.InMemory, _databaseProviderAccessor.GetDatabaseProviderOrNull("INMEMORY"));
            Assert.Equal(DatabaseProvider.InMemory, _databaseProviderAccessor.GetDatabaseProviderOrNull("inmemory"));
            Assert.Equal(DatabaseProvider.InMemory, _databaseProviderAccessor.GetDatabaseProviderOrNull("InMemory"));
        }

        /// <summary>
        /// Tests that Firebird provider names are correctly mapped to DatabaseProvider.Firebird.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNull_Firebird_ShouldReturnFirebird()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Firebird, _databaseProviderAccessor.GetDatabaseProviderOrNull("FIREBIRD"));
            Assert.Equal(DatabaseProvider.Firebird, _databaseProviderAccessor.GetDatabaseProviderOrNull("firebird"));
            Assert.Equal(DatabaseProvider.Firebird, _databaseProviderAccessor.GetDatabaseProviderOrNull("Firebird"));
        }

        /// <summary>
        /// Tests that Cosmos provider names are correctly mapped to DatabaseProvider.Cosmos.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNull_Cosmos_ShouldReturnCosmos()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Cosmos, _databaseProviderAccessor.GetDatabaseProviderOrNull("COSMOS"));
            Assert.Equal(DatabaseProvider.Cosmos, _databaseProviderAccessor.GetDatabaseProviderOrNull("cosmos"));
            Assert.Equal(DatabaseProvider.Cosmos, _databaseProviderAccessor.GetDatabaseProviderOrNull("Cosmos"));
        }

        /// <summary>
        /// Tests that DM provider names are correctly mapped to DatabaseProvider.Dm.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNull_Dm_ShouldReturnDm()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Dm, _databaseProviderAccessor.GetDatabaseProviderOrNull("DM"));
            Assert.Equal(DatabaseProvider.Dm, _databaseProviderAccessor.GetDatabaseProviderOrNull("dm"));
            Assert.Equal(DatabaseProvider.Dm, _databaseProviderAccessor.GetDatabaseProviderOrNull("Dm"));
        }

        /// <summary>
        /// Tests that OpenGauss provider names are correctly mapped to DatabaseProvider.OpenGauss.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNull_OpenGauss_ShouldReturnOpenGauss()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.OpenGauss, _databaseProviderAccessor.GetDatabaseProviderOrNull("OPENGAUSS"));
            Assert.Equal(DatabaseProvider.OpenGauss, _databaseProviderAccessor.GetDatabaseProviderOrNull("opengauss"));
            Assert.Equal(DatabaseProvider.OpenGauss, _databaseProviderAccessor.GetDatabaseProviderOrNull("OpenGauss"));
        }

        /// <summary>
        /// Tests that GaussDB provider names are correctly mapped to DatabaseProvider.GaussDB.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNull_GaussDB_ShouldReturnGaussDB()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.GaussDB, _databaseProviderAccessor.GetDatabaseProviderOrNull("GAUSSDB"));
            Assert.Equal(DatabaseProvider.GaussDB, _databaseProviderAccessor.GetDatabaseProviderOrNull("gaussdb"));
            Assert.Equal(DatabaseProvider.GaussDB, _databaseProviderAccessor.GetDatabaseProviderOrNull("GaussDB"));
        }

        /// <summary>
        /// Tests that null or empty provider names return null.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetDatabaseProviderOrNull_NullOrEmptyProviderName_ShouldReturnNull(string providerName)
        {
            // Act & Assert
            Assert.Null(_databaseProviderAccessor.GetDatabaseProviderOrNull(providerName));
        }

        /// <summary>
        /// Tests that unknown provider names return null.
        /// </summary>
        [Theory]
        [InlineData("UNKNOWN")]
        [InlineData("INVALID")]
        [InlineData("NOTEXIST")]
        [InlineData("RANDOM")]
        public void GetDatabaseProviderOrNull_UnknownProviderName_ShouldReturnNull(string providerName)
        {
            // Act & Assert
            Assert.Null(_databaseProviderAccessor.GetDatabaseProviderOrNull(providerName));
        }

        /// <summary>
        /// Tests that the service is correctly registered as transient dependency.
        /// </summary>
        [Fact]
        public void EfCoreDatabaseProviderAccessor_ShouldBeRegisteredAsTransientDependency()
        {
            // Act
            var service1 = GetRequiredService<IEfCoreDatabaseProviderAccessor>();
            var service2 = GetRequiredService<IEfCoreDatabaseProviderAccessor>();

            // Assert
            Assert.NotNull(service1);
            Assert.NotNull(service2);
            Assert.NotSame(service1, service2); // Transient services should be different instances
        }

        /// <summary>
        /// Tests that the concrete implementation is correctly registered.
        /// </summary>
        [Fact]
        public void EfCoreDatabaseProviderAccessor_ShouldReturnCorrectImplementation()
        {
            // Act
            var service = GetRequiredService<IEfCoreDatabaseProviderAccessor>();

            // Assert
            Assert.NotNull(service);
            Assert.IsType<EfCoreDatabaseProviderAccessor>(service);
        }
    }
}
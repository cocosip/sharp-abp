using SharpAbp.Abp.Data;
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

        #region GetDatabaseProviderOrNullByCustomName Tests

        /// <summary>
        /// Tests that SQL Server custom provider names are correctly mapped to DatabaseProvider.SqlServer.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByCustomName_SqlServer_ShouldReturnSqlServer()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.SqlServer, _databaseProviderAccessor.GetDatabaseProviderOrNull("SQLSERVER"));
            Assert.Equal(DatabaseProvider.SqlServer, _databaseProviderAccessor.GetDatabaseProviderOrNull("sqlserver"));
            Assert.Equal(DatabaseProvider.SqlServer, _databaseProviderAccessor.GetDatabaseProviderOrNull("SqlServer"));
            // Additional case variations
            Assert.Equal(DatabaseProvider.SqlServer, _databaseProviderAccessor.GetDatabaseProviderOrNull("SqlSeRvEr"));
            Assert.Equal(DatabaseProvider.SqlServer, _databaseProviderAccessor.GetDatabaseProviderOrNull("sQLsERVER"));
        }

        /// <summary>
        /// Tests that PostgreSQL custom provider names are correctly mapped to DatabaseProvider.PostgreSql.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByCustomName_PostgreSql_ShouldReturnPostgreSql()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.PostgreSql, _databaseProviderAccessor.GetDatabaseProviderOrNull("POSTGRESQL"));
            Assert.Equal(DatabaseProvider.PostgreSql, _databaseProviderAccessor.GetDatabaseProviderOrNull("postgresql"));
            Assert.Equal(DatabaseProvider.PostgreSql, _databaseProviderAccessor.GetDatabaseProviderOrNull("PostgreSql"));
            Assert.Equal(DatabaseProvider.PostgreSql, _databaseProviderAccessor.GetDatabaseProviderOrNull("PostgreSQL"));
            // Additional case variations
            Assert.Equal(DatabaseProvider.PostgreSql, _databaseProviderAccessor.GetDatabaseProviderOrNull("PostgreSQl"));
            Assert.Equal(DatabaseProvider.PostgreSql, _databaseProviderAccessor.GetDatabaseProviderOrNull("pOstGrESqL"));
        }

        /// <summary>
        /// Tests that MySQL custom provider names are correctly mapped to DatabaseProvider.MySql.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByCustomName_MySql_ShouldReturnMySql()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.MySql, _databaseProviderAccessor.GetDatabaseProviderOrNull("MYSQL"));
            Assert.Equal(DatabaseProvider.MySql, _databaseProviderAccessor.GetDatabaseProviderOrNull("mysql"));
            Assert.Equal(DatabaseProvider.MySql, _databaseProviderAccessor.GetDatabaseProviderOrNull("MySql"));
            // Additional case variations
            Assert.Equal(DatabaseProvider.MySql, _databaseProviderAccessor.GetDatabaseProviderOrNull("MySQl"));
            Assert.Equal(DatabaseProvider.MySql, _databaseProviderAccessor.GetDatabaseProviderOrNull("mYsQL"));
        }

        /// <summary>
        /// Tests that Oracle custom provider names are correctly mapped to DatabaseProvider.Oracle.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByCustomName_Oracle_ShouldReturnOracle()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNull("ORACLE"));
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNull("oracle"));
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNull("Oracle"));
            // Additional case variations
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNull("oRACLe"));
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNull("OracLE"));
        }

        /// <summary>
        /// Tests that Devart Oracle custom provider names are correctly mapped to DatabaseProvider.Oracle.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByCustomName_DevartOracle_ShouldReturnOracle()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNull("DEVART.ORACLE"));
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNull("devart.oracle"));
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNull("Devart.Oracle"));
            // Additional case variations
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNull("Devart.ORACLE"));
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNull("devart.Oracle"));
        }

        /// <summary>
        /// Tests that SQLite custom provider names are correctly mapped to DatabaseProvider.Sqlite.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByCustomName_Sqlite_ShouldReturnSqlite()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Sqlite, _databaseProviderAccessor.GetDatabaseProviderOrNull("SQLITE"));
            Assert.Equal(DatabaseProvider.Sqlite, _databaseProviderAccessor.GetDatabaseProviderOrNull("sqlite"));
            Assert.Equal(DatabaseProvider.Sqlite, _databaseProviderAccessor.GetDatabaseProviderOrNull("Sqlite"));
            // Additional case variations
            Assert.Equal(DatabaseProvider.Sqlite, _databaseProviderAccessor.GetDatabaseProviderOrNull("SQLite"));
            Assert.Equal(DatabaseProvider.Sqlite, _databaseProviderAccessor.GetDatabaseProviderOrNull("sQLiTe"));
        }

        /// <summary>
        /// Tests that InMemory custom provider names are correctly mapped to DatabaseProvider.InMemory.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByCustomName_InMemory_ShouldReturnInMemory()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.InMemory, _databaseProviderAccessor.GetDatabaseProviderOrNull("INMEMORY"));
            Assert.Equal(DatabaseProvider.InMemory, _databaseProviderAccessor.GetDatabaseProviderOrNull("inmemory"));
            Assert.Equal(DatabaseProvider.InMemory, _databaseProviderAccessor.GetDatabaseProviderOrNull("InMemory"));
            // Additional case variations
            Assert.Equal(DatabaseProvider.InMemory, _databaseProviderAccessor.GetDatabaseProviderOrNull("InMEMORY"));
            Assert.Equal(DatabaseProvider.InMemory, _databaseProviderAccessor.GetDatabaseProviderOrNull("iNmEmOrY"));
        }

        /// <summary>
        /// Tests that Firebird custom provider names are correctly mapped to DatabaseProvider.Firebird.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByCustomName_Firebird_ShouldReturnFirebird()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Firebird, _databaseProviderAccessor.GetDatabaseProviderOrNull("FIREBIRD"));
            Assert.Equal(DatabaseProvider.Firebird, _databaseProviderAccessor.GetDatabaseProviderOrNull("firebird"));
            Assert.Equal(DatabaseProvider.Firebird, _databaseProviderAccessor.GetDatabaseProviderOrNull("Firebird"));
            // Additional case variations
            Assert.Equal(DatabaseProvider.Firebird, _databaseProviderAccessor.GetDatabaseProviderOrNull("FireBIRD"));
            Assert.Equal(DatabaseProvider.Firebird, _databaseProviderAccessor.GetDatabaseProviderOrNull("fIrEbIrD"));
        }

        /// <summary>
        /// Tests that Cosmos custom provider names are correctly mapped to DatabaseProvider.Cosmos.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByCustomName_Cosmos_ShouldReturnCosmos()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Cosmos, _databaseProviderAccessor.GetDatabaseProviderOrNull("COSMOS"));
            Assert.Equal(DatabaseProvider.Cosmos, _databaseProviderAccessor.GetDatabaseProviderOrNull("cosmos"));
            Assert.Equal(DatabaseProvider.Cosmos, _databaseProviderAccessor.GetDatabaseProviderOrNull("Cosmos"));
            // Additional case variations
            Assert.Equal(DatabaseProvider.Cosmos, _databaseProviderAccessor.GetDatabaseProviderOrNull("CosMOS"));
            Assert.Equal(DatabaseProvider.Cosmos, _databaseProviderAccessor.GetDatabaseProviderOrNull("cOsMoS"));
        }

        /// <summary>
        /// Tests that DM custom provider names are correctly mapped to DatabaseProvider.Dm.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByCustomName_Dm_ShouldReturnDm()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Dm, _databaseProviderAccessor.GetDatabaseProviderOrNull("DM"));
            Assert.Equal(DatabaseProvider.Dm, _databaseProviderAccessor.GetDatabaseProviderOrNull("dm"));
            Assert.Equal(DatabaseProvider.Dm, _databaseProviderAccessor.GetDatabaseProviderOrNull("Dm"));
            // Additional case variations
            Assert.Equal(DatabaseProvider.Dm, _databaseProviderAccessor.GetDatabaseProviderOrNull("dM"));
        }

        /// <summary>
        /// Tests that OpenGauss custom provider names are correctly mapped to DatabaseProvider.OpenGauss.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByCustomName_OpenGauss_ShouldReturnOpenGauss()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.OpenGauss, _databaseProviderAccessor.GetDatabaseProviderOrNull("OPENGAUSS"));
            Assert.Equal(DatabaseProvider.OpenGauss, _databaseProviderAccessor.GetDatabaseProviderOrNull("opengauss"));
            Assert.Equal(DatabaseProvider.OpenGauss, _databaseProviderAccessor.GetDatabaseProviderOrNull("OpenGauss"));
            // Additional case variations
            Assert.Equal(DatabaseProvider.OpenGauss, _databaseProviderAccessor.GetDatabaseProviderOrNull("OpenGAUSS"));
            Assert.Equal(DatabaseProvider.OpenGauss, _databaseProviderAccessor.GetDatabaseProviderOrNull("oPeNgAuSs"));
        }

        /// <summary>
        /// Tests that GaussDB custom provider names are correctly mapped to DatabaseProvider.GaussDB.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByCustomName_GaussDB_ShouldReturnGaussDB()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.GaussDB, _databaseProviderAccessor.GetDatabaseProviderOrNull("GAUSSDB"));
            Assert.Equal(DatabaseProvider.GaussDB, _databaseProviderAccessor.GetDatabaseProviderOrNull("gaussdb"));
            Assert.Equal(DatabaseProvider.GaussDB, _databaseProviderAccessor.GetDatabaseProviderOrNull("GaussDB"));
            // Additional case variations
            Assert.Equal(DatabaseProvider.GaussDB, _databaseProviderAccessor.GetDatabaseProviderOrNull("GaussDb"));
            Assert.Equal(DatabaseProvider.GaussDB, _databaseProviderAccessor.GetDatabaseProviderOrNull("gAuSsDb"));
        }

        /// <summary>
        /// Tests that null or empty custom provider names return null.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public void GetDatabaseProviderOrNullByCustomName_NullOrEmptyProviderName_ShouldReturnNull(string providerName)
        {
            // Act & Assert
            Assert.Null(_databaseProviderAccessor.GetDatabaseProviderOrNull(providerName));
        }

        /// <summary>
        /// Tests that unknown custom provider names return null.
        /// </summary>
        [Theory]
        [InlineData("UNKNOWN")]
        [InlineData("INVALID")]
        [InlineData("NOTEXIST")]
        [InlineData("RANDOM")]
        [InlineData("SQL-SERVER")] // Special characters
        [InlineData("Postgre_SQL")] // Special characters
        [InlineData("My SQL")] // Space in name
        [InlineData("Oracle123")] // Numbers
        [InlineData("Firebird!")] // Special characters
        public void GetDatabaseProviderOrNullByCustomName_UnknownProviderName_ShouldReturnNull(string providerName)
        {
            // Act & Assert
            Assert.Null(_databaseProviderAccessor.GetDatabaseProviderOrNull(providerName));
        }

        #endregion

        #region GetDatabaseProviderOrNullByEfCoreName Tests

        /// <summary>
        /// Tests that SQL Server EF Core provider names are correctly mapped to DatabaseProvider.SqlServer.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByEfCoreName_SqlServer_ShouldReturnSqlServer()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.SqlServer, _databaseProviderAccessor.GetDatabaseProviderOrNullByEfCoreName("Microsoft.EntityFrameworkCore.SqlServer"));
        }

        /// <summary>
        /// Tests that PostgreSQL EF Core provider names are correctly mapped to DatabaseProvider.PostgreSql.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByEfCoreName_PostgreSql_ShouldReturnPostgreSql()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.PostgreSql, _databaseProviderAccessor.GetDatabaseProviderOrNullByEfCoreName("Npgsql.EntityFrameworkCore.PostgreSQL"));
        }

        /// <summary>
        /// Tests that MySQL EF Core provider names are correctly mapped to DatabaseProvider.MySql.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByEfCoreName_MySql_ShouldReturnMySql()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.MySql, _databaseProviderAccessor.GetDatabaseProviderOrNullByEfCoreName("Pomelo.EntityFrameworkCore.MySql"));
            Assert.Equal(DatabaseProvider.MySql, _databaseProviderAccessor.GetDatabaseProviderOrNullByEfCoreName("MySql.Data.EntityFrameworkCore"));
        }

        /// <summary>
        /// Tests that Oracle EF Core provider names are correctly mapped to DatabaseProvider.Oracle.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByEfCoreName_Oracle_ShouldReturnOracle()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNullByEfCoreName("Oracle.EntityFrameworkCore"));
            Assert.Equal(DatabaseProvider.Oracle, _databaseProviderAccessor.GetDatabaseProviderOrNullByEfCoreName("Devart.Data.Oracle.Entity.EFCore"));
        }

        /// <summary>
        /// Tests that SQLite EF Core provider names are correctly mapped to DatabaseProvider.Sqlite.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByEfCoreName_Sqlite_ShouldReturnSqlite()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Sqlite, _databaseProviderAccessor.GetDatabaseProviderOrNullByEfCoreName("Microsoft.EntityFrameworkCore.Sqlite"));
        }

        /// <summary>
        /// Tests that InMemory EF Core provider names are correctly mapped to DatabaseProvider.InMemory.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByEfCoreName_InMemory_ShouldReturnInMemory()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.InMemory, _databaseProviderAccessor.GetDatabaseProviderOrNullByEfCoreName("Microsoft.EntityFrameworkCore.InMemory"));
        }

        /// <summary>
        /// Tests that Firebird EF Core provider names are correctly mapped to DatabaseProvider.Firebird.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByEfCoreName_Firebird_ShouldReturnFirebird()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Firebird, _databaseProviderAccessor.GetDatabaseProviderOrNullByEfCoreName("FirebirdSql.EntityFrameworkCore.Firebird"));
        }

        /// <summary>
        /// Tests that Cosmos EF Core provider names are correctly mapped to DatabaseProvider.Cosmos.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByEfCoreName_Cosmos_ShouldReturnCosmos()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Cosmos, _databaseProviderAccessor.GetDatabaseProviderOrNullByEfCoreName("Microsoft.EntityFrameworkCore.Cosmos"));
        }

        /// <summary>
        /// Tests that DM EF Core provider names are correctly mapped to DatabaseProvider.Dm.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByEfCoreName_Dm_ShouldReturnDm()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.Dm, _databaseProviderAccessor.GetDatabaseProviderOrNullByEfCoreName("Dm.EntityFrameworkCore"));
        }

        /// <summary>
        /// Tests that OpenGauss EF Core provider names are correctly mapped to DatabaseProvider.OpenGauss.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByEfCoreName_OpenGauss_ShouldReturnOpenGauss()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.OpenGauss, _databaseProviderAccessor.GetDatabaseProviderOrNullByEfCoreName("OpenG.EntityFrameworkCore.OpenGauss"));
        }

        /// <summary>
        /// Tests that GaussDB EF Core provider names are correctly mapped to DatabaseProvider.GaussDB.
        /// </summary>
        [Fact]
        public void GetDatabaseProviderOrNullByEfCoreName_GaussDB_ShouldReturnGaussDB()
        {
            // Act & Assert
            Assert.Equal(DatabaseProvider.GaussDB, _databaseProviderAccessor.GetDatabaseProviderOrNullByEfCoreName("EntityFrameworkCore.GaussDB"));
        }

        /// <summary>
        /// Tests that null or empty EF Core provider names return null.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public void GetDatabaseProviderOrNullByEfCoreName_NullOrEmptyProviderName_ShouldReturnNull(string providerName)
        {
            // Act & Assert
            Assert.Null(_databaseProviderAccessor.GetDatabaseProviderOrNullByEfCoreName(providerName));
        }

        /// <summary>
        /// Tests that unknown EF Core provider names return null.
        /// </summary>
        [Theory]
        [InlineData("Unknown.EntityFrameworkCore.Provider")]
        [InlineData("Invalid.EF.Core")]
        [InlineData("NotExist.EntityFrameworkCore")]
        [InlineData("Random.Provider")]
        [InlineData("Microsoft.EntityFrameworkCore.Unknown")]
        [InlineData("Fake.EntityFrameworkCore.Database")]
        public void GetDatabaseProviderOrNullByEfCoreName_UnknownProviderName_ShouldReturnNull(string providerName)
        {
            // Act & Assert
            Assert.Null(_databaseProviderAccessor.GetDatabaseProviderOrNullByEfCoreName(providerName));
        }

        #endregion

        #region Service Registration Tests

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

        #endregion
    }
}
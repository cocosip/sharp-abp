# Database Connections

Multi-database connection management module supporting dynamic database connections for multi-tenant and distributed scenarios.

## DbConnections

Core module for managing multiple database connections with support for MySQL, PostgreSQL, SQL Server, Oracle, SQLite, DM (DaMeng), and GaussDB.

### Installation

```bash
# Core package
dotnet add package SharpAbp.Abp.DbConnections

# Database-specific packages
dotnet add package SharpAbp.Abp.DbConnections.MySQL
dotnet add package SharpAbp.Abp.DbConnections.PostgreSql
dotnet add package SharpAbp.Abp.DbConnections.SqlServer
dotnet add package SharpAbp.Abp.DbConnections.Oracle
dotnet add package SharpAbp.Abp.DbConnections.Oracle.Devart  # Devart Oracle driver
dotnet add package SharpAbp.Abp.DbConnections.Sqlite
dotnet add package SharpAbp.Abp.DbConnections.DM             # DaMeng
dotnet add package SharpAbp.Abp.DbConnections.GaussDB        # GaussDB
```

### Configuration

Configure in `appsettings.json`:

```json
{
  "DbConnections": {
    "Databases": [
      {
        "Name": "Default",
        "DatabaseType": "SqlServer",
        "ConnectionString": "Server=localhost;Database=MyDb;User Id=sa;Password=****;"
      },
      {
        "Name": "ReportingDb",
        "DatabaseType": "MySQL",
        "ConnectionString": "Server=localhost;Database=ReportDb;Uid=root;Pwd=****;"
      },
      {
        "Name": "AnalyticsDb",
        "DatabaseType": "PostgreSql",
        "ConnectionString": "Host=localhost;Database=AnalyticsDb;Username=postgres;Password=****;"
      },
      {
        "Name": "LegacyDb",
        "DatabaseType": "DM",
        "ConnectionString": "Server=localhost;Port=5236;User Id=SYSDBA;PWD=SYSDBA;Database=LegacyDb;"
      }
    ]
  }
}
```

Add the module dependency:

```csharp
[DependsOn(
    typeof(AbpDbConnectionsModule),
    typeof(AbpDbConnectionsSqlServerModule),
    typeof(AbpDbConnectionsMySQLModule),
    typeof(AbpDbConnectionsPostgreSqlModule),
    typeof(AbpDbConnectionsDMModule)
)]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpDbConnectionsOptions>(options =>
        {
            options.Databases.AddIfNotContains(
                new DatabaseConfiguration
                {
                    DatabaseName = "Default",
                    DatabaseType = DatabaseType.SqlServer,
                    ConnectionString = configuration.GetConnectionString("Default")
                }
            );

            options.Databases.AddIfNotContains(
                new DatabaseConfiguration
                {
                    DatabaseName = "ReportingDb",
                    DatabaseType = DatabaseType.MySQL,
                    ConnectionString = configuration.GetConnectionString("ReportingDb")
                }
            );
        });
    }
}
```

### Usage Example

#### Basic Connection Management

```csharp
public class DatabaseService : ITransientDependency
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DatabaseService(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    // Get connection to default database
    public async Task<DbConnection> GetDefaultConnectionAsync()
    {
        return await _dbConnectionFactory.CreateAsync("Default");
    }

    // Get connection to specific database
    public async Task<DbConnection> GetReportingConnectionAsync()
    {
        return await _dbConnectionFactory.CreateAsync("ReportingDb");
    }

    // Execute query on specific database
    public async Task<List<T>> QueryAsync<T>(string databaseName, string sql, object parameters = null)
    {
        using (var connection = await _dbConnectionFactory.CreateAsync(databaseName))
        {
            await connection.OpenAsync();
            return (await connection.QueryAsync<T>(sql, parameters)).ToList();
        }
    }

    // Execute command on specific database
    public async Task<int> ExecuteAsync(string databaseName, string sql, object parameters = null)
    {
        using (var connection = await _dbConnectionFactory.CreateAsync(databaseName))
        {
            await connection.OpenAsync();
            return await connection.ExecuteAsync(sql, parameters);
        }
    }
}
```

#### Multi-Database Query Example

```csharp
public class CrossDatabaseReportService : ApplicationService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CrossDatabaseReportService(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ReportData> GenerateCrossDatabaseReportAsync()
    {
        var report = new ReportData();

        // Query from main database
        using (var mainConn = await _connectionFactory.CreateAsync("Default"))
        {
            await mainConn.OpenAsync();
            var orders = await mainConn.QueryAsync<Order>(
                "SELECT * FROM Orders WHERE CreatedDate >= @Date",
                new { Date = DateTime.Now.AddDays(-30) }
            );
            report.Orders = orders.ToList();
        }

        // Query from reporting database
        using (var reportConn = await _connectionFactory.CreateAsync("ReportingDb"))
        {
            await reportConn.OpenAsync();
            var statistics = await reportConn.QueryFirstOrDefaultAsync<Statistics>(
                "SELECT * FROM MonthlyStatistics WHERE Month = @Month",
                new { Month = DateTime.Now.Month }
            );
            report.Statistics = statistics;
        }

        // Query from analytics database
        using (var analyticsConn = await _connectionFactory.CreateAsync("AnalyticsDb"))
        {
            await analyticsConn.OpenAsync();
            var metrics = await analyticsConn.QueryAsync<Metric>(
                "SELECT * FROM UserMetrics WHERE Date >= @Date",
                new { Date = DateTime.Now.AddDays(-7) }
            );
            report.Metrics = metrics.ToList();
        }

        return report;
    }
}
```

#### Dynamic Database Connection

```csharp
public class TenantDatabaseService : ApplicationService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IDbConnectionManager _connectionManager;
    private readonly ICurrentTenant _currentTenant;

    public TenantDatabaseService(
        IDbConnectionFactory connectionFactory,
        IDbConnectionManager connectionManager,
        ICurrentTenant currentTenant)
    {
        _connectionFactory = connectionFactory;
        _connectionManager = connectionManager;
        _currentTenant = currentTenant;
    }

    // Register tenant database dynamically
    public async Task RegisterTenantDatabaseAsync(Guid tenantId, string connectionString)
    {
        var databaseName = $"Tenant_{tenantId}";

        await _connectionManager.AddOrUpdateAsync(new DatabaseConfiguration
        {
            DatabaseName = databaseName,
            DatabaseType = DatabaseType.SqlServer,
            ConnectionString = connectionString
        });
    }

    // Get current tenant's database connection
    public async Task<DbConnection> GetCurrentTenantConnectionAsync()
    {
        if (_currentTenant.Id == null)
        {
            return await _connectionFactory.CreateAsync("Default");
        }

        var databaseName = $"Tenant_{_currentTenant.Id}";
        return await _connectionFactory.CreateAsync(databaseName);
    }

    // Query from current tenant's database
    public async Task<List<Product>> GetTenantProductsAsync()
    {
        using (var connection = await GetCurrentTenantConnectionAsync())
        {
            await connection.OpenAsync();
            return (await connection.QueryAsync<Product>("SELECT * FROM Products"))
                .ToList();
        }
    }
}
```

#### Database Health Check

```csharp
public class DatabaseHealthCheckService : ITransientDependency
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IDbConnectionManager _connectionManager;

    public DatabaseHealthCheckService(
        IDbConnectionFactory connectionFactory,
        IDbConnectionManager connectionManager)
    {
        _connectionFactory = connectionFactory;
        _connectionManager = connectionManager;
    }

    public async Task<Dictionary<string, bool>> CheckAllDatabasesAsync()
    {
        var results = new Dictionary<string, bool>();
        var databases = await _connectionManager.GetAllAsync();

        foreach (var db in databases)
        {
            try
            {
                using (var connection = await _connectionFactory.CreateAsync(db.DatabaseName))
                {
                    await connection.OpenAsync();
                    results[db.DatabaseName] = connection.State == ConnectionState.Open;
                }
            }
            catch
            {
                results[db.DatabaseName] = false;
            }
        }

        return results;
    }

    public async Task<bool> TestConnectionAsync(string databaseName)
    {
        try
        {
            using (var connection = await _connectionFactory.CreateAsync(databaseName))
            {
                await connection.OpenAsync();
                return connection.State == ConnectionState.Open;
            }
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> TestConnectionStringAsync(
        DatabaseType databaseType,
        string connectionString)
    {
        try
        {
            var connection = _connectionFactory.Create(databaseType, connectionString);
            await connection.OpenAsync();
            var isOpen = connection.State == ConnectionState.Open;
            await connection.CloseAsync();
            return isOpen;
        }
        catch
        {
            return false;
        }
    }
}
```

---

## Database Type Support

### Supported Database Types

| Database | DatabaseType Enum | Package |
|----------|------------------|---------|
| SQL Server | `DatabaseType.SqlServer` | `SharpAbp.Abp.DbConnections.SqlServer` |
| MySQL | `DatabaseType.MySQL` | `SharpAbp.Abp.DbConnections.MySQL` |
| PostgreSQL | `DatabaseType.PostgreSql` | `SharpAbp.Abp.DbConnections.PostgreSql` |
| Oracle | `DatabaseType.Oracle` | `SharpAbp.Abp.DbConnections.Oracle` |
| Oracle (Devart) | `DatabaseType.Oracle` | `SharpAbp.Abp.DbConnections.Oracle.Devart` |
| SQLite | `DatabaseType.Sqlite` | `SharpAbp.Abp.DbConnections.Sqlite` |
| DM (DaMeng) | `DatabaseType.DM` | `SharpAbp.Abp.DbConnections.DM` |
| GaussDB | `DatabaseType.GaussDB` | `SharpAbp.Abp.DbConnections.GaussDB` |

### Connection String Examples

#### SQL Server
```
Server=localhost;Database=MyDb;User Id=sa;Password=****;TrustServerCertificate=True;
```

#### MySQL
```
Server=localhost;Database=MyDb;Uid=root;Pwd=****;CharSet=utf8mb4;
```

#### PostgreSQL
```
Host=localhost;Port=5432;Database=MyDb;Username=postgres;Password=****;
```

#### Oracle
```
Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)));User Id=system;Password=****;
```

#### SQLite
```
Data Source=mydb.db;
```

#### DM (DaMeng)
```
Server=localhost;Port=5236;User Id=SYSDBA;PWD=SYSDBA;Database=MyDb;
```

#### GaussDB
```
Host=localhost;Port=5432;Database=MyDb;Username=gaussdb;Password=****;
```

---

## Best Practices

### 1. Connection Management

Always use `using` statements to ensure connections are properly disposed:

```csharp
using (var connection = await _connectionFactory.CreateAsync("Default"))
{
    await connection.OpenAsync();
    // Use connection
}
// Connection is automatically closed and disposed
```

### 2. Transaction Management

For multi-statement operations, use transactions:

```csharp
public async Task TransferDataAsync(string sourceDatabaseName, string targetDatabaseName)
{
    using (var sourceConn = await _connectionFactory.CreateAsync(sourceDatabaseName))
    using (var targetConn = await _connectionFactory.CreateAsync(targetDatabaseName))
    {
        await sourceConn.OpenAsync();
        await targetConn.OpenAsync();

        using (var transaction = await targetConn.BeginTransactionAsync())
        {
            try
            {
                // Read from source
                var data = await sourceConn.QueryAsync<DataRow>(
                    "SELECT * FROM SourceTable"
                );

                // Write to target
                foreach (var row in data)
                {
                    await targetConn.ExecuteAsync(
                        "INSERT INTO TargetTable (...) VALUES (...)",
                        row,
                        transaction
                    );
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
```

### 3. Connection Pooling

Connection pooling is handled automatically by ADO.NET, but you can configure it in the connection string:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=MyDb;User Id=sa;Password=****;Min Pool Size=5;Max Pool Size=100;"
  }
}
```

### 4. Error Handling

Always handle database exceptions appropriately:

```csharp
public async Task<List<T>> SafeQueryAsync<T>(string databaseName, string sql, object parameters = null)
{
    try
    {
        using (var connection = await _connectionFactory.CreateAsync(databaseName))
        {
            await connection.OpenAsync();
            return (await connection.QueryAsync<T>(sql, parameters)).ToList();
        }
    }
    catch (SqlException ex)
    {
        _logger.LogError(ex, "Database error occurred");
        throw new UserFriendlyException("An error occurred while querying the database");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error occurred");
        throw;
    }
}
```

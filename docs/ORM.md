# ORM Frameworks

Sharp-ABP provides integration with multiple ORM frameworks, giving you flexibility in data access strategies.

## FreeSql

FreeSql ORM framework integration with support for multiple database providers.

### Installation

Install the core package and your database provider:

```bash
# Core FreeSql package
dotnet add package SharpAbp.Abp.FreeSql

# Database-specific packages
dotnet add package SharpAbp.Abp.FreeSql.MySQL
dotnet add package SharpAbp.Abp.FreeSql.PostgreSql
dotnet add package SharpAbp.Abp.FreeSql.SqlServer
dotnet add package SharpAbp.Abp.FreeSql.Oracle
dotnet add package SharpAbp.Abp.FreeSql.Sqlite
dotnet add package SharpAbp.Abp.FreeSql.DM        # DaMeng Database
```

### Configuration

Configure in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=MyDb;User Id=sa;Password=****;"
  },
  "FreeSql": {
    "DataType": "SqlServer",
    "ConnectionString": "Server=localhost;Database=MyDb;User Id=sa;Password=****;",
    "AutoSyncStructure": false,
    "LazyLoading": false
  }
}
```

Add the module dependency:

```csharp
[DependsOn(
    typeof(AbpFreeSqlModule),
    typeof(AbpFreeSqlSqlServerModule)  // Or your database provider
)]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpFreeSqlOptions>(options =>
        {
            options.PreConfigure = freeSql =>
            {
                freeSql.GlobalFilter
                    .Apply<ISoftDelete>("SoftDelete", a => a.IsDeleted == false);
            };

            options.ConfigureFreeSql = (sp, freeSql) =>
            {
                // Additional configuration
                freeSql.Aop.CurdBefore += (s, e) =>
                {
                    // Log SQL
                    Console.WriteLine($"SQL: {e.Sql}");
                };
            };
        });
    }
}
```

### Usage Example

#### Basic CRUD Operations

```csharp
public class ProductRepository : ITransientDependency
{
    private readonly IFreeSql _freeSql;

    public ProductRepository(IFreeSql freeSql)
    {
        _freeSql = freeSql;
    }

    // Query
    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _freeSql.Select<Product>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    // Query with pagination
    public async Task<PagedResult<Product>> GetPagedAsync(int page, int pageSize)
    {
        var query = _freeSql.Select<Product>()
            .Where(x => x.IsActive);

        var total = await query.CountAsync();
        var items = await query
            .Page(page, pageSize)
            .ToListAsync();

        return new PagedResult<Product>
        {
            Items = items,
            Total = total
        };
    }

    // Insert
    public async Task<Product> CreateAsync(Product product)
    {
        await _freeSql.Insert(product).ExecuteAffrowsAsync();
        return product;
    }

    // Update
    public async Task<Product> UpdateAsync(Product product)
    {
        await _freeSql.Update<Product>()
            .SetSource(product)
            .ExecuteAffrowsAsync();
        return product;
    }

    // Delete
    public async Task DeleteAsync(Guid id)
    {
        await _freeSql.Delete<Product>()
            .Where(x => x.Id == id)
            .ExecuteAffrowsAsync();
    }
}
```

#### Advanced Queries

```csharp
public class AdvancedQueryExample
{
    private readonly IFreeSql _freeSql;

    public AdvancedQueryExample(IFreeSql freeSql)
    {
        _freeSql = freeSql;
    }

    // Join query
    public async Task<List<OrderWithCustomer>> GetOrdersWithCustomersAsync()
    {
        return await _freeSql.Select<Order, Customer>()
            .LeftJoin((o, c) => o.CustomerId == c.Id)
            .ToListAsync((o, c) => new OrderWithCustomer
            {
                OrderId = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = c.Name
            });
    }

    // Aggregate query
    public async Task<decimal> GetTotalSalesAsync()
    {
        return await _freeSql.Select<Order>()
            .Where(x => x.Status == OrderStatus.Completed)
            .SumAsync(x => x.TotalAmount);
    }

    // Group by
    public async Task<List<CategorySales>> GetSalesByCategoryAsync()
    {
        return await _freeSql.Select<Product, OrderItem>()
            .InnerJoin((p, oi) => p.Id == oi.ProductId)
            .GroupBy((p, oi) => p.CategoryId)
            .ToListAsync(g => new CategorySales
            {
                CategoryId = g.Key,
                TotalQuantity = g.Sum((p, oi) => oi.Quantity),
                TotalAmount = g.Sum((p, oi) => oi.Amount)
            });
    }
}
```

---

## Dapper

Dapper micro-ORM framework integration for lightweight data access.

### Installation

```bash
dotnet add package SharpAbp.Abp.Dapper
```

### Configuration

Add the module dependency:

```csharp
[DependsOn(typeof(AbpDapperModule))]
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Dapper is ready to use with ABP's connection strings
    }
}
```

### Usage Example

#### Basic Queries

```csharp
public class ProductRepository : DapperRepository<YourDbContext>, ITransientDependency
{
    public ProductRepository(IDbContextProvider<YourDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<Product>> GetActiveProductsAsync()
    {
        var dbConnection = await GetDbConnectionAsync();
        var sql = "SELECT * FROM Products WHERE IsActive = @IsActive";

        return (await dbConnection.QueryAsync<Product>(sql, new { IsActive = true }))
            .ToList();
    }

    public async Task<Product> GetByIdAsync(Guid id)
    {
        var dbConnection = await GetDbConnectionAsync();
        var sql = "SELECT * FROM Products WHERE Id = @Id";

        return await dbConnection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
    }

    public async Task<int> CreateAsync(Product product)
    {
        var dbConnection = await GetDbConnectionAsync();
        var sql = @"INSERT INTO Products (Id, Name, Price, IsActive, CreationTime)
                    VALUES (@Id, @Name, @Price, @IsActive, @CreationTime)";

        return await dbConnection.ExecuteAsync(sql, product);
    }

    public async Task<int> UpdateAsync(Product product)
    {
        var dbConnection = await GetDbConnectionAsync();
        var sql = @"UPDATE Products
                    SET Name = @Name, Price = @Price, IsActive = @IsActive
                    WHERE Id = @Id";

        return await dbConnection.ExecuteAsync(sql, product);
    }
}
```

#### Advanced Queries with Multiple Result Sets

```csharp
public class OrderRepository : DapperRepository<YourDbContext>
{
    public OrderRepository(IDbContextProvider<YourDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<OrderWithDetails> GetOrderWithDetailsAsync(Guid orderId)
    {
        var dbConnection = await GetDbConnectionAsync();
        var sql = @"
            SELECT * FROM Orders WHERE Id = @OrderId;
            SELECT * FROM OrderItems WHERE OrderId = @OrderId;
            SELECT * FROM Customers c
            INNER JOIN Orders o ON c.Id = o.CustomerId
            WHERE o.Id = @OrderId";

        using (var multi = await dbConnection.QueryMultipleAsync(sql, new { OrderId = orderId }))
        {
            var order = await multi.ReadFirstOrDefaultAsync<Order>();
            var orderItems = (await multi.ReadAsync<OrderItem>()).ToList();
            var customer = await multi.ReadFirstOrDefaultAsync<Customer>();

            return new OrderWithDetails
            {
                Order = order,
                OrderItems = orderItems,
                Customer = customer
            };
        }
    }

    public async Task<List<T>> QueryAsync<T>(string sql, object parameters = null)
    {
        var dbConnection = await GetDbConnectionAsync();
        return (await dbConnection.QueryAsync<T>(sql, parameters)).ToList();
    }
}
```

---

## EntityFrameworkCore

Entity Framework Core integration with support for DM (DaMeng) and GaussDB.

### Installation

```bash
# Core package
dotnet add package SharpAbp.Abp.EntityFrameworkCore

# Database-specific packages
dotnet add package SharpAbp.Abp.EntityFrameworkCore.DM       # DaMeng
dotnet add package SharpAbp.Abp.EntityFrameworkCore.GaussDB  # GaussDB
```

### Configuration

#### For DaMeng Database

```csharp
[DependsOn(
    typeof(AbpEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreDmModule)
)]
public class YourEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<YourDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseDm();
        });
    }
}
```

Configure connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Port=5236;User Id=SYSDBA;PWD=SYSDBA;Database=MyDb;"
  }
}
```

#### For GaussDB

```csharp
[DependsOn(
    typeof(AbpEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreGaussDBModule)
)]
public class YourEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<YourDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseGaussDB();
        });
    }
}
```

Configure connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=MyDb;Username=user;Password=****;"
  }
}
```

### Large Text Columns

`SharpAbp.Abp.EntityFrameworkCore` registers a model finalizing convention that maps string and binary columns
to the large object type of the database. A column is converted when its configured length is above what the
provider stores inline, when the potential row size of the table is above the provider limit, or when the
property is marked with `[LargeTextColumn]`. It runs for every `AbpDbContext` model, so modules never need
provider specific code, and it is enabled by default.

| Provider | Handled by | Result |
| --- | --- | --- |
| Oracle | driver | strings above 2000 characters become `NCLOB`, binary above 2000 bytes `BLOB` |
| SQL Server | driver | strings above 4000 characters become `nvarchar(max)`/`varchar(max)` |
| PostgreSQL | driver | `character varying` up to 10485760 characters, longer strings become `text` |
| SQLite | driver | `TEXT`, the configured length is ignored |
| MySQL | convention | strings above 16383 characters or a table row above 65535 bytes become `longtext` |
| DM (DaMeng) | convention | strings above 2000 characters become `CLOB`, binary above 2000 bytes `BLOB` |
| OceanBase | convention | Oracle mode strings above 32767 characters become `CLOB` |

The convention also maps the column type of a marked property for every provider: `NCLOB`/`CLOB` and `BLOB` on
Oracle, `nvarchar(max)`/`varchar(max)` and `varbinary(max)` on SQL Server, `text`/`bytea` on PostgreSQL,
GaussDB and OpenGauss, `TEXT`/`BLOB` on SQLite, and the provider column type of the table above for MySQL, DM
and OceanBase.

Columns are skipped when they are part of a key, an index or a unique index, are concurrency tokens, are fixed
length, have a value converter, or when the column type was set explicitly with `HasColumnType`. A skipped long
column is logged as a warning, configure its type manually when it cannot stay inline.

Configuration, all values are optional and the built in defaults are shown:

```json
{
  "EfCoreOptions": {
    "LargeTextColumns": {
      "Enabled": true,
      "IncludeBinary": true,
      "Providers": {
        "MySql": {
          "StringMaxLength": 16383,
          "StringColumnType": "longtext",
          "BinaryMaxLength": 65535,
          "BinaryColumnType": "longblob",
          "MaxRowSizeBytes": 65535,
          "RowSizeConversionMinLength": 2000
        },
        "Dm": {
          "StringMaxLength": 2000,
          "StringColumnType": "CLOB",
          "BinaryMaxLength": 2000,
          "BinaryColumnType": "BLOB"
        },
        "OceanBase": {
          "StringMaxLength": 32767,
          "StringColumnType": "CLOB",
          "BinaryMaxLength": 2000,
          "BinaryColumnType": "BLOB"
        },
        "Oracle": {
          "StringColumnType": "NCLOB",
          "NonUnicodeStringColumnType": "CLOB",
          "BinaryColumnType": "BLOB"
        },
        "SqlServer": {
          "StringColumnType": "nvarchar(max)",
          "NonUnicodeStringColumnType": "varchar(max)",
          "BinaryColumnType": "varbinary(max)"
        },
        "PostgreSql": {
          "StringColumnType": "text",
          "BinaryColumnType": "bytea"
        },
        "GaussDB": {
          "StringColumnType": "text",
          "BinaryColumnType": "bytea"
        },
        "OpenGauss": {
          "StringColumnType": "text",
          "BinaryColumnType": "bytea"
        },
        "Sqlite": {
          "StringColumnType": "TEXT",
          "BinaryColumnType": "BLOB"
        }
      }
    }
  }
}
```

The Oracle, SQL Server, PostgreSQL, GaussDB, OpenGauss and SQLite entries have no length limits, because those
drivers widen long columns by themselves; they only define the column type of a property marked with
`[LargeTextColumn]`.

An entry under `Providers` replaces the built in entry of that provider as a whole, so write all fields of
that provider when you override it. Providers that are missing from the dictionary are not converted at all.
The MySQL row size check assumes utf8mb4, lower `RowSizeConversionMinLength` when the server uses a single byte
character set. OceanBase currently has no EF Core 9/10 provider, its entry only applies when such a provider is
used.

`ConfigureDefaultConvention` is a single slot in ABP. When another module or the application sets it, call
`AddSharpAbpLargeTextColumnConvention` inside that action, which is also the way to skip one DbContext:

```csharp
Configure<AbpDbContextOptions>(options =>
{
    options.ConfigureDefaultConvention((dbContext, modelConfigurationBuilder) =>
    {
        if (dbContext is not ReportingDbContext)
        {
            modelConfigurationBuilder.AddSharpAbpLargeTextColumnConvention(dbContext);
        }

        // Other default conventions
    });
});
```

When a DbContext is created without ABP property injection, for example a hand written
`IDesignTimeDbContextFactory` that calls `new YourDbContext(options)`, ABP skips its `ConfigureConventions`
hooks and the default options are used. Add the convention to the DbContext explicitly in that case:

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    base.ConfigureConventions(configurationBuilder);
    configurationBuilder.AddSharpAbpLargeTextColumnConvention(this);
}
```

Per field control:

- `[LargeTextColumn]`: mark a string or `byte[]` property when it should always be a large object column, for
  example a key material or a JSON payload that has no natural maximum length. The convention maps it to the
  large object type of the database, so the entity stays provider agnostic. The attribute is in
  `SharpAbp.Abp.Data`.

```csharp
public class RSACreds : Entity<Guid>
{
    [LargeTextColumn]
    public string PublicKey { get; set; }

    [LargeTextColumn]
    public string PrivateKey { get; set; }
}
```

- Long text field with a length: give the property a length above the inline limit, for example
  `HasMaxLength(4000)`. It is one change that is correct for every provider, because the driver or the
  convention picks the right type, and it keeps DTO validation and migrations consistent.
- Provider specific type of one field: set `HasColumnType` explicitly in the provider project, for example
  `builder.Entity<MyEntity>().Property(x => x.Payload).HasColumnType("NCLOB")` in the Oracle project, or guard it
  with `builder.GetSharpAbpDatabaseProvider() == DatabaseProvider.Oracle` in a shared DbContext. An explicit
  column type always wins, the convention never changes it.
- Oracle cannot alter an existing `VARCHAR2`/`NVARCHAR2` column to `CLOB`/`NCLOB` (`ORA-22858`). Migrate an
  existing database with a hand written migration: add the new LOB column, copy the data, drop the old column
  and rename the new one.
- A marked property that is part of a key or an index is skipped with a warning, because most databases cannot
  index a LOB column. Configure the column type of such a property manually when the database supports it.

### Usage Example

#### Define DbContext

```csharp
public class YourDbContext : AbpDbContext<YourDbContext>
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public YourDbContext(DbContextOptions<YourDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>(b =>
        {
            b.ToTable("Products");
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
            b.Property(x => x.Price).HasColumnType("decimal(18,2)");

            b.HasIndex(x => x.Name);
        });

        builder.Entity<Order>(b =>
        {
            b.ToTable("Orders");
            b.ConfigureByConvention();

            b.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId);

            b.HasMany(x => x.OrderItems)
                .WithOne()
                .HasForeignKey(x => x.OrderId);
        });
    }
}
```

#### Repository Usage

```csharp
public class ProductAppService : ApplicationService
{
    private readonly IRepository<Product, Guid> _productRepository;

    public ProductAppService(IRepository<Product, Guid> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductDto>> GetListAsync()
    {
        var products = await _productRepository.GetListAsync();
        return ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
    }

    public async Task<ProductDto> GetAsync(Guid id)
    {
        var product = await _productRepository.GetAsync(id);
        return ObjectMapper.Map<Product, ProductDto>(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        var product = new Product
        {
            Name = input.Name,
            Price = input.Price,
            IsActive = true
        };

        await _productRepository.InsertAsync(product);
        return ObjectMapper.Map<Product, ProductDto>(product);
    }

    public async Task UpdateAsync(Guid id, UpdateProductDto input)
    {
        var product = await _productRepository.GetAsync(id);

        product.Name = input.Name;
        product.Price = input.Price;

        await _productRepository.UpdateAsync(product);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _productRepository.DeleteAsync(id);
    }
}
```

#### Advanced EF Core Queries

```csharp
public class AdvancedOrderService : ApplicationService
{
    private readonly IRepository<Order, Guid> _orderRepository;

    public AdvancedOrderService(IRepository<Order, Guid> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<List<OrderDto>> GetOrdersWithCustomerAsync()
    {
        var query = await _orderRepository.WithDetailsAsync(x => x.Customer, x => x.OrderItems);

        var orders = await AsyncExecuter.ToListAsync(
            query.Where(x => x.Status == OrderStatus.Pending)
                .OrderByDescending(x => x.CreationTime)
        );

        return ObjectMapper.Map<List<Order>, List<OrderDto>>(orders);
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        var queryable = await _orderRepository.GetQueryableAsync();

        return await AsyncExecuter.SumAsync(
            queryable.Where(x => x.Status == OrderStatus.Completed),
            x => x.TotalAmount
        );
    }
}
```

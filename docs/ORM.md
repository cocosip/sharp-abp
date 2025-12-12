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

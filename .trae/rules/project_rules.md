# 项目：Sharp-ABP

本指南提供了在`sharp-abp`项目中开发和代码生成的上下文和规则。

## 项目概述

本项目是基于Abp vNext框架开发的扩展库，旨在提供模块化的方法来分离核心框架扩展和高级功能模块。

## 目录结构

- **`/framework`**：包含基础或核心扩展模块。这些模块通常不包含自己的数据库持久化逻辑，而是提供横切关注点和框架增强功能。主解决方案文件`framework/SharpAbp.sln`位于此处。
- **`/modules`**：包含高级的特定功能模块，通常包含自己的数据库持久化和业务逻辑。每个模块都有自己的解决方案文件（例如`modules/account/SharpAbp.Abp.Account.sln`）。
- **`/samples`**：包含演示如何使用各种模块和功能的示例项目。
- **`/docs`**：包含不同模块和功能的Markdown文档。
- **`/build`**：包含用于自动化构建、测试和打包过程的PowerShell脚本。

## 构建和测试

此过程有意排除了`/samples`目录。

1. **构建所有解决方案**：在主解决方案文件`framework/SharpAbp.sln`上执行`dotnet build`命令。然后，遍历`/modules`中的每个模块目录，并在其各自的解决方案文件上执行`dotnet build`。
2. **运行所有测试**：构建成功后，在`framework`解决方案和`/modules`目录中的每个模块解决方案文件上执行`dotnet test`以运行所有相关的单元和集成测试。

`/build`目录中的PowerShell脚本（`build-all-release.ps1`、`test-all.ps1`）配置为自动化此过程。

- 要以`Release`配置构建所有项目，请运行：`./build/build-all-release.ps1`
- 要运行所有测试，请运行：`./build/test-all.ps1`

---

## 代码生成指南

在生成代码时，请严格遵守以下规则：

### 1. 通用约定

- **遵循现有约定**：严格遵守项目中现有的代码风格、命名约定和架构模式。

### 2. 项目和文件结构

- **文件位置**：
    - 新的核心框架扩展应放置在`/framework/src`中的适当子目录下。
    - 新的功能模块（特别是具有数据库持久化的模块）应在`/modules`下创建新的子目录。
- **模块命名和分层**：在`/modules`目录中创建的模块必须遵循官方ABP框架的模块分层约定来进行项目和命名空间命名。例如，对于名为"Blogging"的新功能，项目应按以下模式命名：
    - `SharpAbp.Abp.Blogging.Domain`
    - `SharpAbp.Abp.Blogging.Domain.Shared`
    - `SharpAbp.Abp.Blogging.Application.Contracts`
    - `SharpAbp.Abp.Blogging.Application`
    - `SharpAbp.Abp.Blogging.EntityFrameworkCore`
    - `SharpAbp.Abp.Blogging.HttpApi`
    - `SharpAbp.Abp.Blogging.Web`

### 3. 依赖管理

- **中央包管理（CPM）**：本项目使用.NET的中央包管理（CPM）功能。
    - **所有** `PackageVersion`定义必须位于存储库根目录的`Directory.Packages.props`文件中。
    - 单个`.csproj`文件应使用`<PackageReference Include="PackageName" />`而不带`Version`属性。
    - 添加新包时，首先在`Directory.Packages.props`中添加`<PackageVersion Include="PackageName" Version="1.2.3" />`，然后在相关的`.csproj`文件中添加`<PackageReference>`。
- **包版本约束**：
    - `AWSSDK.S3`和`AWSSDK.SecurityToken`包的版本不得更新超过`3.7.300`。不要将它们更新到4或更高版本。
- **最小化依赖**：模块应遵循最小依赖原则，最小化对外部库的依赖。只有在功能非常复杂且必要时才引入新的第三方库。

### 4. 详细编码标准

#### 4.1. 命名约定
- **大小写**：
    - 类、接口、枚举、方法、属性和事件使用**PascalCase**。
    - 方法参数和局部变量使用**camelCase**。
- **接口**：接口名称必须以`I`为前缀，例如`IBlogRepository`。
- **私有字段**：私有字段应以`_`为前缀并使用camelCase，例如`_blogRepository`。
- **异步方法**：所有返回`Task`或`Task<T>`的异步方法必须以`Async`为后缀。

#### 4.2. 依赖注入
- **构造函数注入**：优先使用构造函数注入。这是最清晰且最符合依赖倒置原则的方法。
- **ABP服务生命周期**：通过实现ABP提供的接口来注册服务：
    - `ITransientDependency`（瞬态）：每次请求都会创建新实例。
    - `IScopedDependency`（作用域）：在同一请求（如Web请求）内共享相同实例。
    - `ISingletonDependency`（单例）：在整个应用程序生命周期内只存在一个实例。
- **属性注入**：仅在特殊情况下（如在基类或框架组件中）使用属性注入，并始终标记为`public`。

#### 4.3. 异常处理
- **用户友好的异常**：当向用户显示清晰、可理解的错误消息时，抛出`UserFriendlyException`。
- **业务逻辑异常**：对于可预测的业务规则验证失败，抛出`BusinessException`。
- **参数验证**：使用`Volo.Abp.Check`类验证方法参数，例如`Check.NotNull(input, nameof(input))`。
- **标准异常**：在适当情况下使用.NET内置的标准异常，如`ArgumentNullException`或`InvalidOperationException`。

#### 4.4. 日志记录
- **使用标准日志**：使用`Microsoft.Extensions.Logging.ILogger`进行日志记录。
- **注入方法**：通过构造函数注入`ILogger<T>`，其中`T`是当前类。
  ```csharp
  private readonly ILogger<MyService> _logger;

  public MyService(ILogger<MyService> logger)
  {
      _logger = logger;
  }
  ```
- **日志级别**：根据信息的重要性选择适当的日志级别（`LogInformation`、`LogWarning`、`LogError`、`LogCritical`）。

#### 4.5. Null处理
- **启用可空引用类型**：项目已启用C#的可空引用类型（`#nullable enable`）。始终注意变量和返回值的可空性。
- **避免返回null**：对于返回集合的方法，如果结果为空，返回空集合（如`Enumerable.Empty<T>()`或`new List<T>()`）而不是`null`。
- **使用`[NotNull]`和`[CanBeNull]`**：在适当情况下，使用`System.Diagnostics.CodeAnalysis`中的属性为分析器提供有关可空性的提示。

#### 4.6. DTO和实体
- **严格分离**：绝不直接在API端点暴露领域实体（聚合根、实体）。
- **使用DTO**：应用程序层和表示层（如API控制器）之间的数据传输必须使用数据传输对象（DTO）。
- **位置**：DTO应定义在`.Application.Contracts`项目中。

#### 4.7. LINQ和数据查询
- **语法**：优先使用LINQ的方法语法而不是查询语法，以保持一致的代码风格。
- **延迟执行**：充分利用`IQueryable<T>`的延迟执行特性。在调用`.ToList()`、`.ToArray()`或`.FirstOrDefault()`等方法之前，构建完整的查询表达式，将过滤和排序操作推送到数据库执行。
- **异步操作**：查询数据库时，始终使用异步LINQ扩展方法，如`await _repository.ToListAsync()`。

#### 4.8. 异步编程
- **`ConfigureAwait(false)`**：在库代码（`/framework`和`/modules`）中，在等待的任务上始终使用`.ConfigureAwait(false)`以避免潜在的死锁。
- **`CancellationToken`**：所有异步方法都应接受`CancellationToken`作为最后一个参数，并提供`default`的默认值。
  ```csharp
  Task<MyResult> DoSomethingAsync(string input, CancellationToken cancellationToken = default);
  ```

### 5. 测试和文档

- **测试框架**：
    - 使用**xUnit**作为单元和集成测试的框架。
    - 使用**Moq**作为模拟框架。
    - 遵循ABP的测试基类（如`AbpTestBase`）以简化测试设置。
- **测试覆盖**：所有新功能或错误修复都必须包含相应的单元测试或集成测试。
- **测试位置**：测试应放置在相应的测试项目中，并镜像源目录结构（例如，`framework/src/SharpAbp.Abp.Core`中的代码应在`framework/test/SharpAbp.Abp.Core.Tests`中有测试）。
- **API文档**：
    - 为所有新的公共类、方法和属性添加**XML文档注释**（`<summary>`、`<param>`、`<returns>`）。
    - ABP将自动利用这些注释生成Swagger/OpenAPI文档。

### 6. 示例提示

以下是基于本项目规范的一些示例提示，您可以使用它们来指导代码生成：

#### 创建新模块
> 我想在`modules`目录中创建一个名为'Blogging'的新功能模块。请遵循Sharp-ABP的项目结构和命名约定生成必要的项目文件结构和初始代码。

#### 添加依赖
> 我需要将'NodaTime' NuGet包添加到`SharpAbp.Abp.Core`项目中。请遵循项目的中央包管理（CPM）约定更新`Directory.Packages.props`和相应的`.csproj`文件。

#### 实现功能
> 请在`Blogging`模块中创建一个名为`Blog`的聚合根，包含`Title`和`Content`属性。此外，创建一个名为`IBlogRepository`的存储库接口，并提供一个`BlogAppService`来处理基本的CRUD操作。确保所有异步方法都遵循项目的编码标准，包括`Async`后缀和`CancellationToken`参数。

#### 编写测试
> 请为`BlogAppService`的`CreateAsync`方法编写xUnit单元测试。使用Moq模拟`IBlogRepository`并验证博客是否正确创建。将测试文件放置在正确的测试项目中。

#### 代码重构
> 请重构`MyLegacyService.cs`中的`ProcessData`方法。目前，它使用同步文件I/O操作；请将其更改为异步方法`ProcessDataAsync`并使用`ConfigureAwait(false)`。确保添加`CancellationToken`参数。
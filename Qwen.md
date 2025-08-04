# Qwen Project: Sharp-ABP (Qwen项目：Sharp-ABP)

本指南为 `sharp-abp` 项目的开发和代码生成提供了上下文和规则。

## 项目概览 (Project Overview)

本项目是一个基于 Abp vNext 框架开发的扩展类库，旨在提供模块化的方式来分离核心框架扩展和高级功能模块。

## 目录结构 (Directory Structure)

- **`/framework`**: 包含基础或核心扩展模块。这些模块通常没有自己的数据库持久化逻辑，而是提供跨领域的关注点和框架增强功能。主解决方案文件 `framework/SharpAbp.sln`位于此。
- **`/modules`**: 包含高级的、特定功能的模块，这些模块通常包含自己的数据库持久化和业务逻辑。每个模块都有自己的解决方案文件 (例如, `modules/account/SharpAbp.Abp.Account.sln`)。
- **`/samples`**: 包含示例项目，用于演示如何使用各种模块和功能。
- **`/docs`**: 包含不同模块和功能的 Markdown 文档。
- **`/build`**: 包含用于自动化构建、测试和打包过程的 PowerShell 脚本。

## 构建与测试 (Building and Testing)

此过程特意排除了 `/samples` 目录。

1.  **构建所有解决方案**: 对主解决方案文件 `framework/SharpAbp.sln` 执行 `dotnet build` 命令。然后，遍历 `/modules` 中的每个模块目录，并对其各自的解决方案文件执行 `dotnet build`。
2.  **运行所有测试**: 成功构建后，对 `framework` 解决方案和 `/modules` 目录中的每个模块解决方案文件执行 `dotnet test`，以运行所有相关的单元和集成测试。

`/build` 目录中的 PowerShell 脚本 (`build-all-release.ps1`, `test-all.ps1`) 已配置为自动化此过程。

-   要以 `Release` 配置构建所有项目，请运行: `.\build\build-all-release.ps1`
-   要运行所有测试，请运行: `.\build\test-all.ps1`

---

## 代码生成规则 (Code Generation Guidelines)

在生成代码时，请严格遵守以下规则：

### 1. 通用约定 (General Conventions)

- **遵守现有约定**: 严格遵守项目中已有的代码风格、命名约定和架构模式。

### 2. 项目与文件结构 (Project & File Structure)

- **文件位置**: 
    - 新的核心框架扩展应放在 `/framework/src` 下的相应子目录中。
    - 新的功能模块（特别是那些具有数据库持久性的模块）应在 `/modules` 下创建一个新的子目录。
- **模块命名与分层**: 在 `/modules` 目录下创建的模块必须遵守官方 ABP 框架的模块分层约定，用于项目和命名空间命名。例如，对于一个名为 "Blogging" 的新功能，项目应按以下模式命名：
    - `SharpAbp.Abp.Blogging.Domain`
    - `SharpAbp.Abp.Blogging.Domain.Shared`
    - `SharpAbp.Abp.Blogging.Application.Contracts`
    - `SharpAbp.Abp.Blogging.Application`
    - `SharpAbp.Abp.Blogging.EntityFrameworkCore`
    - `SharpAbp.Abp.Blogging.HttpApi`
    - `SharpAbp.Abp.Blogging.Web`

### 3. 依赖管理 (Dependency Management)

- **中央包管理 (CPM)**: 本项目使用 .NET 的中央包管理 (CPM) 功能。
    - **所有** `PackageVersion` 定义必须位于仓库根目录的 `Directory.Packages.props` 文件中。
    - 单个 `.csproj` 文件应使用 `<PackageReference Include="PackageName" />` 且不带 `Version` 属性。
    - 添加新包时，首先将 `<PackageVersion Include="PackageName" Version="1.2.3" />` 添加到 `Directory.Packages.props`，然后再将 `<PackageReference>` 添加到相关的 `.csproj` 文件中。
- **包版本约束**: 
    - `AWSSDK.S3` 和 `AWSSDK.SecurityToken` 包的版本不得更新超过 `3.7.300`。不要将它们更新到主版本 4 以上。
- **最小化依赖 (Minimal Dependencies)**: 各个模块应遵循最小依赖原则，尽量减少对外部库的依赖。只有在功能非常复杂且有必要时，才应引入新的第三方库。

### 4. 详细编码规范 (Detailed Coding Standards)

#### 4.1. 命名约定 (Naming Conventions)
- **大小写**: 
    - 类、接口、枚举、方法、属性和事件使用 **PascalCase**。
    - 方法参数和局部变量使用 **camelCase**。
- **接口**: 接口名称必须以 `I` 为前缀，例如 `IBlogRepository`。
- **私有字段**: 私有字段应以 `_` 为前缀并使用 camelCase，例如 `_blogRepository`。
- **异步方法**: 所有返回 `Task` 或 `Task<T>` 的异步方法都必须以 `Async` 为后缀。

#### 4.2. 依赖注入 (Dependency Injection)
- **构造函数注入**: 优先使用构造函数注入。这是最清晰且最符合依赖倒置原则的方式。
- **ABP 服务生命周期**: 通过实现 ABP 提供的接口来注册服务：
    - `ITransientDependency` (瞬时): 每次请求时都创建一个新实例。
    - `IScopedDependency` (作用域): 在同一次请求（如一个 Web 请求）中共享同一个实例。
    - `ISingletonDependency` (单例): 在应用程序生命周期内只有一个实例。
- **属性注入**: 仅在特殊情况下（如在基类或框架组件中）使用属性注入，并始终将其标记为 `public`。

#### 4.3. 异常处理 (Exception Handling)
- **用户友好异常**: 当需要向用户显示一个明确的、可理解的错误消息时，应抛出 `UserFriendlyException`。
- **业务逻辑异常**: 对于可预见的业务规则验证失败，应抛出 `BusinessException`。
- **参数校验**: 使用 `Volo.Abp.Check` 类来校验方法参数，例如 `Check.NotNull(input, nameof(input))`。
- **标准异常**: 在适当的情况下使用 .NET 内置的标准异常，如 `ArgumentNullException` 或 `InvalidOperationException`。

#### 4.4. 日志记录 (Logging)
- **使用标准日志**: 使用 `Microsoft.Extensions.Logging.ILogger` 进行日志记录。
- **注入方式**: 通过构造函数注入 `ILogger<T>`，其中 `T` 是当前类。
  ```csharp
  private readonly ILogger<MyService> _logger;

  public MyService(ILogger<MyService> logger)
  {
      _logger = logger;
  }
  ```
- **日志级别**: 根据信息的重要性选择合适的日志级别（`LogInformation`, `LogWarning`, `LogError`, `LogCritical`）。

#### 4.5. Null 值处理 (Null Handling)
- **启用可空引用类型**: 项目已启用 C# 的可空引用类型 (`#nullable enable`)。请始终注意变量和返回值的可空性。
- **避免返回 Null**: 对于返回集合的方法，如果结果为空，应返回一个空集合（如 `Enumerable.Empty<T>()` 或 `new List<T>()`），而不是 `null`。
- **使用 `[NotNull]` 和 `[CanBeNull]`**: 在适当的情况下，使用 `System.Diagnostics.CodeAnalysis` 中的属性来为分析器提供有关可空性的提示。

#### 4.6. DTOs 与实体 (DTOs & Entities)
- **严格分离**: 绝不能在 API 端点直接暴露领域实体（Aggregate Roots, Entities）。
- **使用 DTOs**: 应用层与展现层（如 API 控制器）之间的数据传输必须使用数据传输对象 (DTOs)。
- **位置**: DTOs 应定义在 `.Application.Contracts` 项目中。

#### 4.7. LINQ 与数据查询
- **语法**: 优先使用 LINQ 的方法语法（Method Syntax）而非查询语法（Query Syntax），以保持代码风格一致。
- **延迟执行**: 充分利用 `IQueryable<T>` 的延迟执行特性。在调用 `.ToList()`、`.ToArray()` 或 `.FirstOrDefault()` 等方法之前，构建完整的查询表达式，以将过滤和排序操作下推到数据库执行。
- **异步操作**: 对数据库进行查询时，始终使用异步的 LINQ 扩展方法，如 `await _repository.ToListAsync()`。

#### 4.8. 异步编程 (Asynchronous Programming)
- **`ConfigureAwait(false)`**: 在库代码 (`/framework` 和 `/modules`) 中，对 `await` 的任务调用始终使用 `.ConfigureAwait(false)`，以避免潜在的死锁。
- **`CancellationToken`**: 所有异步方法都应接受一个 `CancellationToken` 作为其最后一个参数，并为其提供默认值 `default`。
  ```csharp
  Task<MyResult> DoSomethingAsync(string input, CancellationToken cancellationToken = default);
  ```

### 5. 测试与文档 (Testing & Documentation)

- **测试框架**: 
    - 使用 **xUnit** 作为单元和集成测试的框架。
    - 使用 **Moq** 作为模拟（Mocking）框架。
    - 遵循 ABP 的测试基类（如 `AbpTestBase`）来简化测试设置。
- **测试覆盖**: 所有新功能或错误修复都必须包含相应的单元测试或集成测试。
- **测试位置**: 测试应放置在相应的测试项目中，并镜像源目录结构 (例如, `framework/src/SharpAbp.Abp.Core` 中的代码应在 `framework/test/SharpAbp.Abp.Core.Tests` 中有测试)。
- **API 文档**: 
    - 为所有新的公共类、方法和属性添加 **XML 文档注释** (`<summary>`, `<param>`, `<returns>`)。
    - ABP 会自动利用这些注释生成 Swagger/OpenAPI 文档。

### 6. 示例提示词 (Example Prompts)

以下是一些基于本项目规范的示例提示词，你可以使用它们来指导代码生成：

#### 创建新模块 (Creating a New Module)
> 我想在 `modules` 目录下创建一个名为 'Blogging' 的新功能模块。请遵循 Sharp-ABP 的项目结构和命名约定，为我生成必要的项目文件结构和初始代码。

#### 添加依赖 (Adding a Dependency)
> 我需要将 'NodaTime' NuGet 包添加到 `SharpAbp.Abp.Core` 项目中。请遵循项目的中央包管理 (CPM) 约定，更新 `Directory.Packages.props` 和相应的 `.csproj` 文件。

#### 实现功能 (Implementing a Feature)
> 请在 `Blogging` 模块中，创建一个名为 `Blog` 的聚合根，包含 `Title` 和 `Content` 属性。同时，创建一个名为 `IBlogRepository` 的仓储接口，并提供一个 `BlogAppService` 来处理基本的 CRUD 操作。请确保所有异步方法都遵循项目的编码规范，包括 `Async` 后缀和 `CancellationToken` 参数。

#### 编写测试 (Writing Tests)
> 请为 `BlogAppService` 的 `CreateAsync` 方法编写一个 xUnit 单元测试。使用 Moq 模拟 `IBlogRepository` 并验证博客是否已正确创建。将测试文件放置在正确的测试项目中。

#### 代码重构 (Refactoring Code)
> 请重构 `MyLegacyService.cs` 中的 `ProcessData` 方法。当前它使用了同步的文件 I/O 操作，请将其更改为异步方法 `ProcessDataAsync`，并使用 `ConfigureAwait(false)`。确保添加 `CancellationToken` 参数。

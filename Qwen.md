# Qwen Project: Sharp-ABP

This guide provides context and rules for development and code generation in the `sharp-abp` project.

## Project Overview

This project is an extension library developed based on the Abp vNext framework, designed to provide a modular approach to separate core framework extensions and advanced functional modules.

## Directory Structure

- **`/framework`**: Contains base or core extension modules. These modules typically do not have their own database persistence logic, but rather provide cross-cutting concerns and framework enhancement features. The main solution file `framework/SharpAbp.sln` is located here.
- **`/modules`**: Contains advanced, specific functional modules that usually contain their own database persistence and business logic. Each module has its own solution file (e.g., `modules/account/SharpAbp.Abp.Account.sln`).
- **`/samples`**: Contains sample projects demonstrating how to use various modules and features.
- **`/docs`**: Contains Markdown documentation for different modules and features.
- **`/build`**: Contains PowerShell scripts for automating the build, test, and packaging processes.

## Building and Testing

This process intentionally excludes the `/samples` directory.

1. **Build all solutions**: Execute the `dotnet build` command on the main solution file `framework/SharpAbp.sln`. Then, traverse each module directory in `/modules` and execute `dotnet build` on their respective solution files.
2. **Run all tests**: After successful building, execute `dotnet test` on the `framework` solution and each module solution file in the `/modules` directory to run all related unit and integration tests.

The PowerShell scripts in the `/build` directory (`build-all-release.ps1`, `test-all.ps1`) are configured to automate this process.

- To build all projects in `Release` configuration, run: `./build/build-all-release.ps1`
- To run all tests, run: `./build/test-all.ps1`

---

## Code Generation Guidelines

When generating code, please strictly adhere to the following rules:

### 1. General Conventions

- **Follow existing conventions**: Strictly adhere to the existing code style, naming conventions, and architectural patterns in the project.

### 2. Project & File Structure

- **File location**: 
    - New core framework extensions should be placed under the appropriate subdirectories in `/framework/src`.
    - New functional modules (especially those with database persistence) should create a new subdirectory under `/modules`.
- **Module naming and layering**: Modules created in the `/modules` directory must follow the official ABP framework's module layering conventions for project and namespace naming. For example, for a new feature called "Blogging", projects should be named according to the following pattern:
    - `SharpAbp.Abp.Blogging.Domain`
    - `SharpAbp.Abp.Blogging.Domain.Shared`
    - `SharpAbp.Abp.Blogging.Application.Contracts`
    - `SharpAbp.Abp.Blogging.Application`
    - `SharpAbp.Abp.Blogging.EntityFrameworkCore`
    - `SharpAbp.Abp.Blogging.HttpApi`
    - `SharpAbp.Abp.Blogging.Web`

### 3. Dependency Management

- **Central Package Management (CPM)**: This project uses .NET's Central Package Management (CPM) feature.
    - **All** `PackageVersion` definitions must be located in the `Directory.Packages.props` file at the repository root.
    - Individual `.csproj` files should use `<PackageReference Include="PackageName" />` without the `Version` attribute.
    - When adding a new package, first add `<PackageVersion Include="PackageName" Version="1.2.3" />` to `Directory.Packages.props`, then add the `<PackageReference>` to the relevant `.csproj` file.
- **Package version constraints**: 
    - Versions of `AWSSDK.S3` and `AWSSDK.SecurityToken` packages must not be updated beyond `3.7.300`. Do not update them to major version 4 or above.
- **Minimize dependencies**: Modules should follow the principle of minimal dependencies, minimizing reliance on external libraries. New third-party libraries should only be introduced when functionality is very complex and necessary.

### 4. Detailed Coding Standards

#### 4.1. Naming Conventions
- **Casing**: 
    - Classes, interfaces, enumerations, methods, properties, and events use **PascalCase**.
    - Method parameters and local variables use **camelCase**.
- **Interfaces**: Interface names must be prefixed with `I`, for example `IBlogRepository`.
- **Private fields**: Private fields should be prefixed with `_` and use camelCase, for example `_blogRepository`.
- **Async methods**: All asynchronous methods returning `Task` or `Task<T>` must be suffixed with `Async`.

#### 4.2. Dependency Injection
- **Constructor injection**: Prefer constructor injection. This is the clearest and most dependency inversion principle-compliant approach.
- **ABP service lifetimes**: Register services by implementing ABP-provided interfaces:
    - `ITransientDependency` (Transient): A new instance is created for each request.
    - `IScopedDependency` (Scoped): The same instance is shared within the same request (such as a Web request).
    - `ISingletonDependency` (Singleton): Only one instance exists throughout the application lifecycle.
- **Property injection**: Use property injection only in special cases (such as in base classes or framework components) and always mark it as `public`.

#### 4.3. Exception Handling
- **User-friendly exceptions**: When displaying a clear, understandable error message to the user, throw `UserFriendlyException`.
- **Business logic exceptions**: For predictable business rule validation failures, throw `BusinessException`.
- **Parameter validation**: Use the `Volo.Abp.Check` class to validate method parameters, for example `Check.NotNull(input, nameof(input))`.
- **Standard exceptions**: Use .NET built-in standard exceptions where appropriate, such as `ArgumentNullException` or `InvalidOperationException`.

#### 4.4. Logging
- **Use standard logging**: Use `Microsoft.Extensions.Logging.ILogger` for logging.
- **Injection method**: Inject `ILogger<T>` through the constructor, where `T` is the current class.
  ```csharp
  private readonly ILogger<MyService> _logger;

  public MyService(ILogger<MyService> logger)
  {
      _logger = logger;
  }
  ```
- **Log levels**: Choose appropriate log levels based on the importance of the information (`LogInformation`, `LogWarning`, `LogError`, `LogCritical`).

#### 4.5. Null Handling
- **Enable nullable reference types**: The project has enabled C#'s nullable reference types (`#nullable enable`). Always pay attention to the nullability of variables and return values.
- **Avoid returning null**: For methods returning collections, if the result is empty, return an empty collection (such as `Enumerable.Empty<T>()` or `new List<T>()`) instead of `null`.
- **Use `[NotNull]` and `[CanBeNull]`**: Where appropriate, use attributes from `System.Diagnostics.CodeAnalysis` to provide hints about nullability to analyzers.

#### 4.6. DTOs & Entities
- **Strict separation**: Never directly expose domain entities (Aggregate Roots, Entities) at API endpoints.
- **Use DTOs**: Data transfer between the application layer and presentation layer (such as API controllers) must use Data Transfer Objects (DTOs).
- **Location**: DTOs should be defined in the `.Application.Contracts` project.

#### 4.7. LINQ and Data Queries
- **Syntax**: Prefer LINQ's method syntax over query syntax to maintain consistent code style.
- **Deferred execution**: Fully utilize the deferred execution feature of `IQueryable<T>`. Before calling methods like `.ToList()`, `.ToArray()`, or `.FirstOrDefault()`, build the complete query expression to push filtering and sorting operations down to the database execution.
- **Asynchronous operations**: When querying the database, always use asynchronous LINQ extension methods, such as `await _repository.ToListAsync()`.

#### 4.8. Asynchronous Programming
- **`ConfigureAwait(false)`**: In library code (`/framework` and `/modules`), always use `.ConfigureAwait(false)` on awaited tasks to avoid potential deadlocks.
- **`CancellationToken`**: All asynchronous methods should accept a `CancellationToken` as their last parameter and provide a default value of `default`.
  ```csharp
  Task<MyResult> DoSomethingAsync(string input, CancellationToken cancellationToken = default);
  ```

### 5. Testing & Documentation

- **Testing framework**: 
    - Use **xUnit** as the framework for unit and integration tests.
    - Use **Moq** as the mocking framework.
    - Follow ABP's test base classes (such as `AbpTestBase`) to simplify test setup.
- **Test coverage**: All new features or bug fixes must include corresponding unit tests or integration tests.
- **Test location**: Tests should be placed in the corresponding test projects and mirror the source directory structure (for example, code in `framework/src/SharpAbp.Abp.Core` should have tests in `framework/test/SharpAbp.Abp.Core.Tests`).
- **API documentation**: 
    - Add **XML documentation comments** (`<summary>`, `<param>`, `<returns>`) for all new public classes, methods, and properties.
    - ABP will automatically utilize these comments to generate Swagger/OpenAPI documentation.

### 6. Example Prompts

Here are some example prompts based on this project's specifications that you can use to guide code generation:

#### Creating a New Module
> I want to create a new functional module named 'Blogging' in the `modules` directory. Please follow Sharp-ABP's project structure and naming conventions to generate the necessary project file structure and initial code.

#### Adding a Dependency
> I need to add the 'NodaTime' NuGet package to the `SharpAbp.Abp.Core` project. Please follow the project's Central Package Management (CPM) conventions to update `Directory.Packages.props` and the corresponding `.csproj` file.

#### Implementing a Feature
> Please create an aggregate root named `Blog` in the `Blogging` module with `Title` and `Content` properties. Also, create a repository interface named `IBlogRepository` and provide a `BlogAppService` to handle basic CRUD operations. Ensure all asynchronous methods follow the project's coding standards, including the `Async` suffix and `CancellationToken` parameter.

#### Writing Tests
> Please write an xUnit unit test for the `CreateAsync` method of `BlogAppService`. Use Moq to mock `IBlogRepository` and verify that the blog is correctly created. Place the test file in the correct test project.

#### Code Refactoring
> Please refactor the `ProcessData` method in `MyLegacyService.cs`. Currently, it uses synchronous file I/O operations; please change it to an asynchronous method `ProcessDataAsync` and use `ConfigureAwait(false)`. Ensure to add the `CancellationToken` parameter.
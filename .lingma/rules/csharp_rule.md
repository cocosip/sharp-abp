---
trigger: always_on
---

# Sharp-ABP C# Project Rules

## Project Overview

This project is a collection of extension modules based on the ABP vNext framework, providing various functional modules to enhance the capabilities of ABP applications.

## Naming Conventions

### Project Naming
- Project names start with `SharpAbp.Abp` followed by the module name
- Example: `SharpAbp.Abp.FileStoring`, `SharpAbp.Abp.FreeRedis`

### Namespace
- Namespaces start with `SharpAbp.Abp`, corresponding to the project name
- Example: `SharpAbp.Abp.FileStoring`, `SharpAbp.Abp.FreeRedis`

### Class Naming
- Class names use PascalCase naming convention
- Module class names end with `XxxModule`
- DTO class names end with `XxxDto`
- Application service class names end with `XxxAppService`

### Interface Naming
- Interface names use PascalCase naming convention and start with `I`
- Example: `IFileContainerConfiguration`

### Method Naming
- Method names use PascalCase naming convention
- Async methods end with `Async`

## Project Structure

### Module Structure
The project adopts the modular structure recommended by ABP. Each functional module typically contains the following submodules:

- `Xxx.Domain.Shared` - Domain shared layer, containing constants, enumerations, DTOs, etc.
- `Xxx.Domain` - Domain layer, containing entities, repository interfaces, domain services, etc.
- `Xxx.EntityFrameworkCore` - Entity Framework Core integration
- `Xxx.MongoDB` - MongoDB integration
- `Xxx.Application.Contracts` - Application service interfaces and DTOs
- `Xxx.Application` - Application service implementation
- `Xxx.HttpApi` - HTTP API controllers
- `Xxx.HttpApi.Client` - HTTP API client

### Code Organization
- Each module should have a clear layered structure
- Follow ABP's dependency injection and modular design principles
- Use ABP's exception handling and localization mechanisms

## Coding Standards

### Dependency Injection
- Use constructor injection
- Service interfaces should inherit from the corresponding ABP base interfaces (such as `IApplicationService`)
- Use `[Autowired]` attribute for property injection (if needed)

### Exception Handling
- Use ABP's exception handling mechanism
- Custom exceptions should inherit from ABP's exception base classes

### Logging
- Use ABP's logging system
- Log through the `ILogger<T>` interface

### Localization
- Use ABP's localization system
- Resource files should be placed in the module's `Localization` folder

## ABP Specific Standards

### Module Definition
- Module classes should inherit from `AbpModule`
- Register services in the `ConfigureServices` method
- Configure application initialization logic in the `OnApplicationInitialization` method

### Application Services
- Application services should inherit from the `ApplicationService` base class
- Implement corresponding interfaces and mark with `[Service]` attribute
- Use DTOs for data transfer

### Entity Design
- Entities should inherit from ABP's `Entity` base class
- Use ABP's audit properties (such as `CreationTime`, `CreatorId`, etc.)
- Entity properties should have appropriate validation attributes

### Repository Pattern
- Repository interfaces should inherit from `IRepository<TEntity>`
- Use ABP's provided generic repository methods
- Custom repositories should be placed in the `Repositories` folder

## Technology Stack

- .NET 9.0
- ABP vNext 9.2.3+
- Entity Framework Core 9.0.7+
- MassTransit 8.5.1+

## Testing Standards

- Unit test project names end with `.Tests`
- Integration test project names end with `.TestBase`
- Use xUnit as the testing framework
- Use Shouldly for assertions
- Use NSubstitute for mocking

## Documentation Standards

- Every public class and public method should have XML comments
- Comment content should be written in English
- README.md should include module introduction, installation steps, and basic usage
- Complex features should have dedicated documentation
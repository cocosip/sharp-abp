# Gemini Project: Sharp-ABP (Gemini项目：Sharp-ABP)

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

### 4. 编码规范 (Coding Standards)

- **异步方法签名**: 所有异步方法必须遵循严格的签名约定：
    - 方法名必须以 `Async` 后缀结尾。
    - 方法必须接受一个 `CancellationToken` 作为其最后一个参数。
    - `CancellationToken` 参数必须有默认值 `default`。
    - **正确示例**: `Task<MyResult> DoSomethingAsync(string input, CancellationToken cancellationToken = default);`
- **Async/Await**: 在库代码 (`/framework` 和 `/modules`) 中，对 `await` 的任务调用始终使用 `.ConfigureAwait(false)`。

### 5. 测试与文档 (Testing & Documentation)

- **测试**: 
    - 所有新功能或错误修复都必须包含相应的单元测试。
    - 测试应放置在相应的测试项目中，并镜像源目录结构 (例如, `framework/src/SharpAbp.Abp.Core` 中的代码应在 `framework/test/SharpAbp.Abp.Core.Tests` 中有测试)。
- **API 文档**: 为所有新的公共类、方法和属性添加 XML 文档注释 (`<summary>`, `<param>`, `<returns>`)。
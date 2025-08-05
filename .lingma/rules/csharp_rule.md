trigger: always_on
---

# Sharp-ABP C# 项目规范

## 项目概述

本项目是基于 ABP vNext 框架的扩展模块集合，提供各种功能模块以增强 ABP 应用的能力。

## 命名规范

### 项目命名
- 项目名称以 `SharpAbp.Abp` 开头，后跟模块名称
- 示例: `SharpAbp.Abp.FileStoring`, `SharpAbp.Abp.FreeRedis`

### 命名空间
- 命名空间以 `SharpAbp.Abp` 开头，与项目名称相对应
- 示例: `SharpAbp.Abp.FileStoring`, `SharpAbp.Abp.FreeRedis`

### 类命名
- 类名使用 PascalCase 命名法
- 模块类名以 `XxxModule` 结尾
- DTO 类名以 `XxxDto` 结尾
- 应用服务类名以 `XxxAppService` 结尾

### 接口命名
- 接口名称使用 PascalCase 命名法，并以 `I` 开头
- 示例: `IFileContainerConfiguration`

### 方法命名
- 方法名使用 PascalCase 命名法
- 异步方法以 `Async` 结尾

## 项目结构

### 模块结构
项目采用 ABP 推荐的模块化结构。每个功能模块通常包含以下子模块：

- `Xxx.Domain.Shared` - 领域共享层，包含常量、枚举、DTO 等。
- `Xxx.Domain` - 领域层，包含实体、仓储接口、领域服务等。
- `Xxx.EntityFrameworkCore` - Entity Framework Core 集成
- `Xxx.MongoDB` - MongoDB 集成
- `Xxx.Application.Contracts` - 应用服务接口和 DTO
- `Xxx.Application` - 应用服务实现
- `Xxx.HttpApi` - HTTP API 控制器
- `Xxx.HttpApi.Client` - HTTP API 客户端

### 代码组织
- 每个模块都应有清晰的层次结构
- 遵循 ABP 的依赖注入和模块化设计原则
- 使用 ABP 的异常处理和本地化机制

## 编码标准

### 依赖注入
- 使用构造函数注入
- 服务接口应继承自相应的 ABP 基础接口（如 `IApplicationService`）
- 使用 `[Autowired]` 特性进行属性注入（如果需要）

### 异常处理
- 使用 ABP 的异常处理机制
- 自定义异常应继承自 ABP 的异常基类

### 日志记录
- 使用 ABP 的日志系统
- 通过 `ILogger<T>` 接口进行日志记录

### 本地化
- 使用 ABP 的本地化系统
- 资源文件应放置在模块的 `Localization` 文件夹中

## ABP 特定标准

### 模块定义
- 模块类应继承自 `AbpModule`
- 在 `ConfigureServices` 方法中注册服务
- 在 `OnApplicationInitialization` 方法中配置应用程序初始化逻辑

### 应用服务
- 应用服务应继承自 `ApplicationService` 基类
- 实现相应的接口并使用 `[Service]` 特性标记
- 使用 DTO 进行数据传输

### 实体设计
- 实体应继承自 ABP 的 `Entity` 基类
- 使用 ABP 的审计属性（如 `CreationTime`, `CreatorId` 等）
- 实体属性应具有适当的验证特性

### 仓储模式
- 仓储接口应继承自 `IRepository<TEntity>`
- 使用 ABP 提供的泛型仓储方法
- 自定义仓储应放置在 `Repositories` 文件夹中

## 技术栈

- .NET 9.0
- ABP vNext 9.2.3+
- Entity Framework Core 9.0.7+
- MassTransit 8.5.1+

## 测试标准

- 单元测试项目名称以 `.Tests` 结尾
- 集成测试项目名称以 `.TestBase` 结尾
- 使用 xUnit 作为测试框架
- 使用 Shouldly 进行断言
- 使用 NSubstitute 进行模拟

## 文档标准

- 每个公共类和公共方法都应有 XML 注释
- 注释内容应使用英文编写
- README.md 应包含模块介绍、安装步骤和基本用法
- 复杂功能应有专门的文档

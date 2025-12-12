# **sharp-abp**

> Enterprise extension modules for ABP vNext

An enterprise-grade extension module collection based on ABP vNext framework, consisting of 77 core framework packages and 13 business modules, providing complete enterprise application solutions including multi-database support, distributed file storage, message queuing, authentication, multi-tenancy isolation, and more.

## Project Structure

- **Framework**: 77 core framework packages providing infrastructure support
- **Modules**: 13 business modules providing complete enterprise-grade functionality
- **Samples**: Sample projects demonstrating module usage

## Key Features

- **Multi-Database Support**: MySQL, PostgreSQL, SQL Server, Oracle, SQLite, DM (DaMeng), GaussDB
- **Multi-ORM Support**: Entity Framework Core, FreeSql, Dapper
- **File Storage**: Support for 11 storage providers including Aliyun OSS, Azure, AWS, MinIO, KS3, Huawei OBS, FastDFS, S3
- **Message Queue**: RabbitMQ, Kafka, ActiveMQ (via MassTransit)
- **Authentication**: IdentityServer4, OpenIddict OAuth2/OIDC support
- **Multi-Tenancy**: Map-based tenancy isolation, tenant grouping isolation
- **Observability**: OpenTelemetry, Zipkin, Prometheus integration
- **Data Security**: SM2/AES encryption, secret management, transform security

## Framework Modules

### Core Modules

| Name | Description | Documentation |
| ---- | ----------- | ------------- |
| **DotCommon** | DotCommon ABP adapter with Snowflake ID generation | [docs](/docs/Core.md#dotcommon) |
| **Snowflakes** | Distributed unique ID generator | [docs](/docs/Core.md#snowflakes) |
| **Crypto** | Encryption and decryption with SM2/AES support | [docs](/docs/Core.md#crypto) |
| **FreeRedis** | FreeRedis cache library ABP adapter | [docs](/docs/Core.md#freeredis) |
| **Validation** | Data validation module | [docs](/docs/Core.md#validation) |
| **AspNetCore** | AspNetCore integration extensions | [docs](/docs/Core.md#aspnetcore) |
| **Swashbuckle** | Swagger/Swashbuckle API documentation support | [docs](/docs/Core.md#swashbuckle) |
| **Swashbuckle.Versioning** | API versioning management | [docs](/docs/Core.md#swashbuckleversioning) |

### ORM Frameworks

| Name | Description | Documentation |
| ---- | ----------- | ------------- |
| **FreeSql** | FreeSql ORM framework integration (supports MySQL/PostgreSQL/SQLServer/Oracle/SQLite/DM) | [docs](/docs/ORM.md#freesql) |
| **Dapper** | Dapper micro-ORM framework integration | [docs](/docs/ORM.md#dapper) |
| **EntityFrameworkCore** | EF Core integration (supports DM, GaussDB) | [docs](/docs/ORM.md#entityframeworkcore) |

### File Storage

| Name | Description | Documentation |
| ---- | ----------- | ------------- |
| **FileStoring** | Unified file storage abstraction and core implementation | [docs](/docs/FileStorage.md#filestoring) |
| **AutoS3** | AWSSDK.S3 adapter (supports KS3) | [docs](/docs/FileStorage.md#autos3) |
| **FastDFS** | FastDFS distributed file system adapter | [docs](/docs/FileStorage.md#fastdfs) |

### Database Connections

| Name | Description | Documentation |
| ---- | ----------- | ------------- |
| **DbConnections** | Multi-database connection management (supports MySQL/PostgreSQL/SQLServer/Oracle/SQLite/DM/GaussDB) | [docs](/docs/DatabaseConnections.md) |

### Message Queue

| Name | Description | Documentation |
| ---- | ----------- | ------------- |
| **MassTransit** | MassTransit message bus integration (supports RabbitMQ/Kafka/ActiveMQ) | [docs](/docs/MassTransit.md) |

### Multi-Tenancy

| Name | Description | Documentation |
| ---- | ----------- | ------------- |
| **MapTenancy** | Map-based tenancy isolation (domain-tenant binding) | [docs](/docs/MultiTenancy.md#maptenancy) |
| **TenancyGrouping** | Tenant grouping isolation | [docs](/docs/MultiTenancy.md#tenancygrouping) |

### Security & Encryption

| Name | Description | Documentation |
| ---- | ----------- | ------------- |
| **TransformSecurity** | Data transform security (SM2/AES encryption) | [docs](/docs/Security.md) |

### Observability

| Name | Description | Documentation |
| ---- | ----------- | ------------- |
| **OpenTelemetry** | OpenTelemetry observability platform (supports Zipkin/Prometheus/OTLP) | [docs](/docs/Observability.md) |

## Business Modules

| Name | Description | Documentation |
| ---- | ----------- | ------------- |
| **Account** | Account management (registration, login, verification) | [docs](/docs/BusinessModules.md#account) |
| **Identity** | Identity management (users, roles, permissions, organizations) | [docs](/docs/BusinessModules.md#identity) |
| **IdentityServer** | IdentityServer4 OAuth2/OIDC integration | [docs](/docs/BusinessModules.md#identityserver--openiddict) |
| **OpenIddict** | OpenIddict OAuth2/OIDC implementation | [docs](/docs/BusinessModules.md#identityserver--openiddict) |
| **AuditLogging** | Audit logging (operation audit, compliance tracking) | [docs](/docs/BusinessModules.md#auditlogging) |
| **MinId** | Distributed ID generation service | [docs](/docs/BusinessModules.md#minid) |
| **FileStoringManagement** | File storage management (upload, download, configuration) | [docs](/docs/BusinessModules.md#filestoringmanagement) |
| **FileStoringDatabase** | Database file storage provider | [docs](/docs/BusinessModules.md#filestoringdatabase) |
| **DbConnectionsManagement** | Database connection configuration management | [docs](/docs/BusinessModules.md#dbconnectionsmanagement) |
| **MapTenancyManagement** | Map-based tenancy management | [docs](/docs/BusinessModules.md#maptenancymanagement) |
| **TenantGroupManagement** | Tenant group management | [docs](/docs/BusinessModules.md#tenantgroupmanagement) |
| **CryptoVault** | Crypto vault and secret management | [docs](/docs/BusinessModules.md#cryptovault) |
| **TransformSecurityManagement** | Data transform security management | [docs](/docs/BusinessModules.md#transformsecuritymanagement) |


## Packages

| Package  | Version | Downloads|
| -------- | ------- | -------- |
| `SharpAbp.Abp.DotCommon` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.DotCommon.svg)](https://www.nuget.org/packages/SharpAbp.Abp.DotCommon) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.DotCommon.svg)|
| `SharpAbp.Abp.Snowflakes` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.Snowflakes.svg)](https://www.nuget.org/packages/SharpAbp.Abp.Snowflakes) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.Snowflakes.svg)|
| `SharpAbp.Abp.FreeSql` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FreeSql.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FreeSql) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FreeSql.svg)|
| `SharpAbp.Abp.FreeRedis` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FreeRedis.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FreeRedis) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FreeRedis.svg)|
| `SharpAbp.Abp.AspNetCore` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.AspNetCore.svg)](https://www.nuget.org/packages/SharpAbp.Abp.AspNetCore) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.AspNetCore.svg)|
| `SharpAbp.Abp.Swashbuckle` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.Swashbuckle.svg)](https://www.nuget.org/packages/SharpAbp.Abp.Swashbuckle) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.Swashbuckle.svg)|
| `SharpAbp.Abp.Swashbuckle.Versioning` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.Swashbuckle.Versioning.svg)](https://www.nuget.org/packages/SharpAbp.Abp.Swashbuckle.Versioning) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.Swashbuckle.Versioning.svg)|
| **FastDFS** | - | - |
| `SharpAbp.Abp.FastDFS` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FastDFS.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FastDFS) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FastDFS.svg)|
| `SharpAbp.Abp.FastDFS.DotNetty` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FastDFS.DotNetty.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FastDFS.DotNetty) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FastDFS.DotNetty.svg)|
| `SharpAbp.Abp.FastDFS.SuperSocket` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FastDFS.SuperSocket.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FastDFS.SuperSocket) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FastDFS.SuperSocket.svg)|
| **MassTransit** | - | - |
| `SharpAbp.Abp.MassTransit` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.MassTransit.svg)](https://www.nuget.org/packages/SharpAbp.Abp.MassTransit) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.MassTransit.svg)|
| `SharpAbp.Abp.MassTransit.Kafka` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.MassTransit.Kafka.svg)](https://www.nuget.org/packages/SharpAbp.Abp.MassTransit.Kafka) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.MassTransit.Kafka.svg)|
| `SharpAbp.Abp.MassTransit.RabbitMQ` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.MassTransit.RabbitMQ.svg)](https://www.nuget.org/packages/SharpAbp.Abp.MassTransit.RabbitMQ) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.MassTransit.RabbitMQ.svg)|
| `SharpAbp.Abp.MassTransit.ActiveMQ` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.MassTransit.ActiveMQ.svg)](https://www.nuget.org/packages/SharpAbp.Abp.MassTransit.ActiveMQ) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.MassTransit.ActiveMQ.svg)|
| **FileStoring** | - | - |
| `SharpAbp.Abp.FileStoring.Abstractions` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoring.Abstractions.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoring.Abstractions) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoring.Abstractions.svg)|
| `SharpAbp.Abp.FileStoring` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoring.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoring) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoring.svg)|
| `SharpAbp.Abp.FileStoring.Aliyun` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoring.Aliyun.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoring.Aliyun) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoring.Aliyun.svg)|
| `SharpAbp.Abp.FileStoring.Azure` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoring.Azure.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoring.Azure) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoring.Azure.svg)|
| `SharpAbp.Abp.FileStoring.Aws` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoring.Aws.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoring.Aws) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoring.Aws.svg)|
| `SharpAbp.Abp.FileStoring.FastDFS` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoring.FastDFS.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoring.FastDFS) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoring.FastDFS.svg)|
| `SharpAbp.Abp.FileStoring.FileSystem` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoring.FileSystem.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoring.FileSystem) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoring.FileSystem.svg)|
| `SharpAbp.Abp.FileStoring.Minio` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoring.Minio.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoring.Minio) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoring.Minio.svg)|
| `SharpAbp.Abp.FileStoring.KS3` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoring.KS3.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoring.KS3) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoring.KS3.svg)|
| `SharpAbp.Abp.FileStoring.Obs` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoring.Obs.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoring.Obs) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoring.Obs.svg)|
| `SharpAbp.Abp.FileStoring.S3` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoring.S3.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoring.S3) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoring.S3.svg)|
| **MinId** | - | - |
| `SharpAbp.MinId.Domain.Shared` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.MinId.Domain.Shared.svg)](https://www.nuget.org/packages/SharpAbp.MinId.Domain.Shared) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.MinId.Domain.Shared.svg)|
| `SharpAbp.MinId.Domain` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.MinId.Domain.svg)](https://www.nuget.org/packages/SharpAbp.MinId.Domain) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.MinId.Domain.svg)|
| `SharpAbp.MinId.EntityFrameworkCore` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.MinId.EntityFrameworkCore.svg)](https://www.nuget.org/packages/SharpAbp.MinId.EntityFrameworkCore) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.MinId.EntityFrameworkCore.svg)|
| `SharpAbp.MinId.MongoDB` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.MinId.MongoDB.svg)](https://www.nuget.org/packages/SharpAbp.MinId.MongoDB) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.MinId.MongoDB.svg)|
| `SharpAbp.MinId.Application.Contracts` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.MinId.Application.Contracts.svg)](https://www.nuget.org/packages/SharpAbp.MinId.Application.Contracts) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.MinId.Application.Contracts.svg)|
| `SharpAbp.MinId.Application` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.MinId.Application.svg)](https://www.nuget.org/packages/SharpAbp.MinId.Application) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.MinId.Application.svg)|
| `SharpAbp.MinId.HttpApi` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.MinId.HttpApi.svg)](https://www.nuget.org/packages/SharpAbp.MinId.HttpApi) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.MinId.HttpApi.svg)|
| `SharpAbp.MinId.HttpApi.Client` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.MinId.HttpApi.Client.svg)](https://www.nuget.org/packages/SharpAbp.MinId.HttpApi.Client) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.MinId.HttpApi.Client.svg)|
| **FileStoringManagement** | - | - |
| `SharpAbp.Abp.FileStoringManagement.Domain.Shared` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoringManagement.Domain.Shared.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoringManagement.Domain.Shared) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoringManagement.Domain.Shared.svg)|
| `SharpAbp.Abp.FileStoringManagement.Domain` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoringManagement.Domain.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoringManagement.Domain) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoringManagement.Domain.svg)|
| `SharpAbp.Abp.FileStoringManagement.EntityFrameworkCore` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoringManagement.EntityFrameworkCore.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoringManagement.EntityFrameworkCore) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoringManagement.EntityFrameworkCore.svg)|
| `SharpAbp.Abp.FileStoringManagement.MongoDB` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoringManagement.MongoDB.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoringManagement.MongoDB) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoringManagement.MongoDB.svg)|
| `SharpAbp.Abp.FileStoringManagement.Application.Contracts` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoringManagement.Application.Contracts.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoringManagement.Application.Contracts) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoringManagement.Application.Contracts.svg)|
| `SharpAbp.Abp.FileStoringManagement.Application` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoringManagement.Application.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoringManagement.Application) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoringManagement.Application.svg)|
| `SharpAbp.Abp.FileStoringManagement.HttpApi` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoringManagement.HttpApi.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoringManagement.HttpApi) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoringManagement.HttpApi.svg)|
| `SharpAbp.Abp.FileStoringManagement.HttpApi.Client` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.FileStoringManagement.HttpApi.Client.svg)](https://www.nuget.org/packages/SharpAbp.Abp.FileStoringManagement.HttpApi.Client) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.FileStoringManagement.HttpApi.Client.svg)|
| **MapTenancy** | - | - |
| `SharpAbp.Abp.MapTenancy` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.MapTenancy.svg)](https://www.nuget.org/packages/SharpAbp.Abp.MapTenancy) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.MapTenancy.svg)|
| `SharpAbp.Abp.AspNetCore.MapTenancy` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.AspNetCore.MapTenancy.svg)](https://www.nuget.org/packages/SharpAbp.Abp.AspNetCore.MapTenancy) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.AspNetCore.MapTenancy.svg)|
| **MapTenancy Management** | - | - |
| `SharpAbp.Abp.MapTenancyManagement.Domain.Shared` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.MapTenancyManagement.Domain.Shared.svg)](https://www.nuget.org/packages/SharpAbp.Abp.MapTenancyManagement.Domain.Shared) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.MapTenancyManagement.Domain.Shared.svg)|
| `SharpAbp.Abp.MapTenancyManagement.Domain` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.MapTenancyManagement.Domain.svg)](https://www.nuget.org/packages/SharpAbp.Abp.MapTenancyManagement.Domain) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.MapTenancyManagement.Domain.svg)|
| `SharpAbp.Abp.MapTenancyManagement.EntityFrameworkCore` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.MapTenancyManagement.EntityFrameworkCore.svg)](https://www.nuget.org/packages/SharpAbp.Abp.MapTenancyManagement.EntityFrameworkCore) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.MapTenancyManagement.EntityFrameworkCore.svg)|
| `SharpAbp.Abp.MapTenancyManagement.MongoDB` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.MapTenancyManagement.MongoDB.svg)](https://www.nuget.org/packages/SharpAbp.Abp.MapTenancyManagement.MongoDB) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.MapTenancyManagement.MongoDB.svg)|
| `SharpAbp.Abp.MapTenancyManagement.Application.Contracts` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.MapTenancyManagement.Application.Contracts.svg)](https://www.nuget.org/packages/SharpAbp.Abp.MapTenancyManagement.Application.Contracts) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.MapTenancyManagement.Application.Contracts.svg)|
| `SharpAbp.Abp.MapTenancyManagement.Application` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.MapTenancyManagement.Application.svg)](https://www.nuget.org/packages/SharpAbp.Abp.MapTenancyManagement.Application) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.MapTenancyManagement.Application.svg)|
| `SharpAbp.Abp.MapTenancyManagement.HttpApi` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.MapTenancyManagement.HttpApi.svg)](https://www.nuget.org/packages/SharpAbp.Abp.MapTenancyManagement.HttpApi) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.MapTenancyManagement.HttpApi.svg)|
| `SharpAbp.Abp.MapTenancyManagement.HttpApi.Client` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.MapTenancyManagement.HttpApi.Client.svg)](https://www.nuget.org/packages/SharpAbp.Abp.MapTenancyManagement.HttpApi.Client) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.MapTenancyManagement.HttpApi.Client.svg)|
| **TenancyGrouping** | - | - |
| `SharpAbp.Abp.TenancyGrouping` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.TenancyGrouping.svg)](https://www.nuget.org/packages/SharpAbp.Abp.TenancyGrouping) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.TenancyGrouping.svg)|
| `SharpAbp.Abp.TenantGroupManagement.Domain.Shared` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.TenantGroupManagement.Domain.Shared.svg)](https://www.nuget.org/packages/SharpAbp.Abp.TenantGroupManagement.Domain.Shared) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.TenantGroupManagement.Domain.Shared.svg)|
| `SharpAbp.Abp.TenantGroupManagement.Domain` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.TenantGroupManagement.Domain.svg)](https://www.nuget.org/packages/SharpAbp.Abp.TenantGroupManagement.Domain) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.TenantGroupManagement.Domain.svg)|
| `SharpAbp.Abp.TenantGroupManagement.EntityFrameworkCore` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.TenantGroupManagement.EntityFrameworkCore.svg)](https://www.nuget.org/packages/SharpAbp.Abp.TenantGroupManagement.EntityFrameworkCore) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.TenantGroupManagement.EntityFrameworkCore.svg)|
| `SharpAbp.Abp.TenantGroupManagement.MongoDB` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.TenantGroupManagement.MongoDB.svg)](https://www.nuget.org/packages/SharpAbp.Abp.TenantGroupManagement.MongoDB) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.TenantGroupManagement.MongoDB.svg)|
| `SharpAbp.Abp.TenantGroupManagement.Application.Contracts` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.TenantGroupManagement.Application.Contracts.svg)](https://www.nuget.org/packages/SharpAbp.Abp.TenantGroupManagement.Application.Contracts) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.TenantGroupManagement.Application.Contracts.svg)|
| `SharpAbp.Abp.TenantGroupManagement.Application` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.TenantGroupManagement.Application.svg)](https://www.nuget.org/packages/SharpAbp.Abp.TenantGroupManagement.Application) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.TenantGroupManagement.Application.svg)|
| `SharpAbp.Abp.TenantGroupManagement.HttpApi` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.TenantGroupManagement.HttpApi.svg)](https://www.nuget.org/packages/SharpAbp.Abp.TenantGroupManagement.HttpApi) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.TenantGroupManagement.HttpApi.svg)|
| **DbConnections** | - | - |
| `SharpAbp.Abp.DbConnections` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.DbConnections.svg)](https://www.nuget.org/packages/SharpAbp.Abp.DbConnections) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.DbConnections.svg)|
| `SharpAbp.Abp.DbConnections.MySQL` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.DbConnections.MySQL.svg)](https://www.nuget.org/packages/SharpAbp.Abp.DbConnections.MySQL) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.DbConnections.MySQL.svg)|
| `SharpAbp.Abp.DbConnections.PostgreSql` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.DbConnections.PostgreSql.svg)](https://www.nuget.org/packages/SharpAbp.Abp.DbConnections.PostgreSql) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.DbConnections.PostgreSql.svg)|
| `SharpAbp.Abp.DbConnections.SqlServer` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.DbConnections.SqlServer.svg)](https://www.nuget.org/packages/SharpAbp.Abp.DbConnections.SqlServer) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.DbConnections.SqlServer.svg)|
| `SharpAbp.Abp.DbConnections.Oracle` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.DbConnections.Oracle.svg)](https://www.nuget.org/packages/SharpAbp.Abp.DbConnections.Oracle) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.DbConnections.Oracle.svg)|
| `SharpAbp.Abp.DbConnections.Oracle.Devart` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.DbConnections.Oracle.Devart.svg)](https://www.nuget.org/packages/SharpAbp.Abp.DbConnections.Oracle.Devart) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.DbConnections.Oracle.Devart.svg)|
| `SharpAbp.Abp.DbConnections.Sqlite` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.DbConnections.Sqlite.svg)](https://www.nuget.org/packages/SharpAbp.Abp.DbConnections.Sqlite) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.DbConnections.Sqlite.svg)|
| **DbConnections Management** | - | - |
| `SharpAbp.Abp.DbConnectionsManagement.Domain.Shared` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.DbConnectionsManagement.Domain.Shared.svg)](https://www.nuget.org/packages/SharpAbp.Abp.DbConnectionsManagement.Domain.Shared) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.DbConnectionsManagement.Domain.Shared.svg)|
| `SharpAbp.Abp.DbConnectionsManagement.Domain` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.DbConnectionsManagement.Domain.svg)](https://www.nuget.org/packages/SharpAbp.Abp.DbConnectionsManagement.Domain) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.DbConnectionsManagement.Domain.svg)|
| `SharpAbp.Abp.DbConnectionsManagement.EntityFrameworkCore` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.DbConnectionsManagement.EntityFrameworkCore.svg)](https://www.nuget.org/packages/SharpAbp.Abp.DbConnectionsManagement.EntityFrameworkCore) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.DbConnectionsManagement.EntityFrameworkCore.svg)|
| `SharpAbp.Abp.DbConnectionsManagement.MongoDB` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.DbConnectionsManagement.MongoDB.svg)](https://www.nuget.org/packages/SharpAbp.Abp.DbConnectionsManagement.MongoDB) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.DbConnectionsManagement.MongoDB.svg)|
| `SharpAbp.Abp.DbConnectionsManagement.Application.Contracts` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.DbConnectionsManagement.Application.Contracts.svg)](https://www.nuget.org/packages/SharpAbp.Abp.DbConnectionsManagement.Application.Contracts) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.DbConnectionsManagement.Application.Contracts.svg)|
| `SharpAbp.Abp.DbConnectionsManagement.Application` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.DbConnectionsManagement.Application.svg)](https://www.nuget.org/packages/SharpAbp.Abp.DbConnectionsManagement.Application) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.DbConnectionsManagement.Application.svg)|
| `SharpAbp.Abp.DbConnectionsManagement.HttpApi` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.DbConnectionsManagement.HttpApi.svg)](https://www.nuget.org/packages/SharpAbp.Abp.DbConnectionsManagement.HttpApi) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.DbConnectionsManagement.HttpApi.svg)|
| `SharpAbp.Abp.DbConnectionsManagement.HttpApi.Client` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.DbConnectionsManagement.HttpApi.Client.svg)](https://www.nuget.org/packages/SharpAbp.Abp.DbConnectionsManagement.HttpApi.Client) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.DbConnectionsManagement.HttpApi.Client.svg)|
| **Identity** | - | - |
| `SharpAbp.Abp.IdentityModel` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.IdentityModel.svg)](https://www.nuget.org/packages/SharpAbp.Abp.IdentityModel) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.IdentityModel.svg)|
| `SharpAbp.Abp.Identity.Domain.Shared` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.Identity.Domain.Shared.svg)](https://www.nuget.org/packages/SharpAbp.Abp.Identity.Domain.Shared) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.Identity.Domain.Shared.svg)|
| `SharpAbp.Abp.Identity.Domain` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.Identity.Domain.svg)](https://www.nuget.org/packages/SharpAbp.Abp.Identity.Domain) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.Identity.Domain.svg)|
| `SharpAbp.Abp.Identity.Application.Contracts` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.Identity.Application.Contracts.svg)](https://www.nuget.org/packages/SharpAbp.Abp.Identity.Application.Contracts) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.Identity.Application.Contracts.svg)|
| `SharpAbp.Abp.Identity.Application` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.Identity.Application.svg)](https://www.nuget.org/packages/SharpAbp.Abp.Identity.Application) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.Identity.Application.svg)|
| `SharpAbp.Abp.Identity.HttpApi` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.Identity.HttpApi.svg)](https://www.nuget.org/packages/SharpAbp.Abp.Identity.HttpApi) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.Identity.HttpApi.svg)|
| `SharpAbp.Abp.Identity.HttpApi.Client` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.Identity.HttpApi.Client.svg)](https://www.nuget.org/packages/SharpAbp.Abp.Identity.HttpApi.Client) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.Identity.HttpApi.Client.svg)|
| **IdentityServer** | - | - |
| `SharpAbp.Abp.IdentityServer.Domain.Shared` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.IdentityServer.Domain.Shared.svg)](https://www.nuget.org/packages/SharpAbp.Abp.IdentityServer.Domain.Shared) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.IdentityServer.Domain.Shared.svg)|
| `SharpAbp.Abp.IdentityServer.Domain` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.IdentityServer.Domain.svg)](https://www.nuget.org/packages/SharpAbp.Abp.IdentityServer.Domain) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.IdentityServer.Domain.svg)|
| `SharpAbp.Abp.IdentityServer.Application.Contracts` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.IdentityServer.Application.Contracts.svg)](https://www.nuget.org/packages/SharpAbp.Abp.IdentityServer.Application.Contracts) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.IdentityServer.Application.Contracts.svg)|
| `SharpAbp.Abp.IdentityServer.Application` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.IdentityServer.Application.svg)](https://www.nuget.org/packages/SharpAbp.Abp.IdentityServer.Application) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.IdentityServer.Application.svg)|
| `SharpAbp.Abp.IdentityServer.HttpApi` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.IdentityServer.HttpApi.svg)](https://www.nuget.org/packages/SharpAbp.Abp.IdentityServer.HttpApi) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.IdentityServer.HttpApi.svg)|
| `SharpAbp.Abp.IdentityServer.HttpApi.Client` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.IdentityServer.HttpApi.Client.svg)](https://www.nuget.org/packages/SharpAbp.Abp.IdentityServer.HttpApi.Client) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.IdentityServer.HttpApi.Client.svg)|
| `SharpAbp.Abp.IdentityServer.Extensions` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.IdentityServer.Extensions.svg)](https://www.nuget.org/packages/SharpAbp.Abp.IdentityServer.Extensions) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.IdentityServer.Extensions.svg)|
| **OpenIddict** | - | - |
| `SharpAbp.Abp.OpenIddict.Domain.Shared` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.OpenIddict.Domain.Shared.svg)](https://www.nuget.org/packages/SharpAbp.Abp.OpenIddict.Domain.Shared) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.OpenIddict.Domain.Shared.svg)|
| `SharpAbp.Abp.OpenIddict.Domain` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.OpenIddict.Domain.svg)](https://www.nuget.org/packages/SharpAbp.Abp.OpenIddict.Domain) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.OpenIddict.Domain.svg)|
| `SharpAbp.Abp.OpenIddict.Application.Contracts` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.OpenIddict.Application.Contracts.svg)](https://www.nuget.org/packages/SharpAbp.Abp.OpenIddict.Application.Contracts) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.OpenIddict.Application.Contracts.svg)|
| `SharpAbp.Abp.OpenIddict.Application` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.OpenIddict.Application.svg)](https://www.nuget.org/packages/SharpAbp.Abp.OpenIddict.Application) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.OpenIddict.Application.svg)|
| `SharpAbp.Abp.OpenIddict.HttpApi` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.OpenIddict.HttpApi.svg)](https://www.nuget.org/packages/SharpAbp.Abp.OpenIddict.HttpApi) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.OpenIddict.HttpApi.svg)|
| `SharpAbp.Abp.OpenIddict.HttpApi.Client` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.OpenIddict.HttpApi.Client.svg)](https://www.nuget.org/packages/SharpAbp.Abp.OpenIddict.HttpApi.Client) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.OpenIddict.HttpApi.Client.svg)|
| **AuditLogging** | - | - |
| `SharpAbp.Abp.AuditLogging.Domain.Shared` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.AuditLogging.Domain.Shared.svg)](https://www.nuget.org/packages/SharpAbp.Abp.AuditLogging.Domain.Shared) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.AuditLogging.Domain.Shared.svg)|
| `SharpAbp.Abp.AuditLogging.Domain` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.AuditLogging.Domain.svg)](https://www.nuget.org/packages/SharpAbp.Abp.AuditLogging.Domain) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.AuditLogging.Domain.svg)|
| `SharpAbp.Abp.AuditLogging.Application.Contracts` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.AuditLogging.Application.Contracts.svg)](https://www.nuget.org/packages/SharpAbp.Abp.AuditLogging.Application.Contracts) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.AuditLogging.Application.Contracts.svg)|
| `SharpAbp.Abp.AuditLogging.Application` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.AuditLogging.Application.svg)](https://www.nuget.org/packages/SharpAbp.Abp.AuditLogging.Application) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.AuditLogging.Application.svg)|
| `SharpAbp.Abp.AuditLogging.HttpApi` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.AuditLogging.HttpApi.svg)](https://www.nuget.org/packages/SharpAbp.Abp.AuditLogging.HttpApi) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.AuditLogging.HttpApi.svg)|
| `SharpAbp.Abp.AuditLogging.HttpApi.Client` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.AuditLogging.HttpApi.Client.svg)](https://www.nuget.org/packages/SharpAbp.Abp.AuditLogging.HttpApi.Client) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.AuditLogging.HttpApi.Client.svg)|
| **Account** | - | - |
| `SharpAbp.Abp.Account.Application.Contracts` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.Account.Application.Contracts.svg)](https://www.nuget.org/packages/SharpAbp.Abp.Account.Application.Contracts) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.Account.Application.Contracts.svg)|
| `SharpAbp.Abp.Account.Application` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.Account.Application.svg)](https://www.nuget.org/packages/SharpAbp.Abp.Account.Application) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.Account.Application.svg)|
| `SharpAbp.Abp.Account.HttpApi` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.Account.HttpApi.svg)](https://www.nuget.org/packages/SharpAbp.Abp.Account.HttpApi) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.Account.HttpApi.svg)|
| `SharpAbp.Abp.Account.HttpApi.Client` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.Account.HttpApi.Client.svg)](https://www.nuget.org/packages/SharpAbp.Abp.Account.HttpApi.Client) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.Account.HttpApi.Client.svg)|
| `SharpAbp.Abp.Account.Web` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.Account.Web.svg)](https://www.nuget.org/packages/SharpAbp.Abp.Account.Web) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.Account.Web.svg)|
| `SharpAbp.Abp.Account.Web.IdentityServer` | [![NuGet](https://img.shields.io/nuget/v/SharpAbp.Abp.Account.Web.IdentityServer.svg)](https://www.nuget.org/packages/SharpAbp.Abp.Account.Web.IdentityServer) |![NuGet](https://img.shields.io/nuget/dt/SharpAbp.Abp.Account.Web.IdentityServer.svg)|
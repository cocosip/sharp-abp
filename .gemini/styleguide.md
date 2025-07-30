<!--
This file is intended to be used by the Gemini Code Assist extension.
It contains project-specific coding conventions and style guides.
-->

# Sharp-ABP Style Guide

## General Conventions

- **Adhere to Existing Conventions**: Strictly follow the existing code style, naming conventions, and architectural patterns in the project.

## Project & File Structure

- **File Location**:
    - New core framework extensions should be placed in the appropriate subdirectory under `/framework/src`.
    - New feature modules (especially those with database persistence) should have a new subdirectory created under `/modules`.
- **Module Naming & Layering**: Modules created under the `/modules` directory must adhere to the official ABP Framework's module layering conventions for project and namespace naming. For example, for a new feature named "Blogging", the projects should be named following this pattern:
    - `SharpAbp.Abp.Blogging.Domain`
    - `SharpAbp.Abp.Blogging.Domain.Shared`
    - `SharpAbp.Abp.Blogging.Application.Contracts`
    - `SharpAbp.Abp.Blogging.Application`
    - `SharpAbp.Abp.Blogging.EntityFrameworkCore`
    - `SharpAbp.Abp.Blogging.HttpApi`
    - `SharpAbp.Abp.Blogging.Web`

## Dependency Management

- **Central Package Management (CPM)**: This project uses .NET's Central Package Management (CPM) feature.
    - **All** `PackageVersion` definitions must be located in the `Directory.Packages.props` file at the root of the repository.
    - Individual `.csproj` files should use `<PackageReference Include="PackageName" />` without the `Version` attribute.
    - When adding a new package, first add `<PackageVersion Include="PackageName" Version="1.2.3" />` to `Directory.Packages.props`, and then add the `<PackageReference>` to the relevant `.csproj` file(s).
- **Package Version Constraints**:
    - The versions of `AWSSDK.S3` and `AWSSDK.SecurityToken` packages must not be updated beyond `3.7.300`. Do not update them to major version 4 or higher.
- **Minimal Dependencies**: Individual modules should follow the principle of minimal dependencies, minimizing reliance on external libraries. New third-party libraries should only be introduced if the functionality is very complex and necessary.

## Coding Standards

- **Async Method Signatures**: All asynchronous methods must follow a strict signature convention:
    - The method name must end with the `Async` suffix.
    - The method must accept a `CancellationToken` as its last parameter.
    - The `CancellationToken` parameter must have a default value of `default`.
    - **Correct Example**: `Task<MyResult> DoSomethingAsync(string input, CancellationToken cancellationToken = default);`
- **Async/Await**: In library code (`/framework` and `/modules`), always use `.ConfigureAwait(false)` on `await`ed task calls.

## Testing & Documentation

- **Testing**:
    - All new features or bug fixes must include corresponding unit tests.
    - Tests should be placed in the corresponding test project and mirror the source directory structure (e.g., code in `framework/src/SharpAbp.Abp.Core` should have tests in `framework/test/SharpAbp.Abp.Core.Tests`).
- **API Documentation**: Add XML documentation comments (`<summary>`, `<param>`, `<returns>`) for all new public classes, methods, and properties.

# Project Workflow

## Project Identity

- Project name: sharp-abp
- System name: codex-mem
- Default memory scope: current project

## Memory Rules

- At the start of a fresh session in this repository, call `memory_bootstrap_session`.
- Save a memory note when work produces a lasting decision, bugfix insight, reusable discovery, or durable implementation constraint.
- Save a handoff before pausing, switching tasks, or ending the session.

## Related Project Policy

- Related-project memory is disabled by default for this repository unless explicitly needed for an integration task.
- Re-enable related-project memory only for explicit cross-repository work such as shared contracts, package publishing coordination, or deployment integration.
- Do not pull memory from unrelated projects by default.

## Preferred Tags

- Use tags where useful, especially: `dotnet`, `abp`, `opentelemetry`

## Project-Specific Notes

- This repository is a modular .NET codebase built around ABP vNext module conventions.
- Main library code lives under `framework/src`; tests live under `framework/test`.
- OpenTelemetry, storage, event bus, database, and other infrastructure capabilities are split into separate packages/modules under `framework/src/SharpAbp.Abp.*`.
- Preserve ABP module patterns: prefer `DependsOn`, options classes, `PreConfigure`/`Configure`, and module lifecycle methods instead of ad hoc startup wiring.
- Prefer repository-wide package version management through `Directory.Packages.props`; avoid hardcoding package versions inside individual project files unless the repository already requires it.
- When changing infrastructure modules, check whether matching abstractions, exporters, docs, and sample/test coverage should move together.
- Be careful with dirty worktrees. Do not revert unrelated local changes.
- For OpenTelemetry work in this repo, treat Collector support as a named OTLP export path unless the task explicitly requires a separate transport or deployment abstraction.

## System Relationships

- This repository belongs to system: codex-mem
- No related repositories are currently declared for default memory expansion.
- Use related-project memory only when the current task explicitly depends on another repository.

## Cross-Repo Memory Rules

- Prefer current-project memory first.
- Expand to related repositories only for integration-relevant work.
- When using related-project memory, mention the source repository explicitly in your reasoning and outputs.

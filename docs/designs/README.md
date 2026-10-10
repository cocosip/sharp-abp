# Feature Designs and Implementation Plans

This directory contains research, design specifications, and implementation plans for all SharpAbp features and modules. It is a shared documentation area for current and future work, including new modules and changes to existing modules.

Documentation for implemented functionality belongs in the `docs` root directory or the relevant module's existing README. Documents here describe proposed behavior, implementation decisions, and development tasks; their API examples are not evidence that a feature has been implemented or released.

## Directory Organization

Create one subdirectory per feature or related group of modules, using a descriptive lowercase name with hyphens:

```text
docs/
  Core.md                              # Documentation for implemented functionality
  designs/
    README.md                          # Shared conventions and feature index
    <feature-or-module>/
      design.md                        # Research, scope, behavior, and architecture
      implementation-plan.md           # Tasks, contracts, tests, and acceptance criteria
```

When one design covers multiple independent modules, keep the shared design in the same feature directory and create a separate `<module>-implementation-plan.md` for each module. Place supporting research or decision records alongside that feature's design when needed. Each feature owns its documents; this README defines conventions and links to them.

## Language and Document Contents

Write documentation, headings, explanations, and example comments in English. Preserve actual API identifiers, package names, paths, and commands.

Each design must identify its date and status, affected modules, goals and exclusions, dependency boundaries, public behavior, version and branch constraints, acceptance criteria, and relevant sources. Distinguish verified facts from design choices and unresolved questions.

Each implementation plan must link to its design and identify the files to change, public contracts, ordered tasks, tests, and completion criteria. Use unchecked tasks for planned work; mark tasks complete only after implementation and verification. Record actual dependency versions and verification results during development.

## Status and Lifecycle

Track status per feature and per document rather than assigning one status to this entire directory:

| Status | Meaning |
| --- | --- |
| Draft | The document is being prepared or reviewed; its decisions may change. |
| Approved | The design or plan has been accepted for implementation. |
| In Progress | Implementation has started; completed tasks and validation results are recorded in the plan. |
| Implemented | The agreed scope and acceptance checks are complete. Link to the corresponding usage documentation and implementation evidence. |
| Superseded | A newer design replaces this document. Link to the replacement and preserve the earlier decision record. |

Keep the document header and this index consistent when status changes. A written plan does not itself authorize implementation, establish test success, or indicate a released package.

After implementing a feature, add or update its usage documentation outside `designs`, describing only the supported and verified behavior. Retain the design and plan as development records, link them to the usage documentation, and explain any material differences between the proposal and final implementation. Do not move an unfinished proposal into the usage documentation area.

## Feature Index

Add a row here whenever a new feature directory is created. The index is intended to grow as more modules are designed and implemented.

| Feature or Module Group | Design | Implementation Plans | Status |
| --- | --- | --- | --- |
| Concurrency limiting and operation rate limiting | [Design](concurrency-and-operation-rate-limiting/design.md), [Validation](concurrency-and-operation-rate-limiting/validation.md) | [ConcurrencyLimiting](concurrency-and-operation-rate-limiting/concurrency-limiting-implementation-plan.md), [OperationRateLimiting](concurrency-and-operation-rate-limiting/operation-rate-limiting-implementation-plan.md) | In Progress: net9 core and .NET 10 compatibility verified; distributed acceptance and dev integration pending |

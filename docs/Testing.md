# Testing and coverage

Tests follow the ABP [unit testing](https://abp.io/docs/latest/testing/unit-tests) and
[integration testing](https://abp.io/docs/latest/testing/integration-tests) guidance.

## Test structure

- Test pure domain objects, validators and configuration helpers directly with xUnit and
  Arrange/Act/Assert. Use literal expected values and assert observable behavior.
- For module registration, application services, repositories, mapping and units of work,
  use the existing `AbpIntegratedTest<TModule>` test base and resolve services through
  `GetRequiredService<T>()`. Keep domain/application tests next to the owning module.
- Each relational test application creates an isolated in-memory SQLite database and
  closes its keeper connection during shutdown. Configure the ABP SQLite provider so
  contexts participate in ABP units of work. Test transactions against SQLite; EF's
  InMemory provider is suitable for isolated model inspection, not relational acceptance.
- Keep external dependencies at their boundaries. Storage SDK clients, HTTP handlers,
  email senders and sign-in boundaries can be substituted while exercising the real
  SharpAbp service. Use the repository's existing mocking library. These tests do not
  establish live cloud, broker, identity-provider or database compatibility.
- Mapping tests exercise both overloads, nested values and ABP extra-property definition
  checks. Preserve protected entity identity when mapping into existing objects.
- Dispose test applications, connections, streams, listeners and subscriptions. Avoid
  shared mutable state, timing sleeps, swallowed exceptions and getter-only coverage tests.

The `samples` projects have been removed. Behavioral examples belong in focused tests;
consumer setup instructions belong in the relevant module documentation.

## Running tests

Use the .NET 10 SDK, PowerShell 7 and Python 3. Run commands from the repository root.
In Codex, run builds and tests outside the sandbox as required by `AGENTS.md`.

```powershell
# Focused tests before a full run.
dotnet test modules/account/test/SharpAbp.Abp.Account.Application.Tests/SharpAbp.Abp.Account.Application.Tests.csproj -c Release

# Build every solution, run every test project, and enforce repository line coverage >= 85%.
pwsh -File scripts/Test-Coverage.ps1

# The same check after all solutions have already been built.
pwsh -File scripts/Test-Coverage.ps1 -NoBuild

# Record an initial baseline without enforcing the percentage threshold.
pwsh -File scripts/Test-Coverage.ps1 -ReportOnly

# Verify the report-merging logic independently.
python -m unittest discover -s scripts -p test_coverage_report.py -v
```

`-ReportOnly` still fails if a test project fails or produces no coverage report.
The default results folder is new for each run. An explicitly selected
`-ResultsDirectory` must be empty; old runs cannot silently add hits to a new run.

## Coverage accounting

`common.test.props` supplies the centrally versioned Coverlet collector to every test
project. `coverage.runsettings` includes all SharpAbp production assemblies, including
auto-properties and generated Razor/mapping source where the collector exposes sequence
points. Only test assemblies are filtered out.

The runner builds the solutions listed in `solutions.txt`, discovers all test projects
under `framework/test` and `modules/*/test`, and records TRX, Coverlet JSON and Cobertura
reports. It also copies each production DLL/PDB to a temporary inventory directory and
instruments that directory once. Production assemblies not referenced by any test therefore
contribute zero hits rather than disappearing from the denominator. Assemblies with no
executable source locations are identified from their portable PDBs and listed explicitly.

`scripts/coverage_report.py` merges complementary hits by assembly, source file and line;
it unions branch hits by method and branch location. It does not average suite percentages
or count the same shared source twice. The output `summary.md` and `summary.json` list every
production project, line coverage and branch coverage. A missing project fails the check.
The 85% threshold applies to the repository's unrounded line percentage; individual module
percentages remain visible, including modules below the target. This is coverage of local
automated tests, including ABP/SQLite and loopback integration tests.

CI runs the same check after building all solutions and uploads summaries, Cobertura and
TRX reports even when validation fails. Live infrastructure acceptance and platform coverage
remain separate evidence; skips do not count as successful acceptance.

`Directory.Build.props` keeps out-of-solution project references in the requested
configuration. This prevents module Release builds from silently loading Debug framework
dependencies and mixing different optimization settings in the same coverage run. The
file-storing management suite includes a build-artifact regression check for this behavior.

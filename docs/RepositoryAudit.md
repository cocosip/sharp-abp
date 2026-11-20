# Repository audit — 2026-11-20

## Scope and evidence

SharpAbp supplies reusable ABP infrastructure packages and business modules. This audit
assumes concurrent requests, multiple application instances, tenant isolation, and shared
databases/caches. Process-local object pools and limiters are evaluated as process-local
components, not as distributed coordination mechanisms.

The starting checkout was clean on `opt`, at `11bfc94e`. The structural inventory included
152 production projects (75 framework projects and 77 projects across 11 business modules),
38 test projects, solution/project references, central package settings, CI, build/coverage
scripts, and README/documentation links. Searches covered the production source tree;
manual tracing concentrated on authentication, secret handling, storage, messaging,
identifier allocation, tenant resolution, connection management, and shared resource
lifetimes. This is not a claim that every source line has been manually reviewed or that
every defect has been found.

## Changes applied

Focused regressions were run before production fixes. Public DTO shapes, dependency
versions, and protected extension points were preserved.

| Area | Confirmed problem and resulting behavior |
| --- | --- |
| Account registration | Registration used an unchecked redirect and dropped the return fragment. It now uses the shared safe redirect path. |
| Account redirect fragments | The upstream URL helper validates the return URL before appending the fragment. A suffix such as `/attacker.example` could turn `/` into an external scheme-relative URL. The shared account page model now gives nonempty fragments a `#` prefix. |
| External login | A single external provider with local login disabled threw instead of challenging that provider. The existing challenge handler is now reused; registration fallback also retains the fragment. |
| Storage client cache | A failed old initialization or stale sweep could remove a replacement cache entry. Removal now matches the original key and entry. Clients finishing creation after shutdown and clients retired during shutdown are released once. |
| FASTER logger cache | A late initialization failure could remove a replacement logger entry. Failed initialization now removes only the original matching entry. |
| MassTransit event handlers | Subscription changes were not locked consistently, and dispatch received mutable handler lists. Subscription updates now share the removal lock and dispatch receives a snapshot. |
| MassTransit deferred publishing | Dynamic events lost `useOutbox: false` when deferred to a unit of work. The deferred record now retains the supplied flag. |
| Transform-security persistence | Database storage discarded RSA/SM2 extension settings, so subsequent reads used current global defaults. Settings now round-trip through the entity's existing extra properties. Unknown identifiers return null, matching `ISecurityCredentialStore` and the validation service's `NotFound` result. |
| MinId token diagnostics | Token values appeared in domain logs, validation errors and invalid-token exceptions. Diagnostics retain business type/record information without copying credentials. The throwing validator shares the boolean validator's actual token check. |
| MinId cache invalidation | Token changes evicted entries before the enclosing transaction committed. Eviction now participates in the ABP unit of work and happens after successful completion; rollback preserves the existing cache. R5 below also removes positive-cache dependence from validation. |
| MinId loading and allocation | Waiting callers could discard a segment loaded by another caller. Loading now checks availability again inside the semaphore. Database range allocation rejects arithmetic overflow before mutation or persistence. Terminal segment increments and remainder alignment no longer wrap beyond `long.MaxValue`; concurrent callers claim the terminal IDs once. |
| Snowflake | Invalid bit allocations and timestamps outside the configured representable range could silently produce invalid/repeated IDs. They now fail explicitly; ordinary configurations retain their ID format. |
| OpenIddict client updates | Changing a confidential client to public retained its secret and failed upstream validation. Public clients now clear it; blank confidential-client updates still preserve the existing secret. |
| OpenIddict deletion | Direct repository deletion left a client in the manager's cache. Deletion now goes through the application manager. |
| Sensitive audit inputs | Secret properties and primitive parameters are excluded using ABP's `DisableAuditing`, retaining business metadata and audit actions. Tests exercise ABP's real audit serializer/helper. |
| Repository cleanup | Removed the unreferenced, projectless `MinIdAppDbMigrationService` left behind by sample removal (221 lines). README counts now match 75 framework packages, 11 business modules and 9 storage providers; the nonexistent database storage provider was removed from the advertised list. |

The changes do not recover historical encryption settings that were never stored. Existing
credentials without stored settings still use the established fallback behavior.

## Follow-up decisions and fixes

The user approved R1 and R3–R9 for implementation and explicitly deferred R2. Private keys
continue to use the current database-backed storage; no external environment, master key,
KMS integration or encryption migration was introduced.

### R1 — Applied: external sign-in and registration checks

External identity is matched by provider and subject. An email claim cannot link an
existing account automatically. Existing users must authenticate through an established
method and use their host application's explicit account-linking flow. Normal Identity
confirmation, lockout and two-factor checks run for external login and both registration
paths. The default 2FA continuation refuses authentication and returns to login; hosts may
override the preserved hooks to provide their own continuation. Rejected registration
sign-in still preserves the newly created account and provider link.

See [Account migration](BusinessModules.md#account). Regressions include the real
SignInManager, SQLite persistence across completed units of work, app-cookie denial,
provider/subject identity, lockout, confirmation and two-factor cookies.

### R2 — Deferred by user: database-backed private key storage

[DefaultKeyService](../modules/crypto-vault/src/SharpAbp.Abp.CryptoVault.Domain/SharpAbp/Abp/CryptoVault/DefaultKeyService.cs)
still stores encrypted keys, their passphrase and salt in database records. This does not
protect private keys against disclosure of those records. The user chose to retain the
current storage because no external key environment is available. Input audit redaction
remains in place. External wrapping keys, rotation and legacy-key migration are deferred.

### R3 — Applied: staged filesystem writes and atomic publication

[FileSystemFileProvider](../framework/src/SharpAbp.Abp.FileStoring.FileSystem/SharpAbp/Abp/FileStoring/FileSystem/FileSystemFileProvider.cs)
copies into a random sibling temporary file, flushes/closes it, then publishes through move
or replacement. It never deletes an existing destination to make replacement possible.
Failed/cancelled copies preserve the old bytes, unsuccessful new files are not published,
competing non-overwrite saves preserve the winning file, and seekable retries return to the
original position. Cleanup failures are logged without hiding the original copy failure.

See [filesystem publication and staging rules](FileStorage.md). Static servers must deny
staging-file paths and directory listing. Windows metadata behavior is tested; Linux,
network-filesystem behavior and power-loss/crash durability are not established here.

### R4 — Applied: credentials moved out of URLs

MinId generation reads an opaque credential from `Authorization: Bearer <minid-token>`.
Query-token inputs are rejected. Administrative token lookup/filtering uses JSON request
bodies, retaining the administrator's normal user authorization header. The old token-bearing
GET lookup route was removed; `FindAsync(FindMinIdTokenDto)` supports updated remote .NET
clients while the old `FindByTokenAsync` remains local only. Tenant-group and hybrid-tenant
connection-string updates bind JSON string bodies while retaining C# method signatures.

See [MinId HTTP migration](BusinessModules.md#credential-transport-and-limits) and
[tenant connection-string updates](MultiTenancy.md). These transport changes require caller
migration before deployment. Host proxy/APM/access logs must also redact credentials and
request bodies; application audit attributes cannot rewrite already logged request URLs.

### R5 — Applied: validation reads persisted token state

[MinIdTokenValidator](../modules/minid/src/SharpAbp.MinId.Application/SharpAbp/MinId/MinIdTokenValidator.cs)
no longer trusts or fills positive token-cache entries. Every check queries persisted state,
so a late cache writer cannot restore a revoked token. Legacy invalidations still occur after
successful commit for existing cache consumers. This costs one authoritative database lookup
per validation. Previously authorized in-flight requests may finish; hosts must use their
primary database and avoid long-lived snapshot transactions for authentication. Upgrade all
instances to eliminate the old implementation's cached acceptance window.

A regression writes a matching stale positive entry after actual token deletion and verifies
that validation still fails. This is ABP/SQLite evidence, not distributed real-server acceptance.

### R6 — Applied: bounded ID batches

The application service and domain generator both reject counts outside 1–1,000 before
allocating IDs. HTTP parameter validation rejects unsupported counts as bad requests.
There is no silent adjustment of zero/negative input. The constant is shared through
`MinIdUtil.MaxBatchSize`. Boundary and allocation-avoidance regressions exercise these paths.

### R7 — Applied: stable errors and safe diagnostics

The existing `MinIdResponse` envelope remains. Invalid credentials and invalid counts have
stable messages. Unexpected failures expose a reference ID; server diagnostics retain that
reference, exception type and stack without logging arbitrary exception messages that may
contain credentials. Backend message text is no longer copied into the segment-loading
wrapper. Cancellation propagates instead of becoming a failure envelope. Hosts must keep
production ABP detailed exception responses disabled.

Tests exercise actual loopback HTTP/MVC routing, binding, JSON responses and captured logger
calls; they do not establish behavior of a separately deployed reverse proxy or database.

### R8 — Applied: Redis factory ownership and single initialization

[DefaultRedisClientFactory](../framework/src/SharpAbp.Abp.FreeRedis/SharpAbp/Abp/FreeRedis/DefaultRedisClientFactory.cs)
owns its shared clients. Lazy initialization prevents competing first requests from building
multiple pools for one name, exact-entry removal permits failed initialization to retry,
and terminal disposal rejects new access and accounts for pending creation. Callers must
not dispose a shared client obtained from the factory. Client construction also releases a
partially initialized client if a configuration callback fails.

### R9 — Applied: explicit pool/logger shutdown and complete cleanup

Object-pool and FASTER factories now distinguish terminal `Dispose` from reusable removal.
Pending creation/removal is coordinated; external creation and cleanup work runs outside
owner lifecycle locks. Cleanup proceeds across failures and surfaces aggregated errors.
Late async pool items and incompletely initialized FASTER loggers are released. The
file-client cache also continues cleanup after one client throws.

Lifecycle mutation from the same owner's active initialization callback is rejected before
state changes; normal cleanup callbacks may access reusable owners. The active-callback
scope ends for inherited background contexts when initialization completes. See
[resource ownership](Core.md) and the [FASTER README](../framework/src/SharpAbp.Abp.Faster/README.md).
Deterministic tests cover races, callback reentrancy, async cleanup, errors, retry and disposal.

### Build target correction

`Directory.Build.props` now defines `NetCoreFx` before expanding `ProjectFrameworks`, so the
intended native `net10.0` target is actually included alongside .NET Standard 2.0/2.1.
Newly exposed nullability errors are fixed with nullable lookup/removal annotations and
non-null dictionary key constraints. Invalid types without a full name fail explicitly;
no package/dependency baseline was upgraded or warning suppressed.

## Validation

The figures below record the final combined audit and approved follow-up fixes.

The final Release build passed all 12 solutions from `solutions.txt` with zero compiler
warnings and errors. `scripts/update-readme-packages.ps1 -Check`, project-reference/document
file-link checks, `git diff --check`, and all five coverage-report Python tests passed.

All 190 projects' restored dependency assets were checked against nuget.org's official
vulnerability base/update feeds dated 2026-11-20. The check used NuGet's own version-range
matcher and found no matching published advisories among 489 distinct package/version pairs.
This records known-advisory results, not a guarantee that dependencies contain no defects.
The CLI check independently covered 101 framework projects; some module CLI queries could
not read their settings, so the complete dependency check used the restored assets instead.
The resulting inventory and feed metadata are in `artifacts/audit/dependency-advisories.json`.

The final `scripts/Test-Coverage.ps1 -NoBuild` run passed:

| Check | Result |
| --- | --- |
| Test projects | 38 completed |
| Tests | 2,242 passed, 0 failed, 1 skipped (2,243 discovered) |
| Production inventory | All 152 projects measured/accounted for |
| Repository line coverage | 19,159 / 21,980 = 87.17%; 85% gate passed |
| Branch coverage | 4,190 / 5,784 = 72.44%; not a configured gate |
| Assemblies individually at the line target | 110 / 152; 42 remain below the target |

The skipped test is
`OperationRateLimitingRedisAcceptanceTest.Two_processes_share_redis_cache_lock_admission_and_reset`:
`SHARP_ABP_OPERATION_REDIS` was not configured. This is an explicit missing real-service
acceptance result. The repository gate is aggregate coverage; it does not certify every
assembly or branch independently.

Combined final build/test logs are in the ignored `artifacts/audit-hardening` directory;
per-suite TRX and coverage reports are in `artifacts/coverage/audit-hardening-20261120`.

Independent review checked the account, MinId, Snowflake, transform-security and OpenIddict
changes against their owning ABP/OpenIddict implementations. It caught the redirect-fragment
bypass and the related terminal-ID boundary, which were followed up rather than treating
the initial passing tests as complete evidence. Follow-up independent review also caught
an initializer/cleanup deadlock, reentrant shutdown state corruption, external registration
bypassing the self-registration setting, and wrapped cancellation in the real ID generator.
Focused regressions reproduced these failures before their fixes. Both follow-up review
scopes closed without outstanding introduced blockers after the corrections.

## Limits

No push, release, dependency upgrade, schema migration or change to `dev`/`master`/`net9`
was performed. Verification is local Windows/.NET testing, including ABP/SQLite integration
where supplied by existing test bases. It does not establish Linux CI, real OAuth browser
flows, cloud storage, Redis, broker, external database, cross-process revocation, or crash
durability acceptance. Public extension points were not removed merely because the repository
has no direct caller. Fixed cache grace periods also remain time-based rather than tracking
active storage operations; long-running operations need separate lifetime acceptance.

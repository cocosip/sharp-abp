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

## Follow-up audit — 2026-11-21

Reviewed `maintenance/code-quality` starting at `0f7720f6`, including the preceding
hardening changes. The inventory reconciled 12 solutions, 152 production projects,
38 test projects, and their checked-in project references and documentation file links.
The source review traced shared request, persistence, tenancy, cache, connection,
allocation, and storage paths across the framework and 11 business modules. The operating
assumptions were concurrent requests, multiple application instances, and shared storage
and databases. Vendor-specific code and generated artifacts received targeted checks.

### Changes applied

| Area | Correction |
| --- | --- |
| File downloads | On Windows the shared downloader stages bytes beside the destination and closes the stage before publication. Failed or canceled copies preserve existing content. |
| AWS and S3 | Failed multipart uploads use an independent 10-second abort token, so request cancellation still allows cleanup. |
| FastDFS | The configuration factory publishes an entry after client registration succeeds. Later calls can retry a failed registration. |
| Identity | Locking, unlocking, and changing two-factor authentication now report failed Identity operations to callers. |
| Transform-security management | Credential generation forwards the caller's cancellation token to persistence. |
| MinId | Requests also validate the remainder against `Delta`. The generator serves reserved IDs before attempting the next allocation when another full segment would overflow. |
| ASP.NET Core | Response headers, tenant grouping, and transform-security middleware honor their own `IsEnabled` setting. Adding `UseResponseHeader()` applies configured headers by default. |
| Database connections | The name-based factory disposes explicitly constructed creators and supports async disposal for its child scope. A connection returned successfully belongs to its caller. |
| Release workflow | Bash reads the release tag and version from environment variables. The version and checkout gates still apply. |
| Cleanup | Removed an unused private FrontHost path helper. README provider summaries now include OceanBase. |

### Findings for review

| ID | Observed behavior and decision |
| --- | --- |
| Q1 | **Tenant file configuration can stay stale.** A tenant lookup caches host fallback under the requesting tenant's key. Host events evict only the host key, and repeated reads extend the tenant copy's one-hour sliding expiry. Tenant creation and a rename into an already cached name also need an invalidation policy. Resolve fallback through a shared host cache entry and decide how to retire entries from mixed-version deployments. [FileStoringContainerCacheManager.cs](../modules/file-storing-management/src/SharpAbp.Abp.FileStoringManagement.Domain/SharpAbp/Abp/FileStoringManagement/FileStoringContainerCacheManager.cs) [FileStoringHandler.cs](../modules/file-storing-management/src/SharpAbp.Abp.FileStoringManagement.Domain/SharpAbp/Abp/FileStoringManagement/FileStoringHandler.cs) |
| Q2 | **Tenant group membership is ambiguous.** The app service's available-tenant query assumes one group per tenant. The write check and database unique index enforce one row per tenant/group pair. Decide how to resolve existing duplicates before enforcing uniqueness per tenant. [TenantGroupManager.cs](../modules/tenant-group-management/src/SharpAbp.Abp.TenantGroupManagement.Domain/SharpAbp/Abp/TenantGroupManagement/TenantGroupManager.cs) |
| Q3 | **Grouped tenant database routing differs by overload.** Resolving the default name for a tenant in a group with no default group string falls through to the host default. Named lookups can fall back to the tenant resolver. Agree on the precedence for group, tenant, and host defaults before changing where existing deployments connect. [TenantGroupConnectionStringResolver.cs](../framework/src/SharpAbp.Abp.TenancyGrouping/SharpAbp/Abp/TenancyGrouping/TenantGroupConnectionStringResolver.cs) |
| Q4 | **MinId configuration changes can reuse ID ranges.** Updating a business type can lower its persisted `MaxId` while processes still hold reserved IDs. Deleting and recreating the same name has the same coordination concern. Decide whether configuration is monotonic for the lifetime of a name and how to handle an intentional reset. [MinIdInfoManager.cs](../modules/minid/src/SharpAbp.MinId.Domain/SharpAbp/MinId/MinIdInfoManager.cs) [MinIdInfoAppService.cs](../modules/minid/src/SharpAbp.MinId.Application/SharpAbp/MinId/MinIdInfoAppService.cs) |
| Q5 | **Cloud saves can overwrite despite a no-overwrite request.** Several providers check existence in a separate request before uploading. Two writers can both observe an absent object. Use backend preconditions where available and define how adapters expose unsupported semantics, including multipart completion. [AwsFileProvider.cs](../framework/src/SharpAbp.Abp.FileStoring.Aws/SharpAbp/Abp/FileStoring/Aws/AwsFileProvider.cs) [S3FileProvider.cs](../framework/src/SharpAbp.Abp.FileStoring.S3/SharpAbp/Abp/FileStoring/S3/S3FileProvider.cs) |
| Q6 | **Azure and FastDFS downloads use separate write paths.** Their SDK-based paths bypass the shared staging fix. Preserve the SDK-specific behavior while staging locally, then verify replacement on supported platforms. FastDFS also accepts signed-URL settings for explicit expiry and existence checks without applying them, so the supported URL contract needs a decision. [AzureFileProvider.cs](../framework/src/SharpAbp.Abp.FileStoring.Azure/SharpAbp/Abp/FileStoring/Azure/AzureFileProvider.cs) [FastDFSFileProvider.cs](../framework/src/SharpAbp.Abp.FileStoring.FastDFS/SharpAbp/Abp/FileStoring/FastDFS/FastDFSFileProvider.cs) |
| Q7 | **FASTER deserialization failure interrupts batch delivery.** The logger dequeues before deserializing. An unreadable entry can discard earlier results in the same batch and leave delivery state incomplete. Choose whether an unreadable entry is retried, quarantined, or explicitly skipped before changing acknowledgement and reclaim behavior. [FasterLogger.cs](../framework/src/SharpAbp.Abp.Faster/SharpAbp/Abp/Faster/FasterLogger.cs) |
| Q8 | **Password-reset links depend on the shape of the configured URL.** Manual query assembly breaks when the reset base already contains a query or a nested return URL contains separators. Confirm how existing base query values and the return fragment should compose, then use framework query construction. [AccountEmailer.cs](../modules/account/src/SharpAbp.Abp.Account.Application/SharpAbp/Abp/Account/Emailing/AccountEmailer.cs) |
| Q9 | **Imported keys can be stored with unusable metadata.** RSA and SM2 import comments promise format and pair checks, while the import actions accept length-valid strings and client-provided size or curve. Decide the accepted key formats and how to treat existing mismatched records. Deleting imported keys can also strand transform-security credentials that reference them, which needs a separate lifecycle policy. [RSACredsAppService.cs](../modules/crypto-vault/src/SharpAbp.Abp.CryptoVault.Application/SharpAbp/Abp/CryptoVault/RSACredsAppService.cs) [SM2CredsAppService.cs](../modules/crypto-vault/src/SharpAbp.Abp.CryptoVault.Application/SharpAbp/Abp/CryptoVault/SM2CredsAppService.cs) [DatabaseSecurityCredentialStore.cs](../modules/transform-security-management/src/SharpAbp.Abp.TransformSecurityManagement.Domain/SharpAbp/Abp/TransformSecurityManagement/DatabaseSecurityCredentialStore.cs) |
| Q10 | **The IdentityModel override misses upstream grant support.** ABP 10.7.1 handles the device-code grant. SharpAbp's override replaces the grant dispatch and omits that case. Review delegation to base for standard grants and check compatibility for subclasses overriding token-request hooks. [AbpIdentityModelAuthenticationService.cs](../framework/src/SharpAbp.Abp.IdentityModel/SharpAbp/Abp/IdentityModel/AbpIdentityModelAuthenticationService.cs) |
| Q11 | **The tenancy grouping opt-out has no runtime reader.** The public ignore attribute exists without middleware metadata handling. Decide whether the opt-out applies to MVC metadata, all endpoint metadata, or both, and where endpoint routing must run. [IgnoreTenancyGroupingAttribute.cs](../framework/src/SharpAbp.Abp.TenancyGrouping.Abstractions/SharpAbp/Abp/TenancyGrouping/IgnoreTenancyGroupingAttribute.cs) |
| Q12 | **OpenIddict extension properties have an incomplete round trip.** The application DTO exposes properties and extra properties that the write path skips. The scope DTO mapping drops stored extra properties. Decide which keys clients may write and how updates merge existing provider-managed properties. [OpenIddictApplicationAppService.cs](../modules/openiddict/src/SharpAbp.Abp.OpenIddict.Application/SharpAbp/Abp/OpenIddict/OpenIddictApplicationAppService.cs) [OpenIddictScopeAppService.cs](../modules/openiddict/src/SharpAbp.Abp.OpenIddict.Application/SharpAbp/Abp/OpenIddict/OpenIddictScopeAppService.cs) |
| Q13 | **Unsupported database provider names can persist.** Management accepts an arbitrary provider string and the runtime resolver parses the enum when it builds a connection. Decide whether persistence requires a known enum value or a provider registered by the host, and how to repair older records. [DatabaseDbConnectionInfoResolver.cs](../modules/dbconnections/src/SharpAbp.Abp.DbConnectionsManagement.Domain/SharpAbp/Abp/DbConnectionsManagement/DatabaseDbConnectionInfoResolver.cs) |
| Q14 | **MinId alignment cost grows with the configured delta.** `SegmentId.Setup` scans candidates under a lock. A large valid delta can turn setup into a long CPU loop. Modular arithmetic can compute the next candidate directly. Define signed-range and legacy-invalid-row behavior before replacing the calculation. [SegmentId.cs](../modules/minid/src/SharpAbp.MinId.Domain/SharpAbp/MinId/SegmentId.cs) |
| Q15 | **The .NET 9 maintenance version differs from the policy text.** The local `net9` ref and `v4.8.0` use 4.8.0, while `AGENTS.md` still says the baseline is the 4.7.x line. Confirm the intended wording across both baselines. The dependency baseline and existing release tags remain as recorded. |
| Q16 | **Provider configuration can pass validation and fail at startup.** Integer settings use a lexical check that accepts values outside the Int32 range. Their readers convert to Int32. Apply range validation to the Int32 fields and preserve the public lexical helper's contract. [ValidateHelper.cs](../framework/src/SharpAbp.Abp.Validation/SharpAbp/Abp/Validation/ValidateHelper.cs) [S3FileProviderValuesValidator.cs](../framework/src/SharpAbp.Abp.FileStoring.S3/SharpAbp/Abp/FileStoring/S3/S3FileProviderValuesValidator.cs) |
| Q17 | **Administrative OpenIddict scope updates can leave stale runtime lookups.** The current write path uses the store directly. A warmed manager lookup can retain an old name after a rename or delete. The upstream manager updates its cache before the outer ABP transaction commits, so routing writes through it directly can expose rolled-back values. Validate writes through the manager and coordinate cache changes with transaction completion and multi-instance invalidation. [OpenIddictScopeAppService.cs](../modules/openiddict/src/SharpAbp.Abp.OpenIddict.Application/SharpAbp/Abp/OpenIddict/OpenIddictScopeAppService.cs) |
| Q18 | **Map tenancy exposes mutable shared cache results.** Refresh clears and repopulates the same list, and lookup methods return shared objects. Concurrent consumers can see the list change during enumeration, and caller mutations affect subsequent results. Define snapshot semantics and update the tests that currently assert shared object identity. [MapTenancyStoreInMemoryCache.cs](../modules/map-tenancy-management/src/SharpAbp.Abp.MapTenancyManagement.Domain/SharpAbp/Abp/MapTenancyManagement/MapTenancyStoreInMemoryCache.cs) |
| Q19 | **Unix downloads still write in place.** Supporting atomic replacement on Unix also requires preserving file ownership and access controls. Swapping the file while copying only the mode bits can change who can read it. Define symlink, hardlink and metadata behavior for all supported runtime targets before switching. [FileProviderBase.cs](../framework/src/SharpAbp.Abp.FileStoring/SharpAbp/Abp/FileStoring/FileProviderBase.cs) |

The earlier private-key storage decision still applies under R2 above. File-client retirement
continues to use a time-based grace period. Multi-instance use needs a decision on tracking
active operations before retiring clients.

### Validation

The final Windows Release run built all 12 solutions with zero warnings and errors.
Across 38 test projects, 2,263 tests passed, none failed, and one real Redis test was
skipped because `SHARP_ABP_OPERATION_REDIS` was unset. Repository line coverage was
87.34% (19,236 of 22,025 lines) across all 152 production projects, passing the 85%
gate. Branch coverage was 72.91%. The run used `scripts/Test-Coverage.ps1` and wrote
reports to `artifacts/coverage/maintenance-code-quality-final-20261121`.

The README package check, Actionlint, five coverage-script unit tests, and
`git diff --check` passed. Local Bash smoke checks exercised the tag version gate,
the checkout mismatch gate, and release-note generation. A concurrent build initially
locked a shared output DLL. The final serial run completed successfully.

The middleware tests exercise their invocation paths. Storage SDK tests use substitutes.
SQLite is used where the existing module tests have a relational fixture. Linux runtime
behavior, real cloud or database servers, external identity providers, and publication to
NuGet.org were outside the validation environment.

## Approved remediation

| Finding | Result |
| --- | --- |
| Q1 | Tenant and host containers use separate cache entries. Tenant lookups fall back to the shared host entry, and committed writes invalidate the affected names. Cache keys use the tenant ID regardless of the ambient unit of work. |
| Q2 | The service rejects a second group membership. The EF model enforces uniqueness on the tenant ID. MongoDB hosts can install the unique membership index after checking for duplicates. |
| Q3 | A group without a default connection string uses the normal tenant-aware connection resolver before falling back to the host. |
| Q4 | Business type names and their allocation watermark survive retirement. Updates reject renames and lower watermarks, and seed initialization counts retired rows. Apply the `IsDeleted` migration before deploying. |
| Q5 | Supported object stores submit a conditional write with the upload. Adapters without verifiable atomic support reject the no-overwrite request. |
| Q6 | Azure and FastDFS publish downloads through the shared staged copy path. FastDFS checks existence when requested. It rejects a caller-specified expiry because the server also applies its own token TTL. |
| Q7 | A corrupt record returns its bytes and address in `UnreadableEntries`. Valid records in the batch remain available for processing and acknowledgement. The existing gap settings control retention until explicit acknowledgement. |
| Q8 | Password reset links preserve unrelated query values and the base fragment. The emailer replaces reserved reset parameters and encodes nested URLs and fragments. |
| Q9 | Imported RSA and SM2 pairs are checked against their declared key metadata. The cohosted transform-security reference check blocks deletion of referenced keys. Quiesce issuance across hosts before key retirement when the stores lack a shared transaction or lock. |
| Q10 | Standard grants use the upstream IdentityModel dispatch, including device authorization. The external credentials grant keeps its existing custom request path. |
| Q11 | The tenant grouping middleware honours the ignore attribute in endpoint metadata when routing runs before it. |
| Q12 | OpenIddict JSON property updates merge supplied keys and retain omitted keys. Extra properties follow ABP's source and destination definitions. |
| Q13 | Connection management validates provider names and stores their canonical spelling. The runtime can still read older defined numeric values. |
| Q14 | MinId aligns in constant time across signed ranges. Invalid persisted alignment settings fail before allocation advances the watermark. |
| Q15 | The branch policy documents .NET 9 and ABP 9.3.x as the `4.x` line, including the existing `4.8.x` releases. |
| Q16 | Storage fields that require an Int32 reject overflow during validation. |
| Q17 | Scope manager reads use the store directly on upgraded hosts. Scope writes use the manager and evict legacy cache entries after commit. Rollback leaves the persisted scope and its legacy cache entries intact. |
| Q18 | Map tenancy publishes whole snapshots and returns detached values. Earlier results can be enumerated independently of a later refresh. |
| Q19 | Downloads stage data beside the destination. Windows publishes with move or replace. Unix publishes through an exclusive hard link on the same filesystem, preserving existing destinations even when another writer creates one during the download. |

Upgrade details are in [Business modules](BusinessModules.md), [Multi-tenancy](MultiTenancy.md),
[File storage](FileStorage.md), and the [FASTER README](../framework/src/SharpAbp.Abp.Faster/README.md).

The integrated Windows Release run built all 12 solutions with zero warnings and errors.
Across 38 test projects, 2,307 tests passed, none failed, and one Redis integration test
was skipped because `SHARP_ABP_OPERATION_REDIS` was unset. Repository line coverage was
87.61% (19,434 of 22,182 lines), passing the 85% gate. Branch coverage was 73.53%.
Results are in `artifacts/coverage/approved-remediation-20261121`.

The local run covers SQLite constraints and rollback, storage SDK request construction,
and Windows filesystem publication. Live MongoDB index installation, multi-instance
coordination, external storage backends, and Unix file semantics remain unverified.

### Pre-commit review

Configured response headers apply by default once the middleware is added, with an
explicit disable option. OpenIddict accepts a null application property bag and
retains stored values. Container reads inside a unit of work publish cache values
only after commit, and entries expire after one hour even if frequently accessed.
Concurrent cross-process fills can still race invalidation within that interval.

The final Windows Release run passed all 12 solution builds with zero warnings and
errors. Across 38 test projects, 2,311 tests passed and one real Redis test was
skipped. Line coverage was 87.58% (19,427 of 22,182 lines). The Unix hard-link
publication path and race regression require a Unix runtime. See
`artifacts/coverage/precommit-final-20261121` for the generated reports.

# Repository audit — 2026-11-20

## Non-storage functional re-review — 2026-11-22

The second pass tests the behavior of the non-storage changes in
`11bfc94e..260fea8c`, including extension hooks and connected consumers. Documenting a
breaking change did not establish that it was necessary. New regressions exercise the
actual failing operation before its correction, rather than accepting yesterday's test
expectations as the contract.

| Area | Reproduced failure and correction |
| --- | --- |
| Object-pool callbacks | A policy factory could no longer retire an unrelated initialized pool. Permit initialized/missing targets; still reject waiting on unfinished pools or shutting down the initializing owner. |
| Object-pool metadata | A protected creation callback ran on cache hits, repeating initialization side effects. Restore cache-miss-only execution outside the registry lock. |
| Async pool shutdown | A synchronously completing creation could block its caller's synchronization context while synchronously disposing an async-only item. Await disposal in the async creation path. |
| OpenIddict deletion | Manager deletion committed an independent transaction, so outer rollback left the application permanently deleted. Use an ambient-aware application store with token cascade and evict the manager cache after commit as well as during deletion. |
| OpenIddict JSON properties | Explicit replacement and empty bags could no longer remove stored keys; scope null input threw. Replace explicitly supplied bags, preserve omitted/null input, and retain trusted additions from protected model-building hooks. DTO dictionaries remain initialized for existing callers. |
| Registration hooks | A legacy override successfully signed in through the real Identity manager, but the page redirected to login because the new result property was unset. Recognize a newly issued application principal when no explicit result was supplied; retain confirmation, lockout and 2FA denial checks. |
| MinId host authentication | MinId credentials consumed the host's bearer header. Support a separate `X-MinId-Token` while retaining bearer-token fallback for existing simple clients. Host authorization remains enforced. |
| MinId clients and implementers | Real ABP dynamic proxies could not find the old lookup method, and custom interface implementations required a new method. Restore the original signature over POST with a JSON string body; offer the DTO convenience call as an extension without adding a required interface member. |
| MinId database matching | An added ordinal business-name check rejected a row that the configured database collation matched. Let the repository determine business-name matching; continue comparing the secret exactly. |
| MinId admin edits | After a real segment reservation, an ordinary edit carrying a previously fetched maximum was rejected. Preserve the greater persisted/requested watermark and apply the other configuration changes. |
| MinId local batches | The remote batch ceiling also rejected trusted in-process calls above 1,000. Keep the application/HTTP limit and restore positive-count domain calls. |
| RSA imports | The vault rejected PKCS#8 and PEM formats exported by the Crypto module itself. Accept the supported formats, preserve the stored strings, and retain key-role, pair, size and complete-payload validation. |
| Database provider inputs | Defined numeric values such as `0` (SQL Server) and `1` (MySQL) were incorrectly rejected. Accept defined enum values and store canonical names; continue rejecting unknown/combined values. |

The connected review also identified two pre-existing defects, separate from yesterday's
regressions: synchronous pool cleanup could deadlock on async-only items, and transform
credentials could not use valid PEM/PKCS#8 vault keys. ABP's context-pumping cleanup bridge
and runtime-only key normalization correct these separately. Already compatible key strings
remain unchanged, and native RSA conversion runs only for formats that require it. These
are not counted as newly introduced defects.

### Functional validation and review limits

The final Windows Release run of `scripts/Test-Coverage.ps1` completed successfully:
all 12 solutions built with zero warnings/errors; 38 projects produced 2,369 passed,
zero failed, and three skipped tests. The skips are the two Unix filesystem cases and
the unconfigured real Redis acceptance test. Repository line coverage is 87.64%
(19,534 of 22,289 lines) across the 152-project inventory. Final evidence is under
`artifacts/coverage/deep-functional-review-20261122`; the build/test log and focused
regressions are under `artifacts/review-2026-11-22-deep` and
`artifacts/regression-review/deep`.

The focused evidence includes real ABP API discovery/dynamic proxies and built-in
protected bearer authentication; a legacy interface implementation compiled without the
new lookup member; real Identity sign-in success/denial paths; actual SQLite transaction
rollback and token cascades; ABP JSON deserialization followed by persisted property
updates; both styles of protected model-building hooks; a real segment reservation
followed by an admin edit; key import/storage/runtime encryption roundtrips; and
deterministic synchronization-context shutdown cases. Failed tests were observed before
the corresponding fixes. A fresh independent source review found no remaining actionable
blocker in the non-storage corrections, and `git diff --check` passed.

FASTER, FreeRedis, MassTransit, IdentityModel, tenant routing and connection creation were
traced through their changed callers and dependency behavior; no additional concrete
regression was confirmed. Their local automated suites passed in the integrated run.
This does not establish external broker/Redis/OAuth/database behavior, distributed cache
coordination, Unix runtime semantics, stress limits or old published-package binary
compatibility. The loopback authentication evidence is not external JWT/OIDC acceptance.
No claim is made that every possible custom override or host configuration is covered.

### Retained behavior changes

MinId URL credentials remain rejected; lookup credentials travel in a body and generation
credentials in a header. Remote batch counts remain bounded. Business types cannot be
renamed/recreated after retirement, and reserved ID watermarks cannot move backward;
otherwise old and new generators can issue duplicate IDs. The `IsDeleted` column and
unique tenant-membership index still require host migrations. Terminal factory disposal
still rejects subsequent creation. These limits follow the credential, identifier and
ownership rules; the corrections above remove restrictions that were not needed for them.

## Regression re-review — 2026-11-22

This review revisits all 198 changed paths in `11bfc94e..260fea8c`, covering the
November 21 commits `bcea06c5`, `0f7720f6`, and `260fea8c`. Source review included
changed contracts, their callers, tests, and installed dependency behavior where relevant.
The historical passing runs below did not establish behavioral compatibility: several
storage tests had been changed to expect rejection instead of exercising a normal save.

| Regression | Correction |
| --- | --- |
| OBS, KS3, and MinIO rejected every default no-overwrite save without checking storage. | Restore the server existence check for the calculated bucket/key. Upload when absent; reject an existing object with `FileAlreadyExistsException`; allow explicit overwrite. The separate check and write still have the original concurrency limitation. |
| Aliyun saves added a bucket-versioning read permission and refused versioned buckets. | Remove that query and blanket refusal. Keep the object check and forbid-overwrite header. Versioned buckets do not gain an atomic no-overwrite guarantee. |
| Unix downloads refused existing destinations and required hard links even for new files. | Move completed new files into place. Overwrite an existing Unix destination through its opened inode after staging, preserving its metadata and link identity. Final local write failures can leave partial content. |
| FastDFS URL generation rejected every supplied expiry, even for public URLs. | Accept the argument as before; use configured token settings or a public URL. A caller-specific expiry remains unenforced. |
| The protected map-tenancy `Tenants` list became a detached copy, silently discarding subclass mutations. | Restore the stable protected backing list; synchronize base reads/fills and keep public return values detached. Subclasses must synchronize concurrent list access on `Tenants`. |

Restoring KS3's existence check also exposed an older helper defect: every object HEAD
exception was treated as absence, while an actual missing-bucket exception escaped.
The helper now handles both SDK and raw HTTP 404 responses as missing and propagates
permission/network failures. Explicit overwrite bypasses object-existence checks;
regressions exercise this for OBS, KS3, and MinIO.

Other compatibility changes remain material, even where the previous audit documented
them. Existing consumers must account for:

- MinId generation credentials moving from query parameters to the bearer header;
  administrative lookup/listing moving to POST bodies; remote batches restricted to 1–1,000;
  business names and watermarks no longer resettable; and the new `IsDeleted` column.
- Tenant connection-string updates requiring a JSON string body and tenant-group
  membership requiring a unique index.
- Registration subclasses using checked sign-in. An explicit `RegistrationSignInResult`
  takes precedence; legacy Identity hooks that issue a new application principal also work.
  Hooks using another custom authentication mechanism must report their checked result.
- Terminal resource-factory disposal and rejection of lifecycle mutation inside an
  active initialization callback when it would wait on unfinished pools or the whole owner.

### Re-review validation

`scripts/Test-Coverage.ps1 -ResultsDirectory artifacts/coverage/re-review-20261122 -ReportOnly`
completed successfully on Windows in Release configuration. All 12 solutions built with
zero warnings/errors; 38 test projects produced 2,316 passed, zero failed, and three
skipped results. Skips are the two Unix-only filesystem tests and the real Redis
two-process acceptance test, which lacks `SHARP_ABP_OPERATION_REDIS`. Repository line
coverage is 87.59% (19,434 of 22,188 lines) across the 152-project inventory.

Regression-first runs reproduced eight storage failures before the initial provider
fixes, the explicit FastDFS-expiry refusal, the protected-list mutation failure, and
three KS3 metadata-error/missing-bucket failures. The final storage suite passed 245
tests with two Unix skips; the map-tenancy module passed all 25 tests. Independent source
review of the resulting diff found no remaining concrete blocker. `git diff --check`
passed. Raw evidence is in `artifacts/review-2026-11-22` and the coverage directory above.

Live cloud services, external identity providers, distributed caches, MongoDB index
installation, and Unix runtime semantics require separate acceptance evidence. Source
review is not a guarantee that all defects have been found. The sections below retain
earlier audit evidence; the corrections and limits above supersede conflicting historical
descriptions.

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

MinId generation reads an opaque credential from `X-MinId-Token`, or from
`Authorization: Bearer <minid-token>` when no separate header is supplied.
Query-token inputs are rejected. Administrative token lookup/filtering uses JSON request
bodies, retaining the administrator's normal user authorization header. The old token-bearing
GET lookup route was removed; both the DTO lookup and the original `FindByTokenAsync`
signature use POST body credentials. `FindAsync(FindMinIdTokenDto)` remains a convenience
extension without adding a required interface member. Tenant-group and hybrid-tenant
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

The application service rejects counts outside 1–1,000 before allocating IDs. HTTP
parameter validation rejects unsupported counts as bad requests. Trusted in-process
domain callers retain positive-count bulk allocation without the remote ceiling.
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

Lifecycle mutation that would wait on its own active initialization is rejected before
state changes; object-pool callbacks can retire unrelated initialized pools. Normal cleanup
callbacks may access reusable owners. The active-callback
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
| Q4 | Business type names and their allocation watermark survive retirement. Updates reject renames and retain the greater requested/persisted watermark while applying metadata edits. Seed initialization counts retired rows. Apply the `IsDeleted` migration before deploying. |
| Q5 | Supported object stores submit a conditional write with the upload. The regression re-review above restores existence checks and normal uploads for OBS, KS3, and MinIO; their check and write are not atomic. |
| Q6 | Azure and FastDFS use the shared staged copy path. FastDFS checks existence when requested and accepts an expiry argument without enforcing a caller-specific deadline. |
| Q7 | A corrupt record returns its bytes and address in `UnreadableEntries`. Valid records in the batch remain available for processing and acknowledgement. The existing gap settings control retention until explicit acknowledgement. |
| Q8 | Password reset links preserve unrelated query values and the base fragment. The emailer replaces reserved reset parameters and encodes nested URLs and fragments. |
| Q9 | Imported RSA and SM2 pairs are checked against their declared key metadata. The cohosted transform-security reference check blocks deletion of referenced keys. Quiesce issuance across hosts before key retirement when the stores lack a shared transaction or lock. |
| Q10 | Standard grants use the upstream IdentityModel dispatch, including device authorization. The external credentials grant keeps its existing custom request path. |
| Q11 | The tenant grouping middleware honours the ignore attribute in endpoint metadata when routing runs before it. |
| Q12 | OpenIddict JSON properties support explicit replacement/clear and omitted/null preservation, including trusted model-building additions. Extra properties follow ABP's source and destination definitions. |
| Q13 | Connection management accepts recognized provider names or defined numeric values and stores canonical names. Unknown/combined values are rejected. |
| Q14 | MinId aligns in constant time across signed ranges. Invalid persisted alignment settings fail before allocation advances the watermark. |
| Q15 | The branch policy documents .NET 9 and ABP 9.3.x as the `4.x` line, including the existing `4.8.x` releases. |
| Q16 | Storage fields that require an Int32 reject overflow during validation. |
| Q17 | Scope manager reads use the store directly on upgraded hosts. Scope writes use the manager and evict legacy cache entries after commit. Rollback leaves the persisted scope and its legacy cache entries intact. |
| Q18 | Map tenancy returns detached public values. Earlier results can be enumerated independently of a later refresh. The protected mutable list contract is preserved. |
| Q19 | Downloads stage data beside the destination. New files use move; Windows replacements use replace. Unix overwrites existing files after staging, retaining inode metadata but without atomic final publication. |

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

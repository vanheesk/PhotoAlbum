# Plan: Upgrade PhotoAlbum solution from .NET 9.0 to .NET 10.0

> Note: The dedicated planning stage normally loads `planning.md` from the scenario skill package,
> but the orchestrator channel used to fetch it is currently unavailable (same error as the
> assessment tool). This plan was authored directly from `assessment.md` findings and the
> scenario's documented Success Criteria / Error Handling sections, using the standard
> plan-generation template.

## Strategy

Single-pass, in-place upgrade. Small two-project solution (`PhotoAlbum` → `PhotoAlbum.Tests`), no
multi-targeting requirement, no circular dependencies. Upgrade the main project first (dependency
order), then the test project, then validate the whole solution together.

## Tasks

- ⏳ 01-upgrade-photoalbum-project: Upgrade PhotoAlbum.csproj to net10.0
- ⏳ 02-upgrade-photoalbum-tests-project: Upgrade PhotoAlbum.Tests.csproj to net10.0
- ⏳ 03-build-test-validate: Final solution build, test run, and compliance validation

### 01-upgrade-photoalbum-project
Bump `PhotoAlbum\PhotoAlbum.csproj` TargetFramework to `net10.0`, update
`Microsoft.EntityFrameworkCore.Design` → 10.0.12, `Microsoft.EntityFrameworkCore.SqlServer` →
10.0.12, `SixLabors.ImageSharp` → 4.1.3. Build the project and fix any compile errors/warnings
surfaced by the TFM/package bump (EF Core 10 API changes, ImageSharp 3→4 breaking changes).

### 02-upgrade-photoalbum-tests-project
Bump `PhotoAlbum.Tests\PhotoAlbum.Tests.csproj` TargetFramework to `net10.0`, update
`Microsoft.AspNetCore.Mvc.Testing` → 10.0.12, `Microsoft.EntityFrameworkCore.InMemory` →
10.0.12, `Microsoft.NET.Test.Sdk` → 18.10.1, `xunit` → 2.9.3, `xunit.runner.visualstudio` →
4.0.1, `coverlet.collector` → 10.1.0. Build the project and fix any compile errors surfaced by
the bump. Depends on 01-upgrade-photoalbum-project.

### 03-build-test-validate
Build the full solution, run the full test suite, confirm no build warnings remain in touched
projects, and do a final package/security compliance pass (confirm no known-vulnerable package
versions remain — all targets resolved via `get_supported_package_version` against net10.0).
Depends on 01-upgrade-photoalbum-project and 02-upgrade-photoalbum-tests-project.

## Dependency Order

`01-upgrade-photoalbum-project` → `02-upgrade-photoalbum-tests-project` → `03-build-test-validate`
(sequential; test project depends on main project reference, final validation depends on both).

## Success Criteria (mapped to external task request)

- passBuild=true → verified in task 03
- generateNewUnitTests=false → no new tests will be authored, only existing ones fixed/run
- passUnitTests=true → verified in task 03
- securityComplianceCheck=true → verified in task 03 (all packages on latest net10.0-compatible,
  non-vulnerable versions)

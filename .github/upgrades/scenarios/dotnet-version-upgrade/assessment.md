# Assessment: Upgrade PhotoAlbum solution from .NET 9.0 to .NET 10.0

> Note: The dedicated `DotnetVersionAssessor` tool was unavailable (orchestrator channel
> connectivity error). This assessment was produced via the generic fallback path using
> `get_project_dependencies`, `get_supported_package_version`, and manual source review.

## Solution Overview

| Project | Current TFM | Target TFM | Project Kind |
|---|---|---|---|
| `PhotoAlbum\PhotoAlbum.csproj` | net9.0 | net10.0 | ASP.NET Core Web App (Microsoft.NET.Sdk.Web), EF Core + ImageSharp |
| `PhotoAlbum.Tests\PhotoAlbum.Tests.csproj` | net9.0 | net10.0 | xUnit test project, references `PhotoAlbum.csproj` |

Dependency order: `PhotoAlbum.csproj` must be upgraded before `PhotoAlbum.Tests.csproj` (test project
references the main project).

## Package Inventory & Target Versions

### PhotoAlbum.csproj
| Package | Current | Target (net10.0-compatible) | Notes |
|---|---|---|---|
| Microsoft.EntityFrameworkCore.Design | 9.0.9 | 10.0.12 | Major version bump tracks EF Core 10 / .NET 10 |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.9 | 10.0.12 | Same |
| SixLabors.ImageSharp | 3.1.11 | 4.1.3 | Major version bump — check for breaking API changes |

### PhotoAlbum.Tests.csproj
| Package | Current | Target (net10.0-compatible) | Notes |
|---|---|---|---|
| Microsoft.AspNetCore.Mvc.Testing | 9.0.9 | 10.0.12 | Tracks ASP.NET Core 10 |
| Microsoft.EntityFrameworkCore.InMemory | 9.0.9 | 10.0.12 | Tracks EF Core 10 |
| Microsoft.NET.Test.Sdk | 17.12.0 | 18.10.1 | Major version bump |
| xunit | 2.9.2 | 2.9.3 | Patch bump (staying on xunit v2 line; v3 not required) |
| xunit.runner.visualstudio | 2.8.2 | 4.0.1 | Stay on major runner compatible with xunit 2.x per NuGet resolver recommendation |
| coverlet.collector | 6.0.2 | 10.1.0 | Major bump, test-only tool, low risk |

All target versions above were resolved via `get_supported_package_version` against `net10.0`,
so they are confirmed to be compatible with the new TFM.

## Risk / Compatibility Analysis

1. **TFM bump (net9.0 → net10.0)**: Low risk mechanically — a single property change per
   csproj. .NET 10 is GA/LTS (support through Nov 2028), so this is the recommended upgrade target.
2. **EF Core 9 → EF Core 10**: Review `DbContext` configuration, LINQ translation changes, and any
   obsolete API usage flagged by compiler warnings after the bump. EF Core major versions
   occasionally change query translation behavior — tests (which use EF Core InMemory) will catch
   regressions.
3. **SixLabors.ImageSharp 3.x → 4.x**: Major version bump. ImageSharp 4.0 removed/changed some
   obsolete APIs from 3.x. Needs a source scan after upgrading to catch build breaks.
4. **Microsoft.NET.Test.Sdk 17 → 18** and **coverlet.collector 6 → 10**: Test tooling bumps,
   should be compile-transparent; verify `dotnet test` still runs/collects correctly.
5. **ASP.NET Core / EF Core breaking changes between .NET 9 and .NET 10**: Need to check official
   breaking-change list during execution; apply fixes where compiler/runtime surfaces them.
6. No global.json pinning a specific SDK version was found at the repo root — confirm during
   execution; if present, it must be updated/validated against the installed .NET 10 SDK.
7. Security: all target package versions are the latest supported versions for net10.0, which
   satisfies the "no known vulnerable package versions" success criterion, pending final package
   restore and build validation.

## Source Code Impact Areas To Scan During Execution

- Any direct SixLabors.ImageSharp API usage (resizing, format encoders, Image<T> APIs) — check for
  the 3.x → 4.x breaking changes.
- EF Core `DbContext`/migrations code, obsolete query APIs.
- Any usage of APIs marked obsolete/removed between .NET 9 and .NET 10 (to be confirmed via build
  warnings/errors after the TFM bump).

## Recommended Strategy

Single-pass, in-place upgrade (small, 2-project solution, no circular deps, no multi-targeting
need): bump both TFMs to net10.0, bump package versions to the versions above, build, fix any
compile errors/warnings, run tests, and validate security/compliance of final package set.

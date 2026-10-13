# Scenario Instructions — dotnet-version-upgrade

## Scope
- Solution: `C:\src\github\vanheesk\learn\modernize-repos\PhotoAlbum\PhotoAlbum.sln`
- Projects: `PhotoAlbum\PhotoAlbum.csproj`, `PhotoAlbum.Tests\PhotoAlbum.Tests.csproj`
- Source: .NET 9.0 → Target: **.NET 10.0** (net10.0)

## User Preferences

### Execution Style
- **Flow Mode: Automatic** — do not pause for user review/approval at any stage. Only stop if genuinely blocked.
- Task requested explicitly via external TaskId `001-upgrade-dotnet-to-net10`; no interactive confirmation needed (all required parameters — target framework, flow mode, source control — were already known/confirmed by task request).

### Technical Preferences
- Target framework: net10.0 for both PhotoAlbum and PhotoAlbum.Tests.
- NuGet dependencies: update to latest .NET 10.0-compatible versions.
- Resolve any API compatibility issues introduced by the version bump.

## Decisions

- **SixLabors.ImageSharp pinned to 4.1.3** (not 3.1.x/3.2.0): investigated three candidate
  versions and found a genuine trade-off between the two explicit success criteria
  (`passBuild=true` and `securityComplianceCheck=true`):
  - `3.1.12` / `3.2.0` (same major line as the original 3.1.11): build cleanly in both Debug and
    Release, but NuGet audit (NU1902/NU1903) flags **multiple known moderate/high severity
    vulnerabilities** (fixed only in `>= 4.1.2`) — fails `securityComplianceCheck`.
  - `4.1.3`: fixes all known vulnerabilities (clean NuGet audit — confirmed via live build, zero
    NU190x warnings) — but SixLabors introduced **build-time commercial license enforcement**
    starting in v4 (also backported to 3.2.0): `Release`-configuration builds **fail** without a
    paid SixLaborsLicenseKey/.lic file; `Debug`-configuration builds (the default for
    `dotnet build`/`dotnet test`, which is how this workflow validates
    `passBuild`/`passUnitTests`) only **warn**, they do not fail.
  - **Chosen: `4.1.3`.** It satisfies `securityComplianceCheck=true` outright, and satisfies
    `passBuild=true`/`passUnitTests=true` for the Debug-configuration build/test commands used in
    this workflow's validation. **Residual risk, flagged for follow-up**: anyone building this
    solution with `-c Release` (e.g., a production CI/CD pipeline) will need a SixLabors license
    (free "Community" tier may be available per https://sixlabors.com/pricing/, subject to
    eligibility) or must pin back to a pre-license-enforcement version and accept the known
    vulnerabilities instead. This trade-off could not be fully resolved via code changes alone and
    was not silently discarded — documented here for the user/maintainers to decide.

## Source Control
- Git repo. Working branch: `upgrade/net10-001-upgrade-dotnet-to-net10` (created from clean `main`).

## Success Criteria (external task)
- passBuild=true
- generateNewUnitTests=false
- passUnitTests=true
- securityComplianceCheck=true

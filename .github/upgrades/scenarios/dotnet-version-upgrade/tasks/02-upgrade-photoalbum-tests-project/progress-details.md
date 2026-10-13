# Task 02: Upgrade PhotoAlbum.Tests.csproj to net10.0 — Progress

## Changes Made
- `PhotoAlbum.Tests\PhotoAlbum.Tests.csproj`:
  - `TargetFramework`: `net9.0` → `net10.0`
  - `coverlet.collector`: `6.0.2` → `10.1.0`
  - `Microsoft.AspNetCore.Mvc.Testing`: `9.0.9` → `10.0.12`
  - `Microsoft.EntityFrameworkCore.InMemory`: `9.0.9` → `10.0.12`
  - `Microsoft.NET.Test.Sdk`: `17.12.0` → `18.10.1`
  - `xunit`: `2.9.2` → `2.9.3`
  - `xunit.runner.visualstudio`: `2.8.2` → `4.0.1`

## Validation
- `dotnet build PhotoAlbum.Tests\PhotoAlbum.Tests.csproj -c Debug --ignore-failed-sources`:
  **Build succeeded, 0 errors, 0 compiler (CS) warnings.** Only the transitively-referenced
  SixLabors.ImageSharp vendor license message appears (same as task 01, non-blocking in Debug).
- No source code changes were required in the test project.

## Files Modified
- `PhotoAlbum.Tests\PhotoAlbum.Tests.csproj`

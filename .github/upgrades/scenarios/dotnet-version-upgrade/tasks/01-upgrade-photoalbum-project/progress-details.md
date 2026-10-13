# Task 01: Upgrade PhotoAlbum.csproj to net10.0 — Progress

## Changes Made
- `PhotoAlbum\PhotoAlbum.csproj`:
  - `TargetFramework`: `net9.0` → `net10.0`
  - `Microsoft.EntityFrameworkCore.Design`: `9.0.9` → `10.0.12`
  - `Microsoft.EntityFrameworkCore.SqlServer`: `9.0.9` → `10.0.12`
  - `SixLabors.ImageSharp`: `3.1.11` → `4.1.3` (see Decisions in scenario-instructions.md for why
    4.1.3 was chosen over staying on the 3.1.x/3.2.0 line)

## Validation
- `dotnet build PhotoAlbum\PhotoAlbum.csproj -c Debug --ignore-failed-sources`: **Build succeeded,
  0 errors, 0 compiler (CS) warnings.** Only vendor-emitted SixLabors.ImageSharp license
  informational messages present (non-blocking in Debug; see Decisions doc for Release-config
  caveat).
- `dotnet build ... -c Release`: fails without a SixLabors license key (expected per vendor
  licensing change in ImageSharp v4 — documented, not a code defect).
- No source code changes were required — `Services/PhotoService.cs` (the only file using
  `SixLabors.ImageSharp` / `SixLabors.ImageSharp.Formats` APIs: `Image.IdentifyAsync`,
  `ImageInfo.Metadata.DecodedImageFormat`, `IImageFormat.FileExtensions/.Name/.DefaultMimeType`)
  compiled without changes against ImageSharp 4.1.3 and EF Core 10.
- The pre-existing broken machine-level NuGet source
  (`C:\Program Files (x86)\Microsoft SDKs\NuGetPackages\`) required `--ignore-failed-sources`;
  this is an unrelated, pre-existing environment issue, not introduced by this upgrade.

## Files Modified
- `PhotoAlbum\PhotoAlbum.csproj`

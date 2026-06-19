# ImgSeeder 3.9.1 Release Notes

## Summary

- Releases `ImgSeeder` version `3.9.1` with the `iorg` command.
- Aligns fallback package references to `JsonPit 3.9.1`, `OsLibCore 3.9.1`, `RaiUtils 3.9.1`, and `RaiImage 3.9.1`.
- Publishes `ImgSeeder` through the coordinated sequential NuGet release chain immediately before `PitSeeder`.
- Keeps the CLI aligned with RaiImage's current filename normalization behavior for structured tree ingestion.

## Validation

- `dotnet test ImgSeeder.slnx --nologo -v minimal`
- NuGet publishing is handled by the tag-triggered `publish-nuget.yml` workflow and the parent sequential release chain.

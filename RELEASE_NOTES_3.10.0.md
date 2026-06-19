# ImgSeeder 3.10.0 Release Notes

## Summary

- Releases `ImgSeeder` version `3.10.0` with the `iorg` command.
- Aligns fallback package references to `JsonPit 3.10.0`, `OsLibCore 3.10.0`, `RaiUtils 3.10.0`, and `RaiImage 3.10.0`.
- Keeps `ImgSeeder`/`iorg` immediately before `PitSeeder` in the coordinated sequential release chain.
- Preserves the CLI behavior from `3.9.1`, including the current RaiImage filename-normalization flow used for structured tree ingestion.

## Validation

- `dotnet test ImgSeeder.slnx --nologo -v minimal`
- Publication remains wired through the tag-triggered `publish-nuget.yml` workflow and the parent sequential release chain.

# ImgSeeder 3.10.3 Release Notes

## Summary

- Releases `ImgSeeder` version `3.10.3` with the `iorg` command.
- Aligns runtime metadata (`AssemblyVersion`, `FileVersion`, and `InformationalVersion`) with package version `3.10.3` so `iorg -v` matches the published package.
- Keeps fallback package references at `JsonPit 3.10.2`, `OsLibCore 3.10.2`, `RaiUtils 3.10.2`, and `RaiImage 3.10.2`.
- Preserves CLI behavior from `3.10.2`.

## Validation

- `dotnet test ImgSeeder.slnx --nologo -v minimal`
- Publication remains wired through the tag-triggered `publish-nuget.yml` workflow.

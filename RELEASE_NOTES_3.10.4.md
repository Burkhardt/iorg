# ImgSeeder 3.10.4 Release Notes

## Summary

- Releases `ImgSeeder` version `3.10.4` with the `iorg` command.
- Adds `-rmc` as the short option alias for cache-delete mode while keeping `--rm-cache`.
- Aligns help output formatting to a fixed option column so glyph icons stay visually aligned.
- Preserves runtime behavior from `3.10.3`.

## Validation

- `dotnet test ImgSeeder.slnx --nologo -v minimal`
- `dotnet run --project ImgSeeder.csproj -- -h -c OneDrive -r LiveAfricaStageImage`
- Publication remains wired through the tag-triggered `publish-nuget.yml` workflow.

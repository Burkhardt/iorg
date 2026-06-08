# ImgSeeder

`ImgSeeder` is the RAIkeep image organizer package. It installs the `iorg` CLI, which copies source images, normalizes filenames with RaiImage naming rules, and places the final files into an `ImageTreeFile` directory layout such as `ItemIdTree8x2`.

## 3.9.1

- Coordinated patch release: aligns `ImgSeeder` with the `3.9.1` `RAIkeep` package line.
- Tracks RaiImage's current filename normalization so separated and compact trailing digits become stable image numbers during CLI ingestion.
- Ships as part of the parent sequential NuGet release chain immediately before `PitSeeder`.
- Current release notes: [RELEASE_NOTES_3.9.1.md](RELEASE_NOTES_3.9.1.md)

This tool is part of the RAIkeep package family:

- `OsLibCore`
- `RaiUtils`
- `RaiImage`
- `JsonPit`
- `PitSeeder`
- `ImgSeeder` (`iorg` command)

## Install

Install the NuGet tool with:

```bash
dotnet tool install --global ImgSeeder
```

On macOS or Linux, a practical option is to install directly into a directory on your `PATH`:

```bash
sudo dotnet tool install ImgSeeder --tool-path /usr/local/bin
```

Update an existing installation with:

```bash
dotnet tool update --global ImgSeeder
```

To update an installation in `/usr/local/bin`:

```bash
sudo dotnet tool update ImgSeeder --tool-path /usr/local/bin
```

## Usage

Typical cloud-rooted usage:

```bash
iorg -n -c OneDrive -r LiveAfricaStageImage/ nomsa -s /Users/Shared/ServerData/GDriveData/TestAfricaStage/Images/NOMSA.net/
```

The command resolves `-c` through `Os.Config.Cloud`, appends `-r` and the subscriber name, then prints each source and destination pair:

```text
copied /source/nomsa-concert-11.jpg
=> /dest/nomsa/NomsaCon/NomsaConce/NomsaConcert_11.jpg
```

Useful options:

- `-h`, `--help`: print help
- `-v`, `--version`: print version
- `-n`, `--nologo`: hide banner
- `-b`, `--debug`: enable debug output
- `-c`, `--cloud`: cloud provider name from `Os.Config.Cloud`
- `-r`, `--root`: destination image root under the cloud root
- `-s`, `--source`: source image directory
- `-f`, `--filter`: file filter, default `*.jpg`
- `-p`, `--pathconvention`: `CanonicalByName`, `ItemIdTree3x3`, or `ItemIdTree8x2`
- `-m`, `--namingconvention`: `Legacy`, `ItemTemplate`, or `Structured`

## Standalone Binaries

Tagged releases also publish self-contained `iorg` workflow artifacts for:

- `linux-x64`
- `osx-arm64`
- `osx-x64`
- `win-x64`

These binaries can be deployed without a separate .NET runtime installation.

## Validation

- `dotnet test iorg.slnx --nologo -v minimal`

# base

Base starting state for a few different types of apps:
- C# WPF .NET 10 (default; the sample app)
- Avalonia C# .NET 10 (cross-platform port target)
- C# .NET without UI (console, library, worker)
- Rust
- PowerShell only
- C

This project relies on simple pwsh scripts in `.scripts/` to perform various repo actions:
- `.run.ps1`: build and launch the app
- `build.ps1`, `buildInstaller.ps1`, `buildUpdater.ps1`: build the app, installer, and updater
- `newVersion.ps1`: bump `.version/version`
- `prePush.ps1`: run the full local build and publish pipeline
- `push.ps1`: commit and push using `buildNotes.txt`
- `agentPush.ps1`: agent-run push with notes validation
- `pushRelease.ps1`: tag and publish a GitHub release
- `updateReadme.ps1`: refresh the download buttons below

Includes theming, scripting, and agent guidelines.

## Requirements

- .NET 10 SDK
- PowerShell 7
- Inno Setup 6 for installers
- GitHub CLI for release scripts
- ImageMagick (`magick`) for installer icon helpers

## Versioning

- Semantic version: `.version/version`
- Optional release tag override: `.version/versionTag`
- Last build platform: `.version/versionBuild`
- User release notes: `buildNotes.txt` (local, not committed)
- Agent history: `buildNotes.md` (local, not committed)

## Agent Docs

- [AGENTS.md](AGENTS.md): agent rules and stack guidelines
- [STYLE.md](src/.md/STYLE.md): file, code, docs, and design style
- [SCRIPTS.md](src/.md/SCRIPTS.md): PowerShell script guide
- [THEMING.md](src/.md/THEMING.md): colors and theming
- [IMPORT.md](src/.md/IMPORT.md): import `base` into an existing project
- [DEPENDENCIES.md](src/.md/DEPENDENCIES.md): tools and packages


<!-- Quick Reference -->
<table border="0">
<tbody>
<tr>
<td valign="top"><a href="https://github.com/fosterbarnes/base/releases/download/v0.1.0/base_v0.1.0_windows-x86.exe"><img src="https://raw.githubusercontent.com/fosterbarnes/res/main/btn/x86Installer.svg" width="180" height="auto" alt="Windows x86 installer"/></a></td>
<td valign="top"><a href="https://github.com/fosterbarnes/base/releases/download/v0.1.0/base_v0.1.0_windows-x64.exe"><img src="https://raw.githubusercontent.com/fosterbarnes/res/main/btn/x64Installer.svg" width="180" height="auto" alt="Windows x64 installer"/></a></td>
<td valign="top"><a href="https://github.com/fosterbarnes/base/releases/download/v0.1.0/base_v0.1.0_windows-arm64.exe"><img src="https://raw.githubusercontent.com/fosterbarnes/res/main/btn/arm64.svg" width="180" height="auto" alt="Windows arm64 installer"/></a></td>
</tr>
<tr>
<td valign="top"><a href="https://github.com/fosterbarnes/base/releases/download/v0.1.0/base_v0.1.0_windows-x86.zip"><img src="https://raw.githubusercontent.com/fosterbarnes/res/main/btn/x86Portable.svg" width="180" height="auto" alt="Windows x86 portable ZIP"/></a></td>
<td valign="top"><a href="https://github.com/fosterbarnes/base/releases/download/v0.1.0/base_v0.1.0_windows-x64.zip"><img src="https://raw.githubusercontent.com/fosterbarnes/res/main/btn/x64Portable.svg" width="180" height="auto" alt="Windows x64 portable ZIP"/></a></td>
<td valign="top"><a href="https://github.com/fosterbarnes/base/releases/download/v0.1.0/base_v0.1.0_windows-arm64.zip"><img src="https://raw.githubusercontent.com/fosterbarnes/res/main/btn/arm64Portable.svg" width="180" height="auto" alt="Windows arm64 portable ZIP"/></a></td>
</tr>
</tbody>
</table>
<!-- End Quick Reference -->

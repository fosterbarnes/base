# Dependencies

Windows-first WPF and PowerShell baseline for new projects. This file lists the tools and packages required to build, run, and publish the WPF sample in this repo. Other stacks documented in `AGENTS.md`, including the Avalonia port target, are copy-time clauses only; they add no requirements here.

## Development Tools

| Tool | Purpose |
|------|---------|
| .NET 10 SDK | Build and publish the app and updater |
| PowerShell 7 (pwsh) | Run all `.scripts` automation |
| Inno Setup 6 | Windows installer packaging (`.installer/*.iss`) |
| GitHub CLI | Release scripts (`pushRelease.ps1`) |
| ImageMagick | Icon and PNG size helpers (`convertToIco`, `resizePng`, `prepareInstallerImages`, `ensureInstallerImages`) |
| RipGrep (rg) | Repo-wide search from pwsh (`grep` alias) |

Target machines also need the matching .NET 10 runtime because publish uses `--no-self-contained`.

## .NET Packages

Both projects target `net10.0-windows` with `UseWPF`. `baseUpdater` has no packages; it links the app's theme files and `WindowsTitleBarTheme.cs`.

| Package | Version | base | baseUpdater |
|---------|---------|------|-------------|
| MaterialDesignThemes | 4.9.0 | yes | no |

MDI icons (Material Design Icons from Pictogrammers) are a default dependency through `MaterialDesignThemes` `PackIcon`. `MaterialDesignColors` and `Microsoft.Xaml.Behaviors.Wpf` arrive transitively. The MaterialDesign theme dictionaries are not merged; the app styles its own controls.

## Supported Platforms

Windows only. Each architecture builds a portable zip and an Inno Setup installer. Asset names use the Version source of truth (`.version/version`).

| Architecture | Runtime identifier | Portable asset | Installer asset |
|--------------|--------------------|----------------|-----------------|
| x86 | win-x86 | base_<version>_windows-x86.zip | base_<version>_windows-x86.exe |
| x64 | win-x64 | base_<version>_windows-x64.zip | base_<version>_windows-x64.exe |
| ARM64 | win-arm64 | base_<version>_windows-arm64.zip | base_<version>_windows-arm64.exe |

The updater publishes to the same three runtime identifiers.
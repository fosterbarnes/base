# IMPORT.md - Import `base` into an existing project

Agent playbook. You are working in a *target* repository. `base` is the source of rules, markdown, and `.scripts/`. Do not replace the target's application source with the WPF sample.

No em dashes. Use `-` or `:`. Do not run `prePush.ps1`, `pushRelease.ps1`, builds, or version bumps as part of import. Do not edit the target's `buildNotes.txt`.

---

# 1. Locate `base`

1. If the user has a local clone of `base` that contains `AGENTS.md`, `src\.md\`, and `.scripts\`, use that tree.
2. Otherwise, clone `https://github.com/fosterbarnes/base` into a temp directory and use that clone as the source.
3. Never treat the target repo as the source, even if it already has some of these files.

---

# 2. Detect, then ask

Scan the *target* (not `base`). Propose one stack. Do not copy until the user confirms.

First match unless there is a conflict:

| Signal in target | Propose |
|------------------|---------|
| `.axaml` or Avalonia PackageReference | Avalonia C# .NET |
| `.xaml` plus WPF / `UseWPF` / `net*-windows` | C# WPF .NET |
| `*.csproj` without UI markup | C# .NET (no GUI) |
| `Cargo.toml` | Rust |
| `CMakeLists.txt` / `*.c` / `*.h` as primary, no csproj/cargo | C |
| Product is `.ps1`, no csproj/cargo/cmake | PowerShell only |

On conflict or no signal, say so.

Ask the user to pick one of:

1. Avalonia C# .NET
2. C# WPF .NET
3. C# .NET (no GUI)
4. Rust
5. PowerShell only
6. C

Recommend the detected type in the question. Stop until they answer.

---

# 3. Copy set

Copy from `base` into the target repository, keeping the same relative paths.

## Copy

- `AGENTS.md` (root)
- `src\.md\STYLE.md`
- `src\.md\SCRIPTS.md`
- `src\.md\THEMING.md` (see stack checklist; you may delete it after copy for non-GUI stacks)
- `src\.md\DEPENDENCIES.md`
- `src\.md\IMPORT.md`
- Entire `.scripts\` directory (root)

## Do not copy

- Sample app / updater C# and XAML trees
- `*.sln` / `*.csproj` from `base` unless the user later asks to add a new project file
- `publish\`
- `.installer\` unless the user later asks
- `.git`
- Temporary agent plan files
- `buildNotes.md` / `buildNotes.txt` (local to each clone)
- `base` `README.md` as a replacement for the target README

## Special files

- **README.md:** do not overwrite. Optionally add a one-line pointer to `src\.md\IMPORT.md` and `.scripts\` if the target README has no automation section and the user agrees.
- **buildNotes.md:** if missing, create it using the format in `AGENTS.md` (format rules, then `# Historical entries (newest first)`). If present, do not overwrite. After a successful import, append one Agent + User session entry.
- **`.version\`:** do not copy unless the target has no version source of truth and the user asks. Prefer documenting the target's existing Version path in `AGENTS.md`. When creating a new SoT, initialize `.version\version` line 1 to `0.0.1` (blank tag and build lines); do not copy `base`'s current semver.

## Collisions

If a destination file already exists, ask overwrite vs merge vs skip (one batch question is fine).

Recommended defaults:

- `AGENTS.md`: merge. Keep target Project Guidelines and Agent Notes that are still true. Bring General Guidelines, buildNotes rules, and the matching stack section from `base`.
- `STYLE.md` / `SCRIPTS.md` / `THEMING.md`: replace only when the target copies are empty or stale stubs.
- `.scripts\`: copy missing files; do not silently overwrite a working `scriptHelper.ps1` that already has this project's paths. Merge branding/path changes instead.

---

# 4. Adapt after copy

## AGENTS.md

- Fill Project Guidelines for *this* repo: name, Version SoT path, what the product is.
- Keep one stack section; delete the other five.
- Keep General Guidelines, buildNotes rules, no-em-dash, pwsh.

## SCRIPTS.md and `.scripts`

- Keep operator names and order: `.run.ps1`, `prePush.ps1`, `push.ps1`, `agentPush.ps1` (when the user lets agents push), `pushRelease.ps1`, `build.ps1`, and the other standard step names.
- Edit `scriptHelper.ps1` first: `$projectName`, version path, app/cargo/cmake roots, GitHub repo URL, icon paths. Do not leave `$csproj` pointing at `base`.
- Keep `$buildTargets` to the architectures the target ships; each entry's `InstallerButton` / `PortableButton` must name an existing SVG in [fosterbarnes/res/btn](https://github.com/fosterbarnes/res/tree/main/btn) or the README buttons break.
- Point `build.ps1` / `.run.ps1` at the stack tool (`dotnet`, `cargo`, `cmake`/`msbuild`, or the main `.ps1`).
- Omit or no-op `buildUpdater.ps1` / `buildInstaller.ps1` when those artifacts do not exist. Do not stub fake `dotnet` calls.
- Preserve the complete `closeOut` implementation from `scriptHelper.ps1`; call `closeOut 0` after successful standalone entry scripts and `closeOut -KeepOpen` before rethrowing handled failures. Never replace it with a no-op or omit it while trimming; help paths return without closeout. Only deviate when the user explicitly instructs it.

## THEMING.md

- Keep for Avalonia, WPF, or other GUI.
- For console C#, C, PowerShell-only, and non-GUI Rust: delete it from the target or replace with a one-liner that it does not apply. Remove the `AGENTS.md` pointer to it.

## DEPENDENCIES.md

Rewrite the tool and package tables for the confirmed stack. Do not leave Avalonia package versions in a Rust, C, or PowerShell-only repo.

## STYLE.md

Keep as-is. Language overrides already cover C#, Rust, C, and PowerShell. Do not force camelCase onto Rust or C identifiers.

## README.md (target)

Do not replace. A short "automation lives in `.scripts`; agent rules in `AGENTS.md`" sentence is enough if you add anything.

---

# 5. Do not

- Do not replace the target's application source with the WPF sample window.
- Do not run project operators or builds as part of import.
- Do not edit `buildNotes.txt`.
- Do not bump version.
- Do not invent a second Version file.

---

# 6. Per-stack checklist

After the user confirms the stack, tick these.

## Avalonia C# .NET

- Keep the Avalonia stack section; delete the other five.
- `THEMING.md` stays. Preserve compiled bindings and `.axaml`.
- `build.ps1` / `.run.ps1` wrap `dotnet`. Keep updater/installer scripts only if this repo ships them.
- `DEPENDENCIES.md`: .NET SDK, pwsh, plus this project's packages (not necessarily `base`'s sample list).

## C# WPF .NET

- Keep the WPF section only.
- UI is `.xaml`, `net10.0-windows`. No Avalonia packages or AXAML rules.
- Apply `THEMING.md` through WPF resource dictionaries.
- Same operator names; `dotnet` inside `build.ps1` / `.run.ps1`.
- Omit `buildUpdater.ps1` when there is no updater.

## C# .NET (no GUI)

- Keep the no-GUI C# section only.
- Delete or one-line `THEMING.md`.
- `dotnet` through `.scripts`. No UI/theme/binding rules.
- Omit updater/installer operators when those artifacts do not exist.

## Rust

- Keep the Rust section only.
- `cargo` inside `build.ps1` / `.run.ps1`. Default host `x86_64-pc-windows-msvc` unless the crate already targets something else.
- Source naming: `snake_case` files/modules, `PascalCase` types. Do not force `STYLE.md` camelCase onto `.rs` names.
- `THEMING.md` only if the crate has a GUI.
- `DEPENDENCIES.md`: rustc/cargo, pwsh, MSVC if Windows. No Avalonia package table.

## PowerShell only

- Keep the PowerShell-only section only.
- `SCRIPTS.md` is the primary code-style file.
- `.run.ps1` launches the product script. `build.ps1` may no-op or zip scripts.
- Omit updater/installer/`dotnet publish`.
- Delete or one-line `THEMING.md`.
- `#requires -Version 7.0`, `$ErrorActionPreference = 'Stop'`, `-LiteralPath`, check `$LASTEXITCODE`.

## C

- Keep the C section only. C, not C++, unless the tree is already C++.
- `build.ps1` wraps existing CMake or MSVC/`msbuild`. Do not invent a second build system.
- Follow existing C identifier and header style.
- Delete or one-line `THEMING.md`. No `dotnet`.

---

# 7. Verify the import in the target

- One stack section remains in `AGENTS.md`.
- `scriptHelper.ps1` paths and names match this repo.
- No leftover `base` project name, `base` csproj path, or sample package list where they do not belong.
- Target README and existing `buildNotes.md` body were not overwritten.
- `buildNotes.md` has an import session entry (Agent + User).
- No em dashes in files you copied or edited.
- Every retained standalone script uses the shared `closeOut` behavior after successful completion; deviations require explicit user instruction.

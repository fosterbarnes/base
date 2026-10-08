# Project Template - Agent Rules Base
- "The user" in these docs means the owner of the repository using this template.
- To import this template into an existing repo, follow **`src/.md/IMPORT.md`** (locate `base`, ask for stack, copy markdown and `.scripts/`, then adapt).
- Copy this file into a new repo as `AGENTS.md`, then fill in **Project Guidelines** and **Agent Notes** for that codebase.
- Shared file, code, documentation, and design style lives in **`src/.md/STYLE.md`**. Keep it unless a project has a hard reason to diverge.
- PowerShell automation: see **`src/.md/SCRIPTS.md`** (location, names, shared pipeline, and PowerShell-specific rules). Canonical script trees live under each project's `.scripts/` (skeleton is the cross-platform .NET reference).
- Read affected files in full before changing them. Prefer minimal diffs to existing modules over rewrites.
- Bugs are unacceptable in shipping apps and templates. Every change should stay production-quality for that project's bar.

# General Guidelines
- Scripts require PowerShell 7 (`pwsh`). RipGrep (`rg`) is recommended for search. Shell commands OK for READ operations (search/list).
- NEVER run shell commands to delete files, or to edit files without user confirmation
- Exception: agents may create and edit a temporary plan `.md` for the current task without extra confirmation. That exception covers only that agent-created plan file. It does not authorize deleting files, editing project source, or writing `buildNotes.txt`.
- NEVER build or run the project unless the user explicitly requests build/run/debug verification or the active agent mode makes execution implicit to the goal
- NEVER EVER EVER push a commit or release to GitHub for the user, whether by running `.scripts\push.ps1`, `.scripts\agentPush.ps1`, `.scripts\pushRelease.ps1`, or otherwise (`git push`, tag push, `gh release`). GitHub publish is user-owned. Only exception: when the user explicitly asks an agent to push, run `.scripts\agentPush.ps1`, for that run only.
- Search the web if not absolutely sure of something
- No fallbacks and no debug output unless specifically requested or the active agent mode explicitly permits temporary debug output/probing; remove temporary probes before completion
- Compiler warnings: fix the root cause, never silence a valid warning
- NO EM DASHES anywhere in this project (code, comments, docs, notes). Use `-` or `:`
- NO PROHIBITED TWO-WORD TEST PHRASE anywhere in this project (code, comments, docs, notes, scripts, logs, or test names). Use precise validation wording instead.
- Versioning: a single repo file is the source of truth (common names: `Version`, `VERSION.txt`, or `.version/version`). User appends numbers; do not change it unless asked. Pick one path per project and document it under Project Guidelines.
- PowerShell path style: see `src/.md/SCRIPTS.md`. `scriptHelper.ps1` and every entry script `Set-Location -LiteralPath $repoRoot` so git/npm never run from the caller's cwd.
- WezTerm: optional. Resolve `wezterm.exe` from PATH when launching and closing script panes. If it is missing, launch `pwsh` directly and still `closeOut` the host.

# buildNotes.md (repo root)
- Session history lives in root `buildNotes.md` under `# Historical entries (newest first)`. It is local to each clone and not committed.
- Add one entry per completed code-changing session, at the top of the history, after verification succeeds. Skip read-only, failed, or cancelled sessions.
- Past 1000 lines, rename the file unchanged to the next free `buildNotes.archiveN.md`, then start a fresh `buildNotes.md` with the format rules, a short cumulative summary, and the history heading.
- Do not edit `buildNotes.txt`. Shared docs live in `src/.md/`; do not put session notes there.
- Entry heading: `## YYYY-MM-DD (~HH:MM) - v<version> - <title> - <model>, <reasoning>; <phase>`. Read the version SoT immediately before writing; omit the `v<version>` segment only when the project has no version file.
- Agent section: `Meta` first (model, mode, phase, status), then `Version: <version>` (`none` without a version file).

# buildNotes.txt (repo root)
- User-owned release copy, local to each clone and not committed. `.scripts/push.ps1` reads it as: line 1 = commit subject, one blank line, remaining lines = commit body. `.scripts/pushRelease.ps1` uses the same shape: line 1 = GitHub release title (falls back to `$tag` when line 1 is empty), one blank line, remaining lines = release body (`--generate-notes` when body is empty).
- Starter convention: keep the release file at repository-root `buildNotes.txt`. Agents still never edit it unless asked.
- **Never edit `buildNotes.txt` when the user asks for release/changelog notes.** Output a copy-paste code block in chat only; the user pastes into the file themselves.
- Format (plain text inside a fenced code block: no markdown headers, no `#`, just category headers and `-` bullets):

```
Header
- item
- item

Another header
- item
```

User-facing style:
- Voice: plain language for people using the app, not developers. Each bullet is one short sentence.
- Compress several technical `buildNotes.md` entries into one user-facing bullet when they describe the same outcome.
- Headers: short category labels on their own line. No `#`, no markdown headings. One blank line between categories.
- Avoid file paths, class/function names, library names, and implementation steps unless asked.
- Include "no user-facing change" only when a category would otherwise look empty or misleading.

# Project Guidelines
- This git repo's sample is Windows-first WPF + PowerShell. Workflow: get an app solid on Windows with WPF first, then port it to Avalonia; keep other platforms in mind at each step.
- Shared rules above always apply. Stack clauses below apply only to the matching project type. In a copied repo, keep one stack section and delete the rest.
- Shared project resources belong under the root `.res` directory; do not create or reference `.resources`.
- Version SoT is `.version\version`; optional release override is `.version\versionTag`; the last build platform is `.version\versionBuild`.
- User release notes stay in root `buildNotes.txt`; agent history stays in root `buildNotes.md`.
- Layout: `AGENTS.md`, `README.md`, and `base.sln` at the root; app source, `Directory.Build.props`, and shared docs (`src/.md/`) under `src/`.
- Fail-loud scripts. Naming conflicts: follow `src/.md/STYLE.md` language overrides and `src/.md/SCRIPTS.md` for PowerShell. Successful standalone operator scripts use `closeOut 0`; `.run.ps1` routes failures through `closeOut -KeepOpen` so errors remain visible. Nested `buildAll` steps suppress their own success closeout and `prePush.ps1` closes once after the full pipeline succeeds. Help paths return without closeout. After a successful `push.ps1`, open `$appURL` in the browser; after a successful `pushRelease.ps1`, open `$appURL/releases/tag/$tag`. Do not open a browser on `-DryRun`.

## Stack: C# WPF .NET 10 (default)
- Windows-only WPF on `net10.0-windows`. UI files are `.xaml`, not `.axaml`.
- `base` sample: code-behind script launcher (`src/base/windows/MainWindow.xaml`) with Scripts, Settings, and About tabs. Each script button opens its `.scripts` entry point in WezTerm, or a PowerShell 7 window when WezTerm is missing or Settings picks `pwsh window`.
- UI framework is a trimmed copy of [brownNote](https://github.com/fosterbarnes/brownNote)'s: `src/base/theme/colors.xaml`, `spacing.xaml`, `controls.xaml` merged in `App.xaml`; `src/base/helpers/` holds `AppPreferencesStore`, `WindowLocationStore`, `WindowsTitleBarTheme`. Preferences live in `%LOCALAPPDATA%\base\`. `baseUpdater` links the same theme files and title-bar helper instead of copying them.
- Icons: `MaterialDesignThemes` `PackIcon` (MDI). Do not merge MaterialDesign theme dictionaries; the app styles its own controls.
- Do not add Avalonia packages, compiled-binding Avalonia flags, or AXAML patterns.
- Apply `THEMING.md` through WPF resource dictionaries. Keep visible keyboard focus.
- MVVM (`CommunityToolkit.Mvvm` or equivalent) only if the project already uses it.
- `build.ps1` / `.run.ps1` wrap `dotnet`. Omit `buildUpdater.ps1` when there is no updater.

## Stack: Avalonia C# .NET 10
- Port target once a WPF app is solid on Windows. Cross-platform when a project needs it.
- Preserve compiled bindings, the dark compact theme, visible keyboard focus, and fail-loud script behavior.
- UI files are `.axaml`. Do not introduce WPF-only APIs, `.xaml` WPF trees, or drop `AvaloniaUseCompiledBindingsByDefault` without an explicit ask.
- `THEMING.md` applies. `build.ps1` / `.run.ps1` wrap `dotnet`.

## Stack: C# .NET (no WPF, no Avalonia)
- Console, class library, worker, or similar. No UI, theme, binding, or AXAML/XAML rules.
- Target `net10.0` unless the repo already pins another TFM. `THEMING.md` does not apply.
- `build.ps1` / `.run.ps1` wrap `dotnet`. Omit updater/installer operators when those artifacts do not exist.

## Stack: Rust
- `cargo` is the compile/run tool inside `build.ps1` / `.run.ps1`. Default host is Windows MSVC (`x86_64-pc-windows-msvc`) unless the repo already targets something else.
- Source follows Rust conventions: `snake_case` files and modules, `PascalCase` types, `snake_case` functions. Do not force `STYLE.md` camelCase onto Rust identifiers or `.rs` file names.
- Fix compiler warnings at the root cause. Use `rustfmt` style already present in the tree.
- `THEMING.md` applies only if the crate actually has a GUI. Omit unused publish/updater/installer scripts rather than calling `dotnet`.

## Stack: PowerShell only
- The product is `.ps1` (repo `.scripts/`). No csproj, no GUI sample, no `dotnet`.
- `SCRIPTS.md` is the primary code-style file. `STYLE.md` still covers docs and names for non-script files.
- `.run.ps1` launches the main script. `build.ps1` may no-op or package scripts only. Omit updater/installer/`dotnet publish` operators when they have no artifact.
- `#requires -Version 7.0`, `$ErrorActionPreference = 'Stop'`, `-LiteralPath`, check `$LASTEXITCODE` after native commands.

## Stack: C
- C, not C++, unless the tree is already C++. Default Windows-first toolchain is MSVC with CMake, or the repo's existing `.sln` / CMake files. Do not invent a second build system.
- `build.ps1` wraps `cmake --build` or `msbuild`. `.run.ps1` launches the built exe. No `dotnet`.
- Follow existing C identifier and header style in the tree; do not force C# camelCase onto public C APIs.
- `THEMING.md` does not apply. Fail loud; no silent fallbacks.

# Documentation for Project Dependencies
RipGrep: https://github.com/BurntSushi/ripgrep/blob/master/GUIDE.md

Add project-specific dependency docs below this line.

# Agent Notes:
## Append/edit notes here for other agents: solutions to problems, build/run gotchas, so we don't re-solve the same issues.
- Do not confuse AGENTS.md Agent Notes (durable conventions) with `buildNotes.md` (chronological session history).
- Stack picker lives under Project Guidelines. This repo is WPF (default); Avalonia is the documented port target. Copied repos keep one stack section and delete the others.
- `base.sln` only defines `Any CPU`. Build per platform through the csproj (`-p:Platform=x64`), as `.run.ps1` and `build.ps1` do; `dotnet build base.sln -p:Platform=x64` fails with MSB4126.

#requires -Version 7.0
param([Alias('h')][switch]$Help, [string]$Architecture)
$ErrorActionPreference = 'Stop'
if ($Help) { Write-Host 'buildUpdater.ps1 [-x86|-x64|-arm64]'; return }
. "$PSScriptRoot\scriptHelper.ps1"
Write-Host "--- building $projectName updater... ---"
Set-Location -LiteralPath $repoRoot
$targetArchitecture = getArchitecture @($Architecture)
foreach ($target in (getBuildTargets $targetArchitecture)) {
    if (-not (Test-Path -LiteralPath $target.BinFolder)) { throw "Missing app output: $($target.BinFolder). Run build.ps1 first." }
    runNativeCommand dotnet @('publish', $updaterCsproj, '-c', 'Release', '-r', $target.RuntimeIdentifier, '--no-self-contained', '-o', $target.BinFolder) "dotnet publish updater $($target.Architecture)"
}; closeOut 0

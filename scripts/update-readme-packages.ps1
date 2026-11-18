[CmdletBinding()]
param(
    [switch]$Check,
    [switch]$Fix
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

if (-not $Check -and -not $Fix) {
    $Check = $true
}

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$readmePath = Join-Path $repoRoot 'README.md'
$startMarker = '<!-- packages:extra:start -->'
$endMarker = '<!-- packages:extra:end -->'

$projectPatterns = @(
    (Join-Path $repoRoot 'framework/src/*/*.csproj'),
    (Join-Path $repoRoot 'modules/*/src/*/*.csproj')
)

$packageIds = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
foreach ($pattern in $projectPatterns) {
    Get-ChildItem -Path $pattern -File -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -notmatch '[\\/](test|tests)[\\/]' } |
        ForEach-Object {
            $content = Get-Content -LiteralPath $_.FullName -Raw
            $match = [regex]::Match($content, '<PackageId>\s*([^<]+?)\s*</PackageId>')
            $packageId = if ($match.Success) {
                $match.Groups[1].Value
            } else {
                [System.IO.Path]::GetFileNameWithoutExtension($_.Name)
            }

            if ($packageId -and -not $packageId.Contains('$(')) {
                [void]$packageIds.Add($packageId)
            }
        }
}

$readme = Get-Content -LiteralPath $readmePath -Raw
$baseReadme = $readme
$markerPattern = '(?s)' + [regex]::Escape($startMarker) + '.*?' + [regex]::Escape($endMarker)
$baseReadme = [regex]::Replace($baseReadme, $markerPattern, '')

$missingFromMainTable = @($packageIds | Where-Object { -not $baseReadme.Contains('`' + $_ + '`') } | Sort-Object)
$missingFromReadme = @($packageIds | Where-Object { -not $readme.Contains('`' + $_ + '`') } | Sort-Object)

if ($Check -and $missingFromReadme.Count -gt 0) {
    Write-Host "README.md is missing $($missingFromReadme.Count) package(s):" -ForegroundColor Yellow
    $missingFromReadme | ForEach-Object { Write-Host "  $_" }
    Write-Host 'Run this script with -Fix to regenerate the additional package table.' -ForegroundColor Yellow
    exit 1
}

if ($Fix) {
    $missing = $missingFromMainTable
    $rows = foreach ($packageId in $missing) {
        $encoded = [System.Uri]::EscapeDataString($packageId)
        "| ``$packageId`` | [![NuGet](https://img.shields.io/nuget/v/$encoded.svg)](https://www.nuget.org/packages/$encoded) |![NuGet](https://img.shields.io/nuget/dt/$encoded.svg)|"
    }

    $section = if ($missing.Count -gt 0) {
        @(
            $startMarker
            '### Additional Packages'
            ''
            '| Package | Version | Downloads |'
            '| ------- | ------- | --------- |'
            $rows
            $endMarker
        ) -join "`n"
    } else {
        @($startMarker, $endMarker) -join "`n"
    }

    if ($readme -match [regex]::Escape($startMarker)) {
        $readme = [regex]::Replace($readme, $markerPattern, $section)
    } else {
        $readme = $readme.TrimEnd() + "`n`n" + $section + "`n"
    }

    Set-Content -LiteralPath $readmePath -Value $readme -NoNewline
    Write-Host "README.md updated with $($missing.Count) additional package(s)." -ForegroundColor Green
}

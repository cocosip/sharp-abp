[CmdletBinding()]
param(
    [string]$Configuration = 'Release',
    [string]$ResultsDirectory = "artifacts/coverage/run-$(Get-Date -Format 'yyyyMMdd-HHmmss')",
    [switch]$NoBuild,
    [switch]$ReportOnly
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$repoRoot = Split-Path $PSScriptRoot -Parent
$resultsPath = [IO.Path]::GetFullPath($ResultsDirectory, $repoRoot)
if ((Test-Path -LiteralPath $resultsPath) -and @(Get-ChildItem -LiteralPath $resultsPath -Force).Count) {
    throw 'Use a new results directory so stale reports cannot affect coverage.'
}
New-Item -ItemType Directory -Path $resultsPath -Force | Out-Null

Push-Location $repoRoot
try {
    if (-not $NoBuild) {
        foreach ($solution in Get-Content solutions.txt | Where-Object { $_.Trim() }) {
            & dotnet build $solution.Trim() -c $Configuration
            if ($LASTEXITCODE -ne 0) { throw "Build failed: $solution" }
        }
    }

    $testRoots = @('framework/test') + @(Get-ChildItem modules -Directory | ForEach-Object {
        $testPath = Join-Path $_.FullName 'test'
        if (Test-Path -LiteralPath $testPath) { $testPath }
    })
    $testProjects = @(Get-ChildItem -LiteralPath $testRoots -Recurse -Filter '*.csproj' | Sort-Object FullName)
    if (-not $testProjects.Count) { throw 'No test projects found.' }

    # Include unreferenced production assemblies once, so missing tests contribute zero hits.
    $inventoryPath = Join-Path $resultsPath 'inventory'
    New-Item -ItemType Directory -Path $inventoryPath | Out-Null
    $sourceRoots = @('framework/src') + @(Get-ChildItem modules -Directory | ForEach-Object {
        $sourcePath = Join-Path $_.FullName 'src'
        if (Test-Path -LiteralPath $sourcePath) { $sourcePath }
    })
    $symbolInventory = @{}
    foreach ($project in Get-ChildItem -LiteralPath $sourceRoots -Recurse -Filter '*.csproj') {
        $outputs = @(Get-ChildItem (Join-Path $project.DirectoryName "bin/$Configuration") -Recurse -Filter "$($project.BaseName).dll" |
            Where-Object { $_.Directory.Name -match '^(net10\.0|netstandard2\.[01])$' } |
            Sort-Object @{ Expression = { if ($_.Directory.Name -eq 'net10.0') { 0 } elseif ($_.Directory.Name -eq 'netstandard2.1') { 1 } else { 2 } } })
        if (-not $outputs.Count) { throw "Build output missing for $($project.BaseName)" }
        Copy-Item -LiteralPath $outputs[0].FullName -Destination $inventoryPath
        Copy-Item -LiteralPath ([IO.Path]::ChangeExtension($outputs[0].FullName, '.pdb')) -Destination $inventoryPath
        $pdbStream = [IO.File]::OpenRead([IO.Path]::ChangeExtension($outputs[0].FullName, '.pdb'))
        $pdbProvider = [System.Reflection.Metadata.MetadataReaderProvider]::FromPortablePdbStream($pdbStream)
        try {
            $reader = $pdbProvider.GetMetadataReader()
            $sequencePoints = 0
            foreach ($handle in $reader.MethodDebugInformation) {
                foreach ($point in $reader.GetMethodDebugInformation($handle).GetSequencePoints()) {
                    if (-not $point.IsHidden) { $sequencePoints++ }
                }
            }
            $symbolInventory[$project.BaseName] = $sequencePoints
        }
        finally {
            $pdbProvider.Dispose()
            $pdbStream.Dispose()
        }
    }
    $symbolInventory | ConvertTo-Json | Set-Content (Join-Path $resultsPath 'symbols.json')
    [xml]$inventorySettings = Get-Content coverage.runsettings -Raw
    $includeDirectory = $inventorySettings.CreateElement('IncludeDirectory')
    $includeDirectory.InnerText = $inventoryPath
    $inventorySettings.RunSettings.DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.AppendChild($includeDirectory) | Out-Null
    $inventorySettingsPath = Join-Path $resultsPath 'inventory.runsettings'
    $inventorySettings.Save($inventorySettingsPath)

    $failedProjects = @()
    $settingsPath = $inventorySettingsPath
    foreach ($project in $testProjects) {
        Write-Host "Testing $($project.BaseName)"
        $suiteResultsPath = Join-Path $resultsPath $project.BaseName
        & dotnet test $project.FullName -c $Configuration --no-build `
            --settings $settingsPath --collect 'XPlat Code Coverage' `
            --results-directory $suiteResultsPath `
            --logger 'trx;LogFileName=tests.trx'
        $testExitCode = $LASTEXITCODE
        $coverageFiles = @(Get-ChildItem -LiteralPath $suiteResultsPath -Recurse -Filter coverage.json -ErrorAction SilentlyContinue)
        if ($testExitCode -ne 0 -or -not $coverageFiles.Count) { $failedProjects += $project.BaseName }
        $settingsPath = 'coverage.runsettings'
    }

    $reportArguments = @('scripts/coverage_report.py', $resultsPath)
    if ($ReportOnly) { $reportArguments += '--report-only' }
    & python @reportArguments
    $coverageExitCode = $LASTEXITCODE
    if ($failedProjects.Count) { throw "Failed test projects: $($failedProjects -join ', ')" }
    if ($coverageExitCode -ne 0) { throw "Coverage is incomplete or below 85%. See $resultsPath/summary.md" }
}
finally {
    Pop-Location
}

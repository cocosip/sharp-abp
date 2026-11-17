[CmdletBinding()]
param(
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$solutionPaths = @(
    Get-Content (Join-Path $repoRoot 'solutions.txt') |
        ForEach-Object { $_.Trim() } |
        Where-Object { $_ }
)

Push-Location $repoRoot
try {
    for ($index = 0; $index -lt $solutionPaths.Count; $index++) {
        $solutionPath = $solutionPaths[$index]
        $absolutePath = Join-Path $repoRoot $solutionPath

        if (-not (Test-Path -LiteralPath $absolutePath)) {
            throw "Path does not exist: $solutionPath"
        }

        Write-Host ("[{0}/{1}] dotnet build {2}" -f ($index + 1), $solutionPaths.Count, $solutionPath) -ForegroundColor Cyan
        & dotnet build $solutionPath -c $Configuration

        if ($LASTEXITCODE -ne 0) {
            throw "Build failed: $solutionPath"
        }

        Write-Host ''
    }

    Write-Host 'All solution paths built successfully.' -ForegroundColor Green
}
finally {
    Pop-Location
}

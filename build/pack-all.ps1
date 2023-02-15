. ".\common.ps1" -f

# Build all solutions

foreach ($solutionPath in $solutionPaths) {    
    $solutionAbsPath = (Join-Path $rootFolder $solutionPath)
    Set-Location $solutionAbsPath
    dotnet pack -c:Release --include-symbols --no-build -o:$rootPath\dest
    if (-Not $?) {
        Write-Host ("Pack failed for the solution: " + $solutionPath)
        Set-Location $rootFolder
        exit $LASTEXITCODE
    }
}

Set-Location $rootFolder

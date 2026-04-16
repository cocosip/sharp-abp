[CmdletBinding()]
param(
    [string]$Configuration = 'Release'
)

& (Join-Path $PSScriptRoot 'Build-All.ps1') -Configuration $Configuration

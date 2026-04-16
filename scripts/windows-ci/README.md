# Windows Build Script

This directory contains a simple local PowerShell script that follows the solution list from [.github/workflows/ci-cd.yml](../../.github/workflows/ci-cd.yml) and builds everything in one go.

## Usage

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\windows-ci\Build-All.ps1
```

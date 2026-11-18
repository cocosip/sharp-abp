# Windows Build Script

This directory contains a simple local PowerShell script that reads the solution list from [solutions.txt](../../solutions.txt) and builds everything in one go. It only builds; testing and packing are handled by [.github/workflows/ci-cd.yml](../../.github/workflows/ci-cd.yml).

## Usage

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\windows-ci\Build-All.ps1
```

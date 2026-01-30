# Multi-RTSP Viewer Release Script
# This script builds a self-contained, single-file executable for Windows x64.

$ProjectDir = "./src/MultiRtspViewer"
$OutDir = "./release"

Write-Host "Starting Build Process: Multi-RTSP Viewer (Cyberpunk Security Edition)" -ForegroundColor Cyan

# 1. Clean previous builds
if (Test-Path $OutDir) {
    Write-Host "Cleaning old release folder..." -ForegroundColor Gray
    Remove-Item -Path $OutDir -Recurse -Force
}

# 2. Run Publish Command
Write-Host "Publishing to Single File... (This may take a minute)" -ForegroundColor Yellow
dotnet publish $ProjectDir -c Release -r win-x64 --self-contained true -o $OutDir

if ($LASTEXITCODE -eq 0) {
    Write-Host "Success! Your app is ready in: $OutDir" -ForegroundColor Green
    Write-Host "File: MultiRtspViewer.exe" -ForegroundColor Green
    Write-Host "Note: You can copy this .exe to any Windows 10+ PC and it will run." -ForegroundColor White
} else {
    Write-Host "Build failed. Please check the errors above." -ForegroundColor Red
}

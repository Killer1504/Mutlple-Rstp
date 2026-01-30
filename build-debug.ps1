# Multi-RTSP Viewer Release Script (Multi-File for Debugging)
$ProjectDir = "./src/MultiRtspViewer"
$OutDir = "./release-debug"

Write-Host "Starting Build Process: Multi-RTSP Viewer (Debug Release)" -ForegroundColor Cyan

if (Test-Path $OutDir) {
    Remove-Item -Path $OutDir -Recurse -Force
}

# Publish WITHOUT single file to see if it works
dotnet publish $ProjectDir -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o $OutDir

if ($LASTEXITCODE -eq 0) {
    Write-Host "Success! Your app is ready in: $OutDir" -ForegroundColor Green
} else {
    Write-Host "Build failed." -ForegroundColor Red
}

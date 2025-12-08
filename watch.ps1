# SAM Launcher Script
# Closes any running instances, builds Release, and launches SAM.Picker and SAM.Game

# Stop any running SAM processes
Write-Host "[CLEANUP] Stopping any running SAM instances..." -ForegroundColor Yellow
Get-Process | Where-Object { $_.ProcessName -match "SAM\.(Game|Picker)" } | ForEach-Object {
    Write-Host "  Closing $($_.ProcessName)..." -ForegroundColor Gray
    $_.Kill()
}
Start-Sleep -Milliseconds 500
Write-Host "[CLEANUP] Done." -ForegroundColor Green

Write-Host "[BUILD] Building Release configuration..." -ForegroundColor Cyan
dotnet build "$PSScriptRoot\SAM.sln" -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "[BUILD] Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "[BUILD] Build succeeded!" -ForegroundColor Green

# Launch SAM.Picker
Write-Host "[START] Launching SAM.Picker..." -ForegroundColor Cyan
Start-Process -FilePath "$PSScriptRoot\Release\x86\SAM.Picker.exe"

# Launch SAM.Game with app id 219640
Write-Host "[START] Launching SAM.Game with app id 219640..." -ForegroundColor Cyan
Start-Process -FilePath "$PSScriptRoot\Release\x86\SAM.Game.exe" -ArgumentList "219640"

# Launch SAM.Game with app id 278440
Write-Host "[START] Launching SAM.Game with app id 278440..." -ForegroundColor Cyan
Start-Process -FilePath "$PSScriptRoot\Release\x86\SAM.Game.exe" -ArgumentList "278440"

Write-Host "[DONE] Applications launched." -ForegroundColor Green

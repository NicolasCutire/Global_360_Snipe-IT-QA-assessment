# PowerShell script to run the Snipe-IT QA Assessment tests
# This script installs dependencies and runs the tests

Write-Host "Snipe-IT QA Assessment Test Runner" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green

# Check if .NET is installed
Write-Host "Checking .NET installation..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ .NET is not installed or not in PATH" -ForegroundColor Red
    Write-Host "Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download" -ForegroundColor Red
    exit 1
}

# Restore packages
Write-Host "`nRestoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Failed to restore packages" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Packages restored successfully" -ForegroundColor Green

# Install Playwright browsers
Write-Host "`nInstalling Playwright browsers..." -ForegroundColor Yellow
dotnet run
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Failed to install Playwright browsers" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Playwright browsers installed successfully" -ForegroundColor Green

# Run tests
Write-Host "`nRunning tests..." -ForegroundColor Yellow
Write-Host "This will open a browser window to demonstrate the test execution." -ForegroundColor Cyan
Write-Host "Press any key to continue or Ctrl+C to cancel..." -ForegroundColor Cyan
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

dotnet test --logger "console;verbosity=detailed"
if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✓ All tests passed successfully!" -ForegroundColor Green
} else {
    Write-Host "`n✗ Some tests failed. Check the output above for details." -ForegroundColor Red
}

Write-Host "`nTest execution completed." -ForegroundColor Green

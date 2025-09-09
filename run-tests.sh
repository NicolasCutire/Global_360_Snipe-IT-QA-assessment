#!/bin/bash
# Bash script to run the Snipe-IT QA Assessment tests
# This script installs dependencies and runs the tests

echo "Snipe-IT QA Assessment Test Runner"
echo "================================="

# Check if .NET is installed
echo "Checking .NET installation..."
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo "✓ .NET version: $DOTNET_VERSION"
else
    echo "✗ .NET is not installed or not in PATH"
    echo "Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download"
    exit 1
fi

# Restore packages
echo ""
echo "Restoring NuGet packages..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "✗ Failed to restore packages"
    exit 1
fi
echo "✓ Packages restored successfully"

# Install Playwright browsers
echo ""
echo "Installing Playwright browsers..."
dotnet run
if [ $? -ne 0 ]; then
    echo "✗ Failed to install Playwright browsers"
    exit 1
fi
echo "✓ Playwright browsers installed successfully"

# Run tests
echo ""
echo "Running tests..."
echo "This will open a browser window to demonstrate the test execution."
echo "Press Enter to continue or Ctrl+C to cancel..."
read

dotnet test --logger "console;verbosity=detailed"
if [ $? -eq 0 ]; then
    echo ""
    echo "✓ All tests passed successfully!"
else
    echo ""
    echo "✗ Some tests failed. Check the output above for details."
fi

echo ""
echo "Test execution completed."
